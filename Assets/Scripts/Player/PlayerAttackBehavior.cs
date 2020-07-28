using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBehavior : MonoBehaviour
{
    private bool attackActive = false; //If true, sends a raycast in attackdirection every frame
    private StrikeType activeStrikeType = StrikeType.Middle;
    private CapsuleCollider bodyCollider; //Ray is always cast from middle of bodyCollider

    private Vector3 additionalOffset = Vector3.zero;

    [SerializeField]
    private PlayerWooshSword swordWoosher; //Plays attack sounds

    [SerializeField]
    private PlayerCharacter playerCharacter;
   
    void Start()
    {
        bodyCollider = GetComponent<CapsuleCollider>();
    }


    void Update()
    {
        if(attackActive){
            RaycastHit hit;  
            Vector3 directionHeightOffset = new Vector3(0f, -0.5f + (((int)activeStrikeType)*0.5f), 0f);

            //Casts a Raycast and checks if it hit a Destructable Object (such as a crate or an enemy).
            if(Physics.Raycast(transform.position + bodyCollider.center + directionHeightOffset + additionalOffset, Vector3.right, out hit, 1.5f)){
                DestructableObject hitObject = hit.transform.GetComponent<DestructableObject>();
                if(hitObject){
                    attackActive = false;
                    hitObject.OnDestruction(activeStrikeType, playerCharacter.attackDamage);
                }
            }
        }
    }


    public void EnableAttackArea(StrikeType type){
        //Plays an attack sound and enables the attack raycast
        swordWoosher.PlayWooshSound();
        activeStrikeType = type;
        attackActive = true;

    }

    public void SetStrikeOffset(float offset){
        //Adds an additional offset to the players attack. This is used by the sliding animations to prevent the players attack from clipping into the ground.
        additionalOffset = new Vector3(0f, offset, 0f);
    }

    public void DisableAttackArea(){
        //Disables the raycast. Gets called at the end of any attack animation
        attackActive = false;
    }
}
