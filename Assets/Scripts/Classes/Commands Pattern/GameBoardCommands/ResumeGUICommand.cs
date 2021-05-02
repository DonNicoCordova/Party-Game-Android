using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class ResumeGUICommand : Command
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
        foreach(PlayerController player in GameManager.Instance.players)
        {
            if (!player.playerStats.mainColor)
            {
                _finished = true;
                _failed = true;
                return;
            }
        }
        SkillsUI.Instance.Initialize();
        GameManager.Instance.playersLadder.gameObject.SetActive(true);
        GameManager.Instance.gameOverUI.gameObject.SetActive(false);
        GameManager.Instance.playersLadder.Initialize();
        GameManager.Instance.energyCounter.Hide();
        SkillsUI.Instance.HideSkills();
        _finished = true;
        return;
    }
    public override bool IsFinished => _finished;
    public override bool Failed => _failed;
}
