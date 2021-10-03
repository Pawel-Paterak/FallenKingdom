using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomData;

namespace KingdomServer
{
    public class BundleManager
    {
        public delegate void _Packet(int connectionID, ByteBuffer data);
        public static Dictionary<int, _Packet> packets = new Dictionary<int, _Packet>();

        #region Delegate
        public delegate void _veryfication(int connectionID);
        public delegate void _SendDataToClient(int connectionId, ByteBuffer data, PacketsConnection.PacketClient packet, int input);
        public delegate void _SendDataToAllClients(ByteBuffer data, PacketsConnection.PacketClient packet, int input, bool onlyInGame = false, int neverConnectionID = -1);
        public delegate void _SendMessage(int connectionID, string message, int input);
        public delegate void _ServerClientJoinGame(int connectionID, Character character);
        public delegate void _ServerClientLogin(int connectionID, Account account);
        public delegate Clients _Client(int connectionID);
        #endregion
        #region InGameDelegates
        public delegate Vector3Dfloat _GetPositionPlayer(int connectionID);
        public delegate bool _Collision(Vector3Dfloat position);
        #endregion
        #region Methods
        public _veryfication veryfication;
        public _SendDataToAllClients sendDataToAllClients;
        public _SendDataToClient sendDataToClient;
        public _SendMessage sendMessageToInformation;
        public _veryfication disconect;
        public _ServerClientJoinGame serverClientJoinGame;
        public _ServerClientLogin serverClientLogin;
        public _Client getClient;
        #endregion
        #region InGameMethods
        public _GetPositionPlayer getPositionPlayer;
        public _Collision getCollision;
        #endregion

        public void Initialize()
        {
            packets.Add((int)PacketsConnection.PacketServer.Information, InformationPacket);
            packets.Add((int)PacketsConnection.PacketServer.Visible, VisiblePacket);
            packets.Add((int)PacketsConnection.PacketServer.Game, GamePacket);
            packets.Add((int)PacketsConnection.PacketServer.Invisible, InvisiblePacket);
            packets.Add((int)PacketsConnection.PacketServer.Veryfication, VeryficationPacket);
            packets.Add((int)PacketsConnection.PacketServer.Disconect, Disconect);
        }

