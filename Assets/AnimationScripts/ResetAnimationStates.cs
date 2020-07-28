using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimationStates : StateMachineBehaviour
{
    public bool resetSlide = false;

    public bool resetJump = false;
    public bool resetAttack = false;

    public bool resetHit = false;

    public bool resetBlock = false;
    

    //Resets Animation States as needed. This prevents the looping of animations.
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(resetSlide){
            animator.SetBool("Slide", false);
        }
        if(resetJump){
             animator.SetBool("Jump", false);
        }
        if(resetAttack){
            animator.SetBool("Attack", false);
        }
        if(resetHit){
            animator.SetBool("isHit", false);
        }
         if(resetBlock){
            animator.SetBool("Block", false);
        }
    }

   
}
