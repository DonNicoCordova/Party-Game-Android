using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FlappyMiniGameOverController : MonoBehaviour
{
    public Image winnerColor;
    public TextMeshProUGUI winnerPoints;
    public TextMeshProUGUI winnerName;
    public void Initialize()
    {
        gameObject.SetActive(false);
        FlappyRoyaleStats winner = FlappyRoyaleGameManager.Instance.GetWinner();
        PlayerController winnerController = GameManager.Instance.GetPlayer(winner.playerId);
        winnerController.WonMiniGame();
        winnerName.text = $"{winnerController.playerStats.nickName}";
        string timeAliveString = String.Format(winner.timeAlive % 1 == 0 ? "{0:0}" : "{0:0.00}", winner.timeAlive);
        winnerPoints.text = $"{timeAliveString} seg.";
        winnerColor.color = winnerController.playerStats.mainColor.color;
        gameObject.SetActive(true);
    }
}
