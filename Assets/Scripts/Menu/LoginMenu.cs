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
    public GameObject message;
    private void Start()
    {
        if (LevelLoader.Instance != null && LevelLoader.Instance.visible)
        {
            LevelLoader.Instance.FadeIn();
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;

    }
    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        if (playerNameInput.text.Length >= 4 && playerNameInput.text.Length <= 40)
        {
            PhotonNetwork.NickName = playerNameInput.text;
            PlayerPrefs.SetString("PlayerName", playerNameInput.text);
            startGameButton.interactable = true;
            message.SetActive(false);
        } else
        {
            message.SetActive(true);
            startGameButton.interactable = false;
        }
    }
    public void OnStartGameButton()
    {
        StartCoroutine(LevelLoader.Instance.LoadLevel("MainMenu"));
    }
}
