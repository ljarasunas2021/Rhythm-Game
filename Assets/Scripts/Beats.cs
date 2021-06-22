using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Beats : MonoBehaviour
{
    public static Beats instance;

    public int beatsPerSecond = 4;    
    public Dropdown beatsDropdown;

    [HideInInspector] public int currentBeat;

    private int currentDrop = 0;
    private AudioSource audioSource;

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

    private void Start()
    {        
        audioSource = EditorManager.instance.audioSource;
    }

    public void SetCurrentBeat()
    {
        currentBeat = (int)(audioSource.time * beatsPerSecond);
        AdjustAudioSource();
    }

    public void IncreaseCurrentBeat(int increment)
    {
        int amount = 4;
        if (currentDrop == 1) amount = 2;
        else if (currentDrop == 2) amount = 1;
        currentBeat += increment * amount;
        if (currentBeat < 0) currentBeat = 0;
        if (currentBeat > audioSource.clip.length * beatsPerSecond) currentBeat = (int)(audioSource.clip.length * beatsPerSecond);
        AdjustAudioSource();
    }

    private void AdjustAudioSource()
    {
        audioSource.time = (float)currentBeat / (float)beatsPerSecond;
    }

    public void ChangeBeatsDropdown()
    {
        currentDrop = beatsDropdown.value;
    }
}

public enum CurrentBeatSetting
{
    Full, Half, Quarter
}