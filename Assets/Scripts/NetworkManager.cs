using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public static NetworkManager instance;
    void Awake()
    {
        if(instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
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
    public void CreateRoom(string roomName, int maxPlayers)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) maxPlayers;
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    [PunRPC]
    public void ChangeScene(string SceneName)
    {
        PhotonNetwork.LoadLevel(SceneName);
    }
}
