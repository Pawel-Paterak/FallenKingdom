using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInformation : MonoBehaviour {

    public string information;
    public Color colorText;

    public float speed;
    public bool flyUp;

    public float destroyTime;
    private float time;
    public bool destroy;

    private TextMesh text;

	void Start () {
        text = GetComponent<TextMesh>();
        text.text = information;
        text.color = colorText;
	}

    public void Update()
    {
        if (flyUp)
            transform.position += Vector3.forward * speed * Time.deltaTime;

        if(destroy)
        {
            time += Time.deltaTime;
            if (time >= destroyTime)
                Destroy(gameObject);
        }
    }
}
