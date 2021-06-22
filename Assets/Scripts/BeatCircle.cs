using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatCircle : MonoBehaviour
{
    [HideInInspector] public Beat beat;

    public InputField speed, duration;

    public GameObject options;

    public void SetupBeat(Beat beat)
    {
        this.beat = beat;
        speed.text = "" + beat.speed;
        if (beat.beatType == BeatType.Hold)
        {
            duration.gameObject.SetActive(true);
            duration.text = "" + ((HoldBeat)beat).duration;
        }

        Image image = transform.GetChild(0).GetComponent<Image>();
        switch (beat.beatType)
        {
            case BeatType.SinglePress:
                image.color = Color.yellow;                
                break;
            case BeatType.Hold:
                image.color = Color.green;
                transform.position += Vector3.down * 67;
                break;
            case BeatType.Scratch:
                image.color = Color.blue;
                transform.position += Vector3.down * 133;
                break;
            case BeatType.DoublePress:
                image.color = Color.magenta;
                transform.position += Vector3.down * 200;
                break;
        }        
    }

    public void ChangeSpeed()
    {
        beat.speed = float.Parse(speed.text);
    }

    public void ChangeDuration()
    {
        ((HoldBeat)beat).duration = float.Parse(duration.text);
    }

    public void Delete()
    {
        EditorManager.instance.DeleteBeat(beat);
        Destroy(gameObject);
    }

    public void ToggleActive()
    {
        if (!EditorManager.instance.audioSource.isPlaying)
        {
            options.SetActive(!options.activeInHierarchy);
        }        
    }

    public void DisableOptions()
    {
        options.SetActive(false);
    }
}
