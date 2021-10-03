using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using KingdomData;

namespace KingdomServer
{
    public class TcpServer
    {
        public TcpListener socket;
        public ServerSettings settingsServer;
        public Clients[] clients = new Clients[0];
        public static TcpServer singleton;


        public TcpServer()
        {
            singleton = this;
            ResourcesManager assets = new ResourcesManager();
            ServerSettings settingsServer = assets.Open<ServerSettings>(ResourcesManager.PathServerSettings);
            if (settingsServer != null)
                this.settingsServer = settingsServer;
            else
                throw new Exception("File not founded \"" + ResourcesManager.PathServerSettings + "\"");

             IPAddress ip = IPAddress.Any;
             IPEndPoint Ipend = new IPEndPoint(ip, settingsServer.port);
            socket = new TcpListener(Ipend);

            if (settingsServer.slots < 0)
                return;

            clients = new Clients[settingsServer.slots];
            for (int i = 0; i < clients.Length; i++)
                clients[i] = new Clients();

            Thread verificationConnects = new Thread(new ThreadStart(VeryficationConnects));
            verificationConnects.Start();
        }
        public void Initialize()
        {
            bool isServer = true;
            socket.Server.ReceiveBufferSize = settingsServer.recaiveBufferSize;
            socket.Server.SendBufferSize = settingsServer.recaiveBufferSize;

            if (isServer)
                MainClass.Print("Server Start", ConsoleColor.Green);
            else
                MainClass.Print("Server Stop", ConsoleColor.Red);
            DataPrint();

            if (settingsServer.slots < 0)
                return;

            try
            {
                 socket.Start();
                socket.BeginAcceptTcpClient(ConnectionCallbak, null);
            }
            catch(Exception ex)
            {
                MainClass.Print(ex.ToString(), ConsoleColor.Red);
                isServer = false;
            }

        }
        public void DataPrint()
        {

            PrintManager info = new PrintManager();

            info.Add(new PrintInfo("-----------------"));
            info.Add(new PrintInfo("Settings Server: "));
            info.Add(new PrintInfo("Port: ", false));
            info.Add(new PrintInfo(settingsServer.port.ToString(), ConsoleColor.Green, false));
            info.Add(new PrintInfo(" Slots: ", false));
            info.Add(new PrintInfo(settingsServer.slots.ToString()));
            if (settingsServer.slots < 0)
                info.Last = ConsoleColor.Red;
            else if (settingsServer.slots == 0)
                info.Last = ConsoleColor.Yellow;
            else
                info.Last = ConsoleColor.Green;

            info.Add(new PrintInfo("Log name: ", false));
            info.Add(new PrintInfo(settingsServer.nameLog));
            if (string.IsNullOrEmpty(settingsServer.nameLog))
                info.Last = ConsoleColor.Red;
            else
                info.Last = ConsoleColor.Green;

            info.Add(new PrintInfo("Debug write: ", false));
            info.Add(new PrintInfo(settingsServer.debugWrite.ToString()));
            if (settingsServer.debugWrite < 1 || settingsServer.debugWrite > 3)
                info.Last = ConsoleColor.Red;
            else
                info.Last = ConsoleColor.Green;

            info.Add(new PrintInfo("Time out count (One = 5s): ", false));
            info.Add(new PrintInfo(settingsServer.timeOutCount.ToString()));
            if (settingsServer.timeOutCount < 2)
                info.Last = ConsoleColor.Red;
            else
                info.Last = ConsoleColor.Green;

            info.Add(new PrintInfo("Send buffer size : ", false));
            info.Add(new PrintInfo(settingsServer.sendBufferSize.ToString()));
            if (settingsServer.sendBufferSize < 1024)
                info.Last = ConsoleColor.Red;
            else if (settingsServer.sendBufferSize == 1024)
                info.Last = ConsoleColor.Yellow;
            else
                info.Last = ConsoleColor.Green;
            info.Add(new PrintInfo("Recaive buffer size : ", false));
            info.Add(new PrintInfo(settingsServer.recaiveBufferSize.ToString()));
            if (settingsServer.recaiveBufferSize < 1024)
                info.Last = ConsoleColor.Red;
            else if (settingsServer.recaiveBufferSize == 1024)
                info.Last = ConsoleColor.Yellow;
            else
                info.Last = ConsoleColor.Green;

            info.Add(new PrintInfo("----------------- "));
            info.Add(new PrintInfo("Settings Game: "));
            info.Add(new PrintInfo("ActiveWorld: ", false));
            info.Add(new PrintInfo(GameManager.singleton.settingsGame.nameWorld.ToString()));
            if (GameManager.singleton.world == null)
                info.Last = ConsoleColor.Red;
            else
                info.Last = ConsoleColor.Green;
            info.Add(new PrintInfo("Library Items: ", false));
            info.Add(new PrintInfo(GameManager.singleton.settingsGame.nameLibraryItems.ToString()));
            if (!GameManager.singleton.itemsManager.loadedLibraryItems)
                info.Last = ConsoleColor.Red;
            else
                info.Last = ConsoleColor.Green;
            info.Add(new PrintInfo("Multiplier Exp: ", false));
            float multiplierExp = GameManager.singleton.settingsGame.multiplierExp;
            info.Add(new PrintInfo(multiplierExp.ToString()));
            if (multiplierExp < 1)
                info.Last = ConsoleColor.Red;
            else if (multiplierExp > 5)
                info.Last = ConsoleColor.Yellow;
            else
                info.Last = ConsoleColor.Green;
            info.Add(new PrintInfo("Not player move time out : ", false));
            info.Add(new PrintInfo(GameManager.singleton.settingsGame.notMoveTimeOut.ToString()));
            if (GameManager.singleton.settingsGame.notMoveTimeOut < 180)
                info.Last = ConsoleColor.Red;
            else if (GameManager.singleton.settingsGame.notMoveTimeOut >= 180 && GameManager.singleton.settingsGame.notMoveTimeOut < 300)
                info.Last = ConsoleColor.Yellow;
            else
                info.Last = ConsoleColor.Green;
            info.Add(new PrintInfo("----------------- "));
            info.Add(new PrintInfo("Debug: "));
            info.Add(new PrintInfo(" "));

            info.Print();
        }
        public void Stop()
        {
            socket.Stop();
            socket = null;
            for (int i = 0; i < clients.Length; i++)
                if(clients[i].socket != null)
                    clients[i].Disconect();
        }
        public void VeryficationConnects()
        {
            do
            {
                for (int i = 0; i < clients.Length; i++)
                {
                    if (clients[i].socket != null)
                    {
                        if (clients[i].timeOut > settingsServer.timeOutCount)
                            clients[i].Disconect();
                        else
                            clients[i].Veryfication();
                    }
                }
                Thread.Sleep(5000);

            } while (true);
        }
        public void Veryfication(int connectionID)
        {
            if(settingsServer.debugWrite == 3)
                Console.WriteLine("ID: " + connectionID + " Veryfication");
            clients[connectionID].timeOut = 0;
        }
        public void ConnectionCallbak(IAsyncResult result)
        {
            if (socket == null)
                return;
            TcpClient client = socket.EndAcceptTcpClient(result);
            socket.BeginAcceptTcpClient(ConnectionCallbak, null);
            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i].socket == null)
                {
                    clients[i].Initialize(client, i);
                    break;
                }
            }
        }
        public void DisconectClient(int connectionID)
        {
            clients[connectionID].Disconect();
        }
        public void SendDataToClient(int connectionID,ByteBuffer data, PacketsConnection.PacketClient packet, int input)
        {
            clients[connectionID].SendDataToClient(data, packet, input);
        }
        public void SendDataToAllClients(ByteBuffer data, PacketsConnection.PacketClient packet, int input, bool onlyInGame = false, int neverConnectionID = -1)
        {
            for (int i = 0; i < clients.Length; i++)
            {
                if (neverConnectionID == i || clients[i].socket == null)
                    continue;

                if (onlyInGame && clients[i].inGame)
                    SendDataToClient(i, data, packet, input);
                else if (!onlyInGame)
                    SendDataToClient(i, data, packet, input);
            }
        }
        public void SendMessageToInformation(int connectionID, string message, int input)
        {
            ByteBuffer data = new ByteBuffer();
            data.WriteString(message);
            clients[connectionID].SendDataToClient(data, PacketsConnection.PacketClient.Information, input);
        }
        public Clients GetClient(int connectionID)
        {
            return clients[connectionID];
        }

    }

    public class ServerSettings
    {
        public int port { get; set; } = 5555;
        public int slots { get; set; } = 100;
        public int recaiveBufferSize { get; set; } = 4096;
        public int sendBufferSize { get; set; } = 4096;
        public int debugWrite { get; set; } = 1;
        public int timeOutCount { get; set; } = 10;
        public string nameLog { get; set; }

        public ServerSettings()
        {

        }
    }

}
