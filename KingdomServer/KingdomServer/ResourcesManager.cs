using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomData;

namespace KingdomServer
{
    public class ResourcesManager
    {
        public static string PathData = "Data/";
        public static string PathAccounts = "Data/Account/";
        public static string PathCharacters = "Data/Characters/";
        public static string PathWorlds = "Data/Worlds/";
        public static string PathItems = "Data/Items/";
        public static string PathServerSettings = "Data/ServerSettings.xml";
        public static string PathGameSettings = "Data/GameSettings.xml";
        public static string PathLibraryItems = "Data/";

        public void Initialize()
        {
            Exists(PathData, true);
            Exists(PathAccounts, true);
            Exists(PathCharacters, true);
            Exists(PathWorlds, true);
            Exists(PathItems, true);
        }
        public bool Exists(string path, bool createFile = false)
        {
            bool exist = false;
            if (Directory.Exists(path))
                exist = true;
            else if(createFile)
                Directory.CreateDirectory(path);

            return exist;
        }
        public void SaveCharacter(Account account, Character character)
        {
            account.character.Add(character.name);
            Save(PathAccounts + account.login.ToLower() + ".xml", account);
            Save(PathCharacters + character.name.ToLower() + ".xml", character);
        }
        public void Save<T>(string path, T obj, bool overrwrite = true)
        {
            if (File.Exists(path) && !overrwrite)
                return;
            else
            {try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    using (FileStream fStream = new FileStream(path, FileMode.Create))
                        serializer.Serialize(fStream, obj);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public T Open<T>(string path)
        {
            T file = default(T);

            if(File.Exists(path))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    using (FileStream fStream = new FileStream(path, FileMode.Open))
                        file = (T)serializer.Deserialize(fStream);

                }
                catch(Exception ex)
                {
                    MainClass.Print(ex.ToString(), ConsoleColor.Red);
                }
            }
            return file;
        }
    }
}
