using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : EntityBehaviour
{
    public GameObject player;
    float lastShot;
    new void Start()
    {
        base.Start();
        worldScript = GameObject.Find("WorldObject").GetComponent<WorldScript>();
        player = GameObject.Find("player");
        lastShot = Time.time;
    }

    new void Update()
    {
        base.Update();
        Vector3 direction = transform.position - player.transform.position;
        Vector3 lookAtAngle = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
    lookAtAngle.y += 180;
        transform.eulerAngles = new Vector3(0, lookAtAngle.y, 0);
        if (Time.time - lastShot > 4) {
            GameObject spell = (GameObject) Instantiate(spellobj, transform.position + transform.forward * 0.8f, transform.rotation);
            spell.transform.Rotate(0, 270, 90);
            spell.GetComponent<SpellBehaviour>().owner = gameObject;
            spell.GetComponent<SpellBehaviour>().dmg = 6;
            worldScript.thrownSpells.Add(spell);
            lastShot = Time.time;
            spell.GetComponent<SpellBehaviour>().activateTime = 0.25f;
            
        }
    }
    
}
