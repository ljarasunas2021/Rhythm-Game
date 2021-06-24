using System.Collections.Generic;
using UnityEngine;

// This class is used in the play scene to score the beats as perfect, great, good or miss
public class BeatsScorer : MonoBehaviour
{
    // singleton reference
    public static BeatsScorer instance;

    // the next beat for each note ({next beats for note 1, next beats for note 2, , etc... })
    private List<Beat> nextBeats = new List<Beat>();
    // maximum distance a beat travels
    private float maxBeatDistance;
    // the position that the beats originate from, the parent of the notes
    private Transform beatsOrigin, notesParent;

    // singleton implementation
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("An instance of BeatsScorer already exists.");
        }
    }

    // initialize variables
    private void Start()
    {
        beatsOrigin = BeatsMover.instance.beatsOrigin;
        notesParent = BeatsMover.instance.notesParent;
    
        maxBeatDistance = Mathf.Max(Vector3.Distance(beatsOrigin.transform.position, notesParent.GetChild(0).position), Vector3.Distance(beatsOrigin.transform.position, notesParent.GetChild(1).position));
    }

    private void Update()
    {
        // set the next beats
        SetNextBeats();


        // loop through all undestroyed beats
        foreach (Beat beat in GameManager.instance.beats)
        {
            if (!beat.destroyed)
            {
                // if the beat hasn't been destroyed yet but has passed, rate that beat as a miss
                if (BeatsMover.instance.GetTiming(beat) == 1)
                {
                    switch (beat.beatType)
                    {
                        case BeatType.SinglePress:
                            PlayUIManager.instance.RateTiming(((SinglePressBeat)beat).note, -100);
                            break;
                        case BeatType.Hold:
                            PlayUIManager.instance.RateTiming(((HoldBeat)beat).note, -100);
                            break;
                        case BeatType.DoublePress:
                        case BeatType.Scratch:
                            PlayUIManager.instance.RateTiming(-1, -100);
                            break;                       
                    }
                    
                    beat.destroyed = true;
                }
                else
                {
                    // if the beat hasn't passed yet, check for player input and rate the player's timing appropriately
                    switch (beat.beatType)
                    {
                        case BeatType.SinglePress:
                            // if the user presses the appropriate key, rate their timing and destroy the beat
                            if (GetKey(beat, true))
                            {
                                PlayUIManager.instance.RateTiming(((SinglePressBeat)beat).note, GetAccuracy(beat));
                                beat.destroyed = true;
                            }
                            break;
                        case BeatType.Hold:
                            // if the user starts to press the appropriate key, set the accuracy of that press
                            if (GetKey(beat, true))
                            {
                                ((HoldBeat)beat).accuracy = GetAccuracy(beat, true);
                            }
                            // if the user releases the press on the appropriate key, rate the timing of that beat using the average of both the press and release and destroy the beat
                            if (GetKey(beat, false) && ((HoldBeat)beat).accuracy != 0)
                            {                                
                                float combinedAccuracy = (GetAccuracy(beat, false) + ((HoldBeat)beat).accuracy) / 2;
                                PlayUIManager.instance.RateTiming(((HoldBeat)beat).note, combinedAccuracy);
                                beat.destroyed = true;
                            }                            
                            break;
                        case BeatType.Scratch:
                            // if the user presses the appropriate key, rate their timing and destroy the beat
                            if (GetScratchKeyDown(beat))
                            {
                                PlayUIManager.instance.RateTiming(-1, GetAccuracy(beat));
                                beat.destroyed = true;
                            }
                            break;
                        case BeatType.DoublePress:
                            // if the user presses the key for the first or second note, save the accuracy of that press and which key was pressed
                            if (GetKey(beat, true, true) && ((DoublePressBeat)beat).accuracy == 0)
                            {
                                ((DoublePressBeat)beat).accuracy = GetAccuracy(beat);
                                ((DoublePressBeat)beat).firstNotePressed = true;
                            }
                            else if (GetKey(beat, true, false) && ((DoublePressBeat)beat).accuracy == 0)
                            {
                                ((DoublePressBeat)beat).accuracy = GetAccuracy(beat);
                                ((DoublePressBeat)beat).firstNotePressed = false;
                            }
                            // check for the other (second) key to be pressed, and rate the timing of that beat using the average of both presses and destroy the beat
                            if (GetKey(beat, true, !((DoublePressBeat)beat).firstNotePressed) && ((DoublePressBeat)beat).accuracy != 0)
                            {
                                float combinedAccuracy = (GetAccuracy(beat) + ((DoublePressBeat)beat).accuracy) / 2;
                                PlayUIManager.instance.RateTiming(-1, combinedAccuracy);
                                beat.destroyed = true;
                            }
                            break;
                    }
                }                
            }            
        }
    }

    // returns true if the user presses the appropriate key for a beat at the appropriate time
    // the down parameter is used to determine whether getKeyDown or getKeyUp should be used
    // the useFirstDoublePressNote parameter is only used for a beat that is a double press beat to determine whether the first or second note should be checked for
    public bool GetKey(Beat beat, bool down, bool useFirstDoublePressNote = true)
    {
        // find the note which input is needed for
        int note = 0;
        switch (beat.beatType)
        {
            case BeatType.SinglePress:
                note = ((SinglePressBeat)beat).note;
                break;
            case BeatType.DoublePress:
                int noteIndex = (useFirstDoublePressNote) ? 0 : 1;
                note = ((DoublePressBeat)beat).notes[noteIndex];
                break;
            case BeatType.Hold:
                note = ((HoldBeat)beat).note;
                break;
        }

        // check that this beat is the nextBeat for that particular note
        if (nextBeats[note - 1] != beat)
        {
            return false;
        }

        // check whether the user did getKeyDown or Up for the particular note
        if (down)
        {
            return Input.GetKeyDown(GameManager.instance.inputs[note]);
        }
        else
        {
            return Input.GetKeyUp(GameManager.instance.inputs[note]);
        }
    }

    // returns true if the user presses the appropriate key for a scratch beat at the appropriate time
    public bool GetScratchKeyDown(Beat beat)
    {
        if (nextBeats[0] != beat)
        {
            return false;
        }

        return Input.GetKeyDown(GameManager.instance.inputs[0]);
    }

    // set up the nextBeats variable
    private void SetNextBeats()
    {
        // set the list to an empty list
        nextBeats = new List<Beat>();

        // loop through all 7 inputs (1 for scratch + 6 for 6 notes)
        for (int i = 1; i < 7; i++)
        {
            bool added = false;

            // loop through each beat
            foreach(Beat beat in GameManager.instance.beats)
            {
                // if the beat hasn't been destroyed and a beat hasn't yet been added for the input
                if (!beat.destroyed && !added)
                {
                    // if the input would be used for the beat's note, add that beat to the list
                    switch(beat.beatType)
                    {
                        case BeatType.SinglePress:
                            if (((SinglePressBeat)beat).note == i)
                            {
                                nextBeats.Add(beat);
                                added = true;
                            }
                            break;
                        case BeatType.Hold:
                            if (((HoldBeat)beat).note == i)
                            {
                                nextBeats.Add(beat);
                                added = true;
                            }
                            break;
                        case BeatType.Scratch:
                            nextBeats.Add(beat);
                            added = true;
                            break;
                        case BeatType.DoublePress:
                            if (((DoublePressBeat)beat).notes[0] == i || ((DoublePressBeat)beat).notes[1] == i)
                            {
                                nextBeats.Add(beat);
                                added = true;
                            }
                            break;
                    }
                }
            }

            // if no beats were added for that particular input, add null to the list
            if (!added)
            {
                nextBeats.Add(null);
            }
        }
    }

    // returns the accuracy of a user's input based on how close the beat is to its corresponding note
    // the firstBeatForHolder parameter is used with a HoldBeat to determine the accuracy of the initial press (true) or release (false)
    public float GetAccuracy(Beat beat, bool useInitialPressForHoldBeat = true)
    {
        switch (beat.beatType)
        {
            case BeatType.Scratch:
                return 1 - Vector3.Distance(beat.mover.transform.GetChild(0).position, notesParent.GetChild(0).position) / maxBeatDistance;
            case BeatType.DoublePress:
                // find the beat that is closer to the first note and record that distance
                float distance = Mathf.Min(Vector3.Distance(beat.mover.transform.GetChild(0).position, notesParent.GetChild(((DoublePressBeat)beat).notes[0] - 1).position), Vector3.Distance(beat.mover.transform.GetChild(1).position, notesParent.GetChild(((DoublePressBeat)beat).notes[0] - 1).position));
                return 1 - distance / maxBeatDistance;
            case BeatType.SinglePress:
                return 1 - Vector3.Distance(beat.mover.transform.position, notesParent.GetChild(((SinglePressBeat)beat).note - 1).position) / maxBeatDistance;
            case BeatType.Hold:
                // find the child index using useInitialPressForHoldBeat
                int childIndex = useInitialPressForHoldBeat ? 0 : 1;                
                return 1 - Vector3.Distance(beat.mover.transform.GetChild(childIndex).position, notesParent.GetChild(((HoldBeat)beat).note - 1).position) / maxBeatDistance;
            default:
                // needed for switch case since GetAccuracy returns float (the switch case should never make it this far)
                return -1;
        }
    }
}
