using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlaybackControlSequence:ControlSequence
{
    public ControlDetail[] controlSequence;
    int idx = -1;
    public PlaybackControlSequence(ControlDetail[] controlSequence)
    {
        this.controlSequence = controlSequence;
    }

    public override ControlDetail GetControl()
    {
        if (idx + 1 >= controlSequence.Length)
        {
            return new ControlDetail()
            {
                Action = PlayerAction.Idle,
                Direction = 1,
                Tick = GameSystem.Instance.Tick
            };
        }
        if (controlSequence[idx + 1].Tick == GameSystem.Instance.Tick)
        {
            return controlSequence[++idx];
        }
        else if (idx < 0)
        {
            return new ControlDetail()
            {
                Action = PlayerAction.Idle,
                Direction = 1,
                Tick = GameSystem.Instance.Tick
            };
        }
        else
            return controlSequence[idx];
    }
}