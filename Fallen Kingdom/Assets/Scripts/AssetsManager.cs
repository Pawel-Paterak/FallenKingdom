using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;
using KingdomData;


public static class AssetsManager {

    public static string textures = "Textures/";
    public static string monsters = "Monsters/";
    public static string worlds = "Worlds/";
    public static string items = "Items/";
    public static string npc = "Npc/";
    public static string pathsItems = "PathsItems";
   // public static string ItemsLibrary = "ItemsLibrary";

    public static string languages = "Languages/";

    public static void Veryfication()
    {
        
    }

    private static bool Exist(string patch, bool createDirectory = true)
    {
        if (Directory.Exists(patch))
            return true;
        else
        {
            if(createDirectory)
                Directory.CreateDirectory(patch);

            return false;
        }
    }

    public static Sprite OpenTexture(string patch)
    {
        Sprite sprite = null;
        Texture2D texture = new Texture2D(1,1);

        texture = Resources.Load<Texture2D>(patch);

        if(texture != null)
            sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2(0.5f,0.5f));

        return sprite;
    }
    public static Sprite[] OpenTextures()
    {
        Texture2D[] textures2d = Resources.LoadAll<Texture2D>(textures);
        Sprite[] sprites = new Sprite[textures2d.Length];
        for (int i = 0; i < textures2d.Length; i++)
        {
            sprites[i] = Sprite.Create(textures2d[i], new Rect(0, 0, textures2d[i].width, textures2d[i].height), new Vector2(0.5f, 0.5f));
            sprites[i].name = textures2d[i].name;
        }

        return sprites;
    }
    public static T OpenResourcesFile<T>(string path)
    {
        T file = default(T);
        TextAsset textFile = Resources.Load<TextAsset>(path);
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(textFile.text)))
            {
                file = (T)serializer.Deserialize(xmlReader);
            }
        }
        catch(Exception e)
        {

        }
        return file;
    }
    public static string[] GetNameFilesFromPatch(string patch)
    {
        if (!Directory.Exists(patch))
            return null;

        string[] patchFiles = Directory.GetFiles(patch);
        string[] nameFiles = new string[patchFiles.Length];
        for(int i=0; i<patchFiles.Length; i++)
        {
            nameFiles[i] = System.IO.Path.GetFileName(patchFiles[i]);

            int indexDot = 0;
            indexDot = nameFiles[i].LastIndexOf('.');
            if (indexDot != -1)
                nameFiles[i] = nameFiles[i].Substring(0, indexDot);
  
        }
        return nameFiles;
    }
    public static string[] GetFilesFromPatch(string patch)
    {
        if (!Directory.Exists(patch))
            return null;

        return Directory.GetFiles(patch);
    }
    public static bool Exists(string path)
    {
        return Directory.Exists(path);
    }
    public static World OpenWorld(string name)
    {
        return ((object)Resources.Load(worlds + name, typeof(World))) as World;
    }
    public static T Load<T> (string patch)
    {
        T file = default(T);
        
        if (!File.Exists(patch) || patch.Substring(patch.Length-4).ToLower() != ".xml")
            return file;

        try
        {
            
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fStream = new FileStream(patch, FileMode.Open))
                    file = (T)serializer.Deserialize(fStream);
        }
        catch (Exception ex)
        {
           // Debug.Log(ex);
        }

        return file;
    }
    public static void Save<T> (string patch, T obj, bool overwrite = true)
    {
        if (File.Exists(patch) && !overwrite)
            return;

        try
        {
            XmlSerializer serialzier = new XmlSerializer(typeof(T));
            using (FileStream fStream = new FileStream(patch, FileMode.Create))
                serialzier.Serialize(fStream, obj);
        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }
    }
}
