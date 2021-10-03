using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class WorldButton : MonoBehaviour {

    public string nameWorld;
    public Text text;
    public Button button;

    public void Initialize(string nameWorld)
    {
        text = transform.Find("Text").GetComponent<Text>();
        button = GetComponent<Button>();

        this.nameWorld = nameWorld;
        text.text = nameWorld;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Click);
    }

    private void Click()
    {
        MenuEditor menu = FindObjectOfType<MenuEditor>();
        menu.window = MenuEditor.Window.WorldInspector;
        menu.inputs.SelectWorld = nameWorld;
    }
}
