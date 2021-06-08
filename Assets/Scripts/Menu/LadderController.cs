using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class LadderController : MonoBehaviourPunCallbacks
{
    public GameObject playerInfoPrefab;
    public Animator animator;
    public PlayerInfoContainer[] playerInfoContainers;
    public void Initialize()
    {
        for(int i = 0; i < playerInfoContainers.Length; ++i)
        {
            PlayerInfoContainer container = playerInfoContainers[i];
            if (i < GameManager.Instance.notActionTakenPlayers.Count)
            {
                container.obj.SetActive(true);
                PlayerController controller = GameManager.Instance.notActionTakenPlayers.Dequeue();
                container.InitializeFromPlayer(controller);
                GameManager.Instance.notActionTakenPlayers.Enqueue(controller);
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
        foreach(PlayerInfoContainer container in playerInfoContainers)
        {
            if (container.obj.activeSelf)
            {
                if (container.playerPlaceText == null)
                {
                    Initialize();
                }
                if (container.associatedPlayer != null)
                {
                    container.UpdatePlayerPlace();
                    container.UpdateIndicator();
                }
                
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
public class PlayerInfoContainer
{
    public GameObject obj;
    public TextMeshProUGUI nicknameText;
    public TextMeshProUGUI zonesCapturedText;
    public TextMeshProUGUI playerPlaceText;
    public Image playingIndicator;
    public Image playerColor;
    public PlayerController associatedPlayer;
    public void InitializeFromPlayer(PlayerController player)
    {
        nicknameText.text = player.playerStats.nickName;
        zonesCapturedText.text = player.playerStats.capturedZones.Count.ToString();
        playerColor.material = player.playerStats.mainColor;
        player.playerStats.CapturedZone += (sender, args) => UpdateCapturedZones();
        associatedPlayer = player;
    }

    public void UpdateCapturedZones()
    {
        zonesCapturedText.text = associatedPlayer.playerStats.capturedZones.Count.ToString();
        GameManager.Instance.SetPlayersPlaces();
    }
    public void UpdatePlayerPlace()
    {
        playerPlaceText.text = associatedPlayer.playerStats.ladderPosition.ToString();
    }
    public void UpdateIndicator()
    {
        if (GameManager.Instance.GetActualPlayer() == associatedPlayer)
        {
            playingIndicator.gameObject.SetActive(true);
        } else
        {
            playingIndicator.gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        associatedPlayer.playerStats.CapturedZone -= (sender, args) => UpdateCapturedZones();
    }
}
