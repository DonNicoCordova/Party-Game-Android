using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class RoomNameInput : MonoBehaviour
{
    public Button createRoomButton;
    public GameObject message;
    public void OnRoomNameUpdate(TMP_InputField roomNameInput)
    {
        if (roomNameInput.text.Length >= 4 && roomNameInput.text.Length <= 40)
        {
            createRoomButton.interactable = true;
        }
        else
        {
            createRoomButton.interactable = false;
        }
    }
}
