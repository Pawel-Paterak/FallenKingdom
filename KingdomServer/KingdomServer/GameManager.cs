using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomData;

namespace KingdomServer
{
    public class GameManager
    {
        public GameSettings settingsGame;
        public World world;
        public ItemsManager itemsManager;
        public List<MapMonster> monsters = new List<MapMonster>();
        private Dictionary<int, Inventory> inventory = new Dictionary<int, Inventory>();

        public static GameManager singleton;

        public GameManager()
        {
            singleton = this;
            ResourcesManager assets = new ResourcesManager();

            GameSettings settingsGame = assets.Open<GameSettings>(ResourcesManager.PathGameSettings);
            if (settingsGame != null)
            {
                this.settingsGame = settingsGame;
                ResourcesManager.PathLibraryItems += settingsGame.nameLibraryItems;
            }
            else
                throw new Exception("File not founded \"" + ResourcesManager.PathGameSettings + "\"");

            itemsManager = new ItemsManager();
            
            world = assets.Open<World>(ResourcesManager.PathWorlds + settingsGame.nameWorld + ".xml");
        }
        public bool Collision(Vector3Dfloat position)
        {
            MapTerrain terrain = world.GetTerrainFromPosition(position);
            if (terrain == null)
                return true;
            else if (terrain.blocking)
                return true;

            MapObject obj = world.GetObjectFromPosition(position);
            if (obj != null)
                return true;
            return false;
        }
        public void AddInventory(ref Inventory inventory, string name = "", InventoryPlayer.TypeInventory typeInventory  = InventoryPlayer.TypeInventory.Null)
        {
            int idInventory = 0;
            bool next = true;
            while (next)
            {
                if (!this.inventory.ContainsKey(idInventory))
                {
                    inventory.Id = idInventory;
                    if (TcpServer.singleton.settingsServer.debugWrite == 3)
                        Console.WriteLine("Add inventory ID:" + inventory.Id);
                    this.inventory.Add(idInventory, inventory);
                    AddInventoryToAllClients(inventory, name, typeInventory);
                    next = false;
                }
                else
                    idInventory++;
            }
        }
        public void AddInventory(ref List<Inventory> inventory, List<InventoryPlayer.TypeInventory> typeInventory, List<string> name)
        {
            Console.WriteLine("Create Inventory count: " + inventory.Count);

            if (typeInventory.Count < inventory.Count || name.Count < inventory.Count)
                return;

            for(int i=0; i<inventory.Count; i++)
            {
                int idInventory = 0;
                bool next = true;
                while (next)
                {
                    if (!this.inventory.ContainsKey(idInventory))
                    {
                        inventory[i].Id = idInventory;
                        if (TcpServer.singleton.settingsServer.debugWrite == 3)
                            Console.WriteLine("Add inventory ID:" + inventory[i].Id);
                        this.inventory.Add(idInventory, inventory[i]);
                        next = false;
                    }
                    else
                        idInventory++;
                }
            }
            AddInventoryToAllClients(inventory, name, typeInventory);
        }
        public void AddInventoryToAllClients(Inventory inventory, string name, InventoryPlayer.TypeInventory typeInventory)
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteInteger(1);
            data.WriteString(name);
            data.WriteInteger((int)typeInventory);
            data.WriteBytes(inventory.Write().ToArray());
            Console.WriteLine("Send inventory: " + name + " type: " + typeInventory + " Slots: "+inventory.slots);
            TcpServer.singleton.SendDataToAllClients(data, PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.AddInventory, true);
        }
        public void AddInventoryToAllClients(List<Inventory> inventory, List<string> name, List<InventoryPlayer.TypeInventory> typeInventory)
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteInteger(inventory.Count);
            for (int i = 0; i < inventory.Count; i++)
            {
                data.WriteString(name[i]);
                data.WriteInteger((int)typeInventory[i]);
                data.WriteBytes(inventory[i].Write().ToArray());
                Console.WriteLine("Send inventory: " + name[i] + " type: " + typeInventory[i] + " Slots: " + inventory[i].slots);
            }
            TcpServer.singleton.SendDataToAllClients(data, PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.AddInventory, true);
        }
        public void RemoveInventoryToAllClients(int id)
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteInteger(id);
            Console.WriteLine("Send Id_inventory to all clients");
            TcpServer.singleton.SendDataToAllClients(data, PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.RemoveInventory, true);
        }
        public void RemoveInventoryFromId(int id)
        {
            Console.WriteLine("Remove invetory id: " + id);
            if (inventory.ContainsKey(id))
            {
                if (TcpServer.singleton.settingsServer.debugWrite == 3)
                    Console.WriteLine("Remove inventory ID:" + id);
                inventory.Remove(id);
               // RemoveInventoryToAllClients(id);
            }
        }
        public Inventory GetInventory(int index)
        {
            if (index >= 0 && inventory.ContainsKey(index))
                return inventory[index];
            return null;
        }
    }
    public class GameSettings
    {
        public string nameWorld { get; set; }
        public string nameLibraryItems { get; set; }
        public float multiplierExp { get; set; } = 1;
        public float notMoveTimeOut { get; set; } = 60;

        public GameSettings()
        {

        }
    }
    public class ItemsManager
    {
        public Dictionary<int,Item> items = new Dictionary<int, Item>();
        public bool loadedLibraryItems;

        public ItemsManager()
        {
            ResourcesManager assets = new ResourcesManager();
            if (!string.IsNullOrEmpty(GameManager.singleton.settingsGame.nameLibraryItems))
            {
                Paths library = assets.Open<Paths>(ResourcesManager.PathLibraryItems);
                if (library != null)
                {
                    ResourcesManager asstes = new ResourcesManager();
                    int wrongItems = 0;
                    foreach (Path coor in library.paths)
                    {
                        Item item = null;
                        switch (coor.typeItem)
                        {
                            case Item.TypeItem.Item:
                                {
                                    Item file = assets.Open<Item>(ResourcesManager.PathItems + coor.path + ".xml");
                                    if (file != null)
                                        item = file;
                                    else
                                        wrongItems++;
                                    break;
                                }
                            case Item.TypeItem.Weapon:
                                {
                                    Weapon file = assets.Open<Weapon>(ResourcesManager.PathItems + coor.path + ".xml");
                                    if (file != null)
                                        item = file;
                                    else
                                        wrongItems++;
                                    break;
                                }
                            case Item.TypeItem.Food:
                                {
                                    Food file = assets.Open<Food>(ResourcesManager.PathItems + coor.path + ".xml");
                                    if (file != null)
                                        item = file;
                                    else
                                        wrongItems++;
                                    break;
                                }
                        }
                        Item itemLibrary = GetItemFromId(coor.id);
                        if (itemLibrary != null)
                        {
                            MainClass.Print("Two items have " + coor.id + " Id", ConsoleColor.Red);
                            MainClass.Print("Item: Name " + itemLibrary.name);
                            MainClass.Print("Item: Name " + item.name);
                        }
                        else
                        {
                            Console.WriteLine("Loaded item " + item.name + " id: " + coor.id);
                            items.Add(coor.id, item);
                        }
                    }
                    if (wrongItems > 0)
                        MainClass.Print("Not loaded Items Count: " + wrongItems, ConsoleColor.Red);

                    loadedLibraryItems = true;
                }
            }
            else
                loadedLibraryItems = false;
        }
        public Item GetItemFromId(int id)
        {
            if (items.ContainsKey(id))
            {
                Console.WriteLine("Return item " + items[id].name + " id: " + id);
                return items[id];
            }
            return null;
        }
    }
}
