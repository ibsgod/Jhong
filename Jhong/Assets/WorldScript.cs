using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WorldScript : MonoBehaviour
{
    public List<GameObject> thrownWeapons = new List<GameObject>();
    public List<GameObject> thrownSpells = new List<GameObject>();
    public List<GameObject> entities = new List<GameObject>();
    public Dictionary<GameObject, List<string>> attacks = new Dictionary<GameObject, List<string>>();
    float lastSpawn;
    float minX = -50;
    float maxX = 50;
    float minZ = -50;
    float maxZ = 50;
    Object turret;
    AudioClip coinsfx;
    AudioSource audioSource;
    public Object coin;
    bool pause = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        turret = Resources.Load("Turret");
        coin = Resources.Load("Coin");
        coinsfx = (AudioClip) Resources.Load("coins");
        lastSpawn = Time.time;
        GameObject plane = GameObject.Find("Plane");
        // minX = plane.GetComponent<MeshFilter>().mesh.bounds.min.x * plane.transform.localScale.x;
        // maxX = plane.GetComponent<MeshFilter>().mesh.bounds.max.x * plane.transform.localScale.x;
        // minZ = plane.GetComponent<MeshFilter>().mesh.bounds.min.z * plane.transform.localScale.z;
        // maxZ = plane.GetComponent<MeshFilter>().mesh.bounds.max.z * plane.transform.localScale.z;
    }

    void Update()
    {
        Time.timeScale = pause ? 0 : 1;
        if (Time.time - lastSpawn > 5) {
            float randX = Random.Range(minX, maxX);
            float randZ = Random.Range(minZ, maxZ);
            // Instantiate(turret, new Vector3(randX, 5, randZ), Quaternion.identity);
            lastSpawn = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            pause = !pause;
        }
    }
    public void enemyKilled(Vector3 position) {
        audioSource.PlayOneShot(coinsfx);
        for (int i = 0; i < Random.Range(20, 50); i++) {
            GameObject c = (GameObject) Instantiate(coin, position, Quaternion.identity);
            c.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(200, 400), UnityEngine.Random.Range(-50, 50));
            c.GetComponent<Rigidbody>().angularVelocity = new Vector3(UnityEngine.Random.Range(-500, 500), UnityEngine.Random.Range(-500, 500), UnityEngine.Random.Range(-500, 500));
        }
    }
}
