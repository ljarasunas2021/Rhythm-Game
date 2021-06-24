using UnityEngine;

// This class moves the beats in the play scene and editor
public class BeatsMover : MonoBehaviour
{
    // singleton reference
    public static BeatsMover instance;

    // where the beats originate from, the parent of all of the notes
    public Transform beatsOrigin, notesParent;
    // the speed at which the beats move (increase/decrease this value to increase/decrease the speed of all of the beats)
    public float beatSpeed = 1;
    // delay to allow the user to press the appropriate input even after the beat has already reached its note
    public float beatDelay = 0.5f;
    // prefabs for single press beat, double press beat, hold beat and scratch beat
    public GameObject singleBeat, doubleBeat, holdBeat, scratchBeat;

    // singleton implementation and initialize beats and extraDelay
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Already an instance of BeatsMover");
        }

        foreach (Beat beat in GameManager.instance.beats)
        {
            beat.destroyed = false;
            if (beat.beatType == BeatType.Hold)
            {
                ((HoldBeat)beat).accuracy = 0;
            } else if (beat.beatType == BeatType.DoublePress)
            {
                ((DoublePressBeat)beat).accuracy = 0;
            }
        }

        if (!GameManager.instance.InPlayScene())
        {
            beatDelay = 0;
        }        
    }

    // move the beats
    private void Update()
    {
        // get the current time in seconds based on the song
        float currentTime = GetCurrentTime();

        // loop through all of the beats
        foreach (Beat beat in GameManager.instance.beats) {
            // make sure the beat's mover is destroyed if it is destroyed
            if (beat.destroyed)
            {                
                if (beat.mover != null)
                {
                    Destroy(beat.mover);
                }
            }
            // if the beat isn't destroyed move it appropriately
            else
            {                
                // get the time in seconds at which the beat must be at the note
                float beatsTime = GetBeatTime(beat);
                // for any beat type, if the beat is supposed to be moving, make sure its mover is instantiated and move the mover
                switch (beat.beatType)
                {
                    case BeatType.SinglePress:                        
                        if (GetTiming(beat) == 0)
                        {
                            // a single press beat's mover is the single beat prefab itself
                            GameObject mover = beat.mover;
                            if (mover == null)
                            {
                                mover = Instantiate(singleBeat, transform);
                                beat.mover = mover;
                            }
                            mover.transform.position = Lerp(beatsOrigin.transform.position, notesParent.GetChild(((SinglePressBeat)beat).note - 1).transform.position, 1 - (beatsTime - currentTime) / (1 / (beatSpeed * beat.speed)));
                        }
                        else if (beat.mover != null)
                        {
                            Destroy(beat.mover);
                        }
                        break;
                    case BeatType.Hold:
                        // find the beat's hold duration
                        float duration = ((HoldBeat)beat).duration;

                        if (GetTiming(beat) == 0)
                        {
                            // a hold beat's mover is an empty gameobject with 2 hold beats as its children (the 1st moves for the intial press of the input, and the 2nd moves after the first for the release of that input)
                            GameObject mover = beat.mover;
                            if (mover == null)
                            {
                                mover = new GameObject("Hold Parent");
                                mover.transform.SetParent(transform);
                                beat.mover = mover;

                                Instantiate(holdBeat, mover.transform);
                                Instantiate(holdBeat, mover.transform);
                            }

                            // for the first mover (the one that moves for the initial press of the input), move it if it hasn't reached the note or otherwise keep it at the note's position
                            if (beatsTime - currentTime >= 0)
                            {
                                mover.transform.GetChild(0).position = Lerp(beatsOrigin.transform.position, notesParent.GetChild(((HoldBeat)beat).note - 1).transform.position, 1 - (beatsTime - currentTime) / (1 / (beatSpeed * beat.speed)));
                            }
                            else
                            {
                                mover.transform.GetChild(0).position = notesParent.GetChild(((HoldBeat)beat).note - 1).transform.position;
                            }

                            // for the second mover (the one that moves for the release of the input), wait at the origin until it must move to make it to the note at its specified time
                            if (beatsTime + duration - currentTime <= (1 / (beatSpeed * beat.speed)))
                            {
                                mover.transform.GetChild(1).position = Lerp(beatsOrigin.transform.position, notesParent.GetChild(((HoldBeat)beat).note - 1).transform.position, 1 - (beatsTime + duration - currentTime) / (1 / (beatSpeed * beat.speed)));
                            }
                            else
                            {
                                mover.transform.GetChild(1).position = beatsOrigin.transform.position;
                            }
                        }
                        else if (beat.mover != null)
                        {
                            Destroy(beat.mover);
                        }

                        break;
                    case BeatType.Scratch:

                        if (GetTiming(beat) == 0)
                        {
                            // a scratch beat's mover is an empty gameobject with 6 scratch beats as its children
                            GameObject mover = beat.mover;
                            if (mover == null)
                            {
                                mover = new GameObject("Scratch Beat Parent");
                                mover.transform.SetParent(transform);
                                beat.mover = mover;

                                for (int i = 0; i < 6; i++)
                                {
                                    Instantiate(scratchBeat, mover.transform);
                                }
                            }
                            for (int i = 0; i < 6; i++)
                            {
                                mover.transform.GetChild(i).position = Lerp(beatsOrigin.transform.position, notesParent.GetChild(i).transform.position, 1 - (beatsTime - currentTime) / (1 / (beatSpeed * beat.speed)));
                            }
                        }
                        else if (beat.mover != null)
                        {
                            Destroy(beat.mover);
                        }

                        break;
                    case BeatType.DoublePress:

                        // a double press beat's mover is an empty gameobject with 2 double beat children (the first travels to notes[0] while the second travels to notes[1])
                        if (GetTiming(beat) == 0)
                        {
                            GameObject mover = beat.mover;
                            if (mover == null)
                            {
                                mover = new GameObject("Double Beat Parent");
                                mover.transform.SetParent(transform);
                                beat.mover = mover;

                                Instantiate(doubleBeat, mover.transform);
                                Instantiate(doubleBeat, mover.transform);
                            }
                            for (int i = 0; i < 2; i++)
                            {
                                mover.transform.GetChild(i).position = Lerp(beatsOrigin.transform.position, notesParent.GetChild(((DoublePressBeat)beat).notes[i] - 1).transform.position, 1 - (beatsTime - currentTime) / (1 / (beatSpeed * beat.speed)));
                            }
                        }
                        else if (beat.mover != null)
                        {
                            Destroy(beat.mover);
                        }

                        break;
                }
            }                       
        }
    }

    // returns -1 if a beat shouldn't be visible yet, 0 if a beat should be visible and should be moving towards its note, and 1 if a beat should have already been destroyed
    public int GetTiming(Beat beat) 
    {
        // get the current time in seconds based on the song
        float currentTime = GetCurrentTime();
        // get the time in seconds at which the beat must be at the note
        float beatsTime = GetBeatTime(beat);

        switch (beat.beatType)
        {
            case BeatType.Scratch:
            case BeatType.DoublePress:
            case BeatType.SinglePress:

                // the time it requires for the beat to make it to the note is 1 / (beatSpeed * beat.speed), so if beatsTime - currentTime is greater than that, then the beat would reach the note too quickly
                if (beatsTime - currentTime > (1 / (beatSpeed * beat.speed)))
                {
                    return -1;
                }
                // if the currentTime was greater than or equal to the beatsTime (plus that beat delay we give the user), then the beat should already be deleted
                else if (beatsTime + beatDelay - currentTime < 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }

            case BeatType.Hold:

                // same as previous 3 cases
                if (beatsTime - currentTime > (1 / (beatSpeed * beat.speed)))
                {
                    return -1;
                }
                // same as previous 3 cases, but factor in the hold duration here
                else if (beatsTime + ((HoldBeat)beat).duration + beatDelay - currentTime < 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            default:
                // required since GetTiming returns float (this case should never be reached)
                return -1;
        }
    }

    // return the current time in seconds based on the song
    public float GetCurrentTime()
    {
        return GameManager.instance.audioSource.time; 
    }

    // return the time in seconds at which the beat must be at the note
    public float GetBeatTime(Beat beat)
    {
        return beat.beatAt / Beats.instance.beatsPerSecond;
    }

    // linearly interpolate between 2 positions (Unity's Vector3.Lerp clamped the positions to between pos1 and pos2 which isn't ideal in this case since the beat should keep moving during the beat delay time) 
    public Vector3 Lerp(Vector3 pos1, Vector3 pos2, float t)
    {
        return pos1 + (pos2 - pos1) * t;
    }
}
