using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Animations.Rigging;
using UnityEditor.Rendering;
using UnityEngine.VFX;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using TMPro;


public class PlayerBehaviour : EntityBehaviour
{
    Animator animator;
    public GameObject spearObj;
    public bool followCamera;
    public Rig rightRig;
    public GameObject thrownSpear;
    public AudioClip teleport;
    public AudioClip dashStart;
    public AudioClip spearReturn;
    public AudioClip takeSpear;
    public AudioClip throwhold;
    public List<AudioClip> slashes = new List<AudioClip>();
    public List<AudioClip> hits = new List<AudioClip>();
    Vector3 cameraOffset;
    public bool windup = false;
    float lastRegen;
    public string prevState = "";
    public float mana;
    public float energy;
    public float maxMana = 20;
    public float maxEnergy = 5;
    public float speed;
    public float jumpForce = 500;
    public bool spear;
    public BarScript mnscr;
    public BarScript enscr;
    public bool dashing;
    public bool runningBack;
    public bool dancing;
    public bool returningSpear = false;
    public GameObject dialogue;
    public InputManager inputScript;
    Dictionary<string, int> inputs;
    public bool cameraLock;
    public bool spearCatchable;
    public float shakeTime = -10;
    float shakeDuration = 0;
    float throwStart = 0;
    private bool throwing;
    public AudioClip jump;
    float dashFactor = 1.5f;
    public AudioClip chaching;
    private float healthRegen = 0.3f;

