using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatsMover : MonoBehaviour
{
    public static BeatsMover instance;

    public Transform beatsOrigin, notesParent;
    public float beatSpeed = 1;
    public float extraDelay = 0.25f;
    public GameObject singleBeat, doubleBeat, holdBeat, scratchBeat;

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
            extraDelay = 0;
        }        
    }

    private void Update()
    {
        float currentTime = GetCurrentTime();
        foreach (Beat beat in GameManager.instance.beats) {
            if (beat.destroyed)
            {
                if (beat.mover != null)
                {
                    Destroy(beat.mover);
                }
            }
            else
            {
                float beatsTime = GetBeatTime(beat);

                switch (beat.beatType)
                {
                    case BeatType.SinglePress:
                        if (GetTiming(beat) == 0)
                        {
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

                        float duration = ((HoldBeat)beat).duration;

                        if (GetTiming(beat) == 0)
                        {
                            GameObject mover = beat.mover;
                            if (mover == null)
                            {
                                mover = new GameObject("Hold Parent");
                                mover.transform.SetParent(transform);
                                beat.mover = mover;

                                Instantiate(holdBeat, mover.transform);
                                Instantiate(holdBeat, mover.transform);
                            }

                            if (beatsTime - currentTime >= 0)
                            {
                                mover.transform.GetChild(0).position = Lerp(beatsOrigin.transform.position, notesParent.GetChild(((HoldBeat)beat).note - 1).transform.position, 1 - (beatsTime - currentTime) / (1 / (beatSpeed * beat.speed)));
                            }
                            else
                            {
                                mover.transform.GetChild(0).position = notesParent.GetChild(((HoldBeat)beat).note - 1).transform.position;
                            }

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

    public int GetTiming(Beat beat) 
    {
        float currentTime = GetCurrentTime();
        float beatsTime = GetBeatTime(beat);

        switch (beat.beatType)
        {
            case BeatType.Scratch:
            case BeatType.DoublePress:
            case BeatType.SinglePress:

                if (beatsTime - currentTime > (1 / (beatSpeed * beat.speed)))
                {
                    return -1;
                }
                else if (beatsTime + extraDelay - currentTime < 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }

            case BeatType.Hold:

                if (beatsTime - currentTime > (1 / (beatSpeed * beat.speed)))
                {
                    return -1;
                }
                else if (beatsTime + ((HoldBeat)beat).duration + extraDelay - currentTime < 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            default:
                return -1;
        }
    }

    public float GetCurrentTime()
    {
        return GameManager.instance.audioSource.time; 
    }

    public float GetBeatTime(Beat beat)
    {
        return beat.beatAt / Beats.instance.beatsPerSecond;
    }

    public Vector3 Lerp(Vector3 pos1, Vector3 pos2, float t)
    {
        return pos1 + (pos2 - pos1) * t;
    }
}
