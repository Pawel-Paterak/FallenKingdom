using System.Collections;
using System.Collections.Generic;
using KingdomData;
using UnityEngine;
using System.IO;

public class WorldManagerEditor : MonoBehaviour {

    public WorldEditor worldEditor;
    public GameResources resources;

    public void AddObjectToWorld(GameObject obj)
    {
        PrefabMap prefab = obj.GetComponent<PrefabMap>();
        if (prefab == null)
            return;

        switch(prefab.type)
        {
            case PrefabMap.TypePrefab.Terrain:
                    worldEditor.terrains.Add(new TerrainWorld(obj, prefab.terrian));
                    break;
            case PrefabMap.TypePrefab.Object:
                    worldEditor.objects.Add(new ObjectsWorld(obj, prefab.obj));
                    break;
            case PrefabMap.TypePrefab.Spawn:
                {

                    break;
                }

        }
    }
    public void RemoveObjectWorld(GameObject obj)
    {
        foreach (TerrainWorld terrain in worldEditor.terrains.ToArray())
            if (terrain.obj == obj)
            {
                worldEditor.terrains.Remove(terrain);
                return;
            }

        foreach (ObjectsWorld objWorld in worldEditor.objects.ToArray())
            if (objWorld.obj == obj)
            {
                worldEditor.objects.Remove(objWorld);
                return;
            }
    }

    public void LaodWorld(string name)
    {
        Debug.Log("Load world");
        ClearWorld();

        EditorControler controler = FindObjectOfType<EditorControler>();
        World world = AssetsManager.Load<World>(EditorManager.pathWorlds+name);
        if(world != null)
        {
            foreach(MapTerrain terrain in world.terrains)
            {
                Vector3 position = terrain.position.Vector3d();
                position.y = 0.1f;
                GameObject obj = Instantiate(controler.prefabTerrain, position, Quaternion.Euler(0, 0, 0), transform);
                PrefabMap prefabMap = obj.GetComponent<PrefabMap>();
                prefabMap.Initialize(PrefabMap.TypePrefab.Terrain, terrain);
                Sprite sprite = resources.GetTexture(terrain.name);
                if (sprite != null)
                    obj.GetComponent<MeshRenderer>().material.mainTexture = sprite.texture;
            }
        }
    }

    public void ClearWorld()
    {
        foreach (TerrainWorld coor in worldEditor.terrains)
            Destroy(coor.obj);

        foreach (ObjectsWorld coor in worldEditor.objects)
            Destroy(coor.obj);
    }
    public void CreateWorld(string name)
    {
        World world = new World();

        world.name = name.Substring(0, name.Length-4);

        foreach (TerrainWorld coor in worldEditor.terrains)
            world.terrains.Add(coor.terrain);

        foreach (ObjectsWorld coor in worldEditor.objects)
            world.objects.Add(coor.mapObj);

        if (!AssetsManager.Exists(EditorManager.pathWorlds + name))
            EditorManager.singleton.CreateButtonWorld(name);

        AssetsManager.Save(EditorManager.pathWorlds+name, world);
        
    }
}
[System.Serializable]
public class WorldEditor
{
    public List<TerrainWorld> terrains = new List<TerrainWorld>();
    public List<ObjectsWorld> objects = new List<ObjectsWorld>();
}
