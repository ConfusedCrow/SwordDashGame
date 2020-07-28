using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelReset : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("ResetAnim", false);    
    }
}
