using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeRunning : StateMachineBehaviour
{

    //Start moving the world around the player again. Used by the PlayerHit AnimationClip
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        if(!playerStats.isDead){
            playerStats.canMove = true;
        }
        
        
    }

}
