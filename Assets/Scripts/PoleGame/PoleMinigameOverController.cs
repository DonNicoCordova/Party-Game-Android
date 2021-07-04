using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;

namespace PoleMiniGame
{
    public class PoleMinigameOverController : MonoBehaviour
    {
        public Image winnerColor;
        public TextMeshProUGUI winnerPoints;
        public TextMeshProUGUI winnerName;
        public void Initialize()
        {
            gameObject.SetActive(false);
            //List<PlayerController> winners = new List<PlayerController>();
            List<PolePlayerStats> winners = PoleGameManager.Instance.GetWinners();
            List<PlayerController> winnersController = new List<PlayerController>();
            foreach(PolePlayerStats winnerStats in winners)
            {
                winnersController.Add(GameManager.Instance.GetPlayer(winnerStats.playerId));
            }
            PlayerController winner = GameManager.Instance.GetPlayer(winners[0].playerId);
            GameManager.Instance.SetMiniGameWinners(winnersController);
            winnerName.text = $"{winner.playerStats.nickName}";
            string timeAliveString = String.Format(winners[0].timeSpent % 1 == 0 ? "{0:0}" : "{0:0.00}", winners[0].timeSpent);
            winnerPoints.text = $"{timeAliveString} seg."; 
            winnerColor.color = winner.playerStats.mainColor.color;
            gameObject.SetActive(true);
        }
    }
}
