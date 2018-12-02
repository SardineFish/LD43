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
    public string url = "http://localhost:9343";
    public string PlayerName;
    public string PlayerID;
    public string RoomID;
    public bool Ready = false;
    public Dictionary<string, Player> Players = new Dictionary<string, Player>();
    public Player PlayerInControl;

    public GameObject PlayerPrefab;
    public GameObject PlaybackPlayerPrefab;
    public LocationMark SpawnLocation;

    WebSocket webSocket;
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
        var handshake = new Message()
        {
            Type = MessageType.HandShake,
            Body = new ClientHandShakeMessage()
            {
                Type = HandShakeType.Join,
                Name = PlayerName,
            }
        };
        webSocket.SendString(JsonConvert.SerializeObject(handshake));
        while (true)
        {
            var recv = webSocket.RecvString();
            if (recv == null)
                yield return null;
            else
            {
                var response = (JsonConvert.DeserializeObject<Message>(webSocket.RecvString()).Body as JObject).Annotation<ServerHandShakeMessage>();
                PlayerID = response.ID;
                RoomID = response.RoomID;
                Ready = true;
                break;
            }
        }

        // Get records
        var request = new WWW($"{url}/get-records?room={RoomID}", new byte[0]);
        yield return request;
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

        // Spawn player
        PlayerInControl = GameSystem.Instance.SpawnPlayer(PlayerPrefab, SpawnLocation);

        while (true)
        {
            var recv = webSocket.RecvString();
            if (recv == null)
                yield return null;
            else
            {
                var sync = (JsonConvert.DeserializeObject<Message>(webSocket.RecvString()).Body as JObject).Annotation<SyncMessage>();
            }
        }
    }

    IEnumerator GameLoopCoroutine()
    {
        GameSystem.Instance.StartGame();
        while (true)
        {
            string recv;
            while((recv=webSocket.RecvString()) == null)
            {
                yield return null;
            }
            var sync = (JsonConvert.DeserializeObject<Message>(webSocket.RecvString()).Body as JObject).Annotation<SyncMessage>();
            

            yield return null;
        }
    }

    IEnumerator SendRecord(PlayerRecord record)
    {
        var request = new WWW($"{url}/record?id={PlayerID}", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(record)));
        yield return request;
    }

    public void JoinGame(string name)
    {
        this.PlayerName = name;
        webSocket = new WebSocket(new Uri($"{url}/ws"));
        StartCoroutine(ConnectCoroutine());
    }
}
