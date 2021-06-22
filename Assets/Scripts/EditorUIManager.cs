using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorUIManager : MonoBehaviour
{
    public static EditorUIManager instance;

    public Text selectNote, selectTwoNotes;
    public GameObject addMenu, musicButtonMenu, beatsMenu, speedMenu, notesMenu, holdInput;
    public InputField holdInputField;
    public Image waveform;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Instance of the EditorManager already exists.");
        }
    }

    public void ShowAddMenu(bool show)
    {
        addMenu.gameObject.SetActive(show);
    }

    public void ShowMusicButtonMenu(bool show)
    {
        musicButtonMenu.gameObject.SetActive(show);
    }

    public void ShowBeatsMenu(bool show)
    {
        beatsMenu.gameObject.SetActive(show);
    }

    public void ShowSpeedMenu(bool show)
    {
        speedMenu.gameObject.SetActive(show);
    }

    public void ShowSelectNote(bool show)
    {
        selectNote.gameObject.SetActive(show);
    }

    public void ShowSelectTwoNotes(bool show)
    {
        selectTwoNotes.gameObject.SetActive(show);
    }

    public void ShowHoldInput(bool show)
    {
        holdInput.gameObject.SetActive(show);
    }

    public bool HighlightNote(int note)
    {
        Button button = notesMenu.transform.GetChild(note - 1).GetComponent<Button>();
        button.interactable = !button.interactable;
        return !button.interactable;
    }

    public void ResetHighlightedNotes()
    {
        foreach(Transform child in notesMenu.transform)
        {
            child.GetComponent<Button>().interactable = true;
        }
    }

    public void FinishTypingHoldDuration()
    {
        EditorManager.instance.FinishTypingHoldDuration(float.Parse(holdInputField.text));
    }

    public void DrawWaveForm(AudioClip audio)
    {
        Texture2D texture = PaintWaveformSpectrum(audio, .5f, 500, 100, Color.yellow);
        waveform.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public Texture2D PaintWaveformSpectrum(AudioClip audio, float saturation, int width, int height, Color col)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] samples = new float[audio.samples];
        float[] waveform = new float[width];
        audio.GetData(samples, 0);
        int packSize = (audio.samples / width) + 1;
        int s = 0;
        for (int i = 0; i < audio.samples; i += packSize)
        {
            waveform[s] = Mathf.Abs(samples[i]);
            s++;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, Color.black);
            }
        }

        for (int x = 0; x < waveform.Length; x++)
        {
            for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
            {
                tex.SetPixel(x, (height / 2) + y, col);
                tex.SetPixel(x, (height / 2) - y, col);
            }
        }
        tex.Apply();

        return tex;
    }
}
