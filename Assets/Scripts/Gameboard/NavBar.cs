using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavBar : MonoBehaviour
{
    public GameObject InventoryTab;
    public GameObject SkillsTab;
    public void OnClickInventoryTab()
    {
        InventoryTab.transform.SetSiblingIndex(1);
    }
    public void OnClickSkillsTab()
    {
        SkillsTab.transform.SetSiblingIndex(1);
    }
}
