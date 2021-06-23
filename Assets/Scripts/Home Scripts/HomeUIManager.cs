using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEditor;

public class HomeUIManager : MonoBehaviour
{
    public static HomeUIManager instance;
    
    public Dropdown optionsDropdown, songDropdown;
    public InputField createNewIF;
    public GameObject standardMenu, optionsMenu, createNewMenu;

    private TextAsset savesFile;
    private bool play = true;
    private int selectedOption = 0, selectedSong = 0;
    private List<Save> saves;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("An instance of HomeUIManager already exists");
        }
    }

    private void Start()
    {
        savesFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Data/Saves.json", typeof(TextAsset));

        Saves savesObj;
        if (savesFile.text.Length > 2)
        {
            savesObj = JsonConvert.DeserializeObject<Saves>(savesFile.text, GameManager.instance.settings); 
        } else
        {
            savesObj = new Saves(new List<Save>());
        }
        
        saves = savesObj.saves;

        foreach (Save save in saves)
        {
            foreach (Beat beat in save.beats)
            {
                beat.mover = null;
            }
            optionsDropdown.options.Add(new Dropdown.OptionData(save.name));
        }

        foreach (AudioClip clip in GameManager.instance.audioClips)
        {
            songDropdown.options.Add(new Dropdown.OptionData(clip.name));
        }
    }

    public void Play()
    {
        play = true;
        ShowStandardMenu(false);
        ShowCreateNew(false);
    }

    public void Edit()
    {
        play = false;
        ShowStandardMenu(false);
        ShowCreateNew(true);
    }

    public void Back()
    {
        ShowStandardMenu(true);
    }

    public void Go()
    {
        if (play && saves.Count > 0)
        {
            GameManager.instance.SetSave(saves[selectedOption]);
            SceneManager.LoadScene("Play");
        }
        else if (saves.Count > 0)
        {
            GameManager.instance.SetSave(saves[selectedOption]);
            SceneManager.LoadScene("Editor");
        }
    }

    public void CreateNew()
    {
        if (createNewIF.text != "")
        {
            GameManager.instance.SetSave(new Save(createNewIF.text, new List<Beat>(), selectedSong));
            SceneManager.LoadScene("Editor");
        }
    }

    public void OnChangeDropdown()
    {
        selectedOption = optionsDropdown.value;
    }

    public void OnChangeSongDropdown()
    {
        selectedSong = songDropdown.value;
    }

    public void ShowStandardMenu(bool show)
    {
        standardMenu.SetActive(show);
        optionsMenu.SetActive(!show);
    }

    public void ShowCreateNew(bool show)
    {
        createNewMenu.SetActive(show);
    }
}
