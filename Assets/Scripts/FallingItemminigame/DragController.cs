using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DragController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] 
    private Canvas canvas;
    private Rigidbody2D rigid;
    private RectTransform rectTransform;
    public void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }
    public void OnBeginDrag(PointerEventData eventData){
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = 0f;
        rigid.Sleep();
        rigid.gravityScale = 0f;
        Debug.Log("OnBeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log($"ONDRAG. Amount moved {eventData.delta} Scale Factor: {canvas.scaleFactor} Scaled: {eventData.delta / canvas.scaleFactor}");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        rigid.gravityScale = 1f;
        rigid.AddForce(eventData.delta * 5f);
        Debug.Log("OnEndDrag");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = 0f;
        rigid.Sleep();
        rigid.gravityScale = 0f;
        Debug.Log("OnPointerDown");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        rigid.gravityScale = 1f;
        Debug.Log("OnPointerUp");
    }
    public void SetCanvas(Canvas newCanvas)
    {
        canvas = newCanvas;
    }
}