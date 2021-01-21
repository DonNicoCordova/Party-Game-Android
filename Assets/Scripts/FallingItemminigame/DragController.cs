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
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        rigid.gravityScale = 1f;
        rigid.AddForce(eventData.delta * 5f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = 0f;
        rigid.Sleep();
        rigid.gravityScale = 0f;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        rigid.gravityScale = 1f;
    }
    public void SetCanvas(Canvas newCanvas)
    {
        canvas = newCanvas;
    }
}