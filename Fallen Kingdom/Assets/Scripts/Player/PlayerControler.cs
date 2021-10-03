using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingdomData;

public class PlayerControler : MonoBehaviour
{

    public float timeMove;
    public float timeDelayMove;


    public GameObject prefabTargetHud;
    public GameObject target;
    public GameObject targetHud;

    void Start()
    {

    }
    void Update()
    {
        if (timeMove <= timeDelayMove)
            timeMove += Time.deltaTime;
        Keyboard();
        Mouse();
    }

    private void Keyboard()
    {
        string key = "";
        if (Input.GetKeyDown(KeyCode.W))
        {
            key = "w";

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            key = "s";

        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            key = "a";

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            key = "d";

        }
        if (!string.IsNullOrEmpty(key) && timeMove >= timeDelayMove)
        {
            SendKey(key);
            timeMove = 0;
        }

        if (Input.GetKeyDown(KeyCode.M))
            ClientManager.clientTcp.SendToServer(new ByteBuffer(), PacketsConnection.PacketServer.Game, (int)PacketsConnection.InputServerGame.InteractionFromServer);
    }
    private void Mouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                PlayerSettings settings = hit.collider.gameObject.GetComponent<PlayerSettings>();
                if (settings != null)
                {
                    Destroy(targetHud);
                    if (settings.character.name != SettingUser.Character.name)
                    {
                        if (hit.collider.gameObject == this.target)
                        {

                            AtackData target = new AtackData("", -1, AtackData.Type.Null);
                            ClientManager.Atack(target);
                            this.target = null;
                        }
                        else
                        {
                            targetHud = Instantiate(prefabTargetHud, new Vector3(hit.collider.transform.position.x, 0, hit.collider.transform.position.z), Quaternion.Euler(0, 0, 0), hit.collider.gameObject.transform);
                            Debug.Log("I send target to server.");
                            this.target = hit.collider.gameObject;
                            AtackData target = new AtackData(settings.character.name);
                            ClientManager.Atack(target);
                        }
                    }
                }
            }
        }
    }
    public void SendKey(string key)
    {
        ByteBuffer data = new ByteBuffer();
        data.WriteString(key);
        ClientManager.clientTcp.SendToServer(data, PacketsConnection.PacketServer.Game, (int)PacketsConnection.InputClientGame.MovePlayer);
    }
}