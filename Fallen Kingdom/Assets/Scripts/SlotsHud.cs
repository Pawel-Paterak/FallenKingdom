using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using KingdomData;

public class SlotsHud : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public Text text;
    public Image image;
    public Sprite texture;
    public Slots slots;
    

    void Start()
    {
        image = GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();
    }
    void Update()
    {
        if (slots != null)
        {
            text.text = slots.count.ToString();
            Item item = GameResources.singleton.GetItemFromId(slots.id);
            if (item != null)
                texture = GameResources.singleton.GetTexture(item.nameTexture);
            else
                texture = null;
        }

        if (texture != null)
        {
            image.sprite = texture;
            image.color = new Color(255, 255, 255, 255);
        }
        else
            image.color = new Color(255, 255, 255, 0);
    }

    public void OnDrop(PointerEventData eventData)
    {
        SlotsHud slots = eventData.pointerDrag.GetComponent<SlotsHud>();
        if (slots != null)
        {
            int fromParentId = slots.slots.parentId;
            int toParentId = this.slots.parentId;
            Inventory inventoryFrom = InventoryManager.singleton.GetInvetory(fromParentId);
            List<int> roadFromSlots = new List<int>();
            if (inventoryFrom != null)
                roadFromSlots = inventoryFrom.FindRoad(slots.slots);

            Inventory inventoryTo = InventoryManager.singleton.GetInvetory(toParentId);
            List<int> roadToSlots = new List<int>();
            if (inventoryTo != null)
                roadToSlots = inventoryTo.FindRoad(this.slots);

            if (roadFromSlots == null || roadToSlots == null)
                return;

            Slots.Move(ref slots.slots, ref this.slots);

            MoveSlots move = new MoveSlots(fromParentId, roadFromSlots, toParentId, roadToSlots);
            ClientManager.MoveSlots(move);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            InventoryHud.singleton.ManagerInventory(!InventoryHud.singleton.hudManager.activeSelf, eventData.position, this);
    }
}
