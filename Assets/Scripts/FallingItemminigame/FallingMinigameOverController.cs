using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class FallingMinigameOverController : MonoBehaviour
{
    public Image winnerColor;
    public TextMeshProUGUI winnerPoints;
    public TextMeshProUGUI winnerName;
    public void Initialize()
    {
        gameObject.SetActive(false);
        //List<PlayerController> winners = new List<PlayerController>();
        List<FallingMinigameStats> winners = FallingGameManager.Instance.playersMinigameStats.OrderByDescending(o => o.points).ToList();
        List<PlayerController> winnersController = new List<PlayerController>();
        foreach(FallingMinigameStats winnerStats in winners)
        {
            winnersController.Add(GameManager.Instance.GetPlayer(winnerStats.playerId));
        }
        PlayerController winner = GameManager.Instance.GetPlayer(winners[0].playerId);
        GameManager.Instance.SetMiniGameWinners(winnersController);
        winnerName.text = $"{winner.playerStats.nickName}";
        winnerPoints.text = $"{winners[0].points}";
        winnerColor.color = winner.playerStats.mainColor.color;
        gameObject.SetActive(true);
    }
}
