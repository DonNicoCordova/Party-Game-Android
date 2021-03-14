using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class FlappyMiniGameOverController : MonoBehaviour
{
    public Image winnerColor;
    public TextMeshProUGUI winnerPoints;
    public TextMeshProUGUI winnerName;
    public void Initialize()
    {
        gameObject.SetActive(false);
        //List<PlayerController> winners = new List<PlayerController>();
        FlappyRoyaleStats winner = FlappyRoyaleGameManager.Instance.GetWinner();
        PlayerController winnerController = GameManager.Instance.GetPlayer(winner.playerId);
        winnerController.WonMiniGame();
        winnerName.text = $"{winnerController.playerStats.nickName}";
        winnerPoints.text = $"{winner.timeAlive}";
        winnerColor.color = winnerController.playerStats.mainColor.color;
        gameObject.SetActive(true);
    }
}
