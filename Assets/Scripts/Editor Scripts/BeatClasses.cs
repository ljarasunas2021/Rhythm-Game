using UnityEngine;

[System.Serializable]
public enum BeatType
{
    SinglePress, Hold, Scratch, DoublePress, None
}

[System.Serializable]
public class Beat
{
    public int beatAt;
    public BeatType beatType;
    public float speed;
    public GameObject mover;

    public Beat(int beatAt, BeatType beatType, float speed)
    {
        this.beatAt = beatAt;
        this.beatType = beatType;
        this.speed = speed;
    }
}

[System.Serializable]
public class SinglePressBeat : Beat
{
    public int note;

    public SinglePressBeat(int beatAt, float speed, int note) : base(beatAt, BeatType.SinglePress, speed)
    {
        this.note = note;
    }
}

[System.Serializable]
public class HoldBeat : Beat
{
    public int note;
    public float duration;

    public HoldBeat(int beatAt, float speed, int note, float duration) : base(beatAt, BeatType.Hold, speed)
    {
        this.note = note;
        this.duration = duration;
    }
}

[System.Serializable]
public class ScratchBeat : Beat
{
    public ScratchBeat(int beatAt, float speed) : base(beatAt, BeatType.Scratch, speed)
    {

    }
}

[System.Serializable]
public class DoublePressBeat : Beat
{
    public int[] notes;

    public DoublePressBeat(int beatAt, float speed, int[] notes) : base(beatAt, BeatType.DoublePress, speed)
    {
        this.notes = notes;
    }
}