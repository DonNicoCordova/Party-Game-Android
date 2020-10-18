using UnityEngine;
using System;

[System.Serializable]
public class PlayerStats
{
    [SerializeField]
    public int id;
    [SerializeField]
    public float capturedZones = 0f;
    [SerializeField]
    public float ladderPosition = 1;
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
    public float actualSpeed;
    [SerializeField]
    public float horizontalDirection;
    [SerializeField]
    public float verticalDirection;
    [SerializeField]
    private GameObject playableCharacter;
    private int maxMoves;
    public event EventHandler<CapturedZoneArgs> CapturedZone;
    public float GetCapturedZones() => capturedZones;
    public bool PlayerDone() => moved && passed || passed;
    public void AddCapturedZones(float amount)
    {
        capturedZones += amount;
        if (CapturedZone != null)
            CapturedZone(this, new CapturedZoneArgs(capturedZones));
    }
    public void ReduceCapturedZones(float amount)
    {
        capturedZones -= amount;
        if (CapturedZone != null)
            CapturedZone(this, new CapturedZoneArgs(capturedZones));
    }
    public void CaptureZone(LocationController location) 
    {
        Debug.Log($"CAPTURING ZONE {location.name}");
        if (maxMoves >= 1)
        {
            maxMoves -= 1;
            location.SetOwner(this);
            AddCapturedZones(1);
            if (maxMoves == 0){
                passed = true;
            }
        } 
    }
    public void SetMaxMoves(int moves)
    {
        maxMoves = moves;
    }
    public int MovesLeft() => maxMoves;
    public class CapturedZoneArgs : EventArgs
    {
        public CapturedZoneArgs(float newCapturedZones)
        {
            NewCapturedZones = newCapturedZones;
        }
        public float NewCapturedZones { get; private set; }
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