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
        newBeatCircle.transform.localPosition = GetBeatCircleCorrectLocalPos(beat);
        newBeatCircle.GetComponent<BeatCircle>().SetupBeat(beat);
    }

    // Adjust the position of the beat circles
    public void AdjustBeatCircles()
    {
        foreach (Transform beatCircle in transform)
        {
            BeatCircle comp = beatCircle.GetComponent<BeatCircle>();
            beatCircle.localPosition = GetBeatCircleCorrectLocalPos(comp.beat);
            comp.SetupBeat(comp.beat);            
        }
    }

    // Get the correct local position for a beat circle
    public Vector3 GetBeatCircleCorrectLocalPos(Beat beat)
    {
        return ((beat.beatAt / (Beats.instance.quarterBeatsPerSecond * GameManager.instance.audioClip.length)) - 0.5f) * Waveform.instance.waveformWidth * Vector3.right + Vector3.up * (Waveform.instance.cursor.transform.localPosition.y + 100);
    }
}
