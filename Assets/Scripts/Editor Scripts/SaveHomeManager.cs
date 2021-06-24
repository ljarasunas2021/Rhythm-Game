using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

// This class handles any saving or navigating to the home scene actions in the editor
public class SaveHomeManager : MonoBehaviour
{
    // singleton reference
    public static SaveHomeManager instance;

    // save input field (top right)
    public InputField save;

    // a JSON file which holds all of the saves
    private TextAsset savesFile;

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

            // get the saves object from the savesFile (my method uses JsonConvert since Unity's standard library to convert objects to JSON isn't able to serialize subclasses)
            savesFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Data/Saves.json", typeof(TextAsset));

            Saves saves;
            if (savesFile.text.Length <= 2)
            {
                saves = new Saves(new List<Save>());
            }
            else
            {
                saves = JsonConvert.DeserializeObject<Saves>(savesFile.text, GameManager.instance.settings);
            }

            // create the new save and add it to the saves object
            Save newSave = new Save(save.text, GameManager.instance.beats, Array.IndexOf(GameManager.instance.audioClips, GameManager.instance.audioClip));

            bool found = false;
            for (int i = 0; i < saves.saves.Count; i++)
            {
                if (saves.saves[i].name == save.text)
                {
                    found = true;
                    saves.saves[i] = newSave;
                }
            }

            if (!found)
            {
                saves.saves.Add(newSave);
            }

            // save that saves object
            GameManager.instance.saves = saves.saves;

            string json = JsonConvert.SerializeObject(saves, Formatting.Indented, GameManager.instance.settings);
            File.WriteAllText(AssetDatabase.GetAssetPath(savesFile), json);
            EditorUtility.SetDirty(savesFile);
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

    // constructor
    public Save(string name, List<Beat> beats, int audioClipIndex)
    {
        this.name = name;
        this.beats = beats;
        this.audioClipIndex = audioClipIndex;
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