using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;

    public Color colorTextHealth;
    public Color colorTextMana;

    public GameObject healthRenderer;
    public GameObject manaRenderer;
    public GameObject textInformation;

    public float maxScaleHud;

    public void SetFields(int maxHealth, int maxMana, int health, int mana)
    {
        this.maxHealth = maxHealth;
        this.maxMana = maxMana;

        this.health = health;
        this.mana = mana;
    }
    public void ChangeFields(int health, int mana)
    {
        ChangeHealth(health);
        ChangeMana(mana);
    }
    public void ChangeHealth(int health)
    {
        int healthDifference = health - this.health;
        this.health = health;

        string text = "";
        if (healthDifference > 0)
            text = "+" + healthDifference;
        else if (healthDifference < 0)
            text = "-" + healthDifference;

        if (healthDifference != 0)
        {
            GameObject textObj = Instantiate(textInformation, transform.position, Quaternion.Euler(90, 0, 0));
            TextInformation information = textObj.GetComponent<TextInformation>();
            information.information = text;
            information.colorText = colorTextHealth;
            information.flyUp = true;
            information.speed = 2;
            information.destroyTime = 2;
            information.destroy = true;
        }
    }
    public void ChangeMana(int mana)
    {
        int manaDifference = mana - this.mana;
        this.mana = mana;

        string text = "";
        if (manaDifference > 0)
            text = "+" + manaDifference;
        else if (manaDifference < 0)
            text = "-" + manaDifference;

        if(manaDifference != 0)
        {
            GameObject textObj = Instantiate(textInformation, transform.position, Quaternion.Euler(90, 0, 0));
            TextInformation information = textObj.GetComponent<TextInformation>();
            information.information = text;
            information.colorText = colorTextMana;
            information.flyUp = true;
            information.speed = 2;
            information.destroyTime = 2;
            information.destroy = true;
        }
    }

    public void Start()
    {
       maxScaleHud = healthRenderer.transform.localScale.x;
    }

    public void Update()
    {
        if (health < 0)
            health = 0;

        if (mana < 0)
            mana = 0;

        if (health > maxHealth)
            health = maxHealth;

        if (mana > maxMana)
            mana = maxMana;

        float percentScaleHealth = (float)health / (float)maxHealth;
        float percentScaleMana = (float)mana / (float)maxMana;

        healthRenderer.transform.localScale = new Vector3((float)(maxScaleHud * percentScaleHealth), healthRenderer.transform.localScale.y, healthRenderer.transform.localScale.z);
        manaRenderer.transform.localScale = new Vector3((float)(maxScaleHud * percentScaleMana), manaRenderer.transform.localScale.y, manaRenderer.transform.localScale.z);
    }
}
