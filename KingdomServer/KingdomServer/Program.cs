using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomData;
using CoreKingdom;

namespace KingdomServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MainClass main = new MainClass();
            main.Main();
        }
    }

    class MainClass
    {
        TcpServer server = new TcpServer();
        BundleManager bundle = new BundleManager();
        GameManager game = new GameManager();
        ResourcesManager assets = new ResourcesManager();
        EngineFallenKingdom engine = new EngineFallenKingdom();

        public void Main()
        {
            engine.Start();

            ExampleClass example = new ExampleClass();
            example.name = "Example test";
            example.tag = "Tag Example";
           // Collider coliderExample = example.AddComponent<Collider>();
          //  coliderExample.size = new Vector3Dfloat(0, 0, 1);


            ExampleClass examplecol = new ExampleClass();
            examplecol.name = "example col";
            examplecol.transform.position = new Vector3Dfloat(0, 0, 10);
            Collider coliderExampleCol = examplecol.AddComponent<Collider>();
            //coliderExampleCol.size = new Vector3Dfloat(1, 1, 1);

            Ray ray = new Ray();
            if(ray.RayCast(new Vector3Dfloat(0, 0, 0), new Vector3Dfloat(0, 0, 0), out RayCastHit hit, 100))
            {
                if(hit != null)
                    Console.WriteLine("HIT IN : " + hit.collision.obj.name);
            }

            bundle.Initialize();
            bundle.veryfication = server.Veryfication;
            bundle.sendDataToClient = server.SendDataToClient;
            bundle.sendDataToAllClients = server.SendDataToAllClients;
            bundle.sendMessageToInformation = server.SendMessageToInformation;
            bundle.disconect = server.DisconectClient;
            bundle.serverClientJoinGame = ActiveCharacter;
            bundle.serverClientLogin = ActiveAccount;
            bundle.getClient = server.GetClient;

            bundle.getPositionPlayer = GetPlayerPosition;
            bundle.getCollision = game.Collision;

            assets.Initialize();
            server.Initialize();

            Console.ReadLine();
            server.Stop();
            engine.Stop();
        }
        public Vector3Dfloat GetPlayerPosition(int connectionID)
        {
            if (server.clients[connectionID].socket != null && server.clients[connectionID].inGame)
                return server.clients[connectionID].activeCharacter.position;

            return null;
        }
        public bool Collision(Vector3Dfloat position)
        {
            bool collision = false;

            collision = GameManager.singleton.world.GetObjectFromPosition(position) != null;



            return collision;
        }
        public void ActiveAccount(int connectionId, Account account)
        {
            server.clients[connectionId].Login(account);
        }
        public void ActiveCharacter(int connectionId, Character character)
        {
            server.clients[connectionId].JoinToGame(character);
        }

        public static void Print(string message, ConsoleColor color = ConsoleColor.White, bool writeLine = true)
        {
            ConsoleColor colour = Console.ForegroundColor;
            Console.ForegroundColor = color;
            if (writeLine)
                Console.WriteLine(message);
            else
                Console.Write(message);
            Console.ForegroundColor = colour;
        }
    }
    public class PrintManager
    {
        public ConsoleColor Last {
            set {
                if (information.Count > 0)
                    information[information.Count - 1].color = value;
            }
        }
        public List<PrintInfo> information = new List<PrintInfo>();

        public void Add(PrintInfo information)
        {
            this.information.Add(information);
        }
        public void Print()
        {
            foreach (PrintInfo coor in information)
                MainClass.Print(coor.message, coor.color, coor.writeLine);
        }
    }
    public class PrintInfo
    {
        public string message;
        public ConsoleColor color = ConsoleColor.Gray;
        public bool writeLine = true;

        public PrintInfo(string message)
        {
            this.message = message;
        }
        public PrintInfo(string message, bool writeLine)
        {
            this.message = message;
            this.writeLine = writeLine;
        }
        public PrintInfo(string message, ConsoleColor color)
        {
            this.message = message;
            this.color = color;
        }
        public PrintInfo(string message, ConsoleColor color, bool writeLine)
        {
            this.message = message;
            this.color = color;
            this.writeLine = writeLine;
        }
    }

    public class ExampleClass : CoreFallenKingdom
    {
        public override void Start()
        {
            
        }
        public override void Update()
        {
            
        }
        public override void CollisionStay(Collision collision)
        {
           // Console.WriteLine("Collision {0} from {1}", name, collision.obj.name);
        }
    }
}
