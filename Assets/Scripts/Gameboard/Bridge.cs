using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections;

public class Bridge : MonoBehaviourPunCallbacks
{
    public Transform cutPosition1;
    public Transform cutPosition2;
    public Transform cameraHighlighPosition;
    public MeshRenderer bridgeRenderer;
    [SerializeField]
    private Animator animator;
    public bool usable = true;

    [Header("Minimap")]
    public SpriteRenderer minimapIcon;
    public SpriteRenderer minimapSelectableIcon;
    private Transform originalPosition;
    private int energyCost = 0;
    private Vector3 originalScale;
    public SkillToUse skillToUse;

    private void Start()
    {
        if (bridgeRenderer == null)
        {
            bridgeRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        }
        if (!bridgeRenderer.enabled)
        {
            usable = false;
        }
        originalPosition = transform;
        if (minimapIcon != null)
        {
            originalScale = minimapIcon.transform.localScale;
        }
    }
    [PunRPC]
    public void CutOut(PhotonMessageInfo info)
    {
        if (usable)
        {
            Vector3 position1 = cutPosition1.position;
            Vector3 position2 = cutPosition2.position;
            SkillsUI.Instance.playerUsingSkills = info.Sender;
            Debug.Log($"{info.Sender.NickName} IS USING CUT");
            animator.SetTrigger("Fall");
            usable = false;
        }
    }
    [PunRPC]
    public void Spawn(PhotonMessageInfo info)
    {
        if (!usable)
        {
            SkillsUI.Instance.playerUsingSkills = info.Sender;
            Debug.Log($"{info.Sender.NickName} IS USING CUT");
            animator.SetTrigger("Spawn");
        }
    }
    [PunRPC]
    public void BecomeSticky()
    {
        energyCost = 3;
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
        Debug.Log($"PROCESSING CLICK ON BRIDGE {gameObject.name}");
        SkillInfo skillInfo = SkillsUI.Instance.GetSkillInfo(skillToUse);
        if (skillInfo.energyCost <= GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft())
        {
            switch (skillToUse)
            {
                case SkillToUse.Cut:
                    photonView.RPC("MoveCameraToHighlightArea", RpcTarget.All);
                    Debug.Log($"CALLING REDUCE ENERGY FROM CUT -{skillInfo.energyCost}");
                    GameboardRPCManager.Instance.photonView.RPC("DebugMessage", RpcTarget.MasterClient, $"UPDATING ENERGY TO PLAYER ID: {GameManager.Instance.GetMainPlayer().playerStats.nickName} WITH: -{skillInfo.energyCost} FOR REASON CUT ");
                    GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost, "CUT");
                    StartCoroutine(DelayRPC("CutOut"));
                    break;
                case SkillToUse.Spawn:
                    photonView.RPC("MoveCameraToHighlightArea", RpcTarget.All);
                    Debug.Log($"CALLING REDUCE ENERGY FROM SPAWN -{skillInfo.energyCost}"); 
                    GameboardRPCManager.Instance.photonView.RPC("DebugMessage", RpcTarget.MasterClient, $"UPDATING ENERGY TO PLAYER ID: {GameManager.Instance.GetMainPlayer().playerStats.nickName} WITH: -{skillInfo.energyCost} FOR REASON SPAWN");
                    GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost, "SPAWN");
                    StartCoroutine(DelayRPC("Spawn"));
                    break;
                case SkillToUse.Sticky:
                    GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost, "STICKY");
                    break;
            }
        }
    }
    public void ShowMap()
    {
        SkillsUI.Instance.ShowMap(skillToUse);
    }
    private IEnumerator DelayRPC(string call)
    {
        Debug.Log("SETTING NOANIMATIONSPLAYING TO FALSE");
        photonView.RPC("SetNoAnimationIsPlaying", RpcTarget.All, false);
        yield return new WaitForSeconds(1.6f);
        photonView.RPC(call, RpcTarget.All);
    }
}

