using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Linq;
using TMPro;
using System;

public class SkillsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject skillsPanel;

    [Header("TEST")]
    public Bridge bridgeToLookAt;

    [Header("Skills")]
    public List<SkillInfo> skills = new List<SkillInfo>();
    public List<SkillButton> skillButtons = new List<SkillButton>();
    [SerializeField]
    private GameObject skillsButton;
    public Button backButton;

    public CinemachineSmoothPath cinemachinePath;
    public Animator cinemachineAnimator;
    public CinemachineVirtualCamera highlightCamera;

    [Header("Mini Map")]
    public Animator minimapAnimator;
    public Camera minimapCamera;
    private List<Bridge> bridges;
    private List<LocationController> locations;
    private static SkillsUI instance;
    public static SkillsUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SkillsUI>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(SkillsUI).Name;
                    instance = obj.AddComponent<SkillsUI>();
                }
            }
            return instance;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(Input.mousePosition, Camera.main.transform.TransformDirection(Vector3.forward) * 1000f, Color.yellow, 3f);
            if (Physics.Raycast(ray, out hit, 1 << 10))
            {
                if (hit.collider.gameObject.tag == "BridgeIcon")
                {
                    Bridge bridgeSelected = hit.collider.gameObject.GetComponentInParent<Bridge>();
                    bridgeSelected.ProcessClick();
                }
            }
        }
    }
    public void Initialize()
    {
        Debug.Log("INITIALIZING SKILLS UI");
        for (int i = 0; i < skillButtons.Count; ++i)
        {
            Debug.Log($"SKILL BUTTON {i} : {JsonUtility.ToJson(skillButtons[i])}");
            SkillButton skillButton = skillButtons[i];
            if (i < skills.Count)
            {
                skillButton.InitializeFromSkill(skills[i]);
                skillButton.obj.SetActive(true);
            }
            else
            {
                skillButton.obj.SetActive(false);
            }
        }
        DisableSkillsButton();
    }
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        bridges = new List<Bridge>(GameObject.FindObjectsOfType<Bridge>());
        locations = new List<LocationController>(GameObject.FindObjectsOfType<LocationController>());
    }
    public void HideSkills()
    {
        if (skillsPanel.activeSelf)
        {
            skillsPanel.SetActive(false);
        }
    }
    public void OnClickHideSkills()
    {
        if (skillsPanel.activeSelf)
        {
            skillsPanel.SetActive(false);
        }
        MoveCameraBackToPlayer();
    }
    public void MoveCameraBackToPlayer()
    {
        if (cinemachineAnimator.GetCurrentAnimatorStateInfo(0).IsName("MoveToShowMap") || cinemachineAnimator.GetCurrentAnimatorStateInfo(0).IsName("MoveToPlace"))
        {
            bridges.ForEach(o => o.RemoveClickable());
            cinemachineAnimator.ResetTrigger("ShowMap");
            cinemachineAnimator.ResetTrigger("MoveToPlace");
            cinemachineAnimator.SetTrigger("BackToOrigin");
            GameManager.Instance.ShowMinimap();
        }
    }
    public void OnClickShowSkills()
    {
        if (!skillsPanel.activeSelf)
        {
            foreach(SkillButton skillButton in skillButtons)
            {
                Button button = skillButton.obj.GetComponent<Button>();
                if (skillButton.CanAfford())
                {
                    button.interactable = true;
                } else
                {
                    button.interactable = false;
                }
            }
            skillsPanel.SetActive(true);
        }
    }
    public void OnClickCutBridge()
    {
        ShowMap(SkillToUse.Cut);
    }
    public void OnClickSpawnBridge()
    {
        ShowMap(SkillToUse.Spawn);
    }
    public void OnClickUseBombs()
    {
        ShowMap(SkillToUse.Paint);
    }
    public void OnClickStickyBridge()
    {
        ShowMap(SkillToUse.Sticky);
    }
    public void MoveCameraToHighlight()
    {
        GameManager.Instance.ShowMinimap();
        cinemachineAnimator.ResetTrigger("ShowMap");
        cinemachineAnimator.ResetTrigger("BackToOrigin");
        cinemachineAnimator.SetTrigger("MoveToPlace");
    }
    public SkillInfo GetSkillInfo(SkillToUse skill)
    {
        bool skillExists = skills.Any(info => info.skill == skill);
        if (skillExists)
        {
            return skills.First(info => info.skill == skill);
        }
        else
        {
            return null;
        }
    }
    public void ShowMap(SkillToUse skill)
    {
        HideSkills();
        GameManager.Instance.HideMinimap();
        cinemachineAnimator.ResetTrigger("BackToOrigin");
        cinemachineAnimator.ResetTrigger("MoveToPlace");
        cinemachineAnimator.SetTrigger("ShowMap");
        switch (skill)
        {
            case SkillToUse.Cut:
                bridges.ForEach(o => CanCut(o));
                break;
            case SkillToUse.Spawn:
                bridges.ForEach(o => CanSpawn(o));
                break;
            case SkillToUse.Sticky:
                bridges.ForEach(o => CanGetSticky(o));
                break;
            case SkillToUse.Paint:
                break;
        }
    }
    private void CanSpawn(Bridge bridge)
    {
        if (!bridge.bridgeRenderer.enabled)
            bridge.BecomeClickable(SkillToUse.Spawn);
    }
    private void CanCut(Bridge bridge)
    {
        if (bridge.bridgeRenderer.enabled)
            bridge.BecomeClickable(SkillToUse.Cut);
    }
    private void CanGetSticky(Bridge bridge)
    {
        if (bridge.bridgeRenderer.enabled)
            bridge.BecomeClickable(SkillToUse.Sticky);
    }
    public void DisableSkillsButton()
    {
        Button skills = skillsButton.GetComponent<Button>();
        skills.interactable = false;
    }
    public void EnableSkillsButton()
    {
        Button skills = skillsButton.GetComponent<Button>();
        skills.interactable = true;
    }
}

public enum SkillToUse
{
    Sticky, Cut, Spawn, Paint
}

[System.Serializable]
public class SkillInfo
{
    [SerializeField]
    public string name;
    [SerializeField]
    public string verboseName;
    [SerializeField]
    public SkillToUse skill;
    [SerializeField]
    public string description;
    [SerializeField]
    public int energyCost;
    [SerializeField]
    public Sprite icon;
}

[System.Serializable]
public class SkillButton
{
    public GameObject obj;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI energyCostText;
    public Image skillIcon;
    private SkillInfo associatedSkill;
    public void InitializeFromSkill(SkillInfo skill)
    {
        Debug.Log($"INITIALIAZING BUTTON {obj.name} WITH DATA {JsonUtility.ToJson(skill)}");
        associatedSkill = skill;
        skillNameText.text = skill.verboseName;
        energyCostText.text = $"-{skill.energyCost}";
        skillIcon.sprite = skill.icon;
        Button button = obj.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        switch (skill.skill)
        {
            case SkillToUse.Cut:
                button.onClick.AddListener(SkillsUI.Instance.OnClickCutBridge);
                break;
            case SkillToUse.Paint:
                button.onClick.AddListener(SkillsUI.Instance.OnClickUseBombs);
                button.interactable = false;
                break;
            case SkillToUse.Spawn:
                button.onClick.AddListener(SkillsUI.Instance.OnClickSpawnBridge);
                break;
            case SkillToUse.Sticky:
                button.onClick.AddListener(SkillsUI.Instance.OnClickStickyBridge);
                button.interactable = false;
                break;
        }
    }
    public bool CanAfford()
    {
        if (GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft() >= associatedSkill.energyCost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}