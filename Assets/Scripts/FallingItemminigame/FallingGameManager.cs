using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
public class FallingGameManager : MonoBehaviourPunCallbacks
{
    public static FallingGameManager instance;
    public TMPro.TextMeshProUGUI pointsText;
    public ItemSpawner itemSpawner;
    public List<PlayerPoints> playersPoints = new List<PlayerPoints>();
    public FallingGameLadderController ladderController;
    public int points = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
        UpdatePointsText();
        foreach(PlayerController player in GameManager.instance.players)
        {
            PlayerPoints newPlayerPoint = new PlayerPoints();
            newPlayerPoint.playerId = player.playerStats.id;
            newPlayerPoint.points = 0;
            playersPoints.Add(newPlayerPoint);
        }
        ladderController.Initialize();
    }
    public void AddPoints(int newPoints)
    {
        PlayerController playerController = GameManager.instance.GetMainPlayer();
        points += newPoints;
        PlayerPoints playerPoints = GetPlayerPoints(playerController.playerStats.id);
        playerPoints.points = points;
        UpdatePointsText();
        ladderController.photonView.RPC("UpdateLadderInfo", RpcTarget.All);
        this.photonView.RPC("UpdatePlayerPoints", RpcTarget.Others, playerController.playerStats.id, points);
    }
    public void ReducePoints(int newPoints)
    {
        PlayerController playerController = GameManager.instance.GetMainPlayer();
        points -= newPoints;
        UpdatePointsText();
        PlayerPoints playerPoints = GetPlayerPoints(playerController.playerStats.id);
        playerPoints.points = points;
        UpdatePointsText();
        ladderController.photonView.RPC("UpdateLadderInfo", RpcTarget.All);
        this.photonView.RPC("UpdatePlayerPoints", RpcTarget.Others, playerController.playerStats.id, points);
    }
    private void UpdatePointsText()
    {
        pointsText.text = points.ToString();
    }
    [PunRPC]
    public void FireLeftCannon(int playerId)
    {
        Debug.Log($"PUN RPC CALLED FOR ID {playerId} MainPlayer: {GameManager.instance.GetMainPlayer().playerStats.id}");
        if (playerId == GameManager.instance.GetMainPlayer().playerStats.id)
        {
            Debug.Log("PLAYER IS TARGET");
            itemSpawner.FireLeftCannon();
        }
    }
    [PunRPC]
    public void FireRightCannon(int playerId)
    {
        Debug.Log($"PUN RPC CALLED FOR ID {playerId} MainPlayer: {GameManager.instance.GetMainPlayer().playerStats.id}");
        if (playerId == GameManager.instance.GetMainPlayer().playerStats.id)
        {
            Debug.Log("PLAYER IS TARGET");
            itemSpawner.FireRightCannon();
        }
    }
    [PunRPC]
    public void UpdatePlayerPoints(int playerId, int points)
    {
        PlayerPoints playerPoints = GetPlayerPoints(playerId);
        if (playerPoints != null)
        {
            playerPoints.points = points;
        }
    }

    public PlayerPoints GetPlayerPoints(int playerId) => playersPoints.First(x => x.playerId == playerId);
    public PlayerPoints GetMostPoints() => playersPoints.OrderByDescending(x => x.points).First();
}

public class PlayerPoints
{
    public int playerId;
    public int points;
}
