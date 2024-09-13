using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public string item = "";
    string[] items = {"bigSpear", "bigBoi", "sped", "yump", "critChance", "anothaJump", "dash", "dmg", "healthRegen"};
    public int price = 0;
    public bool attacked = false;
    void Start()
    {
        int rand = Random.Range(0, items.Length);
        item = items[rand];
        price = Random.Range(20, 50);
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<EntityBehaviour>().health < GetComponent<EntityBehaviour>().maxHealth && !attacked) {
            attacked = true;
            GetComponent<InputManager>().player.GetComponent<Dialogue>().current = null;
            GetComponent<InputManager>().inputType = "kite";
        }
    }
}
