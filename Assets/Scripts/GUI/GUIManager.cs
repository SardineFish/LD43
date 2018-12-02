using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIManager : Singleton<GUIManager>
{
    public GameObject Loading;
    public Text InputName;
    public GameObject MainUI;
    public GameObject GameOverUI;
    public GameObject Background;
    // Use this for initialization
    void Start()
    {
        Background.SetActive(true);
        GameOverUI.SetActive(false);
        MainUI.SetActive(true);
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
        MainUI.SetActive(false);
        Loading.SetActive(false);
        yield return HideUI(Background);
    }

    public IEnumerator UIDisplay(GameObject ui)
    {
        ui.SetActive(true);
        var color = ui.GetComponent<Image>().color;
        color.a = 0;
        ui.GetComponent<Image>().color = color;
        yield return Utility.NumericAnimateEnumerator(1, (t) =>
        {
            color = ui.GetComponent<Image>().color;
            color.a = t;
            ui.GetComponent<Image>().color = color;

        }, () =>
        {
            color = ui.GetComponent<Image>().color;
            color.a = 1;
            ui.GetComponent<Image>().color = color;
        });
    }

    public IEnumerator HideUI(GameObject ui)
    {
        yield return Utility.NumericAnimateEnumerator(1, (t) =>
        {
            var color = ui.GetComponent<Image>().color;
            color.a = 1 - t;
            ui.GetComponent<Image>().color = color;

        }, () =>
        {
            var color = ui.GetComponent<Image>().color;
            color.a = 0;
            ui.GetComponent<Image>().color = color;
        });
        ui.SetActive(false);
    }

    public IEnumerator GameOver()
    {
        yield return UIDisplay(Background);
        GameOverUI.SetActive(true);
    }

    public IEnumerator UploadCoroutine()
    {
        Loading.SetActive(true);
        yield return GameClient.Instance.SendRecord();
        Loading.SetActive(false);
        GameOverUI.SetActive(false);
        yield return new WaitForSeconds(1);
        MainUI.SetActive(true);
        started = false;
    }

    public void Upload()
    {
        StartCoroutine(UploadCoroutine());
    }
}
