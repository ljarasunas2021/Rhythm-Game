using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEditor;

// This class is on a don't destroy on load gameobject to hold data needed throughout playing the game
public class GameManager : MonoBehaviour
{
    // singleton reference
    public static GameManager instance;

    // options for songs the user can play
    public AudioClipAndBPM[] audioClips;

    // name of the saved beat map
    [HideInInspector] public string savedName;
    // a list of beats to be used in the editor and play scene
    [HideInInspector] public List<Beat> beats;
    // the current audio clip playing
    [HideInInspector] public AudioClip audioClip;
    // the audio source that plays the audio clip
    [HideInInspector] public AudioSource audioSource;
    // JSON settings to allow subclasses to be serialized
    [HideInInspector] public JsonSerializerSettings settings;
    // an array of inputs to be used for notes ({input to be used for scratch beats, input to be used for Note 1s, input to be used for Note 2s, etc...)
    [HideInInspector] public KeyCode[] inputs;
    // a list of the saved beatmaps
    [HideInInspector] public List<Save> saves;
    // bpm of saved beat map
    [HideInInspector] public float bpm = 60;

    // singleton implementation, make gameobject don't destroy on load and initialize audioSource, inputs, settings, and saves
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("An instance of GameManager already exists.");
        }

        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        GetInputs();
        settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        GetSaves();
    }

    // load the home screen
    void Start()
    {
        SceneManager.LoadScene("Home");
    }

    // set the current save's variables
    public void SetSave(Save save)
    {
        savedName = save.name;
        beats = save.beats;
        audioClip = audioClips[save.audioClipIndex].audioClip;
        bpm = save.bpm;
    }

    // whether the user is in the play scene
    public bool InPlayScene()
    {
        return SceneManager.GetActiveScene().name == "Play";
    }

    // save the inputs to the inputs file
    public void SaveInputs()
    {
        TextAsset inputsFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Data/Inputs.json", typeof(TextAsset));
        Inputs inputsObj = new Inputs(inputs);
        string json = JsonConvert.SerializeObject(inputsObj, Formatting.Indented, settings);
        File.WriteAllText(AssetDatabase.GetAssetPath(inputsFile), json);
        EditorUtility.SetDirty(inputsFile);
    }

    // get the inputs from the inputs file
    public void GetInputs()
    {
        TextAsset inputsFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Data/Inputs.json", typeof(TextAsset));
        inputs = JsonConvert.DeserializeObject<Inputs>(inputsFile.text, settings).inputs;
    }

    // get the saves from the saves file
    public void GetSaves()
    {
        List<Save> newSaves = new List<Save>();
        
        foreach (FileInfo file in new DirectoryInfo("Assets/Data/Saves").GetFiles())
        {
            if (file.Extension != ".meta")
            {
                string text = System.IO.File.ReadAllText(file.ToString());
                Save save = JsonConvert.DeserializeObject<Save>(text, settings);
                newSaves.Add(save);
            }            
        }

        saves = newSaves;
    }
}

// Inputs class to store the inputs array (necessary for JSON serialization)
public class Inputs
{
    // inputs array
    public KeyCode[] inputs;

    // constructor
    public Inputs(KeyCode[] inputs)
    {
        this.inputs = inputs;
    }
}

// Audio Clip and BPM class to save an audio clip and its beats per minute
[System.Serializable]
public class AudioClipAndBPM
{
    // audio
    public AudioClip audioClip;
    // bpm
    public float bpm; 
}