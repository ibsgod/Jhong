using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DroneBehaviour : EntityBehaviour
{
    public GameObject player;
    float lastShot;
    new void Start()
    {
        base.Start();
        worldScript = GameObject.Find("WorldObject").GetComponent<WorldScript>();
        player = GameObject.Find("player").transform.Find("Center").gameObject;
        lastShot = Time.time;
    }

    new void Update()
    {
        base.Update();
        transform.parent = null;
        transform.LookAt(player.transform.position);
        if (Time.time - lastShot > 2) {
            GameObject spell = (GameObject) Instantiate(spellobj, transform.position + transform.forward * 0.8f, transform.rotation);
            spell.transform.Rotate(0, 270, 90);
            spell.GetComponent<SpellBehaviour>().owner = gameObject;
            spell.GetComponent<SpellBehaviour>().dmg = 1;
            worldScript.thrownSpells.Add(spell);
            lastShot = Time.time;
            spell.GetComponent<SpellBehaviour>().activateTime = 0.25f;
            
        }
        if ((transform.position - (player.transform.position + new Vector3(0, 1, 0))).magnitude > 2) {
            GetComponent<Rigidbody>().velocity = (player.transform.position + new Vector3(0, 1, 0) - transform.position).normalized * 4;
        }
        else {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    
}
