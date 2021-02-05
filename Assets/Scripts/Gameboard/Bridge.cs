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

    [Header("Minimap")]
    public SpriteRenderer minimapIcon;
    public SpriteRenderer minimapSelectableIcon;
    public bool usable = true;
    [SerializeField]
    private Animator animator;
    private Transform originalPosition;
    private int energyCost = 0;
    private Vector3 originalScale;
    public SkillToUse skillToUse;

    private void Start()
    {
        bridgeRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        if (!bridgeRenderer.enabled)
        {
            usable = false;
        }
        animator = gameObject.GetComponentInChildren<Animator>();
        originalPosition = transform;
        if (minimapIcon != null)
        {
            originalScale = minimapIcon.transform.localScale;
        }
    }
    [PunRPC]
    public void CutOut()
    {
        Vector3 position1 = cutPosition1.position;
        Vector3 position2 = cutPosition2.position;
        animator.SetTrigger("Fall");
        usable = false;
    }
    [PunRPC]
    public void Spawn()
    {
        if (!usable)
        {
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
        if (!minimapSelectableIcon.gameObject.activeSelf)
        {
            Button button = minimapIcon.GetComponent<Button>();
            minimapIcon.gameObject.SetActive(true);
            minimapSelectableIcon.gameObject.SetActive(true);
            minimapIcon.sortingOrder += 4;
            minimapSelectableIcon.sortingOrder += 4;
            skillToUse = mode;
            if (mode == SkillToUse.Spawn)
            {
                minimapIcon.enabled = true;
            }
        }
    }
    public void RemoveClickable()
    {
        if (minimapSelectableIcon.gameObject.activeSelf)
        {
            if (!bridgeRenderer.enabled) {
                minimapIcon.gameObject.SetActive(false);
            }
            minimapSelectableIcon.gameObject.SetActive(false);
            minimapIcon.sortingOrder -= 4;
            minimapSelectableIcon.sortingOrder -= 4;
            Button button = minimapIcon.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
        }
    }

    public void ProcessClick()
    {
        SkillInfo skillInfo = SkillsUI.Instance.GetSkillInfo(skillToUse);
        if (skillInfo.energyCost <= GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft())
        {
            switch (skillToUse)
            {
                case SkillToUse.Cut:
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
        RemoveClickable();
        SkillsUI.Instance.ShowMap(skillToUse);
    }
    private IEnumerator DelayRPC(string call)
    {
        yield return new WaitForSeconds(1.8f);
        photonView.RPC(call, RpcTarget.All);
    }
}

