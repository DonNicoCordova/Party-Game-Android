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
        if (!gameManager.throwController.IsThrowFinished())
        {
            if (gameManager.DicesStopped() && sideColliders.Count == 2)
            {
                float totalAmount = 0f;
                Throw actualThrow = new Throw();
                actualThrow.isMainPlayer = true;
                foreach(Collider collider in sideColliders)
                {
                    DiceController controller = collider.GetComponentInParent<DiceController>();
                    actualThrow.throwValues.Add(controller.GetSideStats(collider));
                    totalAmount += 7-controller.GetSideValue(collider);
                }
                resultText.text = totalAmount.ToString();
                gameManager.throwController.FinishedThrow();
                gameManager.AddThrow(actualThrow);
            }
        }

    }

}
