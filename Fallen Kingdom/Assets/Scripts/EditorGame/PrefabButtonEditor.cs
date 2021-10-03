using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabButtonEditor : MonoBehaviour {

    public Image image;
    public Button button;
    public string namePrefab;

    public void InitializeButton(Sprite texture, string name)
    {
        if (texture != null)
        {
            image = GetComponent<Image>();
            image.sprite = texture;
        }
        namePrefab = name;
        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Click);
    }
    private void Click()
    {
        EditorControler controler = FindObjectOfType<EditorControler>();
        controler.typeBuild = EditorControler.TypeBuild.Terrain;
        controler.index = FindObjectOfType<GameResources>().GetIndexTextureFromName(namePrefab);

    }
}