    new void Start() {
        hurtIndex = 1;
        base.Start();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spearObj.SetActive(spear);
        rightRig.weight = 0;
        if (followCamera) {
            cameraOffset = thecamera.transform.position - transform.position;
        }
        // spearObj.transform.Find("spearfx").gameObject.GetComponent<VisualEffect>().Stop();
        lastRegen = Time.time;
        inputScript = gameObject.GetComponent<InputManager>();
        worldScript = GameObject.Find("WorldObject").GetComponent<WorldScript>();
        mnscr.setMaxValue(maxMana);
        mnscr.setValue(maxMana);
        enscr.setMaxValue(maxEnergy);
        enscr.setValue(maxEnergy);

    }
    public void setShakeTime(float dmg) {
        shakeTime = Time.time;
        shakeDuration = dmg;
    }
    new void Update()
    { 
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        if (altitudeText) {
            altitudeText.GetComponent<TextMeshProUGUI>().text = transform.position.y.ToString();
        }
        base.Update();
        mana = mnscr.val;
        energy = enscr.val;
        inputs = inputScript.inputs;
        if (Time.time - lastRegen > 1) {
            lastRegen = Time.time;
            hbscr.setValue(health + healthRegen);
            mnscr.setValue(mana + 1);
            enscr.setValue(energy + 0.8f);
            shieldHealth = Mathf.Min(shieldHealth + 0.01f, 1);
            transform.Find("Center/shield").localScale = Vector3.one * 2 * shieldHealth;
        }
        running = false;
        if (inputs == null) {
            return;
        }
        if (!onGround) {
            jumping = false;
        }
        if (inputs["jump"] == 1 && onGround && !jumping) {
            GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
            jumping = true;
            onGround = false;
            audioSource.PlayOneShot(jump);
        }
        else if (inputs["jump"] == 1 && aerialJumped < jumpMax) {
            GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
            audioSource.PlayOneShot(jump);
            animator.SetTrigger("doublejump");
            aerialJumped += 1;
        }
        if ((inputs["dash"] == 1 || inputs["dash"] == 2 && inputs["block"] == 3) && energy > 0){
            dashing = true;
        }
        if (dashing && (inputs["dash"] == 3 || energy <= 0|| inputs["block"] == 1)) {
            dashing = false;
        }
        if (
            inputs["block"] == 2 && 
            shieldHealth > 0 
        ) {
            animator.SetBool("blocking", true);
            blocking = true;
            transform.Find("Center/shield").gameObject.SetActive(true);
            running = false;
            dashing = false;
        }
        else {
            animator.SetBool("blocking", false);
            blocking = false;
            transform.Find("Center/shield").gameObject.SetActive(false);
            if (dashing) {
                transform.position += transform.forward * speed * dashFactor * Time.deltaTime;
                enscr.setValue(energy - Time.deltaTime);
            }
            else if (inputs["forward"] == 2){
                transform.position += transform.forward * speed * Time.deltaTime;
                running = true;
            }
            else if (inputs["back"] == 2){
                transform.position -= transform.forward * speed * Time.deltaTime;
                running = true;
                runningBack = true;
            }
            else {
                runningBack = false;
            }
        }
        if (inputs["left"] == 2){
            transform.Rotate(0, -300 * Time.deltaTime, 0);
        }
        if (inputs["right"] == 2){
            transform.Rotate(0, 300 * Time.deltaTime, 0);
        }
        if (inputs["fire"] == 1 && mana >= 1 && !blocking){
            animator.SetTrigger("raise");
            GameObject spell = (GameObject) Instantiate(spellobj, transform.position + new Vector3(0, 1.35f, 0) + transform.forward * 0.8f, transform.rotation);
            spell.transform.Rotate(0, 270, 90);
            spell.GetComponent<SpellBehaviour>().owner = gameObject;
            spell.GetComponent<SpellBehaviour>().dmg = 1;
            worldScript.thrownSpells.Add(spell);
            spell.GetComponent<SpellBehaviour>().activateTime = (running || dashing) ? 0 : 0.25f;
            mnscr.setValue(mana - 1);
        }
        if (spear && !blocking) {
            if (inputs["attack"] == 1 && energy >= 1) {
                animator.SetTrigger("downswing");
            }
            if (inputs["throw"] == 1 && energy >= 3) {
                animator.SetBool("throwhold", true);
                throwStart = Time.time;
                animator.ResetTrigger("throw");
            }
        }
        if (!throwing && animator.GetCurrentAnimatorStateInfo(2).IsName("throwhold")) {
            throwStart = Time.time;
            throwing = true;
        }

        if (animator.GetBool("throwhold")) {
            if (inputs["throw"] == 3 || Time.time - throwStart > 5) {
                animator.SetTrigger("throw");
                animator.SetBool("throwhold", false);
                throwing = false;
                audioSource.Stop();
            }
        }

        if (thrownSpear && !spear) {
            if (inputs["throw"] == 1) {
                transform.Find("Center/telefx").gameObject.GetComponent<VisualEffect>().SetVector3("starttele", transform.position);
                transform.Find("Center/telefx").gameObject.GetComponent<VisualEffect>().SetVector3("endtele", thrownSpear.transform.position);
                transform.position = thrownSpear.transform.position;
                transform.Find("Center/telefx").gameObject.GetComponent<VisualEffect>().Play();
                audioSource.PlayOneShot(teleport);
                spear = true;
                spearObj.GetComponent<BoxCollider>().isTrigger = true;
                spearObj.transform.Find("Cone").GetComponent<CapsuleCollider>().isTrigger = true;
                returningSpear = false;
                thrownSpear.GetComponent<AudioSource>().Stop();
                audioSource.PlayOneShot(takeSpear);
                worldScript.thrownWeapons.Remove(thrownSpear);
                Destroy(thrownSpear);
                thrownSpear = null;
                spearObj.SetActive(true);
                aerialJumped = 0;
            }
            if (inputs["attack"] == 1 && !returningSpear) {
                if (thrownSpear) {
                    thrownSpear.GetComponent<BoxCollider>().isTrigger = true;
                    thrownSpear.transform.Find("Cone").GetComponent<CapsuleCollider>().isTrigger = true;
                    returnSpear();
                }
            }
        }
        if (returningSpear) {
            Vector3 direction = thrownSpear.transform.position - transform.Find("Center").position;
            Vector3 lookAtAngle = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
            lookAtAngle.x += 90;
            lookAtAngle.z += 180;
            thrownSpear.transform.eulerAngles = lookAtAngle;
            thrownSpear.GetComponent<Rigidbody>().velocity = thrownSpear.transform.up * 20;
        }
        if (followCamera) {
            if (Input.GetKeyDown(KeyCode.C)) {
                cameraLock = !cameraLock;
            }
            thecamera.transform.parent.position = transform.position;
            if (Time.time - shakeTime < shakeDuration) {
                float range = shakeDuration - Time.time + shakeTime;
                thecamera.transform.parent.position += new Vector3(
                    UnityEngine.Random.Range(-range, range), 
                    UnityEngine.Random.Range(-range, range), 
                    UnityEngine.Random.Range(-range, range)
                );
            }
            if (cameraLock) {
                thecamera.transform.parent.position = Vector3.Lerp(thecamera.transform.parent.position, transform.position, 0.04f);
                thecamera.transform.parent.rotation = Quaternion.Lerp(thecamera.transform.parent.rotation, transform.rotation, 0.009f);
            }
            else {
                if (Input.GetKey(KeyCode.LeftArrow)) {
                    thecamera.transform.parent.RotateAround(transform.position, Vector3.up, -600 * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.RightArrow)) {
                    thecamera.transform.parent.RotateAround(transform.position, Vector3.up, 600 * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.UpArrow)) {
                    thecamera.transform.parent.Rotate(300 * Time.deltaTime, 0, 0);
                }
                if (Input.GetKey(KeyCode.DownArrow)) {
                    thecamera.transform.parent.Rotate(-300 * Time.deltaTime, 0, 0);
                }
            }
        }
        if (GetComponent<Dialogue>()?.canBuy && Input.GetKeyDown(KeyCode.M)) {
            buyItem(GetComponent<Dialogue>().canBuy.item, GetComponent<Dialogue>().canBuy.price);
        }
    }

