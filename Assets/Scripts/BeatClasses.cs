using UnityEngine;

// the different types of beats
[System.Serializable]
public enum BeatType
{
    SinglePress, Hold, Scratch, DoublePress, None
}

// standard beat class
[System.Serializable]
public class Beat
{
    // on which beat should the user press the appropriate input
    public int beatAt;
    // what type of beat is this beat
    public BeatType beatType;
    // how fast should the beat move
    public float speed;
    // a reference to a gameobject or parent of a gameobject that moves for the beat (depends on the beat's type)
    public GameObject mover;
    // whether the beat has been destroyed or not
    public bool destroyed;

    // constructor
    public Beat(int beatAt, BeatType beatType, float speed, bool destroyed=false)
    {
        this.beatAt = beatAt;
        this.beatType = beatType;
        this.speed = speed;
        this.destroyed = destroyed;
    }
}

// beat class for a single press beat
[System.Serializable]
public class SinglePressBeat : Beat
{
    // which note the beat travels to
    public int note;

    // constructor
    public SinglePressBeat(int beatAt, float speed, int note) : base(beatAt, BeatType.SinglePress, speed)
    {
        this.note = note;
    }
}

// beat class for a hold beat
[System.Serializable]
public class HoldBeat : Beat
{
    // which note the beat travels to
    public int note;
    // the duration of the hold
    public float duration;
    // a stored accuracy of the initial press
    public float accuracy; 

    // constructor
    public HoldBeat(int beatAt, float speed, int note, float duration) : base(beatAt, BeatType.Hold, speed)
    {
        this.note = note;
        this.duration = duration;
        this.accuracy = 0;
    }
}

// beat class for a scratch beat
[System.Serializable]
public class ScratchBeat : Beat
{
    public ScratchBeat(int beatAt, float speed) : base(beatAt, BeatType.Scratch, speed)
    {

    }
}

// beat class for a double press beat
[System.Serializable]
public class DoublePressBeat : Beat
{
    // which 2 notes the beat travels to
    public int[] notes;
    // a stored accuracy for the first note pressed
    public float accuracy;
    // whether the firstNote (notes[0]) was pressed before (true) or after (false) the second note (notes[1])
    public bool firstNotePressed;

    // constructor
    public DoublePressBeat(int beatAt, float speed, int[] notes) : base(beatAt, BeatType.DoublePress, speed)
    {
        this.notes = notes;
        this.accuracy = 0;
        this.firstNotePressed = true;
    }
}