using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject roomSelect;
    public GameObject lobbyList;
    public GameObject chooseWhatToDo;
    public GameObject createRoom;

    [Header("Room Select")]
    public Button createRoomButton;
    public Button joinRoomButton;
    public PlayersSelectController playerSelectController;
    public Toggle privateToggle;

    [Header("Lobby List")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI playersCountText;
    public TextMeshProUGUI roomNameText;
    public Button launchGameButton;

    private void Awake()
    {
        LevelLoader.Instance.FadeIn();
        Destroy(GameObject.Find("GameManager"));
        if (PhotonNetwork.IsConnected)
        {
            createRoomButton.interactable = true;
            joinRoomButton.interactable = true;
        }
    }
    private void Start()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }
    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {

        SetScreen(lobbyList);
        if (PhotonNetwork.IsConnected)
        {
            createRoomButton.interactable = true;
            joinRoomButton.interactable = true;
        }
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
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
        if (roomSelect.activeSelf)
            roomSelect.SetActive(false);
        if (lobbyList.activeSelf)
            lobbyList.SetActive(false);
        if (chooseWhatToDo.activeSelf)
            chooseWhatToDo.SetActive(false);
        if (createRoom.activeSelf)
            createRoom.SetActive(false);

        screen.SetActive(true);
    }

    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.Instance.CreateRoom(roomNameInput.text, playerSelectController.GetPlayers(), privateToggle.isOn);
    }

    public void OnJoinRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.Instance.JoinRoom(roomNameInput.text);
    }

    public void OnLeaveRoomButton()
    {
        NetworkManager.Instance.LeaveRoom();
        SetScreen(chooseWhatToDo);
    }
    public void OnJoinListedRoom(TMP_InputField roomNameInput)
    {
        NetworkManager.Instance.JoinRoom(roomNameInput.text);
    }
    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }
    public void OnBackToChoose()
    {
        SetScreen(chooseWhatToDo);
    }
    public void OnChoosePartyButton()
    {
        SetScreen(roomSelect);
        if (PhotonNetwork.IsConnected)
        {
            createRoomButton.interactable = true;
            joinRoomButton.interactable = true;
        }
    }
    public void OnChooseCreateButton()
    {
        SetScreen(createRoom); 
        if (PhotonNetwork.IsConnected)
        {
            createRoomButton.interactable = true;
            joinRoomButton.interactable = true;
        }
    }
    public void OnReturnToMainMenu()
    {
        StartCoroutine(LevelLoader.Instance.LoadLevel("Login"));
    }
    public void OnLaunchGameButton()
    {
        NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, "GameBoardScene");
    }
}
