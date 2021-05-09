using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class LocationController : MonoBehaviourPunCallbacks
{
    public Transform waypoint;
    public MeshRenderer locationDome;
    public SpriteRenderer minimapIcon;
    private PlayerController owner = null;
    private List<PlayerController> playersOnTop = new List<PlayerController>();
    private Queue<int> setOwnerQueue = new Queue<int>();

    public void Update()
    {
        if (setOwnerQueue.Count > 0)
        {
            int newOwnerId = setOwnerQueue.Peek();
            StartCoroutine(setOwnerProcess(newOwnerId));
        }
    }
    public void ChangeColor(PlayerController newOwner)
    {
        
        locationDome.material = newOwner.playerStats.orbColor;
        minimapIcon.material = newOwner.playerStats.mainColor;
    }
    [PunRPC]
    public void SetOwner(int newOwnerId)
    {
        setOwnerQueue.Enqueue(newOwnerId);
    }
    private IEnumerator setOwnerProcess(int newOwnerId)
    {
        if ( GameManager.Instance.players.Count > 0 && GameManager.Instance.GetPlayer(newOwnerId) != null)
        {
            if (owner != null)
            {
                owner.playerStats.RemoveCapturedZones(this);
            }
            PlayerController newOwner = GameManager.Instance.GetPlayer(newOwnerId);
            owner = newOwner;
            newOwner.playerStats.AddCapturedZone(this);
            ChangeColor(newOwner);
            setOwnerQueue.Dequeue();
        }
        yield return new WaitForSeconds(1f);
    }
    public PlayerController GetOwner()
    {
        return owner;
    }
    public void AddPlayer(PlayerController newPlayer)
    {
        Debug.Log($"ADDING NEW PLAYER TO LOCATION: {newPlayer.playerStats.nickName}");
        Debug.Log($"PLAYERS ON TOP BEFORE: {playersOnTop.Count}");
        playersOnTop.Add(newPlayer);
        Debug.Log($"PLAYERS ON TOP AFTER: {playersOnTop.Count}");
    }
    public void RemovePlayer(PlayerController newPlayer)
    {
        if (playersOnTop.Contains(newPlayer))
        {
            Debug.Log($"REMOVING PLAYER {newPlayer.playerStats.nickName} {playersOnTop.Contains(newPlayer)}");
            playersOnTop.Remove(newPlayer);
        }
    }
    public bool CheckIfPlayerOnTop(PlayerController newPlayer)
    {
        Debug.Log($"PLAYERS ON TOP {playersOnTop.Count}: {playersOnTop.Contains(newPlayer)}");
        return playersOnTop.Contains(newPlayer);
    }
}
