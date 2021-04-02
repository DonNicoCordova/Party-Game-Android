using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LoginMenu : MonoBehaviourPunCallbacks
{
    [Header("Main Screen")]
    public Button startGameButton;

    private void Start()
    {
        if (LevelLoader.Instance != null && LevelLoader.Instance.visible)
        {
            LevelLoader.Instance.FadeIn();
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        if (PhotonNetwork.IsConnected)
        {
            startGameButton.interactable = true;
        }
    }
    public override void OnConnectedToMaster()
    {
        startGameButton.interactable = true;
    }
    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
        PlayerPrefs.SetString("PlayerName", playerNameInput.text);
    }
    public void OnStartGameButton()
    {
        StartCoroutine(LevelLoader.Instance.LoadLevel("MainMenu"));
    }
}
