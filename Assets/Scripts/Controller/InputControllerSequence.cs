using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InputControlSequence : ControlSequence
{
    public KeyCode[] JumpKeys = new KeyCode[]
    {
        KeyCode.Space,
        KeyCode.W,
        KeyCode.K
    };
    public KeyCode[] RunKey = new KeyCode[]
    {
        KeyCode.LeftShift,
        KeyCode.RightShift
    };

    public override ControlDetail GetControl()
    {
        var axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        var control = new ControlDetail() { Direction = axis.x, Tick = GameSystem.Instance.Tick };
        if (JumpKeys.Any(key => Input.GetKey(key)))
        {
            control.Action = PlayerAction.Jump;
        }
        else if (Mathf.Approximately(axis.x, 0))
        {
            control.Action = PlayerAction.Idle;
        }
        else if (RunKey.Any(key => Input.GetKey(key)))
        {
            control.Action = PlayerAction.Run;
        }
        else
        {
            control.Action = PlayerAction.Move;
        }
        return control;
    }
}