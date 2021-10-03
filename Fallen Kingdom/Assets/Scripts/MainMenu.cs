using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using KingdomData;

public class MainMenu : MonoBehaviour {

    public enum Window {Menu,Login,Register,Languages,Exit,CharacterManager,CreateCharacter};
    private Window window;
    public Window activeWindow { get { return window; }set { window = value; SetWindow(value); } }

    [Header("Canvas")]
    public GameObject canvasMenu;
    public GameObject canvasLogin;
    public GameObject canvasRegister;
    public GameObject canvasLanguages;
    public GameObject canvasExit;
    public GameObject canvasCharacterManager;
    public GameObject canvasCreateCharacter;

    [Header("Inputs")]
    public LoginInput loginInput;
    public RegisterInput registerInput;
    public CharacterInput characterInput;
    [Header("Prefabs")]
    public GameObject characterPref;
    public GameObject languageButton;
    public GameObject contentLanguages;

    #region "Thread Field"
    private string recaiveServerInformationLogin = "";
    private string recaiveServerInformationRegister = "";
    private string recaiveServerInformationCreateCharacter = "";

    private string nameCharacter = "";
    private bool refreshCharacter = false;

    private bool refreshHud = false;
    private bool refreshAccount = false;
    private Account accountHud;
    private Account AccountHud {
        get { return accountHud; }
        set
        {
            accountHud = value;
            refreshAccount = true;
        }
    }
    #endregion

    public List<CharacterButton> characterButton = new List<CharacterButton>();

    void Awake () {
        AssetsManager.Veryfication();
        LanguageManager.createButton = CreateButtonLanguage;
        LanguageManager.LoadLanguages();
        LanguageManager.SetIndexLanguage = 1;

        SettingUser.characterManagerHud = SetCharacterManager;

        BundleManager.informationLogin = InformationLogin;
        BundleManager.informationRegister = InformationRegister;
        BundleManager.informationCreateCharacter = InformationCreateCharacter;
        BundleManager.addCharacter = new BundleManager._Information(delegate(string name)
        {
            nameCharacter = name;
            refreshCharacter = true;
        });

        activeWindow = Window.Menu;
    }
    void Update () {

       

        if(recaiveServerInformationLogin.Length > 0)
            loginInput.information.text = recaiveServerInformationLogin.Translate();

        if (recaiveServerInformationRegister.Length > 0)
            registerInput.information.text = recaiveServerInformationRegister.Translate();

        if (recaiveServerInformationCreateCharacter.Length > 0)
            characterInput.information.text = recaiveServerInformationCreateCharacter.Translate();

        if(refreshHud)
        {
            refreshHud = false;
            RefreshHud();
        }

        if(refreshAccount)
        {
            refreshAccount = false;
            CreateCharacters();
            BtnCharacters();
        }

        if(refreshCharacter)
        {
            if(!String.IsNullOrEmpty(nameCharacter))
                AddCharacter(nameCharacter);
            refreshCharacter = false;
            nameCharacter = "";
        }
    }

    private void RefreshCanvas()
    {
        canvasMenu.SetActive(false);
        canvasLogin.SetActive(false);
        canvasRegister.SetActive(false);
        canvasLanguages.SetActive(false);
        canvasExit.SetActive(false);
        canvasCharacterManager.SetActive(false);
        canvasCreateCharacter.SetActive(false);
    }
    private void RefreshHud()
    {
        recaiveServerInformationLogin = "";
        recaiveServerInformationRegister = "";
        recaiveServerInformationCreateCharacter = "";
        loginInput.login.text = "";
        loginInput.password.text = "";
        registerInput.login.text = "";
        registerInput.password.text = "";
        registerInput.tryPassword.text = "";
        characterInput.name.text = "";
        characterInput.information.text = "";
    }
    private void SetWindow(Window window)
    {
        RefreshCanvas();
        switch(window)
        {
            case Window.Menu:
                canvasMenu.SetActive(true);
                break;
            case Window.Login:
                canvasLogin.SetActive(true);
                break;
            case Window.Register:
                canvasRegister.SetActive(true);
                break;
            case Window.Languages:
                canvasLanguages.SetActive(true);
                break;
            case Window.Exit:
                canvasExit.SetActive(true);
                break;
            case Window.CharacterManager:
                canvasCharacterManager.SetActive(true);
                break;
            case Window.CreateCharacter:
                canvasCreateCharacter.SetActive(true);
                break;
        }
    }

