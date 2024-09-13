using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VFX;

public class playsfx : StateMachineBehaviour
{
    public AudioClip dashstart;
    public AudioClip dashloop;
    public AudioSource audioSource;
    public bool speedChange = false;
    public GameObject player;
    // Start is called before the first frame update
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        if (!animator.gameObject.GetComponent<PlayerBehaviour>()) {
            return;
        }
        audioSource = animator.gameObject.GetComponent<AudioSource>();
        if (layerIndex == 0 && animatorStateInfo.IsName("dash")) {
            animator.gameObject.transform.Find("Center/dashfx").gameObject.GetComponent<VisualEffect>().Play();
            audioSource.PlayOneShot(dashstart);
            audioSource.clip = dashloop;
            audioSource.loop = true;
            audioSource.Play();
        }
        if (layerIndex == 0 && animatorStateInfo.IsName("run") && animator.gameObject.GetComponent<PlayerBehaviour>().runningBack) {
            animator.gameObject.GetComponent<PlayerBehaviour>().speed /= 2;
            speedChange = true;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        if (layerIndex == 0 && animatorStateInfo.IsName("dash")) {
            animator.gameObject.transform.Find("Center/dashfx").gameObject.GetComponent<VisualEffect>().Stop();
            audioSource.Stop();
            audioSource.loop = false;
        }
        if (layerIndex == 0 && animatorStateInfo.IsName("run") && speedChange) {
            animator.gameObject.GetComponent<PlayerBehaviour>().speed *= 2;
            speedChange = false;
        }
        
    }
}
