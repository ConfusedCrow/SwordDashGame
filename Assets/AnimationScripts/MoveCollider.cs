using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveState {Ground, Slide, Jump}

public class MoveCollider : StateMachineBehaviour
{
    public MoveState activeState;
    
    //Moves the players hitbox to align with the current animation.
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch(activeState){
            case MoveState.Slide: GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider>().center = new Vector3(0f,-1f,0f);
            break;
            case MoveState.Jump: GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider>().center = new Vector3(0f,2.5f,0f);
            break;
            default: GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider>().center = new Vector3(0f,0f,0f);
            break;
        }
        
    }

 
}
