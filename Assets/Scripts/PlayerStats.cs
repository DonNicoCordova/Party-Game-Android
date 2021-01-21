using UnityEngine;
using System;
using Photon.Pun;
using System.Collections.Generic;

[System.Serializable]
public class PlayerStats
{
    [SerializeField]
    public int id;
    public List<LocationController> capturedZones = new List<LocationController>();
    [SerializeField]
    public float ladderPosition = 1;
    [SerializeField]
    public float throwOrder = 1;
    [SerializeField]
    public Material mainColor;
    [SerializeField]
    public Material orbColor;
    [SerializeField]
    public int money = 0;
    [SerializeField]
    public int mana = 20;
    [SerializeField]
    public string nickName;
    [SerializeField]
    public bool moved = false;
    [SerializeField]
    public bool usedSkill = false;
    [SerializeField]
    public bool passed = false;
    [SerializeField]
    public bool currentStateFinished = false;
    [SerializeField]
    public bool currentMinigameStateFinished = false;
    [SerializeField]
    public bool currentMinigameOver = false;
    [SerializeField]
    public Transform lastSpawnPosition; 
    [SerializeField]
    private GameObject playableCharacter;
    private int movesLeft;
    public event EventHandler<CapturedZoneArgs> CapturedZone;
    public int GetCapturedZonesAmount() => capturedZones.Count;
    public bool PlayerDone()
    {
        return moved || passed;
    }
    public void AddCapturedZone(LocationController newCapturedZone)
    {
        if (!capturedZones.Contains(newCapturedZone))
        {
            capturedZones.Add(newCapturedZone);
            int newAmount = capturedZones.Count;
            if (CapturedZone != null)
                CapturedZone(this, new CapturedZoneArgs(newAmount));
        }
    }
    public void RemoveCapturedZones(LocationController capturedZoneToRemove)
    {
        if (capturedZones.Contains(capturedZoneToRemove))
            capturedZones.Remove(capturedZoneToRemove);
        if (CapturedZone != null)
        {
            int newAmount = capturedZones.Count;
            CapturedZone(this, new CapturedZoneArgs(newAmount));
        }
    }
    public void CaptureZone(LocationController location)
    {
        if (movesLeft >= 1)
        {
            lastSpawnPosition = location.waypoint.transform;
            movesLeft -= 1;
            location.photonView.RPC("SetOwner",RpcTarget.All,id);
            if (movesLeft == 0){
                moved = true;
            }
        } 
    }
    public void SetMovesLeft(int moves)
    {
        movesLeft = moves;
    }
    public int MovesLeft() => movesLeft;
    public class CapturedZoneArgs : EventArgs
    {
        public CapturedZoneArgs(int newCapturedZones)
        {
            NewCapturedZones = newCapturedZones;
        }
        public int NewCapturedZones { get; private set; }
    }
    public void SetPlayerGameObject(GameObject playerGO)
    {
        playableCharacter = playerGO;
    }
    public GameObject GetPlayerGameObject()
    {
        return playableCharacter;
    }
}