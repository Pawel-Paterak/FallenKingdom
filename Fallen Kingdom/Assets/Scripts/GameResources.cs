using System;
using System.Collections;
using System.Collections.Generic;
using IO = System.IO;
using UnityEngine;
using KingdomData;

public class GameResources : MonoBehaviour {

    public static GameResources singleton;
    public List<TextureGame> textures = new List<TextureGame>();
    public Dictionary<int,ItemGame> items = new Dictionary<int, ItemGame>();
    public bool Editor;
    public GameObject prefabTexture;
    public GameObject contentPrefabsTextures;
    public GameObject prefabTexturePath;
    public GameObject contentPrefabsTexturesPath;
    void Awake()
    {
        singleton = this;
        LoadResources();
        if (Editor)
            foreach (TextureGame texture in textures)
            {
                CreateButtonTexture(texture);
                CreateButtonPathTexture(texture);
            }

    }

    private void LoadResources()
    {
        Sprite[] textures = AssetsManager.OpenTextures();
        foreach (Sprite texture in textures)
            this.textures.Add(new TextureGame(texture.name, texture));

        Paths pathsItem = AssetsManager.OpenResourcesFile<Paths>(AssetsManager.pathsItems);
        if (pathsItem != null)
        {
            foreach (Path pathItem in pathsItem.paths.ToArray())
            {
                Item item = null;
                switch (pathItem.typeItem)
                {
                    case Item.TypeItem.Item:
                        item = AssetsManager.OpenResourcesFile<Item>("Items/" + pathItem.path);
                        break;
                    case Item.TypeItem.Food:
                        item = AssetsManager.OpenResourcesFile<Food>("Items/" + pathItem.path);
                        break;
                    case Item.TypeItem.Weapon:
                        item = AssetsManager.OpenResourcesFile<Weapon>("Items/" + pathItem.path);
                        break;

                }

                if (item != null)
                    items.Add(pathItem.id, new ItemGame(item));
                else
                {
                    Debug.Log(pathItem.path + " is null");
           
                }
            }
        }
    }
    public void CreateButtonTexture(TextureGame texture)
    {
        GameObject obj = Instantiate(prefabTexture, contentPrefabsTextures.transform);
        PrefabButtonEditor btn = obj.GetComponent<PrefabButtonEditor>();
        btn.InitializeButton(texture.sprite, texture.name);
    }
    public void CreateButtonPathTexture(TextureGame texture)
    {
        GameObject obj = Instantiate(prefabTexturePath, contentPrefabsTexturesPath.transform);
        ButtonPathTexture btn = obj.GetComponent<ButtonPathTexture>();
        btn.Initialize(texture);
    }
    public Sprite GetTexture(string name)
    {
        foreach (TextureGame coor in textures)
            if (coor.name == name)
                return coor.sprite;

        return null;
    }
    public Item GetLoadItem(string name)
    {
        foreach (ItemGame coor in items.Values)
            if (coor.name == name)
                return coor.item;
        return null;
    }
    public Item GetItemFromId(int id)
    {
        if (items.ContainsKey(id))
            return items[id].item;

        return null;
    }
    public int GetIndexTextureFromName(string name)
    {
        for (int i=0; i<textures.Count; i++)
            if (textures[i].name == name)
                return i;

        return -1;
    }
}
[Serializable]
public class TextureGame
{
    public string name;
    public Sprite sprite;

    public TextureGame()
    {

    }
    public TextureGame(string name,Sprite sprite)
    {
        this.name = name;
        this.sprite = sprite;
    }
   
}
[Serializable]
public class ItemGame
{
    public string name;
    public Item item;

    public ItemGame()
    {

    }
    public ItemGame(Item item)
    {
        if(item != null)
            name = item.name;
        this.item = item;
    }
    public Item.TypeItem GetTypeItem()
    {
        return item.typeItem;
    }
    public T GetItem<T>() where T : Item
    {
        return (T)item;
    }
}