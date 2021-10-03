using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingdomData;

public static class BundleManager {

    public delegate void _Packet(ByteBuffer data);
    public delegate void _Information(string information);
    public delegate void _Void();
    public delegate void _Character(Character character, bool changePlayers = false);
    public delegate void _InventoryAdd(Inventory inventory, string name, InventoryPlayer.TypeInventory typeInventory);
    public delegate void _InventoryRemove(int id);
    public static Dictionary<int, _Packet> packets = new Dictionary<int, _Packet>();

    public static _Void veryfication;
    public static _Void disconect;
    public static _Information informationLogin;
    public static _Information informationRegister;
    public static _Information informationCreateCharacter;
    public static _Information addCharacter;
    public static _InventoryAdd addInventory;
    public static _InventoryRemove removeInventory;

    public static _Character addPlayer;

    public static List<Character> sleepPlayers = new List<Character>();
    public static List<ContentInventory> sleepInventory = new List<ContentInventory>();
    public static bool refreshPlayers;
    public static bool refreshInventory;
    public static bool isThreadUpdate = false;
    public static Thread threadUpdate = new Thread(Update);
    public static bool isThreadUpdateInventory = false;
    public static Thread threadUpdateInventory = new Thread(UpdateInventory);

    public static void Initialize()
    {
        packets.Add((int)PacketsConnection.PacketClient.Information, InformationPacket);
        packets.Add((int)PacketsConnection.PacketClient.Visible, Visiblepacket);
        packets.Add((int)PacketsConnection.PacketClient.Game, GamePacket);
        packets.Add((int)PacketsConnection.PacketClient.Veryfication, Veryfication);
        packets.Add((int)PacketsConnection.PacketClient.Disconect, Disconect);
        isThreadUpdate = true;
        isThreadUpdateInventory = true;
        if (!threadUpdate.IsAlive)
            threadUpdate.Start();
        if (!threadUpdateInventory.IsAlive)
            threadUpdateInventory.Start();
    }
    public static void Update()
    {
        do
        {
            if (ClientManager.activeScene == "Game" && GameManager.ISettedAll)
            {
                if (refreshPlayers)
                {
                    Debug.Log("Refresh " + sleepPlayers.Count + " Players");
                    Character[] characters = sleepPlayers.ToArray();
                    for (int i = 0; i < characters.Length; i++)
                    {
                        Debug.Log("Add player number: " + (i + 1) + " from " + characters.Length);
                        if (i == (characters.Length - 1))
                            addPlayer(characters[i], true);
                        else
                            addPlayer(characters[i]);
                        sleepPlayers.Remove(characters[i]);
                    }
                    refreshPlayers = false;
                }

                          
            }
        }
        while (isThreadUpdate);
    }
    public static void UpdateInventory()
    {
        do
        {

            if (refreshInventory)
            {
                ContentInventory[] invetoryies = sleepInventory.ToArray();
                int lenght = sleepInventory.ToArray().Length;
                Debug.Log("REFRESH INVENTORY Count: " + lenght);
                refreshInventory = false;
                for (int i = 0; i < lenght; i++)
                {
                    Debug.Log("Invetory " + (i + 1) + " From " + invetoryies.Length);
                    Debug.Log("Invetory: " + (invetoryies[i] != null) + " Id: " + invetoryies[i].inventory.Id);
                    
                    if (invetoryies[i] != null)
                    {
                        // Debug.Log("Type: " + invetoryies[i].typeInventory.ToString());
                        Debug.Log("Add: " + invetoryies[i].name);
                        addInventory(invetoryies[i].inventory, invetoryies[i].name, invetoryies[i].typeInventory);
                       // sleepInventory.RemoveAt(i);
                        sleepInventory.Remove(invetoryies[i]);
                    }
                    
                }
                Debug.Log("Exit from REFRESH INVENTORY count : " + sleepInventory.Count);

            }
            if (sleepInventory.Count > 0)
                refreshInventory = true;
        }
        while (isThreadUpdateInventory);
    }

