using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KingdomData;

public class GameManager : MonoBehaviour
{

    public GameObject camera;

    [Header("Players")]
    public static List<Player> players = new List<Player>();
    public List<Character> threadPlayers = new List<Character>();
    public List<string> threadRemovePlayers = new List<string>();
    public bool refreshRemovePlayers;
    public bool changePlayers;
    [Header("Chat")]
    public InputField inputChat;
    public Color colorChat;
    public GameObject informationChat;
    public List<ChatTextField> fieldsChat = new List<ChatTextField>();
    public static List<ChatThreadMessage> chatNewMessages = new List<ChatThreadMessage>();
    public static bool refreshChat;

    public static GameManager singleton;

    public static bool ISettedAll = false;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    [Header("Contains")]
    public GameObject worldContain;
    public GameObject playersContain;

    private void Awake()
    {
        singleton = this;
    }
    void Start()
    {
        BundleManager.addPlayer = AddPlayersThread;
        BundleManager.addInventory = InventoryManager.singleton.Add;
        BundleManager.removeInventory = InventoryManager.singleton.Remove;

        ISettedAll = true;
    }
    void Update()
    {

        if (!inputChat.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Return) && inputChat.text.Length > 0)
            {
                ClientManager.SendMessageToChat(inputChat.text);
                inputChat.text = "";
            }
        }

        if (changePlayers)
        {
            int lenght = this.threadPlayers.Count;
            Character[] threadPlayers = this.threadPlayers.ToArray();
            for (int i = 0; i < lenght; i++)
            {
                Debug.Log(threadPlayers[i].name + " " + i + "/" + (lenght + 1));
                AddPlayer(threadPlayers[i]);
                this.threadPlayers.Remove(threadPlayers[i]);
            }
            changePlayers = false;
        }

        RefreshPlayerPosition();
        RefreshFieldsPlayer();
        if (refreshChat)
            RefreshChat();
        if(refreshRemovePlayers)
        {
            refreshRemovePlayers = false;
            foreach(string playerName in threadRemovePlayers.ToArray())
            {
                for(int i=0; i<players.ToArray().Length; i++)
                    if(players[i].character.name == playerName)
                    {
                        Destroy(players[i].player);
                        RemovePlayer(players[i]);
                        threadRemovePlayers.Remove(playerName);
                        break;
                    }
            }
        }
    }
    public void RefreshPlayerPosition()
    {
        foreach(Player coor in players.ToArray())
            if(coor.refreshPosition)
            {
                coor.refreshPosition = false;
                Vector3 position = coor.character.position.Vector3d();
                position.y = coor.player.transform.localScale.y;
                coor.player.transform.position = position;
            }
    }
    public void RefreshFieldsPlayer()
    {
        foreach (Player coor in players.ToArray())
            if (coor.refreshFields)
            {
                coor.refreshFields = false;
                coor.health.ChangeHealth(coor.character.life);
                coor.health.ChangeMana(coor.character.mana);
            }
    }
    public void RefreshChat()
    {
        refreshChat = false;
        for (int i=fieldsChat.Count;i>0; i--)
        {
            if ((i-1) == 0)
            {
                fieldsChat[(i - 1)].message = chatNewMessages[0].message;
                foreach(Player player in players)
                    if(player.character.name == chatNewMessages[0].player)
                    {
                        GameObject msgObj = Instantiate(informationChat, player.player.transform.position, Quaternion.Euler(90, 0, 0));
                        TextInformation text = msgObj.GetComponent<TextInformation>();
                        text.information = chatNewMessages[0].message;
                        text.destroy = true;
                        text.destroyTime = 5;
                        text.colorText = colorChat;
                        text.flyUp = false;
                        break;
                    }
                chatNewMessages.Remove(chatNewMessages[0]);
            }
            else if((i - 2) >= 0)
                fieldsChat[(i - 1)].message = fieldsChat[(i - 2)].message;
        }
        if (chatNewMessages.Count > 0)
            RefreshChat();
    }
    public void AddPlayer(Character character)
    {
        GameObject player = Instantiate(playerPrefab, character.position.Vector3d(), Quaternion.Euler(0, 0, 0), playersContain.transform);
        PlayerSettings settings = player.GetComponent<PlayerSettings>();
        settings.Initialize(character);
        Player endPlayer = new Player(player, settings);
        players.Add(endPlayer);

        if(character.name == SettingUser.Character.name)
        {
            camera.transform.position = new Vector3(player.transform.position.x, camera.transform.position.y, player.transform.position.z - 4.5f); 
            camera.transform.SetParent(player.transform);
        }
    }
    public void AddPlayersThread(Character character, bool changePlayers = false)
    {
        Debug.Log("[GameManager] I get character.");
        threadPlayers.Add(character);
        this.changePlayers = changePlayers;
    }
    public void RemovePlayerThread(string name)
    {
        threadRemovePlayers.Add(name);
        refreshRemovePlayers = true;
    }
    public void RemovePlayer(Player player)
    {
        if (players.Contains(player))
            players.Remove(player);
    }
    public void RemovePlayer(Character character)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].character == character)
            {
                players.RemoveAt(i);
                return;
            }
        }
    }
    public void RemovePlayerFromIndex(int index)
    {
        players.RemoveAt(index);
    }
    public static void MovePlayers(string name, Vector3Dfloat position)
    {
        foreach (Player coor in players.ToArray())
        {
            if (coor.character.name == name)
            {
                coor.Move(position.Vector3d());
                break;
            }
        }
    }
    public static void MessageToChat(string message, string player)
    {
        chatNewMessages.Add(new ChatThreadMessage(player, message));
        refreshChat = true;
    }
}
[System.Serializable]
public class Player
{
    public GameObject player;

    public Character character;
    public Health health;
    public bool refreshPosition;
    public bool refreshFields;
    public Player()
    {

    }
    public Player(GameObject player, PlayerSettings settings)
    {
        this.player = player;
        character = settings.character;
        health = settings.health;
    }

    public void Move(Vector3 newPosition)
    {
        character.position = newPosition.Vector3dfloat();
        refreshPosition = true;
    }
    public void ChangeHealth(int health)
    {
        character.life = health;
        refreshFields = true;
    }
    public void ChangeMana(int mana)
    {
        character.mana = mana;
        refreshFields = true;
    }
}
[System.Serializable]
public class ChatTextField
{
    public Text text;
    public string message {
        get
        {
            return text.text;
        }
        set
        {
            text.text = value;
        }
    }
}
[System.Serializable]
public class ChatThreadMessage
{
    public string player;
    public string message;

    public ChatThreadMessage(string player, string message)
    {
        this.player = player;
        this.message = message;
    }
}
