# Rhythm-Game

This README will contain a more general non-programming specific view at this Rhythm Game demo. View the comments in each script to more fully understand the programming specifics of this demo.

## Scenes

This demo has 4 scenes. Start the game by playing the loading scene.

### Loading Scene

The loading scene is an empty scene with the Game Manager (a DontDestroyOnLoad gameobject). The Game Manager stores important data that is necessary to be transported across all scenes. 

Key Fields and Methods in `GameManager.cs`:
```cs
// add audio clips to this public field to be able to create beat maps with that audio clip.
public AudioClip[] audioClips;

// a list of beats to be used in the editor and play scene
public List<Beat> beats;

// an array of inputs to be used for notes 
public KeyCode[] inputs;
    
// a list of the saved beatmaps
public List<Save> saves;

// whether the user is in the play scene
public bool InPlayScene() { ... }
```

Once the scene loads, the user is immediately transported to the home scene. 

### Home Scene

The home scene is a menu scene. Press on the edit button to create a new beatmap, where the user is able to specify the beatmap's name and song. The user is also able to edit existing beatmaps. If the user pressed the play button, the user is able to select which beatmap he/she would like to play. Finally, if the user pressed the "Change Inputs" button, he or she would be able to set custom inputs. 

Key Fields and Methods in `HomeUIManager.cs`:
```cs
// create a new beatmap save from the edit menu
public void CreateNew() { ... }

// handles custom input by reading the input and setting it appropriately
private void OnGUI()
```

If the user were to try to create a new beatmap or edit an existing beatmap, he/she would be taken to the editor scene.

### Editor Scene

In the middle of the screen, there are 6 notes in a circle formation. Each beats will start at the middle of that cirlce and travel to a note on the edge of that circle. On the bottom of the screen is a waveform of the beatmap's song, along with a cursor that runs along it and a play and pause button. When the user pauses the song, an add menu, speed menu, and beats menu will pop up. In the add menu, the user is able to add Single Press, Hold, Scratch, and Double Press beats. After the user adds one of these beats, a mini beat (beat circle) will appear on the bottom cursor. If a user clicks on this beat cicle, it will give the user the ability to change different properties of that beat. In the speed menu, the user is able to change the default speed of a beat (i.e. how fast the beat moves in game). If a user wants to change a single beat's speed, he/she can click on that beat circle and change the speed property of the beat. In the beats menu, the user is able to go to the next or previous full, half, or quarter beat. In the top right of the screen, the user is also able to save his/her beatmap or go back to the home scene. 

Key Fields and Methods in `EditorManager.cs`:
```cs
// the current beat type that the user has selected after adding a beat
[HideInInspector] public BeatType currentSelectedBeatType = BeatType.None;

// add a beat
public void AddBeat(Beat beat) { ... }

// delete a beat
public void DeleteBeat(Beat beat) { ... }

// further the process of creating a beat after the user has selected a note to go with that beat
public void SelectNote(int note) { ... }
```

Key Fields and Methods in `BeatCircle.cs`:
```cs
// the beat that this beat circle represents
public Beat beat;

// called when the beat is instantiated
public void SetupBeat(Beat beat) { ... }

// Delete the beat circle
public void Delete()

// Toggle the options menu
public void ToggleActive()
```

Key Fields and Methods in `BeatCircles.cs`:
```cs
// create a beat circle from its beat
public void AddBeatCircle(Beat beat) { ... }
```

Key Fields and Methods in `SaveHomeManager.cs`:
```cs
// save the current beat map
public void Save() { ... }
```

And scripts that aren't exclusive to the editor scene (also used in the play scene):

Key Fields and Methods in `Beats.cs`:
```cs
// how many (quarter) beats happen each second
public int beatsPerSecond = 4;

// the current beat that the song is at
public int currentBeat;

// the current beat amount (0 -> full beats, 1 -> half beats, 2 -> quarter beats)
private int currentBeatAmount = 0;

// set the current beat and adjust the audio source accordingly
public void SetCurrentBeat() { ... }

// adjust the audio source to start at the current beat
private void AdjustAudioSource() { ... }
```

Key Fields and Methods in `BeatsMover.cs`:
```cs
// the speed at which the beats move (increase/decrease this value to increase/decrease the speed of all of the beats)
public float beatSpeed = 1;

// delay to allow the user to press the appropriate input even after the beat has already reached its note
public float beatDelay = 0.5f;

// move all of the beats appropriately
private void Update() { ... }

// returns -1 if a beat shouldn't be visible yet, 0 if a beat should be visible and should be moving towards its note, and 1 if a beat should have already been destroyed
public int GetTiming(Beat beat) { ... }

// return the current time in seconds based on the song
public float GetCurrentTime() { ... }

// return the time in seconds at which the beat must be at the note
public float GetBeatTime(Beat beat) { ... }    
```

Once the user saves his/her beatmap, he/she can play that beatmap in the play scene.

### Play Scene

In the play scene, the user will see a similar circle formation as in the editor scene, as well as a home button in the top right. The beats will start to play, and the user will press the appropriate input when a beat gets close enough to a note. The notes are ordered 1-6 in a clockwise fashion, starting with 1 at the top right. The default inputs (changeable in the home scene) are 1 -> h, 2 -> j, 3 -> k, 4 -> d, 5-> f, 6 -> g, and any scratch can be answered using the space bar. Every input will be ranked as Perfect, Great, Good, or a Miss, on the screen.

Key Fields and Methods in `BeatsScorer.cs`:
```cs
// the next beat for each note ({next beats for note 1, next beats for note 2, , etc... })
private List<Beat> nextBeats = new List<Beat>();

// check for inputs and score the inputs appropriately
private void Update() { ... }

// returns true if the user presses the appropriate key for a beat at the appropriate time
public bool GetKey(Beat beat, bool down, bool useFirstDoublePressNote = true) { ... }

// returns true if the user presses the appropriate key for a scratch beat at the appropriate time
public bool GetScratchKeyDown(Beat beat)

// set up the nextBeats variable
private void SetNextBeats() { ... }

// returns the accuracy of a user's input based on how close the beat is to its corresponding note
public float GetAccuracy(Beat beat, bool useInitialPressForHoldBeat = true) { ... }
```

Key Fields and Methods in `PlayUIManager.cs`:
```cs
// thresholds to judge rankings (e.g. if an accuracy is < perfectGreatThreshold but > greatGoodThreshold, it will be judged as great), the time for the rating text to disappear    
public float perfectGreatThreshold, greatGoodThreshold, goodMissThreshold, ratingUITime;

// rate the timing of an input based on its accuracy
public void RateTiming(int note, float accuracy) { ... }
```

## Folders

This demo has 5 main folders.

### Audio

The audio folder houses the audio clips that make up the songs for the beat maps. 

### Data

The data folder houses the Inputs.json and Saves.json which store data about the custom inputs and custom beat maps, respectively. 

### Prefabs

The prefabs folder stores the prefabs for the project.

### Scenes

The scenes folder houses the 4 scenes.

### Scripts

The scripts folder stores the scripts for the project, breaking them down by their scene. 

