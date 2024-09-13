using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawn : MonoBehaviour
{
    public GameObject player; 
    public UnityEngine.Object platform;
    public UnityEngine.Object platformFall;
    public UnityEngine.Object turret;
    public UnityEngine.Object drone;
    public UnityEngine.Object playerobj;
    public UnityEngine.Object merchant;
    public UnityEngine.Object ninja;
    float heightGap = 3;
    List<GameObject> platforms = new List<GameObject>();
    void Start()
    {
        platform = Resources.Load("Platform");
        platformFall = Resources.Load("PlatformFall");
        turret = Resources.Load("Turret");
        drone = Resources.Load("Drone");
        playerobj = Resources.Load("player");
        merchant = Resources.Load("merchant");
        ninja = Resources.Load("ninja");
    }

    void Update()
    {
        if (System.Math.Floor((player.transform.position.y + heightGap * 3) / heightGap) > platforms.Count) {
            float xRand = UnityEngine.Random.Range(4, 6) * Mathf.Sign(UnityEngine.Random.Range(-1, 1f));
            float yRand = UnityEngine.Random.Range(0, 1);
            float zRand = UnityEngine.Random.Range(4, 6) * Mathf.Sign(UnityEngine.Random.Range(-1, 1f));
            UnityEngine.Object randPlatform = UnityEngine.Random.Range(0, 1f) > 0.2 ? platform : platformFall;
            GameObject newPlatform = (GameObject) Instantiate(randPlatform, new Vector3 (
                xRand + ((platforms.Count > 0) ? platforms[platforms.Count - 1].transform.position.x : 0),
                yRand + platforms.Count * heightGap, 
                zRand + ((platforms.Count > 0) ? platforms[platforms.Count - 1].transform.position.z : 0)
            ), Quaternion.identity);
            float scaleRand = UnityEngine.Random.Range(4, 8);
            newPlatform.transform.Find("PlatformChild").localScale = new Vector3(scaleRand, 0.3f, scaleRand);
            Vector3 randomRot = new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10));
            // newPlatform.transform.eulerAngles = randomRot;
            if (newPlatform.GetComponent<MovingPlatform>()) {
                newPlatform.GetComponent<MovingPlatform>().moveX = UnityEngine.Random.Range(0, 1f) > 0.5;
                newPlatform.GetComponent<MovingPlatform>().moveY = UnityEngine.Random.Range(0, 1f) > 0.5;
                newPlatform.GetComponent<MovingPlatform>().moveZ = UnityEngine.Random.Range(0, 1f) > 0.5;
                newPlatform.GetComponent<MovingPlatform>().moveDir = UnityEngine.Random.Range(0.2f, 1f);
            }
            
            for (int i = 0; i < Math.Floor(scaleRand / 3.5); i++) {
                float spawnRand = UnityEngine.Random.Range(0f, 1);
                if (spawnRand > 0.8) {
                    Instantiate(turret, newPlatform.transform.position + Vector3.up, Quaternion.identity);
                }
                else if (spawnRand > 0.7) {
                    Instantiate(drone, newPlatform.transform.position + Vector3.up, Quaternion.identity);
                }
                else if (spawnRand > 0.6) {
                    GameObject newplayer = (GameObject) Instantiate(merchant, newPlatform.transform.position + Vector3.up, Quaternion.identity);
                    // newplayer.GetComponent<InputManager>().inputType = "evade";
                }
                else if (spawnRand > 0.4) {
                    GameObject newplayer = (GameObject) Instantiate(ninja, newPlatform.transform.position + Vector3.up, Quaternion.identity);
                    newplayer.GetComponent<InputManager>().inputType = "chase";
                }
                else if (spawnRand > 0.2) {
                    GameObject newplayer = (GameObject) Instantiate(playerobj, newPlatform.transform.position + Vector3.up, Quaternion.identity);
                    newplayer.GetComponent<InputManager>().inputType = "evade";
                }
            }
            platforms.Add(newPlatform);
        }

    }
}
