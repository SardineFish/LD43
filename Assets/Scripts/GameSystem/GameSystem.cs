using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSystem : Singleton<GameSystem>
{
    public Player PlayerInControl;
    public List<Player> PlayBackPlayers = new List<Player>();
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

    public void StartGame(int startTick = 0)
    {
        Tick = startTick;
        GameStarted = true;
    }
    public void EndGame()
    {
        GameStarted = false;
        Destroy(PlayerInControl.gameObject);
        PlayBackPlayers.ForEach(player => Destroy(player.gameObject));
        PlayBackPlayers = new List<Player>();
    }

    public Player SpawnPlayer(GameObject prefab, LocationMark location)
    {
        var player = Instantiate(prefab).GetComponent<Player>();
        player.GetComponent<PlayerController>().ControlSequence = new InputControlSequence();
        player.transform.position = location.transform.position;
        PlayerInControl = player;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().followTarget = player.transform;
        return player;
    }

    public Player SpawnPlayBackPlayer(GameObject prefab,LocationMark location, ControlDetail[] controlSequence)
    {
        var player = Instantiate(prefab).GetComponent<Player>();
        player.GetComponent<PlayerController>().ControlSequence = new PlaybackControlSequence(controlSequence);
        player.transform.position = location.transform.position;
        PlayBackPlayers.Add(player);
        return player;
    }
}