    private void CreateCharacters()
    {
        foreach (CharacterButton btn in characterButton.ToArray())
            Destroy(btn.transform.gameObject);

        characterButton.Clear();
        foreach (string character in AccountHud.character)
            AddCharacter(character);
    }
    public void AddCharacter(string name)
    {
        GameObject obj = Instantiate(characterPref, characterInput.content.transform);
        CharacterButton button = obj.GetComponent<CharacterButton>();
        button.Create(name);
        characterButton.Add(button);
    }
    public void SetCharacterManager(Account account)
    {
        refreshHud = true;
        AccountHud = account;
    }
    public void CreateButtonLanguage(string name, int index)
    {
        GameObject btn = Instantiate(languageButton, contentLanguages.transform);
        LanguageButton languageBtn = btn.GetComponent<LanguageButton>();
        languageBtn.Create(name, index);
    }
    private void InformationLogin(string info)
    {
        recaiveServerInformationLogin = info;
    }
    private void InformationRegister(string info)
    {
        recaiveServerInformationRegister = info;
    }
    private void InformationCreateCharacter(string info)
    {
        recaiveServerInformationCreateCharacter = info;
    }

    #region "Buttons"
    public void BtnMenu()
    {
        activeWindow = Window.Menu;
    }
    public void BtnLogin()
    {
        activeWindow = Window.Login;
        RefreshHud();
    }
    public void BtnLoginToServer()
    {
        Account account = new Account();
        account.login = loginInput.login.text;
        account.password = loginInput.password.text;
        ClientManager.Login(account);
    }
    public void BtnRegister()
    {
        activeWindow = Window.Register;
        RefreshHud();
    }
    public void BtnRegisterToServer()
    {
        string login = registerInput.login.text;
        string password = registerInput.password.text;
        string tryPassword = registerInput.tryPassword.text;
        registerInput.information.text = "";
        if (login.Length < 5)
            registerInput.information.text += "Login is short. Min 5 chars.\n".Translate();
        if (password.Length < 5)
            registerInput.information.text += "Password is short. Min 5 chars.\n".Translate();
        if (password != tryPassword)
            registerInput.information.text += "Password != try Password.\n".Translate();

        if (registerInput.information.text.Length < 1)
        {
            Account account = new Account();
            account.login = login;
            account.password = password;
            ClientManager.Register(account);
        }
    }
    public void BtnLanguages()
    {
        activeWindow = Window.Languages;
    }
    public void BtnExit()
    {
        activeWindow = Window.Exit;
    }
    public void BtnExitYes()
    {
        Application.Quit();
    }
    public void BtnExitNo()
    {
        activeWindow = Window.Menu;
    }
    public void BtnCreateCharacter()
    {
        activeWindow = Window.CreateCharacter;
        RefreshHud();
    }
    public void BtnCreateCharacterToServer()
    {
        if(characterInput.name.text.Length < 5)
            characterInput.information.text += "Name is short. Min 5 chars.\n".Translate();
        Debug.Log("Is good");
        if (characterInput.information.text.Length < 1)
        {
            Character character = new Character(characterInput.name.text, characterInput.type.value);
            ClientManager.CharacterCreate(character, SettingUser.Account);
            Debug.Log("send character");
        }
    }
    public void BtnCharacters()
    {
        activeWindow = Window.CharacterManager;
    }
    #endregion
}
[Serializable]
public class LoginInput
{
    public InputField login;
    public InputField password;
    public Text information;
}
[Serializable]
public class RegisterInput
{
    public InputField login;
    public InputField password;
    public InputField tryPassword;
    public Text information;
}
[Serializable]
public class CharacterInput
{
    public InputField name;
    public Dropdown type;
    public Text information;
    public GameObject content;
}
