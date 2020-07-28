using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBehavior : MonoBehaviour
{
    //

    private Animator animator;

    private PlayerStats player;

    [SerializeField]
    private float holdDistance = 0f;

    [SerializeField]
    private float strikeDistance = 0f;

    private bool hasAttacked = false;

    private bool waitingForAttack = false;

    public bool isBlocked = false;

    public bool isStaggered = false;

    public bool isDead = false;

    public bool canMove = true;

    EnemyAttackBehavior attackBehavior;

    WorldMover worldMover;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        worldMover = GameObject.FindGameObjectWithTag("WorldSpawner").GetComponent<WorldMover>();
        attackBehavior = GetComponent<EnemyAttackBehavior>();
    }

    void Update()
    {
           
        if(player.canMove && canMove && !isBlocked){
            MoveCharacter();
        }

        if(!hasAttacked && !isDead){
            if(!waitingForAttack){
                //Check if enemy is close enough to the player to start displaying their attack intention.
                float distance = (transform.position - player.transform.position).magnitude;
	            if(distance <= holdDistance){
                    waitingForAttack = true;
                    int directionValue = Random.Range(-1,2);
                    attackBehavior.SetAttackType((StrikeType)(directionValue+1));
                    animator.SetInteger("AttackDirection", directionValue);
		            animator.SetBool("HoldAttack", true);
	            }
            }else{
                //Check if the enemy is close enough to actually hit the player and strike if they are.
                float distance = (transform.position - player.transform.position).magnitude;
	            if(distance <= strikeDistance){
                    hasAttacked = true;
                    waitingForAttack = false;
                    animator.SetBool("HoldAttack", false);
		            animator.SetBool("Attack", true);
	            }
            }
        }
        
    }

    void MoveCharacter(){ //Moves Enemy towards the player and removes them once they are offscreen.
        transform.position+= new Vector3(player.currentPlayerSpeed*Time.deltaTime*-0.01f,0f,0f);
            if((transform.position.x < player.transform.position.x) && ((player.transform.position - transform.position).magnitude >= 10f)){
                worldMover.isInFight = false;
                Destroy(gameObject);
            }
    }


    public void OnBlocked(bool wasPerfectBlock){
        //Gets called by PlayerBlockBehavior if the players block was active while the enemy attacked. A perfect block causes the enemy to enter the "Staggered" state.
        isBlocked = true;
        if(wasPerfectBlock){
            animator.SetBool("Stagger", true);
            isBlocked = false;
            isStaggered = true;
            canMove = false;
        }else{
            animator.SetBool("Blocked", true);
            isBlocked = true;
        }
    }

    //ResumeAttacking gets called in the last frame of the "Blocked" Animation, causing the enemy to resume attacking the player
    public void ResumeAttacking(){
        animator.SetBool("Blocked", false);
        hasAttacked = false;
        isBlocked = false;
        isStaggered = false;
        GetComponent<EnemyAttackBehavior>().playerHit = false;
        animator.SetBool("HitPlayer", false);
    }

    //StopStaggering gets called in the last of the "Stagger" Animation
    public void StopStaggering(){
        Debug.Log("Stop Staggering");
        animator.SetBool("Damaged", false);
        animator.SetBool("Stagger", false);
        isBlocked = false;
        isStaggered = false;
        hasAttacked = false;
    }

    public void OnDeath(){
        if(isDead){
            return;
        }
        isDead = true;
        animator.SetBool("Dead", true);

    }

    //AllowMovement and BlockMovement both control the enemies ability to towards the player. They are called by specific animations that require control of the enemies movement without affecting the blocked or staggered state (such as the dying animation)
    public void AllowMovement(){
        canMove = true;
    }

    public void BlockMovement(){
        canMove = false;
    }

    

}
