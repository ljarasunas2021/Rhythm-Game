using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waveform : MonoBehaviour
{
    public static Waveform instance;

    public Image waveform, cursor;

    [HideInInspector] public float waveformWidth;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("An instance of Waveform already exists");
        }

        waveformWidth = waveform.GetComponent<RectTransform>().sizeDelta.x;
        audioSource = GameManager.instance.audioSource;
    }

    private void Update()
    {
        cursor.transform.position = waveform.transform.position + Vector3.right * waveformWidth * ((audioSource.time)/(audioSource.clip.length) - 0.5f);
    }
}
