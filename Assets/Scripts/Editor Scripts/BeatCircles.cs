using UnityEngine;

// This class handles the addition of beat circles
public class BeatCircles : MonoBehaviour
{
    //singleton reference
    public static BeatCircles instance;

    // beat circle prefab
    public GameObject beatCircle;

    // singleton implementation
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("An instance of Beat Circles already exists");
        }
    }

    // create a beat circle from its beat
    public void AddBeatCircle(Beat beat)
    {
        GameObject newBeatCircle = Instantiate(beatCircle, transform);                
        newBeatCircle.transform.localPosition = ((beat.beatAt / (Beats.instance.beatsPerSecond * GameManager.instance.audioClip.length)) - 0.5f) * Waveform.instance.waveformWidth * Vector3.right + Vector3.up * (Waveform.instance.cursor.transform.localPosition.y + 100);
        newBeatCircle.GetComponent<BeatCircle>().SetupBeat(beat);
    }
}
