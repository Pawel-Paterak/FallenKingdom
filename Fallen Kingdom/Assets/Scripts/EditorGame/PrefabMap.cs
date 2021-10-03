using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingdomData;

public class PrefabMap : MonoBehaviour {

    public enum TypePrefab {Terrain, Object, Spawn}
    public TypePrefab type;
    public MapTerrain terrian;
    public MapObject obj;
    public MapSpawn spawn;

    public void Initialize(TypePrefab type, object objMap)
    {
        this.type = type;
        switch(type)
        {
            case TypePrefab.Terrain:
                {
                    terrian = objMap as MapTerrain;
                    break;
                }
            case TypePrefab.Object:
                {
                    obj = objMap as MapObject;
                    break;
                }
            case TypePrefab.Spawn:
                {
                    spawn = objMap as MapSpawn;
                    break;
                }
        }

        FindObjectOfType<WorldManagerEditor>().AddObjectToWorld(gameObject);
    }
    public void Destroy()
    {
        FindObjectOfType<WorldManagerEditor>().RemoveObjectWorld(gameObject);
        Destroy(gameObject);
    }
}
