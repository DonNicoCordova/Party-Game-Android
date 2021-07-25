using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBottle : MonoBehaviour
{

    public SpriteRenderer paintColor;
    public Animator animator;
    public void ChangeColor(PlayerController skillUser)
    {
        paintColor.material = skillUser.playerStats.mainColor;
    }
    public void ImpactEvent()
    {
        if (SkillsUI.Instance.playerUsingSkills != null)
        {
            LocationController locationController = gameObject.GetComponentInParent<LocationController>();
            locationController.SetOwner(SkillsUI.Instance.playerUsingSkills.playerStats.id);
        }
    }
    public void ShowMap()
    {
        if (SkillsUI.Instance.playerUsingSkills != null)
        {
            LocationController locationController = gameObject.GetComponentInParent<LocationController>();
            locationController.ShowMap();
            gameObject.SetActive(false);
        }
    }
    public void Drop()
    {
        gameObject.SetActive(true);
        animator.Play("Drop");
    }

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        if (SkillsUI.Instance.playerUsingSkills != null)
        {
            ChangeColor(SkillsUI.Instance.playerUsingSkills);
        }
    }
}
