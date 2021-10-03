using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KingdomData;

public class ItemButton : MonoBehaviour {

    public Text text;
    public Button button;

    public void Initialize(string name)
    {
        text = transform.Find("Text").GetComponent<Text>();
        button = GetComponent<Button>();

        text.text = name;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Click);
    }

    private void Click()
    {
        Item item = null;
        item = AssetsManager.Load<Item>(EditorManager.pathItems + text.text);
        Food food = AssetsManager.Load<Food>(EditorManager.pathItems + text.text);
        Weapon weapon = AssetsManager.Load<Weapon>(EditorManager.pathItems+text.text);

        if (weapon != null)
            item = weapon; 
        else if (food != null)
            item = food;

        if (item != null)
        {
            MenuEditor.singleton.LoadItem(item);
            MenuEditor.singleton.BtnToItemInspector();
        }
    }
}
