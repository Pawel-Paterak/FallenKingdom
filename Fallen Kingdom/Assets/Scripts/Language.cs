using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

[Serializable]
public class Language
{
    public string fileName;

    public List<Translation> translations = new List<Translation>();

    public Language ()
    {

    }
    public Language(string fileName)
    {
        this.fileName = fileName;
    }

    public Translation FindTranslation(string english)
    {
        foreach(Translation coor in translations)
        {
            if (coor.englishWord.ToLower() == english.ToLower())
                return coor;
        }
        return null;
    }

}
[Serializable]
public class Translation
{
    public string englishWord;
    public string languageWord;

    public Translation()
    {

    }

    public Translation(string englishWord, string languageWord)
    {
        this.englishWord = englishWord;
        this.languageWord = languageWord;
    }
}