using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KingdomData;

public class MenuEditor : MonoBehaviour {

    public enum Window { Menu, Exit, Worlds, Items, WorldInspector, ItemInspector };
    private Window activeWindow;
    public Window window {
        get {
            return activeWindow;
        }
        set
        {
            activeWindow = value;
            RefreshWindow();
        }
    }
    public static MenuEditor singleton;
    [Header("Windows")]
    public GameObject windowMenu;
    public GameObject windowWorlds;
    public GameObject windowExit;
    public GameObject windowWorldInspector;
    public GameObject windowItems;
    public GameObject windowItemInspector;
    public List<WindowInWindow> windowsInWindow;
    [Header("Inputs")]
    public InputEditor inputs;

    void Start() {
        inputs.selectType.value = 2;
        window = Window.Menu;
        if (singleton != null)
            singleton = null;
        singleton = this;

    }
    void Update() {
        if (inputs.loadWorld)
        {
            inputs.loadWorld = false;
            FindObjectOfType<WorldManagerEditor>().LaodWorld(inputs.selectWorld);
        }

        if ((Item.TypeItem)inputs.selectType.value != inputs.typeItem)
        {
            inputs.typeItem = (Item.TypeItem)inputs.selectType.value;
            SetActiveWindowInWindows(false, WindowInWindow.TypeWindow.Weapon);
            SetActiveWindowInWindows(false, WindowInWindow.TypeWindow.Food);
            if (inputs.typeItem == Item.TypeItem.Weapon)
                SetActiveWindowInWindows(true, WindowInWindow.TypeWindow.Weapon);
            else if (inputs.typeItem == Item.TypeItem.Food)
                SetActiveWindowInWindows(true, WindowInWindow.TypeWindow.Food);

        }
    }

    private void DisableWindows()
    {
        windowMenu.SetActive(false);
        windowWorlds.SetActive(false);
        windowExit.SetActive(false);
        windowWorldInspector.SetActive(false);
        windowItems.SetActive(false);
        windowItemInspector.SetActive(false);
    }
    public void RefreshWindow()
    {
        FindObjectOfType<EditorControler>().build = false;
        DisableWindows();
        switch (activeWindow)
        {
            case Window.Menu:
                windowMenu.SetActive(true);
                break;
            case Window.Worlds:
                windowWorlds.SetActive(true);
                break;
            case Window.WorldInspector:
                {
                    FindObjectOfType<EditorControler>().build = true;
                    windowWorldInspector.SetActive(true);

                    break;
                }
            case Window.Exit:
                windowExit.SetActive(true);
                break;
            case Window.Items:
                windowItems.SetActive(true);
                break;
            case Window.ItemInspector:
                windowItemInspector.SetActive(true);
                break;
            default:
                Debug.Log("this window " + activeWindow.ToString() + " is not exist ");
                break;
        }
    }
    public void SetActiveWindowInWindows(bool enable, WindowInWindow.TypeWindow type)
    {
        foreach (WindowInWindow coor in windowsInWindow)
            if (coor.typeWindow == type)
            {
                coor.window.SetActive(enable);
                return;
            }
    }
    public void LoadItem(Item item)
    {
        if (item == null)
            return;

        inputs.damage.text = "";
        inputs.distanceAtack.text = "";
        inputs.health.text = "";
        inputs.mana.text = "";

        Weapon weapon = null;
        Food food = null;
        switch(item.typeItem)
        {
            case Item.TypeItem.Weapon:
                weapon = (Weapon)item;
                break;
            case Item.TypeItem.Food:
                food = (Food)item;
                break;
        }

        inputs.name.text = item.name;
        inputs.weight.text = item.weight.ToString();
        inputs.Texture = item.nameTexture;
        inputs.selectType.value = 0;

        if(weapon != null)
        {
            inputs.damage.text = weapon.damage.ToString();
            inputs.distanceAtack.text = weapon.distanceAtack.ToString();
            inputs.selectType.value = 1;
        }
        else if(food != null)
        {
            Debug.Log("Food");
            inputs.health.text = food.health.ToString();
            inputs.mana.text = food.mana.ToString();
            inputs.selectType.value = 2;
        }
    }

    #region "Buttons"
    public void BtnToMenu()
    {
        window = Window.Menu;
    }
    public void BtnToWorlds()
    {
        window = Window.Worlds;
    }
    public void BtnToExit()
    {
        window = Window.Exit;
    }
    public void BtnExitFromInspector()
    {
        SetActiveWindowInWindows(true, WindowInWindow.TypeWindow.ExitInspector);
    }
    public void BtnExitFromInspectorNo()
    {
        SetActiveWindowInWindows(false, WindowInWindow.TypeWindow.ExitInspector);
    }
    public void BtnExitFromInspectorYes()
    {
        SetActiveWindowInWindows(false, WindowInWindow.TypeWindow.ExitInspector);
        window = Window.Menu;
    }
    public void BtnExitFromEditor()
    {
        Application.Quit();
    }
    public void BtnSaveWorld()
    {
        FindObjectOfType<WorldManagerEditor>().CreateWorld(inputs.selectWorld);
    }
    public void BtnToItems()
    {
        window = Window.Items;
    }
    public void BtnToItemInspector()
    {
        window = Window.ItemInspector;
    }
    public void BtnSaveItem()
    {
        float weight = 0;
        float.TryParse(inputs.weight.text, out weight);
        string name = inputs.name.text;
        Item.TypeItem typeItem = Item.TypeItem.Item;
        if (inputs.typeItem == Item.TypeItem.Item)
        {
            typeItem = Item.TypeItem.Item;
            Item item = new Item(name, inputs.Texture, typeItem, weight);
            AssetsManager.Save(EditorManager.pathItems + item.name + ".xml", item);
        }
        else if(inputs.typeItem == Item.TypeItem.Weapon)
        {
            int damage = 0;
            int defense = 0;
            float distanceAtack = 0;
            int.TryParse(inputs.damage.text, out damage);
            int.TryParse(inputs.defense.text, out defense);
            float.TryParse(inputs.distanceAtack.text, out distanceAtack);
            typeItem = Item.TypeItem.Weapon;
            Weapon weapon = new Weapon(name, inputs.Texture, typeItem, weight, damage, defense, distanceAtack);
            AssetsManager.Save(EditorManager.pathItems + weapon.name + ".xml", weapon);
        }
        else if(inputs.typeItem == Item.TypeItem.Food)
        {
            int health = 0;
            int mana = 0;
            int.TryParse(inputs.health.text, out health);
            int.TryParse(inputs.mana.text, out mana);
            typeItem = Item.TypeItem.Food;
            Food food = new Food(name, inputs.Texture, typeItem, weight, health, mana);
            AssetsManager.Save(EditorManager.pathItems + food.name + ".xml", food);
        }
        
        Item exitItem = GameResources.singleton.GetLoadItem(name);
        if (exitItem == null)
        {
            EditorManager.singleton.CreateButtonItems(name+".xml");
#if UNITY_EDITOR
            Paths paths = AssetsManager.Load<Paths>("Assets/Resources/" + AssetsManager.pathsItems + ".xml");
            if (paths != null)
            {
                paths.Add(new Path(0, name, typeItem));
                Debug.Log("Add paths");
                AssetsManager.Save("Assets/Resources/" + AssetsManager.pathsItems + ".xml", paths);
            }
            Debug.Log("Editor");
#endif
        }
    }
    #endregion
    
}
[System.Serializable]
public class WindowInWindow
{
    public enum TypeWindow {Terrains,ExitInspector,Weapon,Food}
    public TypeWindow typeWindow;
    public GameObject window;
}
[System.Serializable]
public class InputEditor
{
    public string selectWorld;
    public string SelectWorld
    {
        get
        {
            return selectWorld;
        }
        set
        {
            selectWorld = value;
            loadWorld = true;
        }
    }
    public bool loadWorld;

    [Header("Item Input")]
    public Dropdown selectType;
    public Item.TypeItem typeItem;
    public InputField name;
    private string texture;
    public string Texture
    {
        get
        {
            return texture;
        }
        set
        {
            texture = value;
            textureText.text = value;
        }
    }
    public Text textureText;
    public InputField weight;
    public InputField damage;
    public InputField defense;
    public InputField distanceAtack;
    public InputField health;
    public InputField mana;
}
