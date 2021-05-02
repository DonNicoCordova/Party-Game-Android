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
    public SkillToUse skillToUse;
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
        if (!bridgeRenderer.enabled)
        {
            newBridgeStats.usable = false;
        }
        originalPosition = transform;
        if (minimapIcon != null)
        {
            originalScale = minimapIcon.transform.localScale;
        }
        newBridgeStats.name = gameObject.name;
        bridgeStats = newBridgeStats;
    }
    [PunRPC]
    public void CutOut(PhotonMessageInfo info)
    {
        if (bridgeStats.usable)
        {
            Vector3 position1 = cutPosition1.position;
            Vector3 position2 = cutPosition2.position;
            SkillsUI.Instance.playerUsingSkills = info.Sender;
            AnimateCut();
            bridgeStats.usable = false;
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
        if (!bridgeStats.usable)
        {
            SkillsUI.Instance.playerUsingSkills = info.Sender;
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
    public void BecomeClickable(SkillToUse mode)
    {
        Button button = minimapIcon.GetComponent<Button>();
        button.interactable = true;
        minimapIcon.enabled = true;
        minimapSelectableIcon.enabled = true;
        minimapIcon.sortingOrder += 10;
        minimapSelectableIcon.sortingOrder += 10;
        skillToUse = mode;
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
            SkillInfo skillInfo = SkillsUI.Instance.GetSkillInfo(skillToUse);
            if (skillInfo != null)
            {
                if (skillInfo.energyCost <= GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft())
                {
                    switch (skillToUse)
                    {
                        case SkillToUse.Cut:
                            photonView.RPC("MoveCameraToHighlightArea", RpcTarget.All);
                            GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost, "CUT");
                            StartCoroutine(DelayRPC("CutOut"));
                            break;
                        case SkillToUse.Spawn:
                            photonView.RPC("MoveCameraToHighlightArea", RpcTarget.All);
                            GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost, "SPAWN");
                            StartCoroutine(DelayRPC("Spawn"));
                            break;
                        case SkillToUse.Sticky:
                            GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost, "STICKY");
                            break;
                    }
                }
            }
        }
    }
    public void ShowMap()
    {
        SkillsUI.Instance.ShowMap(skillToUse);
    }
    private IEnumerator DelayRPC(string call)
    {
        photonView.RPC("SetNoAnimationIsPlaying", RpcTarget.All, false);
        yield return new WaitForSeconds(1.6f);
        photonView.RPC(call, RpcTarget.All);
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