using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VFX;

public class sfx : StateMachineBehaviour
{
    public AudioClip low;
    public AudioClip high;
    public AudioClip stab;
    public AudioClip toss;
    public AudioSource audioSource;
    public GameObject player;
    PlayerBehaviour behaviour;
    List<string> attackStates = new List<string>{"downswing", "upswing", "stab"};
    GameObject spear;
    WorldScript worldScript;
    string prevState = "bruh";
    // Start is called before the first frame update
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        behaviour = animator.gameObject.GetComponent<PlayerBehaviour>();
        if (!behaviour) {
            return;
        }
        worldScript = behaviour.worldScript;
        spear = behaviour.spearObj;
        audioSource = animator.gameObject.GetComponent<AudioSource>();
        if (!animatorStateInfo.IsName(prevState)) {
            for (int i = 0; i < attackStates.Count; i++) {
                if (layerIndex == 2 && animatorStateInfo.IsName(attackStates[i])) {
                    worldScript.attacks[animator.gameObject].Add(attackStates[i]);
                }
            }
            if (layerIndex == 2 && animatorStateInfo.IsName("downswing")) {
                audioSource.PlayOneShot(low);
                prevState = "downswing";
                behaviour.enscr.setValue(behaviour.energy - 1);
            }
            if (layerIndex == 2 && animatorStateInfo.IsName("upswing")) {
                audioSource.PlayOneShot(high);
                prevState = "upswing";
                behaviour.enscr.setValue(behaviour.energy - 1);
            }
            if (layerIndex == 2 && animatorStateInfo.IsName("stab")) {
                audioSource.PlayOneShot(stab);
                prevState = "stab";
                behaviour.enscr.setValue(behaviour.energy - 1);
            }
            if (layerIndex == 2 && animatorStateInfo.IsName("throwhold")) {
                audioSource.clip = behaviour.throwhold;
                audioSource.Play();
            }
            if (layerIndex == 2 && animatorStateInfo.IsName("throwthrow")) {
                audioSource.PlayOneShot(toss);
                prevState = "throw";
                behaviour.enscr.setValue(behaviour.energy - 3);
            }
            if (layerIndex == 2 && animatorStateInfo.IsName("holdspear")) {
                prevState = "holdspear";
            }
        }
        behaviour.prevState = prevState;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        behaviour = animator.gameObject.GetComponent<PlayerBehaviour>();
        for (int i = 0; i < attackStates.Count; i++) {
            if (layerIndex == 2 && animatorStateInfo.IsName(attackStates[i])) {
                worldScript.attacks[animator.gameObject].Remove(attackStates[i]);
                behaviour.windup = false;
            }
        }
        if (layerIndex == 2 && animatorStateInfo.IsName("throwthrow")) {
            animator.ResetTrigger("throw");
            behaviour.throwSpear();
            behaviour.spear = false;
            behaviour.rightRig.weight = 0;
            Physics.IgnoreCollision(animator.gameObject.GetComponent<BoxCollider>(), behaviour.thrownSpear.GetComponent<BoxCollider>());
            Physics.IgnoreCollision(animator.gameObject.GetComponent<BoxCollider>(), behaviour.thrownSpear.transform.Find("Cone").GetComponent<CapsuleCollider>());
            behaviour.thrownSpear.GetComponent<BoxCollider>().isTrigger = false;
            behaviour.thrownSpear.transform.Find("Cone").GetComponent<CapsuleCollider>().isTrigger = false;
        }
        if (layerIndex == 2 && animatorStateInfo.IsName("throwend")) {
            if (behaviour.thrownSpear) {
                Physics.IgnoreCollision(animator.gameObject.GetComponent<BoxCollider>(), behaviour.thrownSpear.GetComponent<BoxCollider>(), false);
                Physics.IgnoreCollision(animator.gameObject.GetComponent<BoxCollider>(), behaviour.thrownSpear.transform.Find("Cone").GetComponent<CapsuleCollider>(), false);
                behaviour.spearCatchable = true;
            }
        }
        if (layerIndex == 2 && animatorStateInfo.IsName("stab")) {
            animator.ResetTrigger("downswing");
        }
        
    }
}
