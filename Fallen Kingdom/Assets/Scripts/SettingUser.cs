using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingdomData;

public class SettingUser {

    private static Account account;
    public static Account Account {
        get {
            return account;
        }
        set
        {
            account = value;

            if (value == null)
            {
                isLogin = false;
                Logout();
            }
            else
            {
                isLogin = true;
                characterManagerHud(account);
            }
        } }

    private static Character character;
    public static Character Character {
        get
        {
            return character;
        }
        set
        {
            if (value == null)
            {
                isGame = false;
                ClientManager.SetScene("Menu");
            }
            else
            {
                isGame = true;
                ClientManager.SetScene("Game");
            }

            character = value;
        }
    }

    public static bool isLogin = false;
    public static bool isGame = false;

    public delegate void _Account(Account account);
    public static _Account characterManagerHud;

    private static void Logout()
    {
        Character = null;
    }
}
