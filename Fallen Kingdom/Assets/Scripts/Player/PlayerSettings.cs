using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingdomData;

public class PlayerSettings : MonoBehaviour {

    public Character character;
    public Health health;

    public void Initialize(Character character)
    {
        health = GetComponent<Health>();
        this.character = character;
        Vector3 position = new Vector3(character.position.Vector3d().x, transform.localScale.y, character.position.Vector3d().z);
        int height = character.position.y;

        TextMesh textName = transform.Find("Player_Objects_Prefab").GetComponentInChildren<TextMesh>();
        textName.text = character.name;

        name = character.name;
        transform.position = position;
        health.ChangeFields(character.life, character.mana);
    }
}
