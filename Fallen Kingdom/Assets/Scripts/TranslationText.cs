using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslationText : MonoBehaviour {

    public string input;
    public Text text;

    void Start () {
        text = GetComponent<Text>();

        LanguageManager.translations.Add(this);
        Translation();
	}

    public void Translation()
    {
        text.text = input.Translate();
    }
}
