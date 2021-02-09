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
        if (!GameManager.Instance.ActualPlayerIsMainPlayer()) 
        {
            SkillsUI.Instance.MoveCameraBackToPlayer();
        } else if (GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft() >= skillInfo.energyCost)
        {
            SkillsUI.Instance.ShowMap(parent.skillToUse);
        } else
        {
            SkillsUI.Instance.MoveCameraBackToPlayer();
            SkillsUI.Instance.backButton.onClick.Invoke();
        }
    }
}
