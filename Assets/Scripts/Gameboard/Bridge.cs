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
            SkillsUI.Instance.noAnimationsPlaying = false;
            SkillsUI.Instance.playerUsingSkills = info.Sender;
            animator.SetTrigger("Fall");
            usable = false;
        }
    }
    [PunRPC]
    public void Spawn(PhotonMessageInfo info)
    {
        if (!usable)
        {
            SkillsUI.Instance.noAnimationsPlaying = false;
            SkillsUI.Instance.playerUsingSkills = info.Sender;
            animator.SetTrigger("Spawn");
        }
    }
    [PunRPC]
    public void BecomeSticky()
    {
        energyCost = 3;
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
            minimapIcon.enabled = false ;
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
        SkillInfo skillInfo = SkillsUI.Instance.GetSkillInfo(skillToUse);
        if (skillInfo.energyCost <= GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft())
        {
            switch (skillToUse)
            {
                case SkillToUse.Cut:
                    Debug.Log($"PROCESSING CUT {gameObject.name}");
                    photonView.RPC("MoveCameraToHighlightArea", RpcTarget.All);
                    GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost);
                    StartCoroutine(DelayRPC("CutOut"));
                    break;
                case SkillToUse.Spawn:
                    photonView.RPC("MoveCameraToHighlightArea", RpcTarget.All);
                    GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost);
                    StartCoroutine(DelayRPC("Spawn"));
                    break;
                case SkillToUse.Sticky:
                    GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost);
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
        yield return new WaitForSeconds(1.8f);
        photonView.RPC(call, RpcTarget.All);
    }
}

