using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Manage the key components of the UI in the home scene
public class HomeUIManager : MonoBehaviour
{
    // singleton reference
    public static HomeUIManager instance;

    // dropdown for the different saves and songs (if the user is creating a new beatmap)
    public Dropdown saveDropdown, songDropdown;
    // input field for the name of the new beat map
    public InputField createNewIF;
    // menus to be displayed
    public GameObject standardMenu, optionsMenu, createNewMenu, inputMenu, enterYourInput;    

    // file that contains all of the saved beatmaps
    private TextAsset savesFile;
    // whether the user selected play or not (the user selected edit)
    private bool play = true;
    // the save and song options that were selected
    private int selectedSave = 0, selectedSong = 0;
    // the note that the class is waiting to change the user's input for (-1 means the script doesn't have to wait, 0 means the script is waiting for the user's input for a scratch beat, 1 means the script is waiting for the user's input for note 1, 2 --> note 2, 3 --> note 3, etc...
    private int waitingForInput = -1;

    // singleton implementation
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

    // setup the dropdowns and input menu
    private void Start()
    {        
        foreach (Save save in GameManager.instance.saves)
        {
            foreach (Beat beat in save.beats)
            {
                beat.mover = null;
            }
            saveDropdown.options.Add(new Dropdown.OptionData(save.name));
        }

        foreach (AudioClip clip in GameManager.instance.audioClips)
        {
            songDropdown.options.Add(new Dropdown.OptionData(clip.name));
        }

        RefreshInputMenu();
    }

    // press the play button
    public void Play()
    {
        play = true;
        ShowOptionsMenu();
        ShowCreateNewOption(false);
    }

    // press the edit button
    public void Edit()
    {
        play = false;
        ShowOptionsMenu();
        ShowCreateNewOption(true);
    }

    // press the input button
    public void Input()
    {
        ShowInputMenu();
    }

    // click the back button
    public void Back()
    {
        ShowStandardMenu();
    }

    // click the go button to load the play scene or editor with the selected saved beatmap
    public void Go()
    {
        if (play && GameManager.instance.saves.Count > 0)
        {
            GameManager.instance.SetSave(GameManager.instance.saves[selectedSave]);
            SceneManager.LoadScene("Play");
        }
        else if (GameManager.instance.saves.Count > 0)
        {
            GameManager.instance.SetSave(GameManager.instance.saves[selectedSave]);
            SceneManager.LoadScene("Editor");
        }
    }

    // create a new beatmap save from the edit menu
    public void CreateNew()
    {
        if (createNewIF.text != "")
        {
            GameManager.instance.SetSave(new Save(createNewIF.text, new List<Beat>(), selectedSong));
            SceneManager.LoadScene("Editor");
        }
    }

    // called when the user changes the save dropdown
    public void OnChangeDropdown()
    {
        selectedSave = saveDropdown.value;
    }

    // called when the user changes the song dropdown
    public void OnChangeSongDropdown()
    {
        selectedSong = songDropdown.value;
    }

    // show the play/edit/input menu
    public void ShowStandardMenu()
    {
        standardMenu.SetActive(true);
        optionsMenu.SetActive(false);
        inputMenu.SetActive(false);
    }

    // show the play/edit menu
    public void ShowOptionsMenu()
    {
        standardMenu.SetActive(false);
        optionsMenu.SetActive(true);
        inputMenu.SetActive(false);
    }

    // show the input menu
    public void ShowInputMenu()
    {
        standardMenu.SetActive(false);
        optionsMenu.SetActive(false);
        inputMenu.SetActive(true);
    }

    // show the create new beatmap option in the options menu
    public void ShowCreateNewOption(bool show)
    {
        createNewMenu.SetActive(show);
    }

    // click an input button in the input menu
    public void ClickInputButton(int button)
    {
        waitingForInput = button;
        ShowEnterInput(true);
    }

    // handles custom input by reading the input and setting it appropriately
    private void OnGUI()
    {
        Event keyEvent = Event.current;
        if (keyEvent.isKey && waitingForInput != -1)
        {
            GameManager.instance.inputs[waitingForInput] = keyEvent.keyCode;
            RefreshInputMenu();
            waitingForInput = -1;
            ShowEnterInput(false);
            GameManager.instance.SaveInputs();
        }
    }

    // refresh the text of buttons in the input menu
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

    // show the enter input text (and hide the input menu with it)
    private void ShowEnterInput(bool show)
    {
        inputMenu.SetActive(!show);
        enterYourInput.SetActive(show);
    } 
}
