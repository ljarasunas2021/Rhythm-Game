using UnityEngine;
using UnityEngine.UI;

// This class manages the key components of the UI in the editor
public class EditorUIManager : MonoBehaviour
{
    // singleton reference
    public static EditorUIManager instance;

    // texts to force user to select 1 or 2 notes
    public Text selectNote, selectTwoNotes;
    // UI menus
    public GameObject addMenu, musicButtonMenu, beatsMenu, speedMenu, notesMenu, holdInput;
    // input field to ask user for how long the beat should be held for
    public InputField holdInputField;

    // singleton implementation
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

    // show the add menu
    public void ShowAddMenu(bool show)
    {
        addMenu.gameObject.SetActive(show);
    }

    // show the music button menu
    public void ShowMusicButtonMenu(bool show)
    {
        musicButtonMenu.gameObject.SetActive(show);
    }

    // show the beats menu
    public void ShowBeatsMenu(bool show)
    {
        beatsMenu.gameObject.SetActive(show);
    }

    // show the speed menu
    public void ShowSpeedMenu(bool show)
    {
        speedMenu.gameObject.SetActive(show);
    }

    // show the the select note text
    public void ShowSelectNote(bool show)
    {
        selectNote.gameObject.SetActive(show);
    }

    // show the select two notes text
    public void ShowSelectTwoNotes(bool show)
    {
        selectTwoNotes.gameObject.SetActive(show);
    }

    // show the hold input text
    public void ShowHoldInput(bool show)
    {
        holdInput.gameObject.SetActive(show);
    }

    // called after the user has selected his/her first note for a double pressed beat to highlight that note
    public bool HighlightNote(int note)
    {
        Button button = notesMenu.transform.GetChild(note - 1).GetComponent<Button>();
        button.interactable = !button.interactable;
        return !button.interactable;
    }

    // unhighlight any highlighted notes
    public void ResetHighlightedNotes()
    {
        foreach(Transform child in notesMenu.transform)
        {
            child.GetComponent<Button>().interactable = true;
        }
    }

    // called after the user types in the hold duration that he/she wants a beat to have
    public void FinishTypingHoldDuration()
    {
        EditorManager.instance.FinishTypingHoldDuration(float.Parse(holdInputField.text));
    }    
}
