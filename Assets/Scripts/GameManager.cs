using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AudioClip[] audioClips;

    [HideInInspector] public string savedName;
    [HideInInspector] public List<Beat> beats;
    [HideInInspector] public AudioClip audioClip;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public JsonSerializerSettings settings;

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
}
