using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

// This class handles any saving or navigating to the home scene actions in the editor
public class SaveHomeManager : MonoBehaviour
{
    // singleton reference
    public static SaveHomeManager instance;

    // save input field (top right)
    public InputField save;

    // singleton implementation
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("An instance of SaveHomeManager already exists");
        }        
    }

    // set the save input field's text to this save's name
    private void Start()
    {
        save.text = GameManager.instance.savedName;
    }

    // save the current beat map
    public void Save()
    {
        // check that the beatmap has a name
        if (save.text != "")
        {
            // set each beat's mover to null
            foreach (Beat beat in GameManager.instance.beats)
            {
                beat.mover = null;
            }

            // save the new save
            int audioClipIndex = -1;
            for (int i = 0; i < GameManager.instance.audioClips.Length; i++)
            {
                if (GameManager.instance.audioClips[i].audioClip == GameManager.instance.audioClip)
                {
                    audioClipIndex = i;
                }
            }

            Save newSave = new Save(save.text, GameManager.instance.beats, audioClipIndex, GameManager.instance.bpm);
            string json = JsonConvert.SerializeObject(newSave, Formatting.Indented, GameManager.instance.settings);
            File.WriteAllText("Assets/Data/Saves/" + save.text, json);

            // merge the save with the other saves
            bool found = false;

            for (int i = 0; i < GameManager.instance.saves.Count; i++)
            {
                if (GameManager.instance.saves[i].name == save.text)
                {
                    found = true;
                    GameManager.instance.saves[i] = newSave;
                }
            }

            if (!found)
            {
                GameManager.instance.saves.Add(newSave);
            }
        }
    }

    // navigate to the home scene
    public void Home()
    {
        SceneManager.LoadScene("Home");
        GameManager.instance.audioSource.Stop();
    }
}

// beatmaps's data
[System.Serializable]
public class Save
{
    // name of the beatmap
    public string name;
    // beats in the beatmap
    public List<Beat> beats;
    // index of the audio clip that the beats are on
    public int audioClipIndex;
    // beats per minute
    public float bpm;

    // constructor
    public Save(string name, List<Beat> beats, int audioClipIndex, float bpm)
    {
        this.name = name;
        this.beats = beats;
        this.audioClipIndex = audioClipIndex;
        this.bpm = bpm;
    }
}

// list of the saved beatmaps (needed for JSON serialization)
[System.Serializable]
public class Saves
{
    // list of saved beatmaps
    public List<Save> saves;

    // constructor
    public Saves(List<Save> saves)
    {
        this.saves = saves;
    }
}