using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeEvent : MonoBehaviour
{
    private Bridge parent;
    private void Start()
    {
        parent = GetComponentInParent<Bridge>();
    }
    public void ShowMap()
    {
        if (!parent.usable)
        {
            parent.bridgeRenderer.enabled = false;
            parent.RemoveClickable();
            parent.minimapIcon.enabled = false;
        }
        else
        {
            parent.minimapIcon.enabled = true;
        }

        SkillInfo skillInfo = SkillsUI.Instance.GetSkillInfo(parent.skillToUse);
        if (GameManager.Instance.GetActualPlayer().photonPlayer != SkillsUI.Instance.playerUsingSkills) 
        {
            Debug.Log("NOT MAIN PLAYER MOVING TO PLAYER");
            SkillsUI.Instance.MoveCameraBackToPlayer();
        } else if (GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft() >= skillInfo.energyCost)
        {
            Debug.Log("MAIN PLAYER CAN AFFORD TO USE SKILL AGAIN");
            SkillsUI.Instance.ShowMap(parent.skillToUse);
        } else
        {

            Debug.Log("MAIN PLAYER CANT AFFORD TO USE SKILL AGAIN... MOVING CAMERA BACK TO PLAYER");
            SkillsUI.Instance.MoveCameraBackToPlayer();
            SkillsUI.Instance.backButton.onClick.Invoke();
        }
        SkillsUI.Instance.noAnimationsPlaying = true;
    }
}
