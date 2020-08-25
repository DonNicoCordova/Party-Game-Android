using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceCheckZoneController : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    // Start is called before the first frame update
    private List<Collider> sideColliders = new List<Collider>();
    private GameManager gameManager;
 
    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        sideColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (sideColliders.Contains(other))
        {
            sideColliders.Remove(other);
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.DicesStopped() && sideColliders.Count != 0)
        {
            float totalAmount = 0f;
            foreach(Collider collider in sideColliders)
            {
                DiceController controller = collider.GetComponentInParent<DiceController>();
                totalAmount += 7-controller.GetSideValue(collider);
            }
            resultText.text = totalAmount.ToString();
            gameManager.throwController.FinishedThrow();
        }

    }

}
