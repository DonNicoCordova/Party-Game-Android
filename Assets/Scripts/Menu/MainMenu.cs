using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public LevelLoader levelLoader;

    [Header("Screens")]
    public GameObject mainMenu;
    public GameObject roomSelect;
    public GameObject lobbyList;

    [Header("Main Screen")]
    public Button startGameButton;

    [Header("Room Select")]
    public Button createRoomButton;
    public Button joinRoomButton;
    public PlayersSelectController playerSelectController;

    [Header("Lobby List")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI playersCountText;
    public TextMeshProUGUI roomNameText;
    public Button launchGameButton;

    private void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
        startGameButton.interactable = false;
    }
    public void PlayGame()
    {
        levelLoader.LoadNextLevel();
    }
    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
        startGameButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyList);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"{returnCode} ERROR CONNECTING: {message}");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        Debug.Log("UPDATING LOBBY UI");
        playerListText.text = "";
        playersCountText.text = $"{PhotonNetwork.CurrentRoom?.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        roomNameText.text = PhotonNetwork.CurrentRoom?.Name;
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        //only the host can start the game
        if (PhotonNetwork.IsMasterClient)
            launchGameButton.interactable = true;
        else
            launchGameButton.interactable = false;

    }
    void SetScreen(GameObject screen)
    {
        mainMenu.SetActive(false);
        roomSelect.SetActive(false);
        lobbyList.SetActive(false);

        screen.SetActive(true);
    }

    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text, playerSelectController.GetPlayers());
    }

    public void OnJoinRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(roomSelect);
    }

    public void OnStartGameButton()
    {
        SetScreen(roomSelect);
    }
    public void OnReturnToMainMenu()
    {
        SetScreen(mainMenu);
    }
    public void OnLaunchGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "GameScene");
    }
}
