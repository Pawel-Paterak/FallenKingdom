using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using KingdomData;

namespace KingdomServer
{
    public class Clients
    {
        public int connectionID;
        public TcpClient socket;
        public NetworkStream nStream;
        public string ip;
        public byte[] bufferBytes;
        public int timeOut;

        #region Variables
        public Account activeAccount;
        public bool isLogin = false;
        public Character activeCharacter;
        public bool inGame = false;

        public AtackData target;
        public double speedAtack
        {
            get
            {
                return timeAtack.Interval;
            }
            set
            {
                timeAtack.Interval = value;
            }
        }
        public Timer timeAtack;
        #endregion

        public void Initialize(TcpClient socket, int connectionID)
        {
            socket.ReceiveBufferSize = TcpServer.singleton.settingsServer.recaiveBufferSize;
            socket.SendBufferSize = TcpServer.singleton.settingsServer.sendBufferSize;
            bufferBytes = new byte[TcpServer.singleton.settingsServer.recaiveBufferSize * 2];
            nStream = socket.GetStream();
            this.socket = socket;
            ip = ((IPEndPoint)socket.Client.RemoteEndPoint).Address.ToString();
            Console.WriteLine("Client connect Id: " + connectionID + " IP: " + ip);
            this.connectionID = connectionID;

            #region "Game Settings"
            target = new AtackData("", -1, AtackData.Type.Null);
            timeAtack = new Timer();
            speedAtack = 10000;
            timeAtack.Elapsed += new ElapsedEventHandler(Atack);
            timeAtack.Start();
            #endregion

            nStream.BeginRead(bufferBytes, 0, bufferBytes.Length, dataCallback, null);
        }
        public void dataCallback(IAsyncResult result)
        {
            try
            {
                int lng = nStream.EndRead(result);
                if (lng <= 0)
                    return;

                Buffer.BlockCopy(bufferBytes, 0, bufferBytes, 0, lng);
            }
            catch (Exception)
            {
                return;
            }


            ByteBuffer data = new ByteBuffer();
            data.WriteBytes(bufferBytes);

            nStream.BeginRead(bufferBytes, 0, bufferBytes.Length, dataCallback, null);
            int input = data.ReadInteger();
            if (BundleManager.packets.ContainsKey(input))
                BundleManager.packets[input](connectionID, data);
            else
                throw new Exception("Input: " + input + " is null in Packets");
        }
        public void SendToClient(ByteBuffer data)
        {
            try
            {
                if (nStream != null)
                    nStream.Write(data.ToArray(), 0, data.Count());
            }
            catch (Exception)
            {
                Disconect();
            }
        }
        public void SendDataToClient(ByteBuffer data, PacketsConnection.PacketClient packet, int input)
        {
            ByteBuffer send = new ByteBuffer();
            send.WriteInteger((int)packet);
            send.WriteInteger(input);
            send.WriteBytes(data.ToArray());
            SendToClient(send);
        }
        public void Veryfication()
        {
            ByteBuffer data = new ByteBuffer();

            timeOut++;

            data.WriteInteger((int)PacketsConnection.PacketClient.Veryfication);
            try
            {
                if (nStream != null)
                    nStream.Write(data.ToArray(), 0, data.Count());
            }
            catch (Exception)
            {

            }
        }

