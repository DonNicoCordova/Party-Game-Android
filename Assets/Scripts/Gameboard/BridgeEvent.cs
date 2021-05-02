using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.AI;
public class BridgeEvent : MonoBehaviour
{
    public NavMeshObstacle obstacle1;
    public NavMeshObstacle obstacle2;
    private Bridge parent;
    private void Start()
    {
        parent = GetComponentInParent<Bridge>();
        if (!parent.Usable())
        {
            parent.bridgeRenderer.enabled = false;
            parent.minimapIcon.enabled = false;
            parent.DisableButtons();
            parent.HideButtons();
            obstacle1.enabled = true;
            obstacle2.enabled = true;
        }
        else
        {
            parent.minimapIcon.enabled = true;
            parent.bridgeRenderer.enabled = true;
            parent.EnableButtons();
            obstacle1.enabled = false;
            obstacle2.enabled = false;
        }
    }
    public void ShowMap()
    {
        if (!parent.Usable())
        {
            parent.bridgeRenderer.enabled = false;
            parent.minimapIcon.enabled = false;
            parent.moveButton1.DisableButton();
            parent.moveButton2.DisableButton();
            obstacle1.enabled = true;
            obstacle2.enabled = true;
        }
        else
        {
            parent.minimapIcon.enabled = true;
            parent.bridgeRenderer.enabled = true;
            parent.moveButton1.EnableButton();
            parent.moveButton2.EnableButton();
            obstacle1.enabled = false;
            obstacle2.enabled = false;
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
