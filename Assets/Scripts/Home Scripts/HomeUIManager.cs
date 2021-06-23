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
    public GameObject standardMenu, optionsMenu, createNewMenu, inputMenu, enterYourInput;    

    private TextAsset savesFile;
    private bool play = true;
    private int selectedOption = 0, selectedSong = 0;
    private List<Save> saves;
    private int waitingForInput = -1;

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

        RefreshInputMenu();
    }

    public void Play()
    {
        play = true;
        ShowOptionsMenu();
        ShowCreateNew(false);
    }

    public void Edit()
    {
        play = false;
        ShowOptionsMenu();
        ShowCreateNew(true);
    }

    public void Input()
    {
        ShowInputMenu();
    }

    public void Back()
    {
        ShowStandardMenu();
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

    public void ShowStandardMenu()
    {
        standardMenu.SetActive(true);
        optionsMenu.SetActive(false);
        inputMenu.SetActive(false);
    }

    public void ShowOptionsMenu()
    {
        standardMenu.SetActive(false);
        optionsMenu.SetActive(true);
        inputMenu.SetActive(false);
    }

    public void ShowInputMenu()
    {
        standardMenu.SetActive(false);
        optionsMenu.SetActive(false);
        inputMenu.SetActive(true);
    }

    public void ShowCreateNew(bool show)
    {
        createNewMenu.SetActive(show);
    }

    public void ClickInputButton(int button)
    {
        waitingForInput = button;
        EnterInput(true);
    }

    private void OnGUI()
    {
        Event keyEvent = Event.current;
        if (keyEvent.isKey && waitingForInput != -1)
        {
            GameManager.instance.inputs[waitingForInput] = keyEvent.keyCode;
            RefreshInputMenu();
            waitingForInput = -1;
            EnterInput(false);
            GameManager.instance.SaveInputs();
        }
    }

    private void RefreshInputMenu()
    {
        for (int i = 0; i < 7; i++)
        {
            string baseStr = "Note " + i + ": ";
            if (i == 0)
            {
                baseStr = "Scratch: ";
            }
            inputMenu.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text = baseStr + GameManager.instance.inputs[i].ToString();
        }
    }

    private void EnterInput(bool show)
    {
        inputMenu.SetActive(!show);
        enterYourInput.SetActive(show);
    } 
}