    private void buyItem(string item, int price)
    {
        if (coinCount < price) {
            return;
        }
        coinCount -= price;
        audioSource.PlayOneShot(chaching);
        if (goldText) {
            goldText.GetComponent<TextMeshProUGUI>().text = coinCount + " Gold";
        }
        if (item == "bigSpear") {
            spearObj.transform.localScale *= 1.1f;
        }
        if (item == "bigBoi") {
            transform.localScale *= 1.1f;
            thecamera.transform.Translate(Vector3.forward * -0.9f);
        }
        if (item == "sped") {
            speed *= 1.1f;
        }
        if (item == "yump") {
            jumpForce *= 1.1f;
        }
        if (item == "critChance") {
            critChance *= 2;
        }
        if (item == "anothaJump") {
            jumpMax += 1;
        }
        if (item == "dash") {
            dashFactor *= 1.5f;
        }
        if (item == "dmg") {
            spearObj.GetComponent<SpearBehaviour>().dmg += 1;
        }
        if (item == "healthRegen") {
            healthRegen *= 1.5f;
        }
    }

    new void OnCollisionEnter(Collision other) {
        base.OnCollisionEnter(other);
        if (other.gameObject.tag == "weapondisplay" || other.gameObject == thrownSpear) {
            spear = true;
            spearObj.GetComponent<BoxCollider>().isTrigger = true;
            spearObj.transform.Find("Cone").GetComponent<CapsuleCollider>().isTrigger = true;
            returningSpear = false;
            audioSource.PlayOneShot(takeSpear);
            Destroy(other.gameObject);
            spearObj.SetActive(true);
            if (other.gameObject == thrownSpear) {
                worldScript.thrownWeapons.Remove(thrownSpear);
                thrownSpear.GetComponent<AudioSource>().Stop();
                thrownSpear = null;
            }
        }
    }

    new void OnTriggerStay(Collider other) {
        base.OnTriggerStay(other);
        if (other.gameObject == thrownSpear && (spearCatchable || returningSpear)) {
            spear = true;
            audioSource.PlayOneShot(takeSpear);
            Destroy(other.gameObject);
            spearObj.SetActive(true);
            worldScript.thrownWeapons.Remove(thrownSpear);
            returningSpear = false;
            thrownSpear.GetComponent<AudioSource>().Stop();
            thrownSpear = null;
        }
    }

    public void throwSpear() {
        spearCatchable = false;
        spearObj.SetActive(false);
        GameObject newspear = Instantiate(spearObj, transform.position + new Vector3(0, 1.5f, 0), transform.rotation);
        newspear.GetComponent<SpearBehaviour>().owner = gameObject;
        newspear.transform.Rotate(90, 0, 0);
        newspear.SetActive(true);
        worldScript.thrownWeapons.Add(newspear);
        thrownSpear = newspear;
        thrownSpear.GetComponent<BoxCollider>().size *= 2;
        thrownSpear.transform.Find("Cone").GetComponent<CapsuleCollider>().radius *= 2;
        thrownSpear.transform.Find("Cone").GetComponent<CapsuleCollider>().height *= 2;
        newspear.GetComponent<Rigidbody>().AddForce(transform.forward * (1000 * (Time.time - throwStart)) + transform.up * (onGround ? 30 : 60));
        newspear.GetComponent<Rigidbody>().useGravity = true;
        newspear.GetComponent<SpearBehaviour>().dmg *= (float) Math.Ceiling(Time.time - throwStart) / 2 ;
    }
    public void returnSpear() {
        returningSpear = true;
        Physics.IgnoreCollision(GetComponent<BoxCollider>(), thrownSpear.GetComponent<BoxCollider>(), false);
        Physics.IgnoreCollision(GetComponent<BoxCollider>(), thrownSpear.transform.Find("Cone").GetComponent<CapsuleCollider>(), false);
        thrownSpear.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        thrownSpear.GetComponent<AudioSource>().clip = spearReturn;
        thrownSpear.GetComponent<AudioSource>().loop = true;
        audioSource.PlayOneShot(dashStart);
        thrownSpear.GetComponent<AudioSource>().Play();
        thrownSpear.GetComponent<Rigidbody>().useGravity = false;
    }
    public void makeDialogue(string str) {
        dialogue.GetComponent<TextMeshProUGUI>().text = str;
    }
    public void clearDialogue() {
        dialogue.GetComponent<TextMeshProUGUI>().text = "";
    }
    public void propel() {
        GetComponent<Rigidbody>().AddForce(animator.gameObject.transform.forward * 200);
    }
    public void windUp() {
        windup = true;
    }
    public void windDown() {
        windup = false;
    }
}
