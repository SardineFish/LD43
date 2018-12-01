using UnityEngine;
using System.Collections;
using System.Linq;

public class GameHost : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject PlaybackPlayerPrefab;
    public LocationMark SpawnLocation;
    // Use this for initialization
    void Start()
    {
        GameSystem.Instance.SpawnPlayer(PlayerPrefab, SpawnLocation);
        GameSystem.Instance.StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GameSystem.Instance.PlayerInControl.GetComponent<PlayerController>().ControlRecord.ForEach(action =>
            {
                Debug.Log($"{action.Tick} {action.Action}");
            });
            var playbacks = GameSystem.Instance.PlayBackPlayers
                .Select(player => (player.GetComponent<PlayerController>().ControlSequence as PlaybackControlSequence).controlSequence)
                .ToList();
            playbacks.Add(GameSystem.Instance.PlayerInControl.GetComponent<PlayerController>().ControlRecord.ToArray());
            GameSystem.Instance.EndGame();
            Utility.WaitForSecond(this, () =>
            {
                GameSystem.Instance.SpawnPlayer(PlayerPrefab, SpawnLocation);
                playbacks.ForEach(playback => GameSystem.Instance.SpawnPlayBackPlayer(PlaybackPlayerPrefab, SpawnLocation, playback));
                GameSystem.Instance.StartGame();
            }, 1);
        }
    }
}
