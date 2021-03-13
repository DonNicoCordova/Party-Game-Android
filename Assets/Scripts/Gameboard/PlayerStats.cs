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
    public int maxEnergy = 20;
    [SerializeField]
    public string nickName;
    [SerializeField]
    public bool passed = false;
    [SerializeField]
    public bool currentStateFinished = false;
    [SerializeField]
    public bool currentMinigameStateFinished = false;
    [SerializeField]
    public bool currentMinigameOver = false;
    [SerializeField]
    public bool wonLastGame = false;
    [SerializeField]
    public Transform lastSpawnPosition; 
    [SerializeField]
    private GameObject playableCharacter;
    [SerializeField]
    private int energy;
    [SerializeField]
    public bool turnDone = false;
    public event EventHandler<CapturedZoneArgs> CapturedZone;
    public event EventHandler<EnergyChangedArgs> EnergyChanged;
    public int GetCapturedZonesAmount() => capturedZones.Count;
    public bool PlayerDone()
    {
        return energy == 0 || passed;
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
        if (energy >= 1)
        {
            lastSpawnPosition = location.waypoint.transform;
            ReduceEnergy(1, "CAPTURE");
            location.photonView.RPC("SetOwner",RpcTarget.All,id);
        } 
    }
    public void SetEnergyLeft(int newEnergy)
    {
        energy = newEnergy;
        if (energy == 0)
        {
            turnDone = true;
        }
        if (EnergyChanged != null)
            EnergyChanged(this, new EnergyChangedArgs(energy));
    }
    public void AddEnergy(int newEnergy)
    {
        if (playableCharacter.gameObject.GetPhotonView().IsMine)
        {
            energy += newEnergy;
            GameboardRPCManager.Instance.photonView.RPC("UpdateEnergy", RpcTarget.Others, id, energy);
            if (EnergyChanged != null)
                EnergyChanged(this, new EnergyChangedArgs(energy));
        }
    }

    public void ReduceEnergy(int newEnergy, string reason)
    {
        if (playableCharacter.gameObject.GetPhotonView().IsMine)
        {
            energy -= newEnergy;
            if (energy == 0)
            {
                turnDone = true;
            }
            GameboardRPCManager.Instance.photonView.RPC("UpdateEnergy", RpcTarget.Others, id, energy);
            if (EnergyChanged != null)
                EnergyChanged(this, new EnergyChangedArgs(energy));
        }
    }
    public int EnergyLeft() => energy;
    public class CapturedZoneArgs : EventArgs
    {
        public CapturedZoneArgs(int newCapturedZones)
        {
            NewCapturedZones = newCapturedZones;
        }
        public int NewCapturedZones { get; private set; }
    }
    public class EnergyChangedArgs : EventArgs
    {
        public EnergyChangedArgs(int newEnergy)
        {
            NewEnergy = newEnergy;
        }
        public int NewEnergy { get; private set; }
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