using UnityEngine;
using UnityEngine.UI;

// This class handles the speed menu in the editor
public class Speed : MonoBehaviour
{
    // singleton reference
    public static Speed instance;

    // speed input field (top left)
    public InputField speedIF;

    // current speed in the speedIF
    [HideInInspector] public float currentSpeed = 1;

    // singleton implementation
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("An instance of Speed already exists.");
        }
    }

    // change the speed's input field
    public void UpdateSpeedIF()
    {
        currentSpeed = float.Parse(speedIF.text);
    }
}
