using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public GameObject Loading;
    public Text InputName;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    bool started = false;
    public void ButtonStart()
    {
        if (!started)
        {
            started = true;
            Loading.SetActive(true);
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        var name = InputName.text == "" ? "Player" : InputName.text;
        yield return GameClient.Instance.JoinGame(name);
        GameSystem.Instance.StartGame();
        gameObject.SetActive(false);
        Loading.SetActive(false);
    }
}
