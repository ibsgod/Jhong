using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class animate : MonoBehaviour
{

    public PlayerBehaviour behaviour;
    public Animator animator;
    void Start() {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        animator.SetFloat("speed", behaviour.speed);
        animator.SetBool("running", behaviour.running);
        animator.SetBool("jumping", behaviour.jumping);
        animator.SetBool("dashing", behaviour.dashing);
        animator.SetBool("onGround", behaviour.onGround);
        animator.SetBool("dancing", behaviour.dancing);
        animator.SetBool("spear", behaviour.spear);
    }
}
