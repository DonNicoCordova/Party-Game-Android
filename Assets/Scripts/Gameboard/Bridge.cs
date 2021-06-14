using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections;
using System;

public class Bridge : MonoBehaviourPunCallbacks
{
    [Header("Configuration")]
    public Transform cutPosition1;
    public Transform cutPosition2;
    public MoveButton moveButton1;
    public MoveButton moveButton2;
    public Transform cameraHighlighPosition;
    public Animator animator;
    public MeshRenderer bridgeRenderer;
    public Skill skill;
    public BridgeStats bridgeStats;
    
    [Header("Minimap")]
    public SpriteRenderer minimapIcon;
    public SpriteRenderer minimapSelectableIcon;
    
    private Transform originalPosition;
    private Vector3 originalScale;

    private void Start()
    {
        BridgeStats newBridgeStats = new BridgeStats();

        if (bridgeRenderer == null)
        {
            bridgeRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        }
        
        originalPosition = transform;
        if (minimapIcon != null)
        {
            originalScale = minimapIcon.transform.localScale;
        }
        newBridgeStats.name = gameObject.name;
        bridgeStats = newBridgeStats;
    }
    public bool Usable()
    {
        if (bridgeRenderer != null && bridgeStats != null)
        {
            if (bridgeRenderer.enabled)
            {
                bridgeStats.usable = true;
                return bridgeStats.usable;
            } else
            {
                bridgeStats.usable = false;
                return bridgeStats.usable;
            }
        } else
        {
            return false;
        }
    }
    [PunRPC]
    public void CutOut(PhotonMessageInfo info)
    {
        if (Usable())
        {
            Vector3 position1 = cutPosition1.position;
            Vector3 position2 = cutPosition2.position;
            //PlayerController playerUsingSkills = GameManager.Instance.GetPlayer(info.Sender.ActorNumber);
            //SkillsUI.Instance.playerUsingSkills = playerUsingSkills;
            //GameboardRPCManager.Instance.photonView.RPC("SetPlayerUsingSkill", RpcTarget.Others, info.Sender.ActorNumber);
            AnimateCut();
        }
    }
    public void AnimateCut()
    {
        if (bridgeRenderer.enabled)
        {
            animator.SetTrigger("Fall");
        }
    }
    [PunRPC]
    public void Spawn(PhotonMessageInfo info)
    {
        if (!Usable())
        {
            //PlayerController playerUsingSkills = GameManager.Instance.GetPlayer(info.Sender.ActorNumber);
            //SkillsUI.Instance.playerUsingSkills = playerUsingSkills;
            //GameboardRPCManager.Instance.photonView.RPC("SetPlayerUsingSkill", RpcTarget.Others, info.Sender.ActorNumber);
            AnimateSpawn();
        }
    }
    public void AnimateSpawn()
    {
        if (!bridgeRenderer.enabled)
        {
            animator.SetTrigger("Spawn");
        }
    }
    [PunRPC]
    public void BecomeSticky()
    {
        bridgeStats.energyCost = 3;
    }
    [PunRPC]
    public void SetNoAnimationIsPlaying(bool state)
    {
        SkillsUI.Instance.noAnimationsPlaying = state;
    }
    [PunRPC]
    public void MoveCameraToHighlightArea()
    {
        SkillsUI.Instance.highlightCamera.transform.parent = transform;
        SkillsUI.Instance.highlightCamera.ForceCameraPosition(cameraHighlighPosition.position, cameraHighlighPosition.rotation);
        SkillsUI.Instance.MoveCameraToHighlight();
    }
    [PunRPC]
    public void SetSkill(Skill mode)
    {
        skill = mode;
    }
    public void BecomeClickable(Skill mode)
    {
        Button button = minimapIcon.GetComponent<Button>();
        button.interactable = true;
        minimapIcon.enabled = true;
        minimapSelectableIcon.enabled = true;
        minimapIcon.sortingOrder += 10;
        minimapSelectableIcon.sortingOrder += 10;
        skill = mode;
        photonView.RPC("SetSkill", RpcTarget.Others, mode);
    }
    public void RemoveClickable()
    {
        if (!bridgeRenderer.enabled) {
            minimapIcon.enabled = false;
        } else
        {
            minimapIcon.enabled = true;
        }
        minimapSelectableIcon.enabled = false;
        minimapIcon.sortingOrder -= 10;
        minimapSelectableIcon.sortingOrder -= 10;
        Button button = minimapIcon.GetComponent<Button>();
        button.interactable = false;
        button.onClick.RemoveAllListeners();
    }

    public void ProcessClick()
    {
        if (SkillsUI.Instance.noAnimationsPlaying)
        {
            SkillInfo skillInfo = SkillsUI.Instance.GetSkillInfo(skill);
            if (skillInfo != null)
            {
                if (skillInfo.energyCost <= GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft())
                {
                    switch (skill)
                    {
                        case Skill.Cut:
                            photonView.RPC("MoveCameraToHighlightArea", RpcTarget.All);
                            if (SkillsUI.Instance.payingMethod == PayingMethod.Token)
                            {
                                SkillsUI.Instance.SpendTokenOnMainPlayer(skill);
                            }
                            else if (SkillsUI.Instance.payingMethod == PayingMethod.Energy)
                            {
                                GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost, "CUT");
                            }
                            StartCoroutine(DelayRPC("CutOut"));
                            break;
                        case Skill.Spawn:
                            photonView.RPC("MoveCameraToHighlightArea", RpcTarget.All);
                            if (SkillsUI.Instance.payingMethod == PayingMethod.Token)
                            {
                                SkillsUI.Instance.SpendTokenOnMainPlayer(skill);
                            }
                            else if (SkillsUI.Instance.payingMethod == PayingMethod.Energy)
                            {
                                GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost, "SPAWN");
                            }
                            StartCoroutine(DelayRPC("Spawn"));
                            break;
                    }
                }
            }
        }
    }
    public void ShowMap()
    {
        SkillsUI.Instance.ShowMap(skill);
    }
    private IEnumerator DelayRPC(string call)
    {
        photonView.RPC("SetNoAnimationIsPlaying", RpcTarget.All, false);
        yield return new WaitForSeconds(1.6f);
        photonView.RPC(call, RpcTarget.All);
    }
    public void EnableButtons()
    {
        moveButton1.EnableButton();
        moveButton2.EnableButton();
    }
    public void DisableButtons()
    {
        moveButton1.DisableButton();
        moveButton2.DisableButton();
    }
}

[Serializable]
public class BridgeStats
{
    [SerializeField]
    public int energyCost = 0;
    [SerializeField]
    public bool usable = true;
    [SerializeField]
    public string name;
}