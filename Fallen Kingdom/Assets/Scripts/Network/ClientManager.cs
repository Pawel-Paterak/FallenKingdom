using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KingdomData;

public class ClientManager : MonoBehaviour {

    public static ClientTcp clientTcp;
    public float timer = 0;
    public float timerConnect = 0;
    public static string activeScene = "Menu";
    public static bool loadScene = false;


    public void Veryfication()
    {
        if (clientTcp != null)
            clientTcp.timeOut = 0;
    }
    private void OnApplicationQuit()
    {
        if (clientTcp != null)
            clientTcp.Disconect();
        Debug.Log("ApplicationQuit");
        BundleManager.isThreadUpdate = false;
        BundleManager.isThreadUpdateInventory = false;
    }
    void Start () {
        ClientManager client = FindObjectOfType<ClientManager>();
        if (client != this)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            BundleManager.Initialize();

            BundleManager.veryfication = Veryfication;
            BundleManager.disconect = Disconect;

            clientTcp = new ClientTcp();
        }
	}
    void Update () {
        timer += Time.deltaTime;
        if (timer >= 5)
        {
            timer = 0;

            if (clientTcp != null)
            {

                if (clientTcp.timeOut > 10)
                    clientTcp.Disconect();
                else
                {
                    clientTcp.timeOut++;
                    clientTcp.Veryfication();
                }
            }
        }
        
        if (clientTcp == null || !clientTcp.socket.Connected)
        {
            if (clientTcp != null && !clientTcp.isDisconect)
                clientTcp.Disconect();

            timerConnect += Time.deltaTime;
            if (timerConnect >= 2)
            {
                timerConnect = 0;
                clientTcp = new ClientTcp();
            }
        }

        if(!BundleManager.threadUpdate.IsAlive)
        {
            BundleManager.threadUpdate = new System.Threading.Thread(BundleManager.Update);
            BundleManager.threadUpdate.Start();
        }
        if (!BundleManager.threadUpdateInventory.IsAlive)
        {
            BundleManager.threadUpdateInventory = new System.Threading.Thread(BundleManager.UpdateInventory);
            BundleManager.threadUpdateInventory.Start();
        }

        if (loadScene)
        {
            BundleManager.isThreadUpdate = false;
            loadScene = false;
            SceneManager.LoadSceneAsync(activeScene);
        }
    }
    public static void Login(Account account)
    {
        clientTcp.SendToServer(account.Write(), PacketsConnection.PacketServer.Visible, (int)PacketsConnection.InputServerVisible.login);
    }
    public static void Register(Account account)
    {
        clientTcp.SendToServer(account.Write(), PacketsConnection.PacketServer.Visible, (int)PacketsConnection.InputServerVisible.Register);
    }
    public static void CharacterCreate(Character character, Account account)
    {
        ByteBuffer data = new ByteBuffer();
        data.WriteBytes(character.Write().ToArray());
        data.WriteBytes(account.Write().ToArray());
        Debug.Log("Send character Name: " + character.name + " Account: " + account.login);
        clientTcp.SendToServer(data, PacketsConnection.PacketServer.Visible, (int)PacketsConnection.InputServerVisible.CreateCharacter);
    }
    public static void JoinToGame(string nameCharacter)
    {
        ByteBuffer data = new ByteBuffer();
        data.WriteString(nameCharacter);
        clientTcp.SendToServer(data, PacketsConnection.PacketServer.Visible, (int)PacketsConnection.InputServerVisible.JoinToGame);
    }
    public static void Atack(AtackData target)
    {
        ByteBuffer data = new ByteBuffer();
        data.WriteBytes(target.Write().ToArray());
        clientTcp.SendToServer(data, PacketsConnection.PacketServer.Game, (int)PacketsConnection.InputServerGame.AtackPlayer);
    }
    public static void SendMessageToChat(string message)
    {
        ByteBuffer data = new ByteBuffer();
        data.WriteString(message);
        clientTcp.SendToServer(data, PacketsConnection.PacketServer.Game, (int)PacketsConnection.InputServerGame.Chat);
    }
    public static void SetScene(string name)
    {
        if(name != activeScene)
        {
            activeScene = name;
            loadScene = true;
        }
    }
    public static void MoveSlots(MoveSlots move)
    {
        ByteBuffer data = new ByteBuffer();
        data.WriteBytes(move.Write().ToArray());
        clientTcp.SendToServer(data, PacketsConnection.PacketServer.Game, (int)PacketsConnection.InputServerGame.MoveSlots);
    }
    public void Disconect()
    {
        clientTcp.Disconect();
        SettingUser.Account = null;
    }
}
