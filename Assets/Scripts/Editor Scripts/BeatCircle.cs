using UnityEngine;
using UnityEngine.UI;

// This script goes on a beat circle, which shows up on the audio's waveform in the editor to symbolize that a beat occurs at that time
public class BeatCircle : MonoBehaviour
{
    // The beat that this beat circle represents
    [HideInInspector] public Beat beat;

    // The speed and duration input fields on the beat circle
    public InputField speed, duration;

    // A child of the beat circle that appears with an options menu (i.e. speed, duration, delete) when the beat circle is clicked
    public GameObject optionsMenu;

    // Called when the beat is instantiated
    public void SetupBeat(Beat beat)
    {
        // setup beat and input fields
        this.beat = beat;
        speed.text = "" + beat.speed;
        if (beat.beatType == BeatType.Hold)
        {
            duration.gameObject.SetActive(true);
            duration.text = "" + ((HoldBeat)beat).duration;
        }

        // change color and position of beat circle based on its type
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

    // Change the beat circles speed (called from speed input field)
    public void ChangeSpeed()
    {
        beat.speed = float.Parse(speed.text);
    }

    // Change the beat circles duration (called from duration input field)
    public void ChangeDuration()
    {
        ((HoldBeat)beat).duration = float.Parse(duration.text);
    }

    // Delete the beat circle
    public void Delete()
    {
        EditorManager.instance.DeleteBeat(beat);
        Destroy(gameObject);
    }

    // Toggle the options menu
    public void ToggleActive()
    {
        if (!GameManager.instance.audioSource.isPlaying)
        {
            optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
        }        
    }

    // Disable the options menu
    public void DisableOptions()
    {
        optionsMenu.SetActive(false);
    }
}
