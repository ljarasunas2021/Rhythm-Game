using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speed : MonoBehaviour
{
    public static Speed instance;

    public InputField speedIF;

    [HideInInspector] public float currentSpeed = 1;

    // Start is called before the first frame update
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

    public void UpdateSpeedIF()
    {
        currentSpeed = float.Parse(speedIF.text);
    }
}
