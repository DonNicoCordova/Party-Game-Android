using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Linq;
using TMPro;
using System;
using Photon.Pun;
using Photon.Realtime;

public class SkillsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject skillsPanel;

    public PayingMethod payingMethod = PayingMethod.Energy;

    [Header("Skills")]
    public List<SkillInfo> skills = new List<SkillInfo>();
    public List<SkillButton> skillButtons = new List<SkillButton>();

    [Header("Tokens")]
    public List<Token> tokens = new List<Token>();
    public List<TokenButton> tokenButtons = new List<TokenButton>();

    [SerializeField]
    private GameObject skillsButton;
    public Button backButton;
    public Player playerUsingSkills;
    public bool noAnimationsPlaying = true;

    public CinemachineSmoothPath cinemachinePath;
    public Animator cinemachineAnimator;
    public CinemachineVirtualCamera highlightCamera;

    [Header("Mini Map")]
    public Animator minimapAnimator;
    public Camera minimapCamera;
    public List<Bridge> bridges;
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

    private void FixedUpdate()
    {
        if (playerUsingSkills != null)
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
                        Button minimapButton = hit.collider.gameObject.GetComponent<Button>();
                        if (minimapButton.interactable)
                        {
                            bridgeSelected.ProcessClick();
                        }
                    }
                }
            }
        }
    }
    public void Initialize()
    {
        for (int i = 0; i < skillButtons.Count; ++i)
        {
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
        for (int i = 0; i < tokenButtons.Count; ++i)
        {
            TokenButton tokenButton = tokenButtons[i];
            if (i < tokens.Count)
            {
                tokenButton.InitializeFromToken(tokens[i]);
                tokenButton.obj.SetActive(true);
            }
            else
            {
                tokenButton.obj.SetActive(false);
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
        playerUsingSkills = null;
        if (skillsPanel.activeSelf)
        {
            skillsPanel.SetActive(false);
        }
        MoveCameraBackToPlayer();
    }
    public void MoveCameraBackToPlayer()
    {
        GameManager.Instance.energyCounter.Hide();
        if (cinemachineAnimator.GetCurrentAnimatorStateInfo(0).IsName("MoveToShowMap") || cinemachineAnimator.GetCurrentAnimatorStateInfo(0).IsName("MoveToPlace"))
        {
            playerUsingSkills = null;
            bridges.ForEach(o => o.RemoveClickable());
            cinemachineAnimator.ResetTrigger("ShowMap");
            cinemachineAnimator.ResetTrigger("MoveToPlace");
            cinemachineAnimator.SetTrigger("BackToOrigin");
            GameManager.Instance.ShowMinimap();
            if (GameManager.Instance.ActualPlayerIsMainPlayer())
            {
                GameManager.Instance.GetMainPlayer().buttonChecker.ShowButtons();
            }
        }
    }
    public void OnClickShowSkills()
    {
        playerUsingSkills = GameManager.Instance.GetMainPlayer().photonPlayer;
        GameManager.Instance.energyCounter.Show();
        if (!skillsPanel.activeSelf)
        {
            foreach (SkillButton skillButton in skillButtons)
            {
                Button button = skillButton.obj.GetComponent<Button>();
                if (skillButton.CanAfford())
                {
                    if (skillButton.associatedSkill.skill != Skill.Paint && skillButton.associatedSkill.skill != Skill.Sticky )
                    {
                        button.interactable = true;
                    }
                } else
                {
                    button.interactable = false;
                }
            }
            foreach (TokenButton tokenButton in tokenButtons)
            {
                Button button = tokenButton.obj.GetComponent<Button>();
                if (tokenButton.CanAfford())
                {
                    if (tokenButton.associatedToken.skillToUse != Skill.Paint && tokenButton.associatedToken.skillToUse != Skill.Teleport)
                    {
                        button.interactable = true;
                    }
                }
                else
                {
                    button.interactable = false;
                }
            }
            skillsPanel.SetActive(true);
        }
    }

    //SKILLS BUTTONS
    public void OnClickCutBridge()
    {
        payingMethod = PayingMethod.Energy;
        ShowMap(Skill.Cut);
    }
    public void OnClickSpawnBridge()
    {
        payingMethod = PayingMethod.Energy;
        ShowMap(Skill.Spawn);
    }
    public void OnClickUseBombs()
    {
        payingMethod = PayingMethod.Energy;
        ShowMap(Skill.Paint);
    }
    public void OnClickStickyBridge()
    {
        payingMethod = PayingMethod.Energy;
        ShowMap(Skill.Sticky);
    }

    //INVENTORY BUTTONS
    public void SpendTokenOnMainPlayer(Skill tokenSkillToSpend)
    {
        PlayerController mainPlayer = GameManager.Instance.GetMainPlayer();
        Token tokenToUse = GetToken(tokenSkillToSpend);
        mainPlayer.inventory.SpendToken(tokenToUse);
      
    }
    public void OnClickSpendCutToken()
    {
        payingMethod = PayingMethod.Token;
        ShowMap(Skill.Cut);
    }
    public void OnClickSpendBombToken()
    {
        payingMethod = PayingMethod.Token;
        SpendTokenOnMainPlayer(Skill.Paint);
    }
    public void OnClickSpendSpawnToken()
    {
        payingMethod = PayingMethod.Token;
        ShowMap(Skill.Spawn);
    }
    public void OnClickSpendTeleportToken()
    {
        payingMethod = PayingMethod.Token;
        SpendTokenOnMainPlayer(Skill.Teleport);
    }
    public void OnClickSpendAddToken()
    {
        payingMethod = PayingMethod.Token;
        GameManager.Instance.GetMainPlayer().playerStats.AddEnergy(3);
    }
    public void OnClickSpendStealToken()
    {
        payingMethod = PayingMethod.Token;
        SpendTokenOnMainPlayer(Skill.StealEnergy);
    }

    //Actions
    public void MoveCameraToHighlight()
    {
        GameManager.Instance.ShowMinimap();
        cinemachineAnimator.ResetTrigger("ShowMap");
        cinemachineAnimator.ResetTrigger("BackToOrigin");
        cinemachineAnimator.SetTrigger("MoveToPlace");
    }
    public SkillInfo GetSkillInfo(Skill skill)
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
    public Token GetToken(Skill skill)
    {
        bool tokenExists = tokens.Any(info => info.skillToUse == skill);
        if (tokenExists)
        {
            return tokens.First(info => info.skillToUse == skill);
        }
        else
        {
            return null;
        }
    }
    public void ShowMap(Skill skill)
    {
        HideSkills();
        GameManager.Instance.HideMinimap();
        cinemachineAnimator.ResetTrigger("BackToOrigin");
        cinemachineAnimator.ResetTrigger("MoveToPlace");
        cinemachineAnimator.SetTrigger("ShowMap");
        if (bridges.Count == 0)
        {
            bridges = GameObject.FindObjectsOfType<Bridge>().ToList();
        }
        
        switch (skill)
        {
            case Skill.Cut:
                bridges.ForEach(o => CanCut(o));
                break;
            case Skill.Spawn:
                bridges.ForEach(o => CanSpawn(o));
                break;
            case Skill.Sticky:
                bridges.ForEach(o => CanGetSticky(o));
                break;
            case Skill.Paint:
                break;
        }
    }
    private void CanSpawn(Bridge bridge)
    {
        if (!bridge.bridgeRenderer.enabled) { 
            bridge.BecomeClickable(Skill.Spawn);
        } else
        {
            bridge.RemoveClickable();
        }
    }
    private void CanCut(Bridge bridge)
    {
        if (bridge.bridgeRenderer.enabled)
        {

            bridge.BecomeClickable(Skill.Cut);
        } else
        {
            bridge.RemoveClickable();
        }
    }
    private void CanGetSticky(Bridge bridge)
    {
        if (bridge.bridgeRenderer.enabled)
        {
            bridge.BecomeClickable(Skill.Sticky);
        } else
        {
            bridge.RemoveClickable();
        }
    }
    public void DisableSkillsButton()
    {
        Button skills = skillsButton.GetComponent<Button>();
        if (skills.interactable)
        {
            skills.interactable = false;
        }
    }
    public void EnableSkillsButton()
    {
        Button skills = skillsButton.GetComponent<Button>();
        if (!skills.interactable)
        {
            skills.interactable = true;
        }
    }
}

public enum Skill
{
    None, Sticky, Cut, Spawn, Paint, AddEnergy, StealEnergy, Teleport
}
public enum PayingMethod
{
    None, Energy, Token
}

[Serializable]
public class SkillInfo
{
    [SerializeField]
    public string name;
    [SerializeField]
    public string verboseName;
    [SerializeField]
    public Skill skill;
    [SerializeField]
    public string description;
    [SerializeField]
    public int energyCost;
    [SerializeField]
    public Sprite icon;
    [SerializeField]
    public Sprite tokenIcon;
}

[Serializable]
public class SkillButton
{
    public GameObject obj;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI energyCostText;
    public Image skillIcon;
    public SkillInfo associatedSkill;
    public void InitializeFromSkill(SkillInfo skill)
    {
        associatedSkill = skill;
        skillNameText.text = skill.verboseName;
        energyCostText.text = $"-{skill.energyCost}";
        skillIcon.sprite = skill.icon;
        Button button = obj.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        switch (skill.skill)
        {
            case Skill.Cut:
                button.onClick.AddListener(SkillsUI.Instance.OnClickCutBridge);
                break;
            case Skill.Paint:
                button.onClick.AddListener(SkillsUI.Instance.OnClickUseBombs);
                button.interactable = false;
                break;
            case Skill.Spawn:
                button.onClick.AddListener(SkillsUI.Instance.OnClickSpawnBridge);
                break;
            case Skill.Sticky:
                button.onClick.AddListener(SkillsUI.Instance.OnClickStickyBridge);
                button.interactable = false;
                break;
            case Skill.None:
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

[System.Serializable]
public class TokenButton
{
    public GameObject obj;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI amountText;
    public Image skillIcon;
    public Token associatedToken;
    public void InitializeFromToken(Token token)
    {
        associatedToken = token;
        skillNameText.text = token.verboseName;
        amountText.text = $"{GameManager.Instance.GetMainPlayer().inventory.GetTokenAmount(token.code)}";
        skillIcon.sprite = token.iconWithBackground;
        Button button = obj.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        switch (token.skillToUse)
        {
            case Skill.Cut:
                button.onClick.AddListener(SkillsUI.Instance.OnClickSpendCutToken);
                break;
            case Skill.Paint:
                button.onClick.AddListener(SkillsUI.Instance.OnClickSpendBombToken);
                button.interactable = false;
                break;
            case Skill.Spawn:
                button.onClick.AddListener(SkillsUI.Instance.OnClickSpendSpawnToken);
                break;
            case Skill.AddEnergy:
                button.onClick.AddListener(SkillsUI.Instance.OnClickSpendAddToken);
                button.interactable = false;
                break;
            case Skill.StealEnergy:
                button.onClick.AddListener(SkillsUI.Instance.OnClickSpendStealToken);
                button.interactable = false;
                break;
            case Skill.Teleport:
                button.onClick.AddListener(SkillsUI.Instance.OnClickSpendTeleportToken);
                button.interactable = false;
                break;
        }
        PlayerController mainPlayer = GameManager.Instance.GetMainPlayer();
        mainPlayer.inventory.TokensChanged += (sender, args) => TokenChangeHandler(args);
    }

    private void TokenChangeHandler(Inventory.TokensChangedArgs args)
    {
        if (args.TokenCode == associatedToken.code)
        {
            UpdateAmount();
        }
    }
    public void UpdateAmount()
    {
        amountText.text = $"{GameManager.Instance.GetMainPlayer().inventory.GetTokenAmount(associatedToken.code)}";
    }
    public bool CanAfford()
    {
        if (GameManager.Instance.GetMainPlayer().inventory.GetTokenAmount(associatedToken.code) > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}