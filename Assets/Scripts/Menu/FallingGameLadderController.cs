using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class FallingGameLadderController : MonoBehaviourPunCallbacks
{
    public GameObject playerInfoPrefab;
    public Animator animator;
    public PlayerPointsContainer[] playerPointsContainers;
    public void Initialize()
    {
        for(int i = 0; i < playerPointsContainers.Length; ++i)
        {
            PlayerPointsContainer container = playerPointsContainers[i];
            Debug.Log($"COUNTS. AT: {GameManager.instance.actionTakenPlayers.Count} NAT: {GameManager.instance.notActionTakenPlayers.Count}");
            if (i < GameManager.instance.notActionTakenPlayers.Count)
            {
                container.obj.SetActive(true);
                PlayerController controller = GameManager.instance.notActionTakenPlayers.Dequeue();
                container.InitializeFromPlayer(controller);
                GameManager.instance.notActionTakenPlayers.Enqueue(controller);
            } else
            {
                container.obj.SetActive(false);
            }
        }
        gameObject.SetActive(true);
    }
    [PunRPC]
    public void UpdateLadderInfo()
    {
        foreach(PlayerPointsContainer container in playerPointsContainers)
        {
            if (container.obj.activeSelf)
            {
                container.UpdatePlayerPoints();
                container.UpdateIndicator();
            }
        }
    }
    public void ToggleLadder()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("LadderIn"))
        {
            animator.ResetTrigger("LadderIn");
            animator.SetTrigger("LadderOut");
        } else
        {
            animator.ResetTrigger("LadderOut");
            animator.SetTrigger("LadderIn");
        }
    }
    
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
        playerPointsText.text = FallingGameManager.instance.GetPlayerPoints(player.playerStats.id).points.ToString();
        associatedPlayer = player;
    }
    public void UpdatePlayerPoints()
    {
        playerPointsText.text = FallingGameManager.instance.GetPlayerPoints(associatedPlayer.playerStats.id).points.ToString();
    }
    public void UpdateIndicator()
    {
        if (FallingGameManager.instance.GetMostPoints().playerId == associatedPlayer.playerStats.id)
        {
            winningIndicator.gameObject.SetActive(true);
        }
        else
        {
            winningIndicator.gameObject.SetActive(false);
        }
    }
}
