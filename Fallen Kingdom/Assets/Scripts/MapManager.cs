using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingdomData;

public class MapManager : MonoBehaviour {

    [Header("Settings")]
    public World activeWorld;
    public WorldGame worldGame;
    public static MapManager singleton;
    private string nameWorld;
    public bool refreshWorld;
    [Header("Prefabs")]
    public GameObject contentWorld;
    public GameObject prefabTerrain;

    private void Start () {
        if (singleton != null)
            singleton = null;

        singleton = this;

        SetWorld("FirstTestWorld");
	}
	private void Update () {
		
        if(refreshWorld)
        {
            refreshWorld = false;
            TextAsset worldTextAssets = Resources.Load(AssetsManager.worlds + nameWorld, typeof(TextAsset)) as TextAsset;
            XmlSerializer serializer = new XmlSerializer(typeof(World));
            World world = null;
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(worldTextAssets.text)))
            {
                world = (World)serializer.Deserialize(xmlReader);
            }
            if (world != null)
            {
                activeWorld = world;
                LoadWorld();
            }
            else
                Debug.LogException(new System.Exception("World \"" + nameWorld + "\" not exist"));

        }
    }
    private void LoadWorld()
    {
        if (activeWorld == null)
            return;

        foreach(TerrainWorld terrain in worldGame.terrains)
            Destroy(terrain.obj);
        foreach (ObjectsWorld objects in worldGame.objects)
            Destroy(objects.obj);

        worldGame = new WorldGame();

        foreach(MapTerrain terrain in activeWorld.terrains)
        {
            GameObject obj = Instantiate(prefabTerrain, terrain.position.Vector3d(), Quaternion.Euler(0, 0, 0), contentWorld.transform);
            Sprite sprite = GameResources.singleton.GetTexture(terrain.name);
            if (sprite == null)
                Debug.LogException(new Exception("This texture\"" + terrain.name + "\" is not exist"));
            else
                obj.GetComponent<MeshRenderer>().material.mainTexture = sprite.texture;

            worldGame.terrains.Add(new TerrainWorld(obj, terrain));
        }
       // foreach (MapObject obj in activeWorld.objects)
       // {
       //     GameObject objec = Instantiate(prefabTerrain, obj.position.Vector3d(), Quaternion.Euler(0, 0, 0), contentWorld.transform);
       //     worldGame.objects.Add(new ObjectsWorld(objec, obj));
       // }
    }
    public void SetWorld(string name)
    {
        nameWorld = name;
        refreshWorld = true;
    }
}
[Serializable]
public class WorldGame
{
    public List<TerrainWorld> terrains = new List<TerrainWorld>();
    public List<ObjectsWorld> objects = new List<ObjectsWorld>();
}
[Serializable]
public class TerrainWorld
{
    public GameObject obj;
    public MapTerrain terrain;

    public TerrainWorld()
    {

    }
    public TerrainWorld(GameObject obj, MapTerrain terrain)
    {
        this.obj = obj;
        this.terrain = terrain;
    }
}
[Serializable]
public class ObjectsWorld
{
    public GameObject obj;
    public MapObject mapObj;

    public ObjectsWorld()
    {

    }
    public ObjectsWorld(GameObject obj, MapObject mapObj)
    {
        this.obj = obj;
        this.mapObj = mapObj;
    }
}
