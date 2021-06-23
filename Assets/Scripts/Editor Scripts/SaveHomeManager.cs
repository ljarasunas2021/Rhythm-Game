using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class SaveHomeManager : MonoBehaviour
{
    public static SaveHomeManager instance;
    
    public InputField save;

    private TextAsset savesFile;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("An instance of SaveHomeManager already exists");
        }

        save.text = GameManager.instance.savedName;        
    }

    public void Save()
    {
        if (save.text != "")
        {
            foreach (Beat beat in GameManager.instance.beats)
            {
                beat.mover = null;
            }

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

            string json = JsonConvert.SerializeObject(saves, Formatting.Indented, GameManager.instance.settings);
            File.WriteAllText(AssetDatabase.GetAssetPath(savesFile), json);
            EditorUtility.SetDirty(savesFile);
        }
    }

    public void Home()
    {
        SceneManager.LoadScene("Home");
        GameManager.instance.audioSource.Stop();
    }
}

[System.Serializable]
public class Save
{
    public string name;
    public List<Beat> beats;
    public int audioClipIndex;

    public Save(string name, List<Beat> beats, int audioClipIndex)
    {
        this.name = name;
        this.beats = beats;
        this.audioClipIndex = audioClipIndex;
    }
}


[System.Serializable]
public class Saves
{
    public List<Save> saves;

    public Saves(List<Save> saves)
    {
        this.saves = saves;
    }
}