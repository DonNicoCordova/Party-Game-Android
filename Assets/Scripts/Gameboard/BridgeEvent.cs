using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class BridgeEvent : MonoBehaviour
{
    private Bridge parent;
    private void Start()
    {
        parent = GetComponentInParent<Bridge>();
    }
    public void ShowMap()
    {
        if (!parent.bridgeStats.usable)
        {
            parent.bridgeRenderer.enabled = false;
            parent.minimapIcon.enabled = false;
            parent.moveButton1.EnableButton();
            parent.moveButton2.EnableButton();
        }
        else
        {
            parent.minimapIcon.enabled = true;
            parent.bridgeRenderer.enabled = true;
            parent.moveButton1.DisableButton();
            parent.moveButton2.DisableButton();
        }
        parent.RemoveClickable();

        SkillInfo skillInfo = SkillsUI.Instance.GetSkillInfo(parent.skillToUse);
        if (GameManager.Instance.GetMainPlayer().photonPlayer != SkillsUI.Instance.playerUsingSkills) 
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
        //GameboardRPCManager.Instance.surface.BuildNavMesh();
        parent.photonView.RPC("SetNoAnimationIsPlaying", RpcTarget.All, true);
    }
}
