using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class EntityBehaviour : MonoBehaviour
{
    public Object spellobj;
    public WorldScript worldScript;
    public AudioClip firespell;
    public Object text;
    public AudioSource audioSource;
    public bool jumping;
    public bool running;
    public bool onGround;
    protected int aerialJumped = 0;
    public int jumpMax = 1;
    public BarScript hbscr;
    public float maxHealth;
    public float health;
    public string lasthit;
    public GameObject lastAttacker;
    public float shieldHealth = 1;
    public bool blocking;
    public AudioClip shield;
    public GameObject thecamera;
    public List<AudioClip> painsfx = new List<AudioClip>();
    Dictionary<GameObject, List<string>> hitList = new Dictionary<GameObject, List<string>>();
    private AudioClip collectCoinsfx;
    protected int coinCount;
    public GameObject goldText;
    public GameObject altitudeText;
    public float critChance = 0.2f;
    public Material hurtMaterial;
    public Material normalMaterial;
    public Renderer meshrend;
    float flickerStart = -100;
    float flickerTimer = -100;
    bool hurtflicker = false; 
    protected int hurtIndex = 0;

    protected void Start() {
        collectCoinsfx = (AudioClip) Resources.Load("coincollect");
        hbscr = transform.Find("HealthBarCanvas/Healthbar").gameObject.GetComponent<BarScript>();
        hbscr.setMaxValue(maxHealth);
        hbscr.setValue(maxHealth);
        text = Resources.Load("Text");
        thecamera = GameObject.Find("Main Camera");
        worldScript = GameObject.Find("WorldObject").GetComponent<WorldScript>();
        if (!worldScript.attacks.ContainsKey(gameObject)) {
            worldScript.attacks[gameObject] = new List<string>();
        }
        worldScript.entities.Add(gameObject);
        audioSource = GetComponent<AudioSource>();

    }
    void flicker() {
        Material[] newlist = meshrend.materials;
        newlist[hurtIndex] = hurtflicker ? normalMaterial : hurtMaterial;
        meshrend.materials = newlist;
        hurtflicker = !hurtflicker;
    }
    protected void Update() {
        if (Time.time - flickerStart < 0.5) {
            if (Time.time - flickerTimer > 0.1) {
                flicker();
                flickerTimer = Time.time;
            }
        }
        else if (hurtflicker) {
            flicker();
        }
        health = hbscr.val;
        List<GameObject> attackers = worldScript.attacks.Keys.ToList();
        for (int i = 0; i < attackers.Count; i++) {
            if (!hitList.ContainsKey(attackers[i])) {
                hitList[attackers[i]] = new List<string>();
            }
            for (int j = 0; j < hitList[attackers[i]].Count; j++) {
                if (!worldScript.attacks[attackers[i]].Contains(hitList[attackers[i]][j])) {
                    hitList[attackers[i]].RemoveAt(j);
                }
            }
        }

        if (lasthit == "returningSpear" && !lastAttacker.GetComponent<PlayerBehaviour>().returningSpear) {
            lastAttacker = null;
            lasthit = "";
        }
        if (transform.position.y < -10) {
            if (name == "player" || name == "Dog") {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            Transform spear = transform.Find("Center/spear(Clone)");
            if (spear) {
                spear.parent = null;
                spear.GetComponent<SpearBehaviour>().owner.GetComponent<PlayerBehaviour>().returnSpear();
            }
            worldScript.entities.Remove(gameObject);
            Destroy(gameObject);
        }
        if (health <= 0) {
            if (name == "player" || name == "Dog") {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            Transform spear = transform.Find("Center/spear(Clone)");
            if (spear) {
                spear.parent = null;
                spear.GetComponent<SpearBehaviour>().owner.GetComponent<PlayerBehaviour>().returnSpear();
            }
            worldScript.enemyKilled(transform.position);
            worldScript.entities.Remove(gameObject);
            Destroy(gameObject);
        }
    }
    public void painAudio() {
        if (painsfx.Count > 0) {
            int rand = UnityEngine.Random.Range(0, painsfx.Count);
            // audioSource.PlayOneShot(painsfx[rand]);
        }
    }

    private void OnCollisionStay(Collision other) {
        GameObject collider = other.gameObject;
        if (collider.tag == "Ground" && !jumping && name != "Dog") {
            onGround = true;
            transform.parent = collider.transform.parent;
            aerialJumped = 0;
        }
    }
    protected void OnCollisionEnter(Collision other) {
        GameObject collider = other.gameObject;
        if (worldScript == null) {
            return;
        }
        if (other.gameObject.name == "Cone") {
            collider = other.transform.parent.gameObject;
        }
        if (
            worldScript.thrownWeapons.Contains(collider) && collider.GetComponent<SpearBehaviour>().owner != gameObject
        ) {
            GameObject attacker = collider.GetComponent<SpearBehaviour>().owner;
            lasthit = attacker.GetComponent<PlayerBehaviour>().prevState;
            lastAttacker = attacker;
            collider.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            collider.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            collider.GetComponent<BoxCollider>().isTrigger = true;
            collider.transform.Find("Cone").GetComponent<CapsuleCollider>().isTrigger = true;
            collider.transform.parent = transform.Find("Center");
            collider.transform.localPosition = Vector3.zero;
            float basedmg = collider.GetComponent<SpearBehaviour>().dmg;
            flickerStart = Time.time;
            bool crit = Random.Range(0, 1f) < attacker.GetComponent<EntityBehaviour>().critChance;
            float dmg = basedmg * (crit ? 2 : 1);
            hbscr.setValue(health - dmg);
            painAudio();
            AudioClip rand = attacker.GetComponent<PlayerBehaviour>().slashes[UnityEngine.Random.Range(0, attacker.GetComponent<PlayerBehaviour>().slashes.Count-1)];
            audioSource.PlayOneShot(rand);
            // transform.Find("Center/bloodfx").gameObject.GetComponent<Rotate>().spear = collider;
            // transform.Find("Center/bloodfx").gameObject.GetComponent<Rotate>().setRot();
            // transform.Find("Center/bloodfx").gameObject.GetComponent<VisualEffect>().Play();
            GameObject.Find("bloodfx").GetComponent<Rotate>().spear = collider;
            GameObject.Find("bloodfx").GetComponent<Rotate>().target = transform.Find("Center").position;
            GameObject.Find("bloodfx").gameObject.GetComponent<Rotate>().setRot();
            GameObject.Find("bloodfx").gameObject.GetComponent<VisualEffect>().Play();
            GameObject.Find("hitfx").GetComponent<Rotate>().spear = collider;
            GameObject.Find("hitfx").GetComponent<Rotate>().target = transform.Find("Center").position;
            GameObject.Find("hitfx").gameObject.GetComponent<Rotate>().setRot();
            GameObject.Find("hitfx").gameObject.GetComponent<VisualEffect>().Play();
            makeText("-" + dmg, transform.Find("Center"), Color.red, (int)dmg * 10);

        }
    }
    private void OnCollisionExit(Collision other) {
        if (other.gameObject.tag  == "Ground" && other.gameObject.transform.parent == transform.parent) {
            transform.parent = null;
            onGround = false;
        }
    }
    protected void OnTriggerStay(Collider other) {
        if (worldScript == null) {
            return;
        }
        GameObject collider = other.gameObject;
        if (other.gameObject.name == "Cone") {
            collider = other.transform.parent.gameObject;
        }
        if (worldScript.thrownSpells.Contains(collider) && 
        (collider.GetComponent<SpellBehaviour>().owner != null) &&
        (name != "player" && collider.GetComponent<SpellBehaviour>().owner.name == "player" ||
        name == "player" && collider.GetComponent<SpellBehaviour>().owner.name != "player")) {
            GameObject.Find("spellhit").GetComponent<Rotate>().target = collider.transform.position;
            GameObject.Find("spellhit").gameObject.GetComponent<Rotate>().setRot();
            GameObject.Find("spellhit").gameObject.GetComponent<VisualEffect>().Play();
            worldScript.thrownSpells.Remove(collider);
            Destroy(collider);
            if (blocking) {
                audioSource.PlayOneShot(shield);
                shieldHealth -= 0.1f;
                transform.Find("Center/shield").localScale = Vector3.one * 2 * shieldHealth;
            }
            else {
                // speed = Math.Max(this.speed -0.2f, 0);
                hbscr.setValue(health - collider.GetComponent<SpellBehaviour>().dmg);
                painAudio();
                audioSource.PlayOneShot(collider.GetComponent<SpellBehaviour>().hitsfx);
                makeText("-" + collider.GetComponent<SpellBehaviour>().dmg.ToString(), transform.Find("Center"), Color.red, 10);
            }
        }
        if (
            worldScript.thrownWeapons.Contains(collider) && 
            collider.GetComponent<SpearBehaviour>().owner != gameObject
        ) {
            GameObject attacker = collider.GetComponent<SpearBehaviour>().owner;
            if (
                !(attacker == lastAttacker && lasthit == "returningSpear") &&
                attacker.GetComponent<PlayerBehaviour>().returningSpear
            ) {
                float basedmg = collider.GetComponent<SpearBehaviour>().dmg;
                bool crit = Random.Range(0, 1f) < attacker.GetComponent<EntityBehaviour>().critChance;
                float dmg = basedmg * (crit ? 2 : 1);
                hbscr.setValue(health - dmg); 
                flickerStart = Time.time;

                painAudio();
                AudioClip rand = attacker.GetComponent<PlayerBehaviour>().slashes[UnityEngine.Random.Range(0, attacker.GetComponent<PlayerBehaviour>().slashes.Count-1)];
                audioSource.PlayOneShot(rand);
                // transform.Find("Center/bloodfx").gameObject.GetComponent<Rotate>().spear = collider;
                // transform.Find("Center/bloodfx").gameObject.GetComponent<Rotate>().setRot();
                // transform.Find("Center/bloodfx").gameObject.GetComponent<VisualEffect>().Play();
                GameObject.Find("bloodfx").GetComponent<Rotate>().spear = collider;
                GameObject.Find("bloodfx").GetComponent<Rotate>().target = transform.Find("Center").position;
                GameObject.Find("bloodfx").gameObject.GetComponent<Rotate>().setRot();
                GameObject.Find("bloodfx").gameObject.GetComponent<VisualEffect>().Play();
                GameObject.Find("hitfx").GetComponent<Rotate>().spear = collider;
                GameObject.Find("hitfx").GetComponent<Rotate>().target = transform.Find("Center").position;
                GameObject.Find("hitfx").gameObject.GetComponent<Rotate>().setRot();
                GameObject.Find("hitfx").gameObject.GetComponent<VisualEffect>().Play();
                makeText("-" + dmg, transform.Find("Center"), Color.red, (int)dmg * 10);
                lasthit = "returningSpear";
                lastAttacker = attacker;
            }
        }
        if (other.gameObject.name == "spear") {
            GameObject attacker = other.transform.GetComponent<SpearBehaviour>().owner;
            string attackerState = attacker.GetComponent<PlayerBehaviour>().prevState;
            if (
                worldScript.attacks[attacker].Contains(attackerState) && 
                !hitList[attacker].Contains(attackerState) &&
                attacker != gameObject
            ) {
                hitList[attacker].Add(attackerState);
                lasthit = attacker.GetComponent<PlayerBehaviour>().prevState;
                lastAttacker = attacker;
                if (blocking) {
                    audioSource.PlayOneShot(shield);
                    shieldHealth -= 0.1f;
                    transform.Find("Center/shield").localScale = Vector3.one * 2 * shieldHealth;
                }
                else {
                    float basedmg = other.gameObject.GetComponent<SpearBehaviour>().dmg / 2;
                    flickerStart = Time.time;
                    bool crit = Random.Range(0, 1f) < attacker.GetComponent<EntityBehaviour>().critChance;
                    float dmg = basedmg * (crit ? 2 : 1);
                    if (attacker.name == "player") {
                        attacker.GetComponent<PlayerBehaviour>().setShakeTime(dmg / 10);
                    }
                    hbscr.setValue(health - dmg);
                    painAudio();
                    AudioClip rand = attacker.GetComponent<PlayerBehaviour>().hits[UnityEngine.Random.Range(0, attacker.GetComponent<PlayerBehaviour>().hits.Count-1)];
                    audioSource.PlayOneShot(rand);
                    // transform.Find("Center/bloodfx").gameObject.GetComponent<Rotate>().spear = other.gameObject;
                    // transform.Find("Center/bloodfx").gameObject.GetComponent<Rotate>().setRot();
                    // transform.Find("Center/bloodfx").gameObject.GetComponent<VisualEffect>().Play();
                    // GameObject.Find("bloodfx").GetComponent<Rotate>().spear = other.transform.parent.gameObject;
                    // GameObject.Find("bloodfx").GetComponent<Rotate>().target = transform.Find("Center").position;
                    // GameObject.Find("bloodfx").gameObject.GetComponent<Rotate>().setRot();
                    // GameObject.Find("bloodfx").gameObject.GetComponent<VisualEffect>().Play();
                    GameObject.Find("hitfx").GetComponent<Rotate>().spear = collider;
                    GameObject.Find("hitfx").GetComponent<Rotate>().target = transform.Find("Center").position;
                    GameObject.Find("hitfx").gameObject.GetComponent<Rotate>().setRot();
                    GameObject.Find("hitfx").gameObject.GetComponent<VisualEffect>().Play();
                    makeText("-" + dmg, transform.Find("Center"), Color.red, (int)dmg * 10);
                }
            }
        }
        if (other.gameObject.name == "Cone") {
            GameObject attacker = other.transform.parent.GetComponent<SpearBehaviour>().owner;
            string attackerState = attacker.GetComponent<PlayerBehaviour>().prevState;
            // print(gameObject + " " + attackerState);
            if (
                worldScript.attacks[attacker].Contains(attackerState) && 
                !hitList[attacker].Contains(attackerState) &&
                attacker != gameObject &&
                !attacker.GetComponent<PlayerBehaviour>().windup
            ) {
                hitList[attacker].Add(attackerState);
                lasthit = attacker.GetComponent<PlayerBehaviour>().prevState;
                lastAttacker = attacker;
                if (blocking) {
                    audioSource.PlayOneShot(shield);
                    shieldHealth -= 0.1f;
                    transform.Find("Center/shield").localScale = Vector3.one * 2 * shieldHealth;
                }
                else {
                    float basedmg = other.transform.parent.GetComponent<SpearBehaviour>().dmg;
                    bool crit = Random.Range(0, 1f) < attacker.GetComponent<EntityBehaviour>().critChance;
                    float dmg = basedmg * (crit ? 2 : 1);
                    flickerStart = Time.time;
                    if (attacker.name == "player") {
                        attacker.GetComponent<PlayerBehaviour>().setShakeTime(dmg / 10);
                    }
                    hbscr.setValue(health - dmg);
                    painAudio();
                    AudioClip rand = attacker.GetComponent<PlayerBehaviour>().slashes[UnityEngine.Random.Range(0, attacker.GetComponent<PlayerBehaviour>().slashes.Count-1)];
                    audioSource.PlayOneShot(rand);
                    // transform.Find("Center/bloodfx").gameObject.GetComponent<Rotate>().spear = other.gameObject;
                    // transform.Find("Center/bloodfx").gameObject.GetComponent<Rotate>().setRot();
                    // transform.Find("Center/bloodfx").gameObject.GetComponent<VisualEffect>().Play();
                    GameObject.Find("bloodfx").GetComponent<Rotate>().spear = other.transform.parent.gameObject;
                    GameObject.Find("bloodfx").GetComponent<Rotate>().target = transform.Find("Center").position;
                    GameObject.Find("bloodfx").gameObject.GetComponent<Rotate>().setRot();
                    GameObject.Find("bloodfx").gameObject.GetComponent<VisualEffect>().Play();
                    GameObject.Find("hitfx").GetComponent<Rotate>().spear = collider;
                    GameObject.Find("hitfx").GetComponent<Rotate>().target = transform.Find("Center").position;
                    GameObject.Find("hitfx").gameObject.GetComponent<Rotate>().setRot();
                    GameObject.Find("hitfx").gameObject.GetComponent<VisualEffect>().Play();
                    makeText("-" + dmg, transform.Find("Center"), Color.red, (int)dmg * 10);
                }
            }
        }
    }
    public void collectCoin(GameObject coin) {
        // audioSource.PlayOneShot(collectCoinsfx);
        coinCount += 1;
        makeText("+1", transform.Find("Center"), Color.yellow, 20);
        if (goldText) {
            goldText.GetComponent<TextMeshProUGUI>().text = coinCount + " Gold";
        }
        Destroy(coin);
    }

    public void makeText(string str, Transform transform, Color color, int size) {
        GameObject newText = (GameObject) Instantiate(text, GameObject.Find("Canvas").transform);
        newText.GetComponent<TextMeshProUGUI>().text = str;
        newText.GetComponent<TextMeshProUGUI>().color = color;
        newText.GetComponent<TextMeshProUGUI>().fontSize = size;
        newText.transform.position = thecamera.GetComponent<Camera>().WorldToScreenPoint(transform.position);
        newText.GetComponent<TextFade>().thecamera = thecamera.GetComponent<Camera>();
        newText.GetComponent<TextFade>().startPos = transform;
    }
}
