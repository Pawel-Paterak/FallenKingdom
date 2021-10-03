using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPathTexture : MonoBehaviour {

    public string text;
    public Button button;
    public Image image;

    public void Initialize(TextureGame texture)
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        text = texture.name;
        image.sprite = texture.sprite;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(Click);
    }

    private void Click()
    {
        MenuEditor.singleton.inputs.Texture = text;
    }
}
