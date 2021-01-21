using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
public class RoomListingController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text roomName;
    [SerializeField]
    private TMP_Text playersCount;

    public RoomInfo SavedRoomInfo { get; private set; }
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        SavedRoomInfo = roomInfo;
        roomName.text = roomInfo.Name;
        playersCount.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";
    }

    public void OnJoinRoomButton()
    {
        PhotonNetwork.JoinRoom(SavedRoomInfo.Name);
    }
}
