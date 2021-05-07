using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickTextFix : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float moveAmount = 5f;
    private TMPro.TextMeshProUGUI text;
    private Button button;
    void Start()
    {
        text = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        button = gameObject.GetComponent<Button>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.IsInteractable())
        {
            text.transform.localPosition -= new Vector3(0, moveAmount, 0);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button.IsInteractable())
        {
            text.transform.localPosition -= new Vector3(0, -moveAmount, 0);
        }
    }
}
