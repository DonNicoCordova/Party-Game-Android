using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChecker : MonoBehaviour
{
    public float searchRadius = 4f;
    private List<MoveButton> moveButtons = new List<MoveButton>();

    public void CheckForButtonsNearby()
    {
        moveButtons.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("MoveButton"))
            {
                AddMoveButton(collider.GetComponent<MoveButton>());
            }

        }
    }
    public void ShowButtons()
    {
        foreach (MoveButton button in moveButtons)
        {
            button.ShowButton();
        }
    }
    public void HideButtons()
    {
        foreach (MoveButton button in moveButtons)
        {
            button.HideButton();
        }
    }
    public void AddMoveButton(MoveButton newMoveButton)
    {
        moveButtons.Add(newMoveButton);
    }
}
