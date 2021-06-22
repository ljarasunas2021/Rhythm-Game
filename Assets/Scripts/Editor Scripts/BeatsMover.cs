using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatsMover : MonoBehaviour
{
    public static BeatsMover instance;

    public Transform beatsOrigin, notesParent;
    public float beatSpeed = 1;
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
    }

    private void Update()
    {        
        float currentTime = GameManager.instance.audioSource.time;
        foreach (Beat beat in GameManager.instance.beats) {
            float beatsTime = beat.beatAt / Beats.instance.beatsPerSecond;

            switch(beat.beatType)
            {
                case BeatType.SinglePress:

                    if (beatsTime - currentTime >= 0 && beatsTime - currentTime <= (1 / (beatSpeed * beat.speed)))
                    {
                        GameObject mover = beat.mover;
                        if (mover == null)
                        {
                            mover = Instantiate(singleBeat, transform);
                            beat.mover = mover;
                        }
                        mover.transform.position = Vector3.Lerp(beatsOrigin.transform.position, notesParent.GetChild(((SinglePressBeat)beat).note - 1).transform.position, 1 - (beatsTime - currentTime) / (1 / (beatSpeed * beat.speed)));
                    }
                    else if (beat.mover != null)
                    {
                        Destroy(beat.mover);
                    }

                    break;
                case BeatType.Hold:

                    float duration = ((HoldBeat)beat).duration;

                    if (beatsTime - currentTime <= (1 / (beatSpeed * beat.speed)) && beatsTime + duration - currentTime > 0)
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
                            mover.transform.GetChild(0).position = Vector3.Lerp(beatsOrigin.transform.position, notesParent.GetChild(((HoldBeat)beat).note - 1).transform.position, 1 - (beatsTime - currentTime) / (1 / (beatSpeed * beat.speed)));                            
                        }
                        else
                        {
                            mover.transform.GetChild(0).position = notesParent.GetChild(((HoldBeat)beat).note - 1).transform.position;
                        }

                        if (beatsTime + duration - currentTime <= (1 / (beatSpeed * beat.speed)))
                        {
                            mover.transform.GetChild(1).position = Vector3.Lerp(beatsOrigin.transform.position, notesParent.GetChild(((HoldBeat)beat).note - 1).transform.position, 1 - (beatsTime + duration - currentTime) / (1 / (beatSpeed * beat.speed)));
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

                    if (beatsTime - currentTime >= 0 && beatsTime - currentTime <= (1 / (beatSpeed * beat.speed)))
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
                            mover.transform.GetChild(i).position = Vector3.Lerp(beatsOrigin.transform.position, notesParent.GetChild(i).transform.position, 1 - (beatsTime - currentTime) / (1 / (beatSpeed * beat.speed)));
                        }
                    }
                    else if (beat.mover != null)
                    {
                        Destroy(beat.mover);
                    }

                    break;
                case BeatType.DoublePress:

                    if (beatsTime - currentTime >= 0 && beatsTime - currentTime <= (1 / (beatSpeed * beat.speed)))
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
                            mover.transform.GetChild(i).position = Vector3.Lerp(beatsOrigin.transform.position, notesParent.GetChild(((DoublePressBeat)beat).notes[i] - 1).transform.position, 1 - (beatsTime - currentTime) / (1 / (beatSpeed * beat.speed)));
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
