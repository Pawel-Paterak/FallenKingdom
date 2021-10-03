using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LanguageManager  {

    private static Language currentLanguage = new Language();
    public static Language SetLanguage{
        get {
            return currentLanguage;
        }
        set {
            currentLanguage = value;
            foreach (TranslationText coor in translations)
                coor.Translation();
        } }
    private static int indexLanguage = 0;
    public static int SetIndexLanguage { get { return indexLanguage; } set { SetIndex(value); } }

    public static List<Language> Languages = new List<Language>();
    public static List<TranslationText> translations = new List<TranslationText>();

    public delegate void LanguageButton(string name, int index);
    public static LanguageButton createButton;

    private static void SetIndex(int index)
    {
        if (index >= Languages.Count)
            return;

        indexLanguage = index;
        SetLanguage = Languages[index];
    }
    public static void LoadLanguages()
    {
        Languages.Clear();
        Languages.Add(null);

        TextAsset[] textAssets = Resources.LoadAll<TextAsset>(AssetsManager.languages);

        foreach(TextAsset coor in textAssets)
        {
            Language language = JsonUtility.FromJson<Language>(coor.text);
            if (language != null)
                AddLanguage(language);
        }
    }
    private static void AddLanguage(Language language)
    {
        createButton(language.fileName, Languages.Count);
        Languages.Add(language);
    }

}
