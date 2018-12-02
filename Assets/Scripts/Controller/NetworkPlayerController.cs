using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MultiPlayer;

public class NetworkPlayerController : PlayerController
{
    public new PlayerSnapShot LastSnapshot;
    private void Update()
    {
        switch ((PlayerAction)LastSnapshot.Control.Action)
        {
            case PlayerAction.Move:
                Move(LastSnapshot.Control.Direction);
                break;
            case PlayerAction.Jump:
                if (!Jump(LastSnapshot.Control.Direction))
                    LastSnapshot.Control.Action = (int)PlayerAction.Move;
                break;
            case PlayerAction.Run:
                Run(LastSnapshot.Control.Direction);
                break;
            case PlayerAction.Idle:
                Move(0);
                break;
        }
    }
}