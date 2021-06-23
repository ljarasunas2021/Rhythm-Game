using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayUIManager : MonoBehaviour
{
    public static PlayUIManager instance;

    public GameObject[] ratingTexts;
    public GameObject notesParent;

    public float perfectGreatThreshold, greatGoodThreshold, goodMissThreshold, ratingUITime;

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
        GameManager.instance.audioSource.Stop();
    }

    public void RateTiming(int note, float accuracy)
    {
        TimingRating rating = TimingRating.Miss;
        if (accuracy > perfectGreatThreshold)
        {
            rating = TimingRating.Perfect;
        }
        else if (accuracy > greatGoodThreshold)
        {
            rating = TimingRating.Great;
        }
        else if (accuracy > goodMissThreshold)
        {
            rating = TimingRating.Good;
        }

        Transform parent = transform;
        if (note > 0)
        {
            parent = notesParent.transform.GetChild(note - 1);
        }
        
        Destroy(Instantiate(ratingTexts[(int)rating], parent), ratingUITime);
    }

}

public enum TimingRating
{
    Perfect, Great, Good, Miss
}
