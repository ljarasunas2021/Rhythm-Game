using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayUIManager : MonoBehaviour
{
    public static PlayUIManager instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("An instance of PlayUIManager already exists.");
        }

        audioSource = GameManager.instance.audioSource;
        audioSource.clip = GameManager.instance.audioClip;
        audioSource.Play();
    }

    public void Home() {
        SceneManager.LoadScene("Home");
    }

}
