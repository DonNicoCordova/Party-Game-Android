using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class BridgeEvent : MonoBehaviour
{
    public NavMeshObstacle obstacle1;
    public NavMeshObstacle obstacle2;
    private Bridge parent;
    private void Awake()
    {
        StartCoroutine(Setup());
    }
    public IEnumerator Setup()
    {
        yield return new WaitForSeconds(1f);
        parent = GetComponentInParent<Bridge>();
        if (!parent.Usable())
        {
            parent.bridgeRenderer.enabled = false;
            parent.minimapIcon.enabled = false;
            parent.DisableButtons();
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
        if (GameManager.Instance.ActualPlayerIsMainPlayer())
        {
            GameManager.Instance.GetMainPlayer().buttonChecker.HideButtons();
            GameManager.Instance.GetMainPlayer().buttonChecker.CheckForButtonsNearby();
        }
        SkillInfo skillInfo = SkillsUI.Instance.GetSkillInfo(parent.skill);
        Token skillToken;
        if (skillInfo == null)
        {
            skillToken = SkillsUI.Instance.GetToken(Skill.None);
        } else
        {
            skillToken = SkillsUI.Instance.GetToken(skillInfo.skill);
        }
        if (GameManager.Instance.GetMainPlayer() != SkillsUI.Instance.playerUsingSkills)
        {
            SkillsUI.Instance.MoveCameraBackToPlayer();
        }
        else if (SkillsUI.Instance.payingMethod == PayingMethod.Energy && (GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft() >= skillInfo.energyCost))
        {
            SkillsUI.Instance.ShowMap(parent.skill);
        }
        else if (SkillsUI.Instance.payingMethod == PayingMethod.Token && GameManager.Instance.GetMainPlayer().inventory.GetTokenAmount(skillToken.code) > 0)
        {
            SkillsUI.Instance.ShowMap(parent.skill);
        }
        else
        {
            SkillsUI.Instance.MoveCameraBackToPlayer();
            SkillsUI.Instance.backButton.onClick.Invoke();
        }
        //GameboardRPCManager.Instance.surface.BuildNavMesh();
        parent.photonView.RPC("SetNoAnimationIsPlaying", RpcTarget.All, true);
    }
}
