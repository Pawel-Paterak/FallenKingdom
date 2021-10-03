using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingdomData;

public class InventoryManager : MonoBehaviour {

    public static InventoryManager singleton;

    public InventoryPlayer ClientInventory = new InventoryPlayer();
    private Dictionary<int, ContentInventory> inventory = new Dictionary<int, ContentInventory>();
    private List<int> removeInventory = new List<int>();
    public bool refreshRemoveInventory;

    public void Awake()
    {
        singleton = this;
    }
    public void Start()
    {
        
       // BundleManager.addInventory = Add;
       
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("Count Inventory : " + inventory.Keys.Count);
            Debug.Log("LeftHand: " + ClientInventory.leftHand.slots);
        }
        if(refreshRemoveInventory)
        {
            foreach (int coor in removeInventory)
                Remove(coor);
            refreshRemoveInventory = false;
        }

    }

    public void Add(Inventory inventory, string name, InventoryPlayer.TypeInventory typeInventory)
    {
        Debug.LogWarning("I got inventoyr to add ID: " + inventory.Id);
        if (!this.inventory.ContainsKey(inventory.Id))
        {
            if (name == SettingUser.Character.name)
            {
                switch (typeInventory)
                {
                    case InventoryPlayer.TypeInventory.Backpack:
                        ClientInventory.backpack = inventory;
                        break;
                    case InventoryPlayer.TypeInventory.Boots:
                        ClientInventory.boots = inventory;
                        break;
                    case InventoryPlayer.TypeInventory.Breastplate:
                        ClientInventory.breastplate = inventory;
                        break;
                    case InventoryPlayer.TypeInventory.Head:
                        ClientInventory.head = inventory;
                        break;
                    case InventoryPlayer.TypeInventory.LeftHand:
                        ClientInventory.leftHand = inventory;
                        break;
                    case InventoryPlayer.TypeInventory.Legs:
                        ClientInventory.legs = inventory;
                        break;
                    case InventoryPlayer.TypeInventory.RightHand:
                        ClientInventory.rightHand = inventory;
                        break;
                }
                Debug.Log("Add iventory do client");
            }
            this.inventory.Add(inventory.Id, new ContentInventory(name, inventory, typeInventory));
            Debug.Log("Added inventory");
        }
        else
            Debug.Log("Warning i get ID inventory(" + inventory.Id + ") is exists!");
    }
    public void Remove(int id)
    {
        if (inventory.ContainsKey(id))
            inventory.Remove(id);
        else
            Debug.Log("Warning i get ID inventory(" + id + ") is not exists!");
    }
    public void ThreadRemove(int id)
    {
        if (inventory.ContainsKey(id))
            removeInventory.Add(id);
    }
    public void Move(SlotsMovment movment)
    {
        Inventory fromInv = GetInvetory(movment.from.parentId);
        Inventory toInv = GetInvetory(movment.to.parentId);
        Debug.Log("From: "+fromInv.slots);
        Debug.Log("To: "+toInv.slots);
        fromInv.Change(movment.from, movment.roadFrom);
        toInv.Change(movment.to, movment.roadTo);
        Debug.Log("From/To: "+fromInv.slots);
        Debug.Log("To/From: " + toInv.slots);
    }
    public Inventory GetInvetory(int id)
    {
        if (inventory.ContainsKey(id))
            return inventory[id].inventory;
        return null;
    }
}
public class ContentInventory
{
    public string name;
    public Inventory inventory;
    public InventoryPlayer.TypeInventory typeInventory;

    public ContentInventory()
    {

    }
    public ContentInventory(string name, Inventory inventory)
    {
        this.name = name;
        this.inventory = inventory;
    }
    public ContentInventory(string name, Inventory inventory, InventoryPlayer.TypeInventory typeInventory)
    {
        this.name = name;
        this.inventory = inventory;
        this.typeInventory = typeInventory;
    }
}
