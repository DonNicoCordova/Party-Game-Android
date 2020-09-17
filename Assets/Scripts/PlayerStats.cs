using UnityEngine;
using System;

[System.Serializable]
public class PlayerStats
{
    [SerializeField]
    public int id;
    [SerializeField]
    private float capturedZones = 0f;
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
    public bool isPlayer = false;
    [SerializeField]
    public bool moved = false;
    [SerializeField]
    public bool usedSkill = false;
    [SerializeField]
    public bool passed = false;
    [SerializeField]
    public GameObject playableCharacter;
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
        location.SetOwner(this);
        maxMoves -= 1;
        AddCapturedZones(1);
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
}