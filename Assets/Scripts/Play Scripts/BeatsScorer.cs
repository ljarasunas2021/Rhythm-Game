using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatsScorer : MonoBehaviour
{
    public static BeatsScorer instance;

    private List<Beat> nextBeats = new List<Beat>();
    private float maxDistance;
    private Transform beatsOrigin, notesParent;

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

    private void Start()
    {
        beatsOrigin = BeatsMover.instance.beatsOrigin;
        notesParent = BeatsMover.instance.notesParent;
    
        maxDistance = Mathf.Max(Vector3.Distance(beatsOrigin.transform.position, notesParent.GetChild(0).position), Vector3.Distance(beatsOrigin.transform.position, notesParent.GetChild(1).position));
    }

    private void Update()
    {
        SetNextBeats();

        float currentTime = BeatsMover.instance.GetCurrentTime();
        float extraDelay = BeatsMover.instance.extraDelay;

        foreach (Beat beat in GameManager.instance.beats)
        {
            if (!beat.destroyed)
            {
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
                    float beatsTime = BeatsMover.instance.GetBeatTime(beat);

                    switch (beat.beatType)
                    {
                        case BeatType.SinglePress:

                            if (GetKey(beat, true))
                            {
                                PlayUIManager.instance.RateTiming(((SinglePressBeat)beat).note, GetAccuracy(beat));
                                beat.destroyed = true;
                            }

                            break;
                        case BeatType.Hold:

                            if (GetKey(beat, true))
                            {
                                ((HoldBeat)beat).accuracy = GetAccuracy(beat, true);
                            }

                            if (GetKey(beat, false) && ((HoldBeat)beat).accuracy != 0)
                            {                                
                                float combinedAccuracy = (GetAccuracy(beat, false) + ((HoldBeat)beat).accuracy) / 2;
                                PlayUIManager.instance.RateTiming(((HoldBeat)beat).note, combinedAccuracy);
                                beat.destroyed = true;
                            }
                            
                            break;
                        case BeatType.Scratch:

                            if (GetScratchKeyDown(beat))
                            {
                                PlayUIManager.instance.RateTiming(-1, GetAccuracy(beat));
                                beat.destroyed = true;
                            }

                            break;
                        case BeatType.DoublePress:

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

    public bool GetKey(Beat beat, bool down, bool useFirstDoublePressNote = true)
    {
        if (!GameManager.instance.InPlayScene())
        {
            return false;
        }

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

        if (nextBeats[note - 1] != beat)
        {
            return false;
        }

        if (down)
        {
            return Input.GetKeyDown(GameManager.instance.inputs[note]);
        }
        else
        {
            return Input.GetKeyUp(GameManager.instance.inputs[note]);
        }
    }

    public bool GetScratchKeyDown(Beat beat)
    {
        if (nextBeats[0] != beat)
        {
            return false;
        }

        return Input.GetKeyDown(GameManager.instance.inputs[0]);
    }

    private void SetNextBeats()
    {
        nextBeats = new List<Beat>();

        for (int i = 1; i < 7; i++)
        {
            bool added = false;

            foreach(Beat beat in GameManager.instance.beats)
            {
                if (!beat.destroyed && !added)
                {
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

            if (!added)
            {
                nextBeats.Add(null);
            }
        }
    }


    public float GetAccuracy(Beat beat, bool firstBeatForHolder = true)
    {
        switch (beat.beatType)
        {
            case BeatType.Scratch:

                return 1 - Vector3.Distance(beat.mover.transform.GetChild(0).position, notesParent.GetChild(0).position) / maxDistance;

            case BeatType.DoublePress:

                float distance = Mathf.Min(Vector3.Distance(beat.mover.transform.GetChild(0).position, notesParent.GetChild(((DoublePressBeat)beat).notes[0] - 1).position), Vector3.Distance(beat.mover.transform.GetChild(1).position, notesParent.GetChild(((DoublePressBeat)beat).notes[0] - 1).position));
                return 1 - distance / maxDistance;

            case BeatType.SinglePress:

                return 1 - Vector3.Distance(beat.mover.transform.position, notesParent.GetChild(((SinglePressBeat)beat).note - 1).position) / maxDistance;

            case BeatType.Hold:

                int child = firstBeatForHolder ? 0 : 1;                
                return 1 - Vector3.Distance(beat.mover.transform.GetChild(child).position, notesParent.GetChild(((HoldBeat)beat).note - 1).position) / maxDistance;

            default:
                return -1;
        }
    }
}
