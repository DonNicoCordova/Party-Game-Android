using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class UpdateEnergyCommand : Command
{
    private readonly int _newEnergy;
    private readonly int _playerId;
    private bool _finished;
    private bool _failed;
    public UpdateEnergyCommand(int playerId, int newEnergy)
    {
        _newEnergy = newEnergy;
        _playerId = playerId;
    }
    public override void Reset()
    {
        _finished = false;
        _failed = false;
    }
    public override void Execute()
    {
        PlayerController playerToUpdate = GameManager.Instance.GetPlayer(_playerId);
        if (playerToUpdate != null && playerToUpdate.playerStats != null)
        {
            playerToUpdate.playerStats.SetEnergyLeft(_newEnergy);
            playerToUpdate.UpdateEnergy();
        }
        else
        {
            _failed = true;
        }
        _finished = true;
    }
    public override bool IsFinished => _finished;
    public override bool Failed => _failed;
}
