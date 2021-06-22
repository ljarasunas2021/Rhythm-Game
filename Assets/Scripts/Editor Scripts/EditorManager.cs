using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorManager : MonoBehaviour
{
    public static EditorManager instance;

    [HideInInspector] public BeatType currentNoteLookingFor = BeatType.None;

    private AudioSource audioSource;
    private List<int> selectedNotes = new List<int>();
    private int holdNote = 0;

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
        audioSource = GameManager.instance.audioSource;
        audioSource.clip = GameManager.instance.audioClip;
        EditorUIManager.instance.DrawWaveForm(GameManager.instance.audioClip);
        ShowNonAudioMenus(false);
        audioSource.Play();
        foreach (Beat beat in GameManager.instance.beats)
        {
            BeatCircles.instance.AddBeatCircle(beat);
        }
    }

    public void AddBeat(Beat beat)
    {
        GameManager.instance.beats.Add(beat);
        BeatCircles.instance.AddBeatCircle(beat);
    }

    public void DeleteBeat(Beat beat)
    {
        GameManager.instance.beats.Remove(beat);
    }

    public void PlayAudio()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            ShowNonAudioMenus(false);
        }
    }

    public void PauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            ShowNonAudioMenus(true);
            Beats.instance.SetCurrentBeat();
        }
    }

    public void SelectNote(int note)
    {
        switch (currentNoteLookingFor)
        {
            case BeatType.SinglePress:
                AddBeat(new SinglePressBeat(Beats.instance.currentBeat, Speed.instance.currentSpeed, note));
                currentNoteLookingFor = BeatType.None;
                ShowMenus(true);
                EditorUIManager.instance.ShowSelectNote(false);
                break;
            case BeatType.DoublePress:
                bool highlighted = EditorUIManager.instance.HighlightNote(note);

                if (highlighted) selectedNotes.Add(note);
                else selectedNotes.Remove(note);
                
                if (selectedNotes.Count == 2)
                {
                    AddBeat(new DoublePressBeat(Beats.instance.currentBeat, Speed.instance.currentSpeed, selectedNotes.ToArray()));
                    EditorUIManager.instance.ResetHighlightedNotes();
                    EditorUIManager.instance.ShowSelectTwoNotes(false);
                    selectedNotes = new List<int>();
                    currentNoteLookingFor = BeatType.None;
                    ShowMenus(true);
                }
                break;
            case BeatType.Hold:
                EditorUIManager.instance.ShowHoldInput(true);
                EditorUIManager.instance.ShowSelectNote(false);
                holdNote = note;
                currentNoteLookingFor = BeatType.None;
                break;
        }
    }

    public void FinishTypingHoldDuration(float duration)
    {      
        AddBeat(new HoldBeat(Beats.instance.currentBeat, Speed.instance.currentSpeed, holdNote, duration));
        holdNote = 0;
        EditorUIManager.instance.ShowHoldInput(false);        
        ShowMenus(true);
    }

    public void AddSinglePress()
    {
        ShowMenus(false);
        EditorUIManager.instance.ShowSelectNote(true);
        currentNoteLookingFor = BeatType.SinglePress;
    }

    public void AddHold()
    {
        ShowMenus(false);
        EditorUIManager.instance.ShowSelectNote(true);
        currentNoteLookingFor = BeatType.Hold;
    }

    public void AddScratch()
    {
        AddBeat(new ScratchBeat(Beats.instance.currentBeat, Speed.instance.currentSpeed));
    }

    public void AddDoublePress()
    {
        ShowMenus(false);
        EditorUIManager.instance.ShowSelectTwoNotes(true);
        currentNoteLookingFor = BeatType.DoublePress;
    }

    private void ShowMenus(bool show)
    {
        EditorUIManager.instance.ShowMusicButtonMenu(show);
        EditorUIManager.instance.ShowAddMenu(show);
        EditorUIManager.instance.ShowBeatsMenu(show);
        EditorUIManager.instance.ShowSpeedMenu(show);
    }

    private void ShowNonAudioMenus(bool show)
    {
        EditorUIManager.instance.ShowAddMenu(show);
        EditorUIManager.instance.ShowBeatsMenu(show);
        EditorUIManager.instance.ShowSpeedMenu(show);
    }

    private void DisableAllBeatCircleOptions()
    {
        foreach(BeatCircle beatCircle in FindObjectsOfType<BeatCircle>())
        {
            beatCircle.DisableOptions();
        }
    }
}