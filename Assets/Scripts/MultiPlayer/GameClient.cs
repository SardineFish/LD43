using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;
using MultiPlayer;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameClient : MonoBehaviour
{
    public string host = "localhost:9343";
    public string PlayerName;
    public string PlayerID;
    public string RoomID;
    public bool Ready = false;
    public Dictionary<string, Player> Players = new Dictionary<string, Player>();
    public Player PlayerInControl;

    public GameObject PlayerPrefab;
    public GameObject PlaybackPlayerPrefab;
    public LocationMark SpawnLocation;

    WebSocketClient webSocket;
    // Use this for initialization
    void Start()
    {

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
            var snapShots = GameSystem.Instance.PlayerInControl.GetComponent<PlayerController>().ControlRecord
                .Select(playback => new PlayerSnapShot()
                {
                    ID = PlayerID,
                    Tick = playback.Tick,
                    Control = new PlayerControl()
                    {
                        Action = (int)playback.Action,
                        direction = playback.Direction
                    },
                    Position = 0,
                    Velocity = 0
                })
                .ToArray();
            var record = new PlayerRecord()
            {
                ID = PlayerID,
                Name = PlayerName,
                LeaveMessage = "",
                Records = snapShots
            };
            StartCoroutine(SendRecord(record));

            GameSystem.Instance.EndGame();
        }
    }

    IEnumerator ConnectCoroutine()
    {
        yield return webSocket.Connect();
        Debug.Log("Connected");
        var handshake = new Message()
        {
            Type = MessageType.HandShake,
            Body = new ClientHandShakeMessage()
            {
                Type = HandShakeType.Join,
                Name = PlayerName,
            }
        };
        
        webSocket.SendMessage(JsonConvert.SerializeObject(handshake));
        Debug.Log("Handshake sent");
        yield return webSocket.WaitMessage();

        Debug.Log(webSocket.Data);
        var response = (JsonConvert.DeserializeObject<Message>(webSocket.Data).Body as JObject).ToObject<ServerHandShakeMessage>();
        PlayerID = response.ID;
        RoomID = response.RoomID;
        Ready = true;

        // Get records
        var request = new WWW($"http://{host}/get-records/{RoomID}", new byte[] { 0 });
        yield return request;
        Debug.Log(request.text);
        var records = JsonConvert.DeserializeObject<PlayerRecord[]>(request.text);
        foreach(var record in records)
        {
            var player = GameSystem.Instance.SpawnPlayBackPlayer(PlaybackPlayerPrefab, SpawnLocation, record.Records.Select(r => new ControlDetail()
            {
                Action = (PlayerAction)r.Control.Action,
                Tick = r.Tick,
                Direction = r.Control.direction
            }).ToArray());
            Players[record.ID] = player;
        }

        yield return webSocket.WaitMessage();
        var sync = (JsonConvert.DeserializeObject<Message>(webSocket.Data).Body as JObject).ToObject<SyncMessage>();
        StartCoroutine(GameLoopCoroutine());
    }

    IEnumerator GameLoopCoroutine()
    {
        // Spawn player
        PlayerInControl = GameSystem.Instance.SpawnPlayer(PlayerPrefab, SpawnLocation);
        GameSystem.Instance.StartGame();
        while (true)
        {
            webSocket.WaitMessage();
            var sync = (JsonConvert.DeserializeObject<Message>(webSocket.Data).Body as JObject).ToObject<SyncMessage>();

            yield return null;
        }
    }

    IEnumerator SendRecord(PlayerRecord record)
    {
        var header = new Dictionary<string, string>();
        header["Content-Type"] = "application/json";
        var request = new WWW($"http://{host}/record/{PlayerID}", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(record)), header);
        yield return request;
    }

    public void JoinGame(string name)
    {
        this.PlayerName = name;
        webSocket = new WebSocketClient($"ws://{host}/ws");
        StartCoroutine(ConnectCoroutine());
    }
}
