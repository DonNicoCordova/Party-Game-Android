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
        List<PlayerController> winners = GameManager.Instance.players.OrderBy(o => o.playerStats.ladderPosition).ToList();
        firstNicknameText.text = $"1º {winners[0].playerStats.nickName}";
        firstZonesCapturedText.text = $"{winners[0].playerStats.capturedZones.Count} zonas";
        if (winners.Count >= 2)
        {
            secondNicknameText.text = $"2º {winners[1].playerStats.nickName}";
            secondZonesCapturedText.text = $"{winners[1].playerStats.capturedZones.Count} zonas";
        }
        if (winners.Count >= 3)
        {
            thirdNicknameText.text = $"3º {winners[2].playerStats.nickName}";
            thirdZonesCapturedText.text = $"{winners[2].playerStats.capturedZones.Count} zonas";
        } else if (thirdZonesCapturedText.gameObject.activeSelf)
        {
            thirdNicknameText.gameObject.SetActive(false);
            thirdZonesCapturedText.gameObject.SetActive(false);
        }
            gameObject.SetActive(true);

        replayButton.interactable = true;
    }

    public void OnBackToMenuClick()
    {
        NetworkManager.Instance.LeaveRoom();
        NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, "MainMenu");
    }
    public void OnReplayClick()
    {
        NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, "GameScene");
    }
}
