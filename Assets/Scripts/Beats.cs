using UnityEngine;
using UnityEngine.UI;

// This class holds the beats/rhythm to the song playing in the play and editor scenes
public class Beats : MonoBehaviour
{
    // singleton reference
    public static Beats instance;

    // how many (quarter) beats happen each second
    public int beatsPerSecond = 4;
    // dropdown to change the beat amount (full beats, half beats, quarter beats)
    public Dropdown beatsDropdown;

    // the current beat that the song is at
    [HideInInspector] public int currentBeat;

    // the current beat amount (0 -> full beats, 1 -> half beats, 2 -> quarter beats)
    private int currentBeatAmount = 0;
    // the audio source that plays the music
    private AudioSource audioSource;

    // singleton implementation
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
    }

    // initialize audio source
    private void Start()
    {
        audioSource = GameManager.instance.audioSource;
    }

    // set the current beat and adjust the audio source accordingly
    public void SetCurrentBeat()
    {
        currentBeat = (int)(audioSource.time * beatsPerSecond);
        AdjustAudioSource();
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
        if (currentBeat > audioSource.clip.length * beatsPerSecond) currentBeat = (int)(audioSource.clip.length * beatsPerSecond);
        // adjust the audio source accordingly
        AdjustAudioSource();
    }

    // adjust the audio source to start at the current beat
    private void AdjustAudioSource()
    {
        audioSource.time = (float)currentBeat / (float)beatsPerSecond;
    }

    // change the beat amount dropdown
    public void ChangeBeatsDropdown()
    {
        currentBeatAmount = beatsDropdown.value;
    }
}