using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorManager : MonoBehaviour {

    [Header("Prefabs")]
    public GameObject prefabWorldHud;
    public GameObject prefabItemHud;
    public GameObject contentWorlds;
    public GameObject contentItems;

    public const string pathDataWork = "DataWork/";
    public const string pathWorlds = "DataWork/Worlds/";
    public const string pathItems = "DataWork/Items/";

    public static EditorManager singleton;

    void Start () {
        if (singleton != null)
            singleton = null;
        singleton = this;

        if (!Directory.Exists(pathDataWork))
            Directory.CreateDirectory(pathDataWork);
        if (!Directory.Exists(pathWorlds))
            Directory.CreateDirectory(pathWorlds);
        if (!Directory.Exists(pathItems))
            Directory.CreateDirectory(pathItems);

        LoadWorlds();
        LoadItems();

    }
	
	void Update () {
		
	}
    private void LoadWorlds()
    {
        string[] paths = Directory.GetFiles(pathWorlds);
        foreach (string path in paths)
        {
            string fileName = System.IO.Path.GetFileName(path);
            if (fileName[fileName.Length - 1] == 'l' && fileName[fileName.Length - 2] == 'm' && fileName[fileName.Length - 3] == 'x' && fileName[fileName.Length - 4] == '.')
                CreateButtonWorld(fileName);
        }
    }
    private void LoadItems()
    {
        string[] paths = Directory.GetFiles(pathItems);
        foreach(string path in paths)
        {
            string fileName = System.IO.Path.GetFileName(path);
            if (fileName[fileName.Length - 1] == 'l' && fileName[fileName.Length - 2] == 'm' && fileName[fileName.Length - 3] == 'x' && fileName[fileName.Length - 4] == '.')
                CreateButtonItems(fileName);
        }
    }
    public void CreateButtonWorld(string nameWorld)
    {
        GameObject obj = Instantiate(prefabWorldHud, contentWorlds.transform);
        WorldButton btn = obj.GetComponent<WorldButton>();
        btn.Initialize(nameWorld);
    }
    public void CreateButtonItems(string nameItems)
    {
        GameObject obj = Instantiate(prefabItemHud, contentItems.transform);
        ItemButton btn = obj.GetComponent<ItemButton>();
        btn.Initialize(nameItems);
    }
}