using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    public Dictionary<string, int> inputs;
    Dictionary<string, KeyCode> keyMappings;
    public string inputType;
    public GameObject player;
    private GameObject jumping;
    float fireTime = 0;
    bool blockToggle = false;

    protected void Start()
    {
        player = GameObject.Find("player");
        keyMappings = new Dictionary<string, KeyCode> {
            ["forward"] = KeyCode.W,
            ["back"] = KeyCode.S,
            ["left"] = KeyCode.A,
            ["right"] = KeyCode.D,
            ["jump"] = KeyCode.Space,
            ["throw"] = KeyCode.J,
            ["dash"] = KeyCode.P,
            ["fire"] = KeyCode.L,
            ["attack"] = KeyCode.K,
            ["block"] = KeyCode.O,
        };
        resetInputs();
    }
    protected void resetInputs() {
        inputs = new Dictionary<string, int> {
            ["forward"] = 0,
            ["back"] = 0,
            ["left"] = 0,
            ["right"] = 0,
            ["jump"] = 0,
            ["throw"] = 0,
            ["dash"] = 0,
            ["fire"] = 0,
            ["attack"] = 0,
            ["block"] = 0,
        };
    }
    protected void Update()
    {
        if (inputType == "human") {
            resetInputs();
            for (int i = 0; i < inputs.Count; i++) {
                string key = inputs.Keys.ToList()[i];
                if (Input.GetKeyDown(keyMappings[key])) {
                    inputs[key] = 1;
                }
                else if (Input.GetKey(keyMappings[key])) {
                    inputs[key] = 2;
                }
                else {
                    inputs[key] = 0;
                }
                if (Input.GetKeyUp(keyMappings[key])) {
                    inputs[key] = 3;
                }
            }
        }
        else if (inputType == "random") {
            for (int i = 0; i < inputs.Count; i++) {
                string key = inputs.Keys.ToList()[i];
                if (inputs[key] == 3) {
                    inputs[key] = 0;
                    continue;
                }
                if (inputs[key] == 1) {
                    inputs[key] = 2;
                    continue;
                }
                if (UnityEngine.Random.Range(0, 100) == 1) {
                    if (inputs[key] % 2 == 0) {
                        inputs[key] += 1;
                    }
                }

                // print(key + " " + inputs[key]);
            }
        }
        else if (inputType == "chase") {
            resetInputs();
            GameObject target = player;
            Vector3 direction = transform.position - target.transform.position;
            Vector3 lookAtAngle = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
            lookAtAngle.y += 180;
            if (180 > transform.eulerAngles.y % 360 - lookAtAngle.y % 360 && transform.eulerAngles.y % 360 - lookAtAngle.y % 360 > 0 ||
            transform.eulerAngles.y % 360 - lookAtAngle.y % 360 < -180) {
                inputs["left"] = 2;
                inputs["right"] = 0;
            }
            else {
                inputs["right"] = 2;
                inputs["left"] = 0;
            }
            if (GameObject.Find("speardisplay") != null && !GetComponent<PlayerBehaviour>().spear) {
                target = GameObject.Find("speardisplay");
                // inputs["fire"] = 1;
            }
            if ((transform.position - target.transform.position).magnitude > 3) {
                return;
            }
            inputs["attack"] = 1;
            if ((transform.position - target.transform.position).magnitude < 1) {
                return;
            }
            inputs["forward"] = 2;
        }
        else if (inputType == "evade") {
            resetInputs();
            GameObject target = player;
            Vector3 direction = transform.position - target.transform.position;
            Vector3 lookAtAngle = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
            if (!jumping && (transform.position - target.transform.position).magnitude > 3) {
                return;
            }
            if (transform.parent && transform.parent.name.Contains("Platform")) {
                float xDis = Mathf.Pow(gameObject.transform.localPosition.x, 2); 
                float zDis = Mathf.Pow(gameObject.transform.localPosition.z, 2); 

                if (xDis + zDis > Mathf.Pow(0.25f * transform.parent.Find("PlatformChild").localScale.x, 2)) {
                    if (!jumping) {
                        inputs["jump"] = 1;
                        jumping = gameObject.transform.parent.gameObject;
                    }
                }
                else {
                    jumping = null;
                }
            }
            else if (jumping) {
                target = jumping;
                direction = transform.position - target.transform.position;
                lookAtAngle = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
                lookAtAngle.y += 180;
            }
            if (180 > transform.eulerAngles.y % 360 - lookAtAngle.y % 360 && transform.eulerAngles.y % 360 - lookAtAngle.y % 360 > 0 ||
            transform.eulerAngles.y % 360 - lookAtAngle.y % 360 < -180) {
                inputs["left"] = 2;
                inputs["right"] = 0;
            }
            else {
                inputs["right"] = 2;
                inputs["left"] = 0;
            }
            inputs["forward"] = 2;
        }
        else if (inputType == "kite") {
            resetInputs();
            GameObject target = player;
            if (Time.time - fireTime > 0.2) {
                float rand = UnityEngine.Random.Range(0, 1f);
                if (rand > 0.4) {
                    inputs["fire"] = 1; 
                }
                else if (rand > 0.2 && !blockToggle ||  blockToggle) {
                    blockToggle = !blockToggle;
                }
                fireTime = Time.time;
            }
            inputs["block"] = blockToggle ? 2 : 0;
            Vector3 direction = transform.position - target.transform.position;
            Vector3 lookAtAngle = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
            lookAtAngle.y += 180;
            if (180 > transform.eulerAngles.y % 360 - lookAtAngle.y % 360 && transform.eulerAngles.y % 360 - lookAtAngle.y % 360 > 0 ||
            transform.eulerAngles.y % 360 - lookAtAngle.y % 360 < -180) {
                inputs["left"] = 2;
                inputs["right"] = 0;
            }
            else {
                inputs["right"] = 2;
                inputs["left"] = 0;
            }
            if (transform.parent && transform.parent.name.Contains("Platform")) {
                float xDis = Mathf.Pow(gameObject.transform.localPosition.x, 2); 
                float zDis = Mathf.Pow(gameObject.transform.localPosition.z, 2); 

                if (xDis + zDis > Mathf.Pow(0.3f * transform.parent.Find("PlatformChild").localScale.x, 2)) {
                    if (!jumping) {
                        inputs["jump"] = 1;
                        jumping = gameObject.transform.parent.gameObject;
                    }
                }
                else {
                    jumping = null;
                    if ((transform.position - target.transform.position).magnitude < 3) {
                        inputs["back"] = 2;
                    }
                    else if ((transform.position - target.transform.position).magnitude > 6) {
                        inputs["forward"] = 2;
                    }
                }
            }
            else if (jumping) {
                target = jumping;
                direction = transform.position - target.transform.position;
                lookAtAngle = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
                lookAtAngle.y += 180;
                inputs["forward"] = 2;
            }
        }
    }
}
