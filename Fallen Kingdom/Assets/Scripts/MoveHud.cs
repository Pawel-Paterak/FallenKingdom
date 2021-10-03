using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveHud : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public CanvasGroup group;
    public Transform parent;
    public Transform parentParent;
    public Transform contentDragSlots;

    public Vector2 offsetCursor;

    public void Start()
    {
        parent = transform.parent;
        parentParent = parent.parent;
        contentDragSlots = GameObject.Find("Content_Slots").transform;
        if (contentDragSlots == null)
            contentDragSlots = parentParent;

        group = GetComponent<CanvasGroup>();
        group.blocksRaycasts = true;
    }

    public void Update()
    {
      
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        group.blocksRaycasts = false;
        transform.SetParent(contentDragSlots);
        offsetCursor = new Vector3(eventData.position.x, eventData.position.y, 0) - transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position-new Vector2(offsetCursor.x, offsetCursor.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        group.blocksRaycasts = true;
        transform.SetParent(parent);
    }
}
