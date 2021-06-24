using System.Collections.Generic;
using UnityEngine;

// This class manages the main key aspects of the editor
public class EditorManager : MonoBehaviour
{
    // singleton reference
    public static EditorManager instance;

    // the current beat type that the user has selected
    [HideInInspector] public BeatType currentSelectedBeatType = BeatType.None;

    // the audio source that plays the music
    private AudioSource audioSource;

    // the notes that have been selected (used for the double press beat)
    private List<int> selectedNotes = new List<int>();

    // the note that the user has selected for a hold beat
    private int holdNote = 0;

    // singleton implementation
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Instance of the EditorManager already exists.");
        }
    }

    void Start()
    {
        // setup audio
        audioSource = GameManager.instance.audioSource;
        audioSource.clip = GameManager.instance.audioClip;
        Waveform.instance.DrawWaveForm(GameManager.instance.audioClip);        
        audioSource.Play();

        // hide appropriate menus
        ShowNonAudioMenus(false);

        // add appropriate beat circles
        foreach (Beat beat in GameManager.instance.beats)
        {
            BeatCircles.instance.AddBeatCircle(beat);
        }
    }

    // add a beat
    public void AddBeat(Beat beat)
    {
        GameManager.instance.beats.Add(beat);
        BeatCircles.instance.AddBeatCircle(beat);
    }

    // delete a beat
    public void DeleteBeat(Beat beat)
    {
        if (beat.mover != null)
        {
            Destroy(beat.mover);
        }
        GameManager.instance.beats.Remove(beat);
    }

    // play the audio
    public void PlayAudio()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            ShowNonAudioMenus(false);
        }
    }

    // pause the audio
    public void PauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            ShowNonAudioMenus(true);
            Beats.instance.SetCurrentBeat();
        }
    }

    // further the process of creating a beat after the user has selected a note to go with that beat
    public void SelectNote(int note)
    {
        switch (currentSelectedBeatType)
        {
            case BeatType.SinglePress:
                // add the beat directly
                AddBeat(new SinglePressBeat(Beats.instance.currentBeat, Speed.instance.currentSpeed, note));
                currentSelectedBeatType = BeatType.None;
                ShowMenus(true);
                EditorUIManager.instance.ShowSelectNote(false);
                break;
            case BeatType.DoublePress:
                // check that both beats have been selected and add the beat
                bool highlighted = EditorUIManager.instance.HighlightNote(note);

                if (highlighted) selectedNotes.Add(note);
                else selectedNotes.Remove(note);
                
                if (selectedNotes.Count == 2)
                {
                    AddBeat(new DoublePressBeat(Beats.instance.currentBeat, Speed.instance.currentSpeed, selectedNotes.ToArray()));
                    EditorUIManager.instance.ResetHighlightedNotes();
                    EditorUIManager.instance.ShowSelectTwoNotes(false);
                    selectedNotes = new List<int>();
                    currentSelectedBeatType = BeatType.None;
                    ShowMenus(true);
                }
                break;
            case BeatType.Hold:
                // show the hold input
                EditorUIManager.instance.ShowHoldInput(true);
                EditorUIManager.instance.ShowSelectNote(false);
                holdNote = note;
                currentSelectedBeatType = BeatType.None;
                break;
        }
    }

    // called after the user types in a beat's hold duration to create a beat with that duration
    public void FinishTypingHoldDuration(float duration)
    {      
        AddBeat(new HoldBeat(Beats.instance.currentBeat, Speed.instance.currentSpeed, holdNote, duration));
        holdNote = 0;
        EditorUIManager.instance.ShowHoldInput(false);        
        ShowMenus(true);
    }

    // called after the user presses the single press beat button to force the user to select a note for the beat
    public void AddSinglePress()
    {
        ShowMenus(false);
        EditorUIManager.instance.ShowSelectNote(true);
        currentSelectedBeatType = BeatType.SinglePress;
    }

    // called after the user presses the hold beat button to force the user to select a note for the beat
    public void AddHold()
    {
        ShowMenus(false);
        EditorUIManager.instance.ShowSelectNote(true);
        currentSelectedBeatType = BeatType.Hold;
    }

    // called after the user presses the scratch beat button to force the user to select a note for the beat
    public void AddScratch()
    {
        AddBeat(new ScratchBeat(Beats.instance.currentBeat, Speed.instance.currentSpeed));
    }

    // called after the user presses the double press beat button to force the user to select a note for the beat
    public void AddDoublePress()
    {
        ShowMenus(false);
        EditorUIManager.instance.ShowSelectTwoNotes(true);
        currentSelectedBeatType = BeatType.DoublePress;
    }

    // show/hide all of the menus
    private void ShowMenus(bool show)
    {
        EditorUIManager.instance.ShowMusicButtonMenu(show);
        EditorUIManager.instance.ShowAddMenu(show);
        EditorUIManager.instance.ShowBeatsMenu(show);
        EditorUIManager.instance.ShowSpeedMenu(show);
    }

    // show/hide all of the non-audio menus
    private void ShowNonAudioMenus(bool show)
    {
        EditorUIManager.instance.ShowAddMenu(show);
        EditorUIManager.instance.ShowBeatsMenu(show);
        EditorUIManager.instance.ShowSpeedMenu(show);
    }

    // hide all of the options on the beat circles
    private void DisableAllBeatCircleOptions()
    {
        foreach(BeatCircle beatCircle in FindObjectsOfType<BeatCircle>())
        {
            beatCircle.DisableOptions();
        }
    }
}