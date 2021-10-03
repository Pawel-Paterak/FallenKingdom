using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingdomData;

public class InventoryHud : MonoBehaviour {

    public List<WindowsInventory> windows = new List<WindowsInventory>();
    public List<HandMadeSlots> handMadeSlots = new List<HandMadeSlots>();
    public GameObject prefabSlots;
    public GameObject prefabWindow;
    public GameObject content;

    public SlotsHud activeSlots;
    public GameObject hudManager;

    public static InventoryHud singleton;
    private InventoryPlayer inventory;

    public void Awake()
    {
        singleton = this;
    }
    public void Start()
    {
        
        if (hudManager != null)
            hudManager.SetActive(false);

        inventory = InventoryManager.singleton.ClientInventory;
    }
    public void Update()
    {
        RefreshEquipment();
    }
    public void RefreshEquipment()
    {
        HandMadeSlots backpack = GetEquipmentSlots(InventoryPlayer.TypeInventory.Backpack);
        if (backpack != null)
            backpack.Change(InventoryManager.singleton.ClientInventory.backpack.slots);

        HandMadeSlots boots = GetEquipmentSlots(InventoryPlayer.TypeInventory.Boots);
        if (boots != null)
            boots.Change(InventoryManager.singleton.ClientInventory.boots.slots);

        HandMadeSlots breastplate = GetEquipmentSlots(InventoryPlayer.TypeInventory.Breastplate);
        if (breastplate != null)
            breastplate.Change(InventoryManager.singleton.ClientInventory.breastplate.slots);

        HandMadeSlots head = GetEquipmentSlots(InventoryPlayer.TypeInventory.Head);
        if (head != null)
            head.Change(InventoryManager.singleton.ClientInventory.head.slots);

        HandMadeSlots leftHand = GetEquipmentSlots(InventoryPlayer.TypeInventory.LeftHand);
        if (leftHand != null)
            leftHand.Change(InventoryManager.singleton.ClientInventory.leftHand.slots);

        HandMadeSlots legs = GetEquipmentSlots(InventoryPlayer.TypeInventory.Legs);
        if (legs != null)
            legs.Change(InventoryManager.singleton.ClientInventory.legs.slots);

        HandMadeSlots rightHand = GetEquipmentSlots(InventoryPlayer.TypeInventory.RightHand);
        if (rightHand != null)
            rightHand.Change(InventoryManager.singleton.ClientInventory.rightHand.slots);
    }
    public int NewWindow(WindowsInventory.TypeWindow typeWindow)
    {
        GameObject window = Instantiate(prefabWindow, content.transform);
        WindowManager windowManager = window.GetComponent<WindowManager>();
        if (windowManager == null)
            return -1;
        int index = windows.Count;
        windows.Add(new WindowsInventory(windowManager, typeWindow));
        return index;
    }
    public int NewWindow(WindowManager windowManager, WindowsInventory.TypeWindow typeWindow)
    {
        if (windowManager == null)
            return -1;

        int index = windows.Count;
        windows.Add(new WindowsInventory(windowManager, typeWindow));
        return index;
    }
    public void ChangeSlotsWindow(int index, Slots slots)
    {
        if (slots == null)
            return;

        WindowsInventory window = GetWindow(index);
        if (window == null || slots.slots.Count < 0)
            return;

        window.ClearSlots();

        foreach (Slots coor in slots.slots)
            window.CreateSlots(coor);
    }
    public WindowsInventory GetWindow(int index)
    {
        if (windows.Count > index && index >= 0)
            return windows[index];
        else
            return null;
    }
    public HandMadeSlots GetEquipmentSlots(InventoryPlayer.TypeInventory typeSlots)
    {
        foreach (HandMadeSlots coor in handMadeSlots)
            if (coor.typeSlots == typeSlots)
                return coor;
        return null;
    }
    public void ManagerInventory(bool enable)
    {
        hudManager.SetActive(enable);
    }
    public void ManagerInventory(bool enable, Vector3 position, SlotsHud slots, bool autoOffset = true)
    {
        if (enable)
        {
            activeSlots = slots;
            Vector3 offset = new Vector3(0, 0, 0);
            if (autoOffset)
            {
                RectTransform transformRect = hudManager.GetComponent<RectTransform>();
                offset = new Vector3(transformRect.sizeDelta.x / 2, -transformRect.sizeDelta.y / 2, 0);
            }
            hudManager.transform.position = position + offset;
        }
        else
            activeSlots = null;

        hudManager.SetActive(enable);
    }

    public void Drop()
    {
        Debug.Log("Drop");
    }
    public void Use ()
    {
        activeSlots.slots.count--;
        Debug.Log("Use");
    }
    public void Open()
    {
        if (activeSlots.slots.slots.Count > 0)
        {
            int index = NewWindow(WindowsInventory.TypeWindow.Inventory);
            ChangeSlotsWindow(index, activeSlots.slots);
        }
        Debug.Log("Open");
    }
}
[Serializable]
public class WindowsInventory
{
    public enum TypeWindow {Inventory, Other}
    public WindowManager window;
    public TypeWindow typeWindow;

    public WindowsInventory(WindowManager window, TypeWindow typeWindow)
    {
        this.window = window;
        this.typeWindow = typeWindow;
    }

    public void ClearSlots()
    {
        if(window != null)
            window.ClearGrid();
    }
    public void CreateSlots(Slots slots)
    {
        if (window == null)
            return;

        GameObject obj = MonoBehaviour.Instantiate(InventoryHud.singleton.prefabSlots, window.transform);
        SlotsHud slotsHud = obj.transform.Find("Item").GetComponent<SlotsHud>();
        if(slotsHud != null)
        {
            slotsHud.slots = slots;
            window.AddToGrid(obj);
            Debug.Log("Add slots");
        }

    }
    public GameObject GetSlots(int index)
    {
        if (window != null && index >= 0 && window.gridObjects.Count > index)
            return window.gridObjects[index];
        else
            return null;
    }
    public List<GameObject> GetSlots()
    {
        if (window != null)
            return window.gridObjects;
        else
            return null;
    }
}
[Serializable]
public class HandMadeSlots
{
    public InventoryPlayer.TypeInventory typeSlots;
    public SlotsHud slots;

    public HandMadeSlots(InventoryPlayer.TypeInventory typeSlots, SlotsHud slots)
    {
        this.typeSlots = typeSlots;
        this.slots = slots;
    }
    public void Change(Slots slots)
    {
        if (this.slots != null && slots != null && this.slots.slots != slots)
            this.slots.slots = slots;
    }
}
