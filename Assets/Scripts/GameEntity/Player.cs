using UnityEngine;
using System.Collections;

public class Player : GameEntity
{
    public string Name;

    public override void Die()
    {
        base.Die();
        GameClient.Instance.SendRecord();
        //GameSystem.Instance.EndGame();
    }
}
