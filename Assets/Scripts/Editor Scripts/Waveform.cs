using UnityEngine;
using UnityEngine.UI;

// This class handles the waveform (bottom) in the editor
public class Waveform : MonoBehaviour
{
    // singleton reference
    public static Waveform instance;

    // image of the waveform and cursor (bar that moves along the waveform)
    public Image waveform, cursor;

    // the width of the waveform
    [HideInInspector] public float waveformWidth;

    // the audiosource that plays the music
    private AudioSource audioSource;

    // singleton implementation, initialize variables
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

    // draw the waveform
    public void DrawWaveForm(AudioClip audio)
    {
        Texture2D texture = PaintWaveformSpectrum(audio, .5f, 500, 100, Color.yellow);
        waveform.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    // obtain the texture for the waveform
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

    // update the cursor's position based on the audio source's time
    private void Update()
    {
        cursor.transform.position = waveform.transform.position + Vector3.right * waveformWidth * ((audioSource.time)/(audioSource.clip.length) - 0.5f);
    }
}
