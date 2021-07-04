using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

namespace PoleMiniGame
{
    public class GameLadder : MonoBehaviourPunCallbacks
    {
        public GameObject playerInfoPrefab;
        public Animator animator;
        public PlayerPointsContainer[] playerPointsContainers;
        public void Initialize()
        {
            Debug.Log("INITIALIZING LADDER");
            for (int i = 0; i < playerPointsContainers.Length; ++i)
            {
                PlayerPointsContainer container = playerPointsContainers[i];
                if (i < GameManager.Instance.players.Count)
                {
                    container.obj.SetActive(true);
                    PlayerController controller = GameManager.Instance.players[i];
                    container.InitializeFromPlayer(controller);
                }
                else
                {
                    container.obj.SetActive(false);
                }
            }
            //UpdateLadderInfo();
            gameObject.SetActive(true);
        }
        public void ToggleLadder()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("LadderIn"))
            {
                animator.ResetTrigger("LadderIn");
                animator.SetTrigger("LadderOut");
            }
            else
            {
                animator.ResetTrigger("LadderOut");
                animator.SetTrigger("LadderIn");
            }
        }

        //[PunRPC]
        //public void UpdateLadderInfo()
        //{
        //    foreach (PlayerPointsContainer container in playerPointsContainers)
        //    {
        //        if (container.obj.activeSelf)
        //        {
        //            container.UpdatePlayerPoints();
        //            container.UpdateIndicator();
        //        }
        //    }
        //}
    }
    [System.Serializable]
    public class PlayerPointsContainer
    {
        public GameObject obj;
        public TextMeshProUGUI nicknameText;
        public TextMeshProUGUI zonesCapturedText;
        public TextMeshProUGUI playerPointsText;
        public Image winningIndicator;
        public Image playerColor;
        private PlayerController associatedPlayer;
        public void InitializeFromPlayer(PlayerController player)
        {
            nicknameText.text = player.playerStats.nickName;
            zonesCapturedText.text = player.playerStats.capturedZones.Count.ToString();
            playerColor.material = player.playerStats.mainColor;
            playerPointsText.text = PoleGameManager.Instance.GetPlayerMinigameStats(player.playerStats.id).piecesLeft.ToString();
            PoleGameManager.Instance.UpdatedPoints += (sender, args) => UpdatePlayerPoints();
            associatedPlayer = player;
        }
        public void UpdatePlayerPoints()
        {
            playerPointsText.text = PoleGameManager.Instance.GetPlayerMinigameStats(associatedPlayer.playerStats.id).piecesLeft.ToString();
            UpdateIndicator();
        }
        public void UpdateIndicator()
        {
            if (PoleGameManager.Instance.GetMostPoints().playerId == associatedPlayer.playerStats.id)
            {
                winningIndicator.gameObject.SetActive(true);
            }
            else
            {
                winningIndicator.gameObject.SetActive(false);
            }
        }
        private void OnDestroy()
        {
            PoleGameManager.Instance.UpdatedPoints -= (sender, args) => UpdateIndicator();
        }
    }

}
