using UnityEngine;
using UnityEngine.SceneManagement;

// This class handles the UI in the play scene
public class PlayUIManager : MonoBehaviour
{
    // singleton reference
    public static PlayUIManager instance;

    // texts to show the rating of the user's timing ({perfect, great, good, miss})
    public GameObject[] ratingTexts;

    // thresholds to judge rankings (e.g. if an accuracy is < perfectGreatThreshold but > greatGoodThreshold, it will be judged as great), the time for the rating text to disappear    
    public float perfectGreatThreshold, greatGoodThreshold, goodMissThreshold, ratingUITime;

    // parent of the notes
    private GameObject notesParent;
    // the audio source that the music comes from
    private AudioSource audioSource;

    // singleton implementation 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("An instance of PlayUIManager already exists.");
        }
    }

    //setup the audio to play, set the notesParent
    private void Start()
    {            
        audioSource = GameManager.instance.audioSource;
        audioSource.clip = GameManager.instance.audioClip;
        audioSource.Play();

        notesParent = BeatsMover.instance.notesParent.gameObject;
    }

    // go to the home scene
    public void Home() {
        SceneManager.LoadScene("Home");
        GameManager.instance.audioSource.Stop();
    }

    // rate the timing
    public void RateTiming(int note, float accuracy)
    {
        // find the timing rating
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

        // find what gameobject the rating should be instantiated as a child of
        Transform parent = transform;
        if (note > 0)
        {
            parent = notesParent.transform.GetChild(note - 1);
        }

        // instantiate the ranking and destroy it in ratingUITime
        Destroy(Instantiate(ratingTexts[(int)rating], parent), ratingUITime);
    }

}

// how good a timing is
public enum TimingRating
{
    Perfect, Great, Good, Miss
}
