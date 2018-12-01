using UnityEngine;
using System.Collections;

public class GameSystem : Singleton<GameSystem>
{
    public GameEntity PlayerInControl;
    public bool GameStarted = false;
    public int Tick;
    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (GameStarted)
            Tick += 1;
    }

    public void StartGame()
    {
        PlayerInControl.GetComponent<PlayerController>().ControlSequence = new InputControlSequence();
    }
}