        public void Login(Account account)
        {
            activeAccount = account;
            isLogin = true;
        }
        public void LogoutFromGame()
        {
            Console.WriteLine("Remove-bacpack-");
            GameManager.singleton.RemoveInventoryFromId(activeCharacter.inventory.backpack.Id);
            Console.WriteLine("Remove-Boots-");
            GameManager.singleton.RemoveInventoryFromId(activeCharacter.inventory.boots.Id);
            Console.WriteLine("Remove-breatplate-");
            GameManager.singleton.RemoveInventoryFromId(activeCharacter.inventory.breastplate.Id);
            Console.WriteLine("Remove-head-");
            GameManager.singleton.RemoveInventoryFromId(activeCharacter.inventory.head.Id);
            Console.WriteLine("Remove-lefthand-");
            GameManager.singleton.RemoveInventoryFromId(activeCharacter.inventory.leftHand.Id);
            Console.WriteLine("Remove-righthand-");
            GameManager.singleton.RemoveInventoryFromId(activeCharacter.inventory.rightHand.Id);
            Console.WriteLine("Remove-legs-");
            GameManager.singleton.RemoveInventoryFromId(activeCharacter.inventory.legs.Id);

            activeCharacter = null;
            inGame = false;
        }
        public void Logout()
        {
            activeAccount = null;
            isLogin = false;
        }
        public void JoinToGame(Character character)
        {
            activeCharacter = character;
            inGame = true;

            List<Inventory> inventory = new List<Inventory>();
            MainClass.Print(character.inventory.leftHand.slots, ConsoleColor.Blue);
            inventory.Add(character.inventory.leftHand);
            inventory.Add(character.inventory.backpack);
            inventory.Add(character.inventory.boots);
            inventory.Add(character.inventory.breastplate);
            inventory.Add(character.inventory.head);
            inventory.Add(character.inventory.rightHand);
            inventory.Add(character.inventory.legs);
            List<string> name = new List<string>();
            for (int i = 0; i < inventory.Count; i++)
                name.Add(activeCharacter.name);
            List<InventoryPlayer.TypeInventory> typeInvetory = new List<InventoryPlayer.TypeInventory>();
            typeInvetory.Add(InventoryPlayer.TypeInventory.LeftHand);
            typeInvetory.Add(InventoryPlayer.TypeInventory.Backpack);
            typeInvetory.Add(InventoryPlayer.TypeInventory.Boots);
            typeInvetory.Add(InventoryPlayer.TypeInventory.Breastplate);
            typeInvetory.Add(InventoryPlayer.TypeInventory.Head);
            typeInvetory.Add(InventoryPlayer.TypeInventory.RightHand);
            typeInvetory.Add(InventoryPlayer.TypeInventory.Legs);
            GameManager.singleton.AddInventory(ref inventory, typeInvetory, name);
        }
        public void DisposeVariables()
        {
            if (inGame)
            {
                try
                {
                    if (socket != null && nStream != null)
                    {
                        ByteBuffer data = new ByteBuffer();
                        data.WriteString(activeCharacter.name);
                        TcpServer.singleton.SendDataToAllClients(data, PacketsConnection.PacketClient.Visible, (int)PacketsConnection.InputClientVisible.ExitTheGame, true, connectionID);
                    }
                }
                catch (Exception)
                {

                }
                LogoutFromGame();
            }

            Logout();
            target = null;
            speedAtack = 99999999;
            timeAtack = null;
        }

        #region "InGame"
        public void ExitFromGame()
        {
            activeCharacter = null;
            inGame = false;
        }
        public void MovePlayer(Vector3Dfloat newPosition)
        {
            activeCharacter.position = newPosition;
            ByteBuffer data = new ByteBuffer();
            data.WriteString(activeCharacter.name);
            data.WriteBytes(activeCharacter.position.Write().ToArray());
            TcpServer.singleton.SendDataToAllClients(data, PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.MovePlayer);
        }
        public void Atack(object sender, EventArgs e)
        {

            if (target.type == AtackData.Type.Player)
            {
                foreach (Clients client in TcpServer.singleton.clients)
                    if (client.inGame && client.activeCharacter.name == target.who)
                    {
                        if (Vector3Dfloat.Distance2D(client.activeCharacter.position, activeCharacter.position) < 3)
                        {
                            int id = activeCharacter.inventory.leftHand.slots.id;
                            Console.WriteLine("Weapon ID: " + id);
                            Weapon weapon = (Weapon)GameManager.singleton.itemsManager.GetItemFromId(id); ;
                            if (weapon != null)
                                client.ChangeHealth(-weapon.damage);
                            else
                                client.ChangeHealth(-5);
                        }
                    }
            }
            else
            {
                //Atack monster
            }


        }
        public void ChangeHealth(int health)
        {
            activeCharacter.life += health;
            ByteBuffer data = new ByteBuffer();
            data.WriteInteger((int)PacketsConnection.InputClientChangeField.Health);
            data.WriteString(activeCharacter.name);
            data.WriteInteger(activeCharacter.life);
            TcpServer.singleton.SendDataToAllClients(data, PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.ChangeField);
        }
        public void ChangeMana(int mana)
        {
            activeCharacter.mana += mana;
            ByteBuffer data = new ByteBuffer();
            data.WriteInteger((int)PacketsConnection.InputClientChangeField.Mana);
            data.WriteString(activeCharacter.name);
            data.WriteInteger(activeCharacter.mana);
            TcpServer.singleton.SendDataToAllClients(data, PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.ChangeField);
        }
        #endregion
        public void Disconect()
        {
            try
            {
                if (socket != null && nStream != null)
                    SendDataToClient(new ByteBuffer(), PacketsConnection.PacketClient.Disconect, 0);
            }
            catch (Exception)
            {

            }

            DisposeVariables();

            if (socket != null)
            {
                socket.Close();
                socket.Dispose();
            }

            if (nStream != null)
            {
                nStream.Close();
                nStream.Dispose();
            }

            socket = null;
            nStream = null;
            timeOut = 0;

            Console.WriteLine("Client Disconect Id: " + connectionID + " IP: " + ip);
        }
    }
}
