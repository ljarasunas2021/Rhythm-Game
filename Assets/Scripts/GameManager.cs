using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AudioClip[] audioClips;

    [HideInInspector] public string savedName;
    [HideInInspector] public List<Beat> beats;
    [HideInInspector] public AudioClip audioClip;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public JsonSerializerSettings settings;
    [HideInInspector] public KeyCode[] inputs;

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
    }

    void Start()
    {
        SceneManager.LoadScene("Home");
    }

    public void SetSave(Save save)
    {
        savedName = save.name;
        beats = save.beats;
        audioClip = audioClips[save.audioClipIndex];
    }

    public bool InPlayScene()
    {
        return SceneManager.GetActiveScene().name == "Play";
    }

    public void SaveInputs()
    {
        TextAsset inputsFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Data/Inputs.json", typeof(TextAsset));
        Inputs inputsObj = new Inputs(inputs);
        string json = JsonConvert.SerializeObject(inputsObj, Formatting.Indented, settings);
        File.WriteAllText(AssetDatabase.GetAssetPath(inputsFile), json);
        EditorUtility.SetDirty(inputsFile);
    }

    public void GetInputs()
    {
        TextAsset inputsFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Data/Inputs.json", typeof(TextAsset));
        inputs = JsonConvert.DeserializeObject<Inputs>(inputsFile.text, settings).inputs;
    }
}

public class Inputs
{
    public KeyCode[] inputs;

    public Inputs(KeyCode[] inputs)
    {
        this.inputs = inputs;
    }
}