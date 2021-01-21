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
            Debug.Log($"CHECKING IF VISIBLE LEVELLOADER: {LevelLoader.Instance.visible}");
            LevelLoader.Instance.FadeIn();
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
