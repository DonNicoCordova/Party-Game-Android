using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public static NetworkManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("CONNECTED TO PHOTON MASTER");
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("ROOM CREATED");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("ROOM JOINED");
    }
    public void CreateRoom(string roomName, int maxPlayers, bool privateRoom)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) maxPlayers;
        roomOptions.IsVisible = !privateRoom;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LevelLoader.Instance.LoadLevel(sceneName));
    }

}
