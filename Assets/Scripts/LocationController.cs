using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour
{
    public Transform waypoint;
    public MeshRenderer locationDome;
    public Collider detectionCollider = null;
    private PlayerStats owner = null;
    private List<PlayerStats> playersOnTop = new List<PlayerStats>();
    public void ChangeColor(Material newMaterial)
    {
        locationDome.material = newMaterial;
    }
    public void SetOwner(PlayerStats newOwner)
    {
        if (owner != null)
        {
            owner.ReduceCapturedZones(1);
        }
        owner = newOwner;
        newOwner.AddCapturedZones(1);
        ChangeColor(newOwner.orbColor);
    }
    public PlayerStats GetOwner()
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
    public void AddPlayer(PlayerStats newPlayer)
    {
        playersOnTop.Add(newPlayer);
    }
    public void RemovePlayer(PlayerStats newPlayer)
    {
        if (playersOnTop.Contains(newPlayer))
            playersOnTop.Remove(newPlayer);
    }
    public bool CheckIfPlayerOnTop(PlayerStats newPlayer)
    {
        return playersOnTop.Contains(newPlayer);
    }
}
