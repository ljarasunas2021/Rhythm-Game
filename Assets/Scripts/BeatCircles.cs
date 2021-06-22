using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatCircles : MonoBehaviour
{
    public static BeatCircles instance;

    public GameObject beatCircle, cursor;

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

    public void AddBeatCircle(Beat beat)
    {
        GameObject newBeatCircle = Instantiate(beatCircle, transform);
        newBeatCircle.transform.position = cursor.transform.position + Vector3.up * 100;
        newBeatCircle.GetComponent<BeatCircle>().SetupBeat(beat);
    }
}
