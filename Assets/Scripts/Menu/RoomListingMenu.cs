using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;

    [SerializeField]
    private RoomListingController _roomListing;

    private List<RoomListingController> _listings = new List<RoomListingController>();
    private List<RoomInfo> _cachedRoomList = new List<RoomInfo>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                int cachedRoomIndex = _cachedRoomList.FindIndex(x => x.Name == room.Name);

                if (cachedRoomIndex != -1)
                {
                    _cachedRoomList.RemoveAt(cachedRoomIndex);
                }
                int index = _listings.FindIndex(x => x.SavedRoomInfo.Name == room.Name);
                if (index != -1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
            }
            else
            {
                int cachedRoomIndex = _cachedRoomList.FindIndex(x => x.Name == room.Name);

                if (cachedRoomIndex != -1)
                {
                    _cachedRoomList[cachedRoomIndex] = room;
                }
                else
                {
                    _cachedRoomList.Add(room);
                }
            }
        }
        UpdateList();
    }
    public void UpdateList()
    {
        foreach (RoomInfo room in _cachedRoomList)
        {
            int index = _listings.FindIndex(x => x.SavedRoomInfo.Name == room.Name);
            if (index != -1)
            {
                _listings[index].SetRoomInfo(room);
            }
            else
            {
                RoomListingController listing = Instantiate(_roomListing, _content);
                if (listing != null)
                {
                    listing.SetRoomInfo(room);
                    _listings.Add(listing);
                }
            }
        }
    }
}
