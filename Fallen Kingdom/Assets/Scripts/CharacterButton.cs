using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour {

    public string nameCharacter;
    public Text text;
    public Button button;

    private void GetField()
    {
        text = transform.Find("Text").GetComponent<Text>();
        button = GetComponent<Button>();
    }

	public void Create(string name)
    {
        GetField();
        nameCharacter = name;
        text.text = name;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);

    }

    public void OnClick()
    {
        ClientManager.JoinToGame(nameCharacter);
    }
}
