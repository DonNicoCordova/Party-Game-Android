using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public TextMeshProUGUI firstNicknameText;
    public TextMeshProUGUI secondNicknameText;
    public TextMeshProUGUI thirdNicknameText;
    public TextMeshProUGUI firstZonesCapturedText;
    public TextMeshProUGUI secondZonesCapturedText;
    public TextMeshProUGUI thirdZonesCapturedText;
    public Button replayButton;
    public void Initialize()
    {
        gameObject.SetActive(false);
        //List<PlayerController> winners = new List<PlayerController>();
        List<PlayerController> winners = GameManager.instance.players.OrderBy(o => o.playerStats.ladderPosition).ToList();
        firstNicknameText.text = $"1º {winners[0].playerStats.nickName}";
        secondNicknameText.text = $"2º {winners[1].playerStats.nickName}";
        thirdNicknameText.text = $"3º {winners[2].playerStats.nickName}";
        firstZonesCapturedText.text = $"{winners[0].playerStats.capturedZones.Count} zonas";
        secondZonesCapturedText.text = $"{winners[1].playerStats.capturedZones.Count} zonas";
        thirdZonesCapturedText.text = $"{winners[2].playerStats.capturedZones.Count} zonas";
        gameObject.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
            replayButton.interactable = true;
        else
            replayButton.interactable = false;
    }

    public void OnBackToMenuClick()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "MainMenu");
    }
    public void OnReplayClick()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "GameScene");
    }
}
