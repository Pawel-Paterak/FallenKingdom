using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour {

    public string nameLanguage;
    public int index;

    private Button btn;
    private Text text;

	public void Create(string name, int index)
    {
        nameLanguage = name;
        this.index = index;

        if (text == null)
            text = transform.Find("Text").GetComponent<Text>();

        if (btn == null)
            btn = GetComponent<Button>();

        text.text = nameLanguage;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(SetLanguage);
    }

    private void SetLanguage()
    {
        LanguageManager.SetIndexLanguage = index;
    }
}