    public static void InformationPacket(ByteBuffer data)
    {
        int input = data.ReadInteger();
        switch((PacketsConnection.InputClientInformation)input)
        {
            case PacketsConnection.InputClientInformation.Error:
                {

                    break;
                }
            case PacketsConnection.InputClientInformation.Login:
                {
                    string information = data.ReadString();
                    informationLogin(information);
                    break;
                }
            case PacketsConnection.InputClientInformation.Register:
                {
                    string information = data.ReadString();
                    informationRegister(information);
                    break;
                }
            case PacketsConnection.InputClientInformation.CreateCharacter:
                {
                    string information = data.ReadString();
                    informationCreateCharacter(information);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
    public static void Visiblepacket(ByteBuffer data)
    {
        int input = data.ReadInteger();
        switch((PacketsConnection.InputClientVisible)input)
        {
            case PacketsConnection.InputClientVisible.Login:
                {
                    
                    Account account = new Account();
                    account.Read(data);
                    SettingUser.Account = account;
                    break;
                }
            case PacketsConnection.InputClientVisible.Register:
                {

                    break;
                }
            case PacketsConnection.InputClientVisible.AddCharacter:
                {
                    string name = data.ReadString();
                    Debug.Log("I get Character Name: " + name);
                    addCharacter(name);
                    break;
                }
            case PacketsConnection.InputClientVisible.JoinToGame:
                {
                    Character character = new Character();
                    character.Read(data);
                    SettingUser.Character = character;
                    break;
                }
            case PacketsConnection.InputClientVisible.ExitTheGame:
                {
                    string playerName = data.ReadString();
                    GameManager.singleton.RemovePlayerThread(playerName);
                    break;
                }
            default:
                {
                    Debug.Log("Input : " + input + " not exist in Bundle.Visible");
                    break;
                }
        }
    }
    public static void GamePacket(ByteBuffer data)
    {
        int input = data.ReadInteger();
        switch ((PacketsConnection.InputClientGame)input)
        {
            case PacketsConnection.InputClientGame.MovePlayer:
                {
                    string playerName = data.ReadString();
                    Vector3Dfloat newPosition = new Vector3Dfloat();
                    newPosition.Read(data);
                    GameManager.MovePlayers(playerName, newPosition);
           
                    break;
                }
            case PacketsConnection.InputClientGame.ChangeField:
                {
                    switch((PacketsConnection.InputClientChangeField)data.ReadInteger())
                    {
                        case PacketsConnection.InputClientChangeField.Health:
                            {
                                string namePlayer = data.ReadString();
                                int health = data.ReadInteger();

                                foreach (Player player in GameManager.players)
                                    if (player.character.name == namePlayer)
                                    {
                                        player.ChangeHealth(health);
                                        return;
                                    }
                                break;
                            }
                        case PacketsConnection.InputClientChangeField.Mana:
                            {
                                string namePlayer = data.ReadString();
                                int mana = data.ReadInteger();
                                foreach (Player player in GameManager.players)
                                    if (player.character.name == namePlayer)
                                    {
                                        player.ChangeMana(mana);
                                        return;
                                    }
                                break;
                            }
                    }
                    break;
                }
            case PacketsConnection.InputClientGame.Chat:
                {
                    string player = data.ReadString();
                    string message = data.ReadString();
                    GameManager.MessageToChat(message, player);
                    break;
                }
            case PacketsConnection.InputClientGame.PlayersAssets:
                {
                    int count = data.ReadInteger();
                    Debug.Log("I get " + count + " Characters from server.");
                    for (int i = 0; i < count; i++)
                    {
                        Debug.Log("Number: " + (i + 1) + " from " + count);
                        Character character = new Character();
                        data = character.Read(data);
                        Debug.Log("I got character");
                        Debug.Log("Name: " + character.name + " Lvl: " + character.lvl);

                        refreshPlayers = true;
                        sleepPlayers.Add(character);
                    }
                    break;
                }
            case PacketsConnection.InputClientGame.AddInventory:
                {
                    int count = data.ReadInteger();
                    for (int i = 0; i < count; i++)
                    {
                        string name = data.ReadString();
                        InventoryPlayer.TypeInventory typeInvetory = (InventoryPlayer.TypeInventory)data.ReadInteger();
                        Inventory inventory = new Inventory();
                        data = inventory.Read(data);
                        sleepInventory.Add(new ContentInventory(name, inventory, typeInvetory));
                    }
                    refreshInventory = true;
                    break;
                }
            case PacketsConnection.InputClientGame.RemoveInventory:
                {
                    int id = data.ReadInteger();
                    removeInventory(id);
                    Debug.Log("Remove indentory id: "+ id);
                   // InventoryManager.singleton.Remove(id);
                    break;
                }
            case PacketsConnection.InputClientGame.MoveSlots:
                {
                    SlotsMovment movmentSlots = new SlotsMovment();
                    movmentSlots.Read(data);

                    InventoryManager.singleton.Move(movmentSlots);
                    break;
                }
            default:
                {
                    Debug.Log("Input: "+input+" is null");
                    break;
                }
        }
    }
    public static void Veryfication(ByteBuffer data)
    {
        veryfication();
    }
    public static void Disconect(ByteBuffer data)
    {
        disconect();
    }
}