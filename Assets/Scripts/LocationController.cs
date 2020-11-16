using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class LocationController : MonoBehaviourPunCallbacks
{
    public Transform waypoint;
    public MeshRenderer locationDome;
    public Collider detectionCollider = null;
    private PlayerController owner = null;
    private List<PlayerController> playersOnTop = new List<PlayerController>();
    public void ChangeColor(Material newMaterial)
    {
        locationDome.material = newMaterial;
    }
    [PunRPC]
    public void SetOwner(int newOwnerId)
    {
        if (owner != null)
        {
            owner.playerStats.RemoveCapturedZones(this);
        }
        PlayerController newOwner = GameManager.instance.GetPlayer(newOwnerId);
        owner = newOwner;
        newOwner.playerStats.AddCapturedZone(this);
        ChangeColor(newOwner.playerStats.orbColor);
    }
    public PlayerController GetOwner()
    {
        return owner;
    }
    public void DisableCollider()
    {
        detectionCollider.enabled = false;
    }
    public void EnableCollider()
    {
        detectionCollider.enabled = true;
    }
    public void AddPlayer(PlayerController newPlayer)
    {
        playersOnTop.Add(newPlayer);
    }
    public void RemovePlayer(PlayerController newPlayer)
    {
        if (playersOnTop.Contains(newPlayer))
            playersOnTop.Remove(newPlayer);
    }
    public bool CheckIfPlayerOnTop(PlayerController newPlayer)
    {
        return playersOnTop.Contains(newPlayer);
    }
}
