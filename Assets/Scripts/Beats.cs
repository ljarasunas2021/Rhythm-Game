using UnityEngine;
using UnityEngine.UI;

// This class holds the beats/rhythm to the song playing in the play and editor scenes
public class Beats : MonoBehaviour
{
    // singleton reference
    public static Beats instance;
    
    // dropdown to change the beat amount (full beats, half beats, quarter beats)
    public Dropdown beatsDropdown;
    // beats per minute input field
    public InputField bpmIF;

    // the current beat that the song is at
    [HideInInspector] public int currentBeat;
    // how many quarter beats happen each second
    [HideInInspector] public float quarterBeatsPerSecond;

    // the current beat amount (0 -> full beats, 1 -> half beats, 2 -> quarter beats)
    private int currentBeatAmount = 0;
    // the audio source that plays the music
    private AudioSource audioSource;

    // singleton implementation, initialize vars
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There already exists an instance of the Beats class");
        }

        audioSource = GameManager.instance.audioSource;

        if (!GameManager.instance.InPlayScene())
        {
            SetBPMText();
        }
        SetQuarterBeatsPerSecond();
    }

    // set the current beat and adjust the audio source accordingly
    public void SetCurrentBeat()
    {
        currentBeat = (int)(audioSource.time * quarterBeatsPerSecond);
        AdjustAudioSource();
    }

    // set bpm if change occurs on input field
    public void ChangeBPM()
    {
        GameManager.instance.bpm = Mathf.Round(float.Parse(bpmIF.text) * 4) / 4;
        SetBPMText();
        SetQuarterBeatsPerSecond();
        BeatCircles.instance.AdjustBeatCircles();
    }

    // set the text of the bpm input field
    private void SetBPMText()
    {
        bpmIF.text = "" + GameManager.instance.bpm;
    }

    // set the quarter beats per second variable
    private void SetQuarterBeatsPerSecond()
    {
        quarterBeatsPerSecond = GameManager.instance.bpm / 15;
    }

    // increment the current beat
    public void IncreaseCurrentBeat(int increment)
    {
        // find the amount to increment it by
        int amount = 4;
        if (currentBeatAmount == 1) amount = 2;
        else if (currentBeatAmount == 2) amount = 1;
        // increment the currentBeat, being weary of its bounds
        currentBeat += increment * amount;
        if (currentBeat < 0) currentBeat = 0;
        if (currentBeat > audioSource.clip.length * quarterBeatsPerSecond) currentBeat = (int)(audioSource.clip.length * quarterBeatsPerSecond);
        // adjust the audio source accordingly
        AdjustAudioSource();
    }

    // adjust the audio source to start at the current beat
    private void AdjustAudioSource()
    {
        audioSource.time = (float)currentBeat / (float)quarterBeatsPerSecond;
    }

    // change the beat amount dropdown
    public void ChangeBeatsDropdown()
    {
        currentBeatAmount = beatsDropdown.value;
    }
}