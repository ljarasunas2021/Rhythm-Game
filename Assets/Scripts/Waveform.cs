using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waveform : MonoBehaviour
{
    public Image waveform, cursor;

    private AudioSource audioSource;
    private float waveformWidth;

    private void Start()
    {
        waveformWidth = waveform.GetComponent<RectTransform>().sizeDelta.x;
        audioSource = EditorManager.instance.audioSource;
    }

    private void Update()
    {
        cursor.transform.position = waveform.transform.position + Vector3.right * waveformWidth * ((audioSource.time)/(audioSource.clip.length) - 0.5f);
    }
}
