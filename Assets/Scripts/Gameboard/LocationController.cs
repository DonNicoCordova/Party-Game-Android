using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;

public class LocationController : MonoBehaviourPunCallbacks
{
    public Transform waypoint;
    public MeshRenderer locationDome;
    public SpriteRenderer minimapIcon;
    public LocationSelectionEffect selectionEffect;
    public Skill skill;
    public Transform cameraHighlighPosition;
    public PaintBottle paintBottle;
    private PlayerController owner = null;
    private List<PlayerController> playersOnTop = new List<PlayerController>();
    private Queue<int> setOwnerQueue = new Queue<int>();
    public void Update()
    {
        if (setOwnerQueue.Count > 0)
        {
            int newOwnerId = setOwnerQueue.Peek();
            StartCoroutine(setOwnerProcess(newOwnerId));
        }
    }
    public void ChangeColor(PlayerController newOwner)
    {
        
        locationDome.material = newOwner.playerStats.orbColor;
        minimapIcon.material = newOwner.playerStats.mainColor;
    }
    [PunRPC]
    public void SetOwner(int newOwnerId)
    {
        setOwnerQueue.Enqueue(newOwnerId);
    }
    private IEnumerator setOwnerProcess(int newOwnerId)
    {
        if ( GameManager.Instance.players.Count > 0 && GameManager.Instance.GetPlayer(newOwnerId) != null)
        {
            if (owner != null)
            {
                owner.playerStats.RemoveCapturedZones(this);
            }
            PlayerController newOwner = GameManager.Instance.GetPlayer(newOwnerId);
            owner = newOwner;
            newOwner.playerStats.AddCapturedZone(this);
            ChangeColor(newOwner);
            setOwnerQueue.Dequeue();
        }
        yield return new WaitForSeconds(1f);
    }
    public PlayerController GetOwner()
    {
        return owner;
    }
    public void AddPlayer(PlayerController newPlayer)
    {
        playersOnTop.Add(newPlayer);
    }
    public void RemovePlayer(PlayerController newPlayer)
    {
        if (playersOnTop.Contains(newPlayer))
        {
            playersOnTop.Remove(newPlayer);
        }
    }
    public bool CheckIfPlayerOnTop(PlayerController newPlayer)
    {
        return playersOnTop.Contains(newPlayer);
    }
    public void BecomeClickable(Skill mode)
    {
        Button button = minimapIcon.GetComponent<Button>();
        button.interactable = true;
        minimapIcon.enabled = true;
        selectionEffect.EnableAnimator();
        minimapIcon.sortingOrder += 10;
        skill = mode;
        photonView.RPC("SetSkill", RpcTarget.Others, mode);
    }
    public void RemoveClickable()
    {
        selectionEffect.DisableAnimator();
        minimapIcon.sortingOrder -= 10;
        Button button = minimapIcon.GetComponent<Button>();
        button.interactable = false;
        button.onClick.RemoveAllListeners();
    }
    public void CanPaint(PlayerController newOwner)
    {
        Debug.Log($"CALLING CAN PAINT {this.name} WIHT NEW OWNER: {newOwner.playerStats.id}");
        if (newOwner != owner)
        {
            Debug.Log("BECOMING CLICKABLE WITH SKILL PAINT");
            BecomeClickable(Skill.Paint);
        }
        else
        {
            Debug.Log("REMOVING CLICKABLE WITH SKILL PAINT");
            RemoveClickable();
        }
    }
    public void ProcessClick()
    {
        if (SkillsUI.Instance.noAnimationsPlaying)
        {
            SkillInfo skillInfo = SkillsUI.Instance.GetSkillInfo(skill);
            Token token = SkillsUI.Instance.GetToken(skill);
            bool canAfford = false;
            if (SkillsUI.Instance.payingMethod == PayingMethod.Token)
                canAfford = GameManager.Instance.GetMainPlayer().inventory.GetTokenAmount(token.code) > 0;
            else if (SkillsUI.Instance.payingMethod == PayingMethod.Energy)
                canAfford = skillInfo.energyCost <= GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft();

            if (skillInfo != null)
            {
                if (canAfford)
                {
                    switch (skill)
                    {
                        case Skill.Paint:
                            photonView.RPC("MoveCameraToHighlightArea", RpcTarget.All);
                            if (SkillsUI.Instance.payingMethod == PayingMethod.Token)
                            {
                                SkillsUI.Instance.SpendTokenOnMainPlayer(skill);
                            }
                            else if (SkillsUI.Instance.payingMethod == PayingMethod.Energy)
                            {
                                GameManager.Instance.GetMainPlayer().playerStats.ReduceEnergy(skillInfo.energyCost, "PAINT");
                            }
                            StartCoroutine(DelayRPC("Drop"));
                            break;
                    }
                }
            }
        }
    }
    public void ShowMap()
    {
        LocationController locationController = gameObject.GetComponentInParent<LocationController>();

        locationController.RemoveClickable();
        if (GameManager.Instance.ActualPlayerIsMainPlayer())
        {
            GameManager.Instance.GetMainPlayer().buttonChecker.HideButtons();
            GameManager.Instance.GetMainPlayer().buttonChecker.CheckForButtonsNearby();
        }
        SkillInfo skillInfo = SkillsUI.Instance.GetSkillInfo(locationController.skill);
        Token skillToken;
        if (skillInfo == null)
        {
            skillToken = SkillsUI.Instance.GetToken(Skill.None);
        }
        else
        {
            skillToken = SkillsUI.Instance.GetToken(skillInfo.skill);
        }
        if (GameManager.Instance.GetMainPlayer() != SkillsUI.Instance.playerUsingSkills)
        {
            SkillsUI.Instance.MoveCameraBackToPlayer();
        }
        else if (SkillsUI.Instance.payingMethod == PayingMethod.Energy && (GameManager.Instance.GetMainPlayer().playerStats.EnergyLeft() >= skillInfo.energyCost))
        {
            SkillsUI.Instance.ShowMap(locationController.skill);
        }
        else if (SkillsUI.Instance.payingMethod == PayingMethod.Token && GameManager.Instance.GetMainPlayer().inventory.GetTokenAmount(skillToken.code) > 0)
        {
            SkillsUI.Instance.ShowMap(locationController.skill);
        }
        else
        {
            SkillsUI.Instance.MoveCameraBackToPlayer();
            SkillsUI.Instance.backButton.onClick.Invoke();
        }
        //GameboardRPCManager.Instance.surface.BuildNavMesh();
        photonView.RPC("SetNoAnimationIsPlaying", RpcTarget.All, true);

    }
    private IEnumerator DelayRPC(string call)
    {
        photonView.RPC("SetNoAnimationIsPlaying", RpcTarget.All, false);
        yield return new WaitForSeconds(1.6f);
        photonView.RPC(call, RpcTarget.All);
    }
    [PunRPC]
    public void SetNoAnimationIsPlaying(bool state)
    {
        SkillsUI.Instance.noAnimationsPlaying = state;
    }
    [PunRPC]
    public void Drop()
    {
        paintBottle.Drop();
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
}
