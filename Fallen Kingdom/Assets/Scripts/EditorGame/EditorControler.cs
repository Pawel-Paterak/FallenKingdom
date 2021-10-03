using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingdomData;

public class EditorControler : MonoBehaviour {

    [Header("Settings")]
    public float move;
    public float run;
    public float scrolSpeed;
    public bool build;
    public enum TypeBuild { Terrain, Object }
    public TypeBuild typeBuild;
    public int index;
    public Vector3 rotation;
    [Header("Prefabs")]
    public GameObject prefabTerrain;

    [Header("Scripts")]
    public GameResources resources;

    private float vertical;
    private float horizontal;

    void Start () {
		
	}
	

	void Update () {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            vertical = Input.GetAxis("Vertical") * run * Time.deltaTime;
            horizontal = Input.GetAxis("Horizontal") * run * Time.deltaTime;
        }
        else
        {
            vertical = Input.GetAxis("Vertical") * move * Time.deltaTime;
            horizontal = Input.GetAxis("Horizontal") * move * Time.deltaTime;
        }

        if(build)
        {
            if(Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit))
                {
                    Vector3 point = hit.point;
                    point = new Vector3(Mathf.Round(point.x), 0.1f, Mathf.Round(point.z));
                    float restX = point.x % 2;
                    float restZ = point.z % 2;

                    if (restX != 0)
                        point.x -= restX;

                    if (restZ != 0)
                        point.z -= restZ;

                    if (hit.collider.name == "World" && typeBuild == TypeBuild.Terrain)
                    {
                        RaycastHit hitTerrain;
                        if (Physics.Linecast(transform.position, point + Vector3.down, out hitTerrain))
                        {
                            PrefabMap prefabmap = hitTerrain.collider.gameObject.GetComponent<PrefabMap>();
                            if (prefabmap != null)
                                if (prefabmap.type == PrefabMap.TypePrefab.Terrain)
                                    return;
                        }

                        GameObject prefab = Instantiate(prefabTerrain, point, Quaternion.Euler(rotation),hit.collider.transform);
                        PrefabMap prefabMap = prefab.GetComponent<PrefabMap>();
                        if (prefabMap == null)
                            return;
                        
                        prefabMap.Initialize(PrefabMap.TypePrefab.Terrain, new MapTerrain(resources.textures[index].name, point.Vector3dfloat()));
                        prefab.GetComponent<MeshRenderer>().material.mainTexture = resources.textures[index].sprite.texture;
                    }
                }
            }
            if (Input.GetMouseButton(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.name != "World")
                    {
                        PrefabMap prefabMap = hit.collider.gameObject.GetComponent<PrefabMap>();
                        if (prefabMap != null)
                            prefabMap.Destroy();
                    }
                }
            }
        }

        if(Input.mouseScrollDelta.y != 0)
            transform.position += new Vector3(0, -Input.mouseScrollDelta.y* scrolSpeed, 0);

        if (transform.position.y < 2)
            transform.position = new Vector3(transform.position.x, 2, transform.position.z);
        else if (transform.position.y > 100)
            transform.position = new Vector3(transform.position.x, 100, transform.position.z);

        transform.position += new Vector3(-horizontal, 0, -vertical);
    }
}
