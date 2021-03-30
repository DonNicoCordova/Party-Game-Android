using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ResumeBridgesCommand : Command
{
    private bool _finished;
    private bool _failed;
    public override void Reset()
    {
        _finished = false;
        _failed = false;
    }
    public override void Execute()
    {
        if (SkillsUI.Instance.bridges == null)
        {
            SkillsUI.Instance.bridges = new List<Bridge>(GameObject.FindObjectsOfType<Bridge>());
            _failed = true;
            return;
        }
        if (!GameManager.Instance.CheckIfBridgesLoaded())
        {
            GameManager.Instance.LoadBridges();
            _failed = true;
            return;
        }
        foreach (Bridge bridge in SkillsUI.Instance.bridges)
        {

            BridgeStats savedBridgeStats = GameManager.Instance.GetSavedBridges(bridge.bridgeStats.name);
            if (savedBridgeStats == null)
            {
                _failed = true;
                return;
            }

            bridge.bridgeStats.usable = savedBridgeStats.usable;
            bridge.bridgeStats.energyCost = savedBridgeStats.energyCost;

            if (savedBridgeStats.usable && !bridge.bridgeRenderer.enabled)
            {
                bridge.AnimateSpawn();
            } else if (!savedBridgeStats.usable && bridge.bridgeRenderer.enabled)
            {
                bridge.AnimateCut();
            }
        }
        _finished = true;
        return;
    }
    public override bool IsFinished => _finished;
    public override bool Failed => _failed;
}