        #region "Packets"
        public void InformationPacket(int connectionID, ByteBuffer data)
        {
            int input = data.ReadInteger();

            switch((PacketsConnection.InputServerInformation)input)
            {
                default:
                    {
                        Console.WriteLine("Input: " + input + " not exist in Information");
                        break;
                    }
            }
        }
        public void VisiblePacket(int connectionID, ByteBuffer data)
        {
            int input = data.ReadInteger();

            switch((PacketsConnection.InputServerVisible)input)
            {
                case PacketsConnection.InputServerVisible.login:
                    Login(connectionID, data);
                        break;
                case PacketsConnection.InputServerVisible.Register:
                    Register(connectionID, data);
                        break;
                case PacketsConnection.InputServerVisible.CreateCharacter:
                    CreateCharacter(connectionID, data);
                    break;
                case PacketsConnection.InputServerVisible.JoinToGame:
                    JoinToGame(connectionID, data);
                    break;
                default:
                    {
                        Console.WriteLine("Input: " + input + " not exist in Visible");
                        break;
                    }
            }
        }
        public void InvisiblePacket(int connectionID, ByteBuffer data)
        {
            int input = data.ReadInteger();
            Console.WriteLine("Input: " + input + " InvisiblePacket");
        }
        public void GamePacket(int connectionID, ByteBuffer data)
        {
            int input = data.ReadInteger();
            switch((PacketsConnection.InputServerGame)input)
            {
                case PacketsConnection.InputServerGame.MovePlayer:
                    {
                        string key = data.ReadString();
                        Vector3Dfloat position = getPositionPlayer(connectionID);
                        switch (key.ToLower())
                        {
                            case "w":
                                {
                                    position += Vector3Dfloat.Forward*2;
                                    break;
                                }
                            case "s":
                                {
                                    position += Vector3Dfloat.Back * 2;
                                    break;
                                }
                            case "a":
                                {
                                    position += Vector3Dfloat.Left * 2;
                                    break;
                                }
                            case "d":
                                {
                                    position += Vector3Dfloat.Right * 2;
                                    break;
                                }
                        }

                        if (!getCollision(position))
                            getClient(connectionID).MovePlayer(position);
                        else
                             if (TcpServer.singleton.settingsServer.debugWrite == 3)
                            Console.WriteLine("ID(" + connectionID + ") collision");


                        break;
                    }
                case PacketsConnection.InputServerGame.AtackPlayer:
                        PlayerAtack(connectionID, data);
                        break;
                case PacketsConnection.InputServerGame.Chat:
                    {
                        Clients client = getClient(connectionID);

                        if (!client.inGame)
                            return;
                        string player = client.activeCharacter.name;
                        string message = client.activeCharacter.name + " [" + client.activeCharacter.lvl + "]: " + data.ReadString();
                        if(TcpServer.singleton.settingsServer.debugWrite >= 2)
                            Console.WriteLine(message);

                        ByteBuffer send = new ByteBuffer();
                        send.WriteString(player);
                        send.WriteString(message);
                        sendDataToAllClients(send, PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.Chat);

                        break;
                    }
                case PacketsConnection.InputServerGame.MoveSlots:
                        MoveSlotsItem(connectionID, data);
                        break;
                case PacketsConnection.InputServerGame.InteractionFromServer:
                        MoveSlotsItem(connectionID, data);
                        break;
                default:
                {
                        break;
                }
            }
        }
        public void VeryficationPacket(int connectionID, ByteBuffer data)
        {
            veryfication(connectionID);
        }
        public void Disconect(int connectionID, ByteBuffer data)
        {
            disconect(connectionID);
        }
        #endregion
        #region "Methods"
        private void Login(int connectionID, ByteBuffer data)
        {
            Account account = new Account();
            account.Read(data);
            if(TcpServer.singleton.settingsServer.debugWrite >= 2)
                Console.WriteLine("ID("+connectionID+") Login: "+ account.login + " Password: " + account.password);
            ResourcesManager assets = new ResourcesManager();
            Account openAccount = assets.Open<Account>(ResourcesManager.PathAccounts + account.login.ToLower() + ".xml");
            if (openAccount != null && account != null)
            {
                if (account.password == openAccount.password)
                {
                    sendDataToClient(connectionID, openAccount.Write(), PacketsConnection.PacketClient.Visible, (int)PacketsConnection.InputClientVisible.Login);
                    serverClientLogin(connectionID, openAccount);
                }
            }
            else
                sendMessageToInformation(connectionID, "Bad login or password!", (int)PacketsConnection.InputClientInformation.Login);
        }
        private void Register(int connectionID, ByteBuffer data)
        {
            Account account = new Account();
            account.Read(data);

            ResourcesManager assets = new ResourcesManager();

            if(!assets.Exists(ResourcesManager.PathAccounts + account.login.ToLower() + ".xml"))
            {
                assets.Save(ResourcesManager.PathAccounts + account.login.ToLower() + ".xml", account);
                sendMessageToInformation(connectionID, "Your account has been registered.", (int)PacketsConnection.InputClientInformation.Register);
            }
            else
                sendMessageToInformation(connectionID, "User is existing", (int)PacketsConnection.InputClientInformation.Register);
        }
        private void CreateCharacter(int connectionID, ByteBuffer data)
        {
            Character character = new Character();
            Account account = new Account();
            data = character.Read(data);
            account.Read(data);

            ResourcesManager assets = new ResourcesManager();

            if (!assets.Exists(ResourcesManager.PathCharacters + character.name.ToLower() + ".xml"))
            {
                assets.SaveCharacter(account, character);
                ByteBuffer send = new ByteBuffer();
                send.WriteString(character.name);
                sendDataToClient(connectionID, send, PacketsConnection.PacketClient.Visible, (int)PacketsConnection.InputClientVisible.AddCharacter);
                if(TcpServer.singleton.settingsServer.debugWrite >= 2)
                    Console.WriteLine("Character create name: " + character.name);
            }
            else
                sendMessageToInformation(connectionID, "This Character existing.", (int)PacketsConnection.InputClientInformation.CreateCharacter);

        }
        private void JoinToGame(int connectionID, ByteBuffer data)
        {
            string nameCharacter = data.ReadString();
            ResourcesManager assets = new ResourcesManager();
            Character character = assets.Open<Character>(ResourcesManager.PathCharacters + nameCharacter + ".xml");

            if (character != null)
            {
                serverClientJoinGame(connectionID, character);
                Console.WriteLine("ID(" + connectionID + "): " + nameCharacter + " joined the game.");
                Character sendCharacter = new Character(character);
                sendCharacter.inventory = new InventoryPlayer();
                sendDataToClient(connectionID, sendCharacter.Write(), PacketsConnection.PacketClient.Visible, (int)PacketsConnection.InputClientVisible.JoinToGame);
                SendAllActivePlayers(connectionID);
                SendPlayerToAllActivePlayers(connectionID);
            }
            else
                Console.WriteLine("Error The selected character does not exist in the file \"Characters\"!");

        }
        private void SendAllActivePlayers(int connectionID)
        {
            if(TcpServer.singleton.settingsServer.debugWrite == 3)
                Console.WriteLine("Send players to ID("+connectionID+")");

            List<Character> characters = new List<Character>();
            int countPlayers = 0;
            for (int player = 0; player < TcpServer.singleton.settingsServer.slots; player++)
            {
                Clients client = getClient(player);
                if (client.inGame && client.activeCharacter != null)
                {
                    characters.Add(client.activeCharacter);
                    countPlayers++;
                }
            }
            ByteBuffer data = new ByteBuffer();
            data.WriteInteger(countPlayers);
            foreach (Character coor in characters)
                data.WriteBytes(coor.Write().ToArray());

            sendDataToClient(connectionID, data, PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.PlayersAssets);

        }
        private void SendPlayerToAllActivePlayers(int connectionID)
        {
            int countPlayer = 1;
            Clients client = getClient(connectionID);
            Character character = client.activeCharacter;
            ByteBuffer data = new ByteBuffer();
            data.WriteInteger(countPlayer);
            data.WriteBytes(character.Write().ToArray());
            sendDataToAllClients(data, PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.PlayersAssets, true, connectionID);

        }
        private void PlayerAtack(int connectionID, ByteBuffer data)
        {
            Clients client = getClient(connectionID);
            AtackData atack = new AtackData();
            atack.Read(data);
            if(atack.who != client.activeCharacter.name)
                client.target = atack;
            Console.WriteLine("I get Target Name:" + atack.who );
        }
        public void MoveSlotsItem(int connectionID, ByteBuffer data)
        {
            try
            {
                MoveSlots move = new MoveSlots();
                data = move.Read(data);
                Inventory fromInventory = GameManager.singleton.GetInventory(move.idInventoryFrom);
                Inventory toInventory = GameManager.singleton.GetInventory(move.idInventoryTo);
                Slots fromSlots = fromInventory.slots.GetSlotsFromRoad(move.roadSlotsFrom);
                Slots toSlots = toInventory.slots.GetSlotsFromRoad(move.roadSlotsTo);
                SlotsMovment movment = new SlotsMovment(toSlots, move.roadSlotsFrom, fromSlots, move.roadSlotsTo);

                Console.WriteLine("From Inv : " + fromInventory.slots.GetSlotsFromRoad(move.roadSlotsFrom));
                Console.WriteLine("To Inv : " + toInventory.slots.GetSlotsFromRoad(move.roadSlotsTo));

                Slots.Move(ref fromSlots, ref toSlots);

                fromInventory.Change(fromSlots, move.roadSlotsFrom);
                Console.WriteLine("From Inv : " + fromInventory.slots.GetSlotsFromRoad(move.roadSlotsFrom));
                toInventory.Change(toSlots, move.roadSlotsTo);
                Console.WriteLine("To Inv : " + toInventory.slots.GetSlotsFromRoad(move.roadSlotsTo));

                ByteBuffer send = new ByteBuffer();
                send.WriteBytes(movment.Write().ToArray());
                sendDataToAllClients(send, PacketsConnection.PacketClient.Game, (int)PacketsConnection.InputClientGame.MoveSlots, true);
            }
            catch(Exception ex)
            {
                MainClass.Print(ex.Message, ConsoleColor.Red);
            }
        }
        #endregion
    }
}
