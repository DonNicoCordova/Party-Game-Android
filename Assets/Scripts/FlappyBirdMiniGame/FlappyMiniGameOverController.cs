using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class FlappyMiniGameOverController : MonoBehaviour
{
    public Image winnerColor;
    public TextMeshProUGUI winnerPoints;
    public TextMeshProUGUI winnerName;
    public void Initialize()
    {
        gameObject.SetActive(false);
        List<FlappyRoyaleStats> winners = FlappyRoyaleGameManager.Instance.GetWinners();
        List<PlayerController> winnersController = new List<PlayerController>();
        foreach (FlappyRoyaleStats winnerStats in winners)
        {
            winnersController.Add(GameManager.Instance.GetPlayer(winnerStats.playerId));
        }
        PlayerController winner = GameManager.Instance.GetPlayer(winners[0].playerId);
        GameManager.Instance.SetMiniGameWinners(winnersController);
        winnerName.text = $"{winner.playerStats.nickName}";
        string timeAliveString = String.Format(winners[0].timeAlive % 1 == 0 ? "{0:0}" : "{0:0.00}", winners[0].timeAlive);
        winnerPoints.text = $"{timeAliveString} seg.";
        winnerColor.color = winner.playerStats.mainColor.color;
        gameObject.SetActive(true);
    }
}
