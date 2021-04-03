using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ResumeCommand : Command
{
    private readonly PlayerController _playerController;
    private readonly Player _newPhotonPlayer;
    private bool _finished;
    private bool _failed;
    public ResumeCommand(Player newPhotonPlayer, PlayerController playerController)
    {
        _newPhotonPlayer = newPhotonPlayer;
        _playerController = playerController;
    }
    public override void Reset()
    {
        _finished = false;
        _failed = false;
    }
    public override void Execute()
    {
        CapturedLocation oldCapturedLocations = GameManager.Instance.GetSavedCapturedLocation(_newPhotonPlayer.ActorNumber);
        PlayerStats oldPlayerStats = GameManager.Instance.GetSavedPlayerStats(_newPhotonPlayer.ActorNumber);
        oldPlayerStats.mainColor = GameManager.Instance.playerConfigs[_newPhotonPlayer.ActorNumber - 1].mainColor;
        oldPlayerStats.orbColor = GameManager.Instance.playerConfigs[_newPhotonPlayer.ActorNumber - 1].orbColor;
        oldPlayerStats.capturedZones.Clear();
        oldPlayerStats.SetPlayerGameObject(_playerController.gameObject);
        oldPlayerStats.passed = false;
        PlayerGraficsController gfxController = _playerController.gameObject.GetComponentInChildren<PlayerGraficsController>();
        gfxController.ChangeMaterial(oldPlayerStats.mainColor);
        oldPlayerStats.currentMinigameOver = false;
        oldPlayerStats.currentMinigameStateFinished = false;
        oldPlayerStats.currentStateFinished = false;
        oldPlayerStats.id = _newPhotonPlayer.ActorNumber;
        foreach (string locationName in oldCapturedLocations.capturedLocations.Split(','))
        {
            GameObject locationGo = GameObject.Find(locationName);
            LocationController location = locationGo.GetComponent<LocationController>();
            oldPlayerStats.SetEnergyLeft(1);
            oldPlayerStats.CaptureZone(location);
        }
        // set photon player
        _playerController.photonPlayer = _newPhotonPlayer;

        _playerController.playerStats = oldPlayerStats;
        // add player to player list

        // Only main player should be affected by physics
        if (!_playerController.photonView.IsMine)
        {
            _playerController.rig.isKinematic = true;
        }
        else
        {
            // set player to main player and assign camera to follow plus enable joystick
            GameManager.Instance.SetMainPlayer(_playerController);
            if (GameManager.Instance.virtualCamera == null && GameObject.FindGameObjectWithTag("VirtualCamera") != null)
            {
                GameManager.Instance.virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>();
            }
            GameManager.Instance.virtualCamera.Follow = _playerController.transform;
            GameManager.Instance.virtualCamera.LookAt = _playerController.transform;
            if (GameManager.Instance.joystick == null && GameObject.FindGameObjectWithTag("Joystick") != null)
            {
                GameManager.Instance.joystick = GameObject.FindGameObjectWithTag("Joystick");
            }
            _playerController.joystick = GameManager.Instance.joystick.GetComponent<FloatingJoystick>();
        }
        _playerController.playerNameText.text = oldPlayerStats.nickName;
        _playerController.energyText.text = "0";
        _playerController.ResetPosition();
        _playerController.enabledToPlay = true;
        oldPlayerStats.EnergyChanged += (sender, args) => _playerController.UpdateEnergy();
        _finished = true;
    }
    public override bool IsFinished => _finished;
    public override bool Failed => _failed;
}
