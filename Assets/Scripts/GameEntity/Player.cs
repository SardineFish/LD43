using UnityEngine;
using System.Collections;

public class Player : GameEntity
{
    public string Name;

    public override void Die()
    {
        base.Die();
        GUIManager.Instance.StartCoroutine(GUIManager.Instance.GameOver());
        
        //GameClient.Instance.SendRecord();
        //GameSystem.Instance.EndGame();
    }
}
