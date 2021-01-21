using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MinigameOverController : MonoBehaviour
{
    public Image winnerColor;
    public TextMeshProUGUI winnerPoints;
    public TextMeshProUGUI winnerName;
    public void Initialize()
    {
        gameObject.SetActive(false);
        //List<PlayerController> winners = new List<PlayerController>();
        List<PlayerMinigameStats> winners = FallingGameManager.Instance.playersMinigameStats.OrderByDescending(o => o.points).ToList();
        PlayerController winner = GameManager.Instance.GetPlayer(winners[0].playerId);
        winnerName.text = $"{winner.playerStats.nickName}";
        winnerPoints.text = $"{winners[0].points}";
        winnerColor.color = winner.playerStats.mainColor.color;
        gameObject.SetActive(true);

    }
}
