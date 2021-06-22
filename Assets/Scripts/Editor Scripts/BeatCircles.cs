using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCircles : MonoBehaviour
{
    public static BeatCircles instance;

    public GameObject beatCircle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("An instance of Beat Circles already exists");
        }
    }

    public void AddBeatCircle(Beat beat)
    {
        GameObject newBeatCircle = Instantiate(beatCircle, transform);                
        newBeatCircle.transform.localPosition = ((beat.beatAt / (Beats.instance.beatsPerSecond * GameManager.instance.audioClip.length)) - 0.5f) * Waveform.instance.waveformWidth * Vector3.right + Vector3.up * (Waveform.instance.cursor.transform.localPosition.y + 100);
        newBeatCircle.GetComponent<BeatCircle>().SetupBeat(beat);
    }
}
