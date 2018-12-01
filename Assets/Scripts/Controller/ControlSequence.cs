using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public abstract class ControlSequence
{
    public abstract ControlDetail GetControl();
}

public enum PlayerAction : byte
{
    Move = 1,
    Idle = 0,
    Jump = 2,
    Run = 3,
}
[Serializable]
public class ControlDetail
{
    public PlayerAction Action;
    public float Direction;
    public int Tick;

    public bool IsSame(ControlDetail control)
    {
        return control.Action == Action && control.Direction == Direction;
    }
}

