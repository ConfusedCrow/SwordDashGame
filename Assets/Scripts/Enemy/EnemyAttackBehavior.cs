using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyAttackBehavior : MonoBehaviour
{
    private bool attackActive = false;
    public StrikeType activeStrikeType = StrikeType.Middle;

    [SerializeField]
    int damage = 1;

    private static float attackRange = 2f;
    public bool playerHit = false;

    private EnemyHealth enemyHealth;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if(attackActive && !playerHit){
            RaycastHit hit;  
            Vector3 directionHeightOffset = new Vector3(0f, -0.5f + (((int)activeStrikeType)*0.5f), 0f);
            Debug.DrawRay(transform.position + directionHeightOffset, Vector3.right*-attackRange, Color.green, 1000f);
            if(Physics.Raycast(transform.position + directionHeightOffset, Vector3.right*-1f, out hit, attackRange)){
                PlayerBlockBehavior hitObject = hit.transform.GetComponent<PlayerBlockBehavior>();
                if(hitObject){
                    attackActive = false;
                    if(hitObject.OnPlayerStruck(damage, activeStrikeType, GetComponent<BasicEnemyBehavior>())){
                        playerHit = true;
                        GetComponent<Animator>().SetBool("HitPlayer", true);
                    }
                }
            }
        }
    }
    
    public void EnableAttackArea(){
        attackActive = true;
    }

    public void SetAttackType(StrikeType type){
        activeStrikeType = type;
    }

    public void DisableAttackArea(){
        attackActive = false;
        playerHit = false;
    }

    void OnTriggerEnter(Collider col){
        PlayerStats playerStats = col.GetComponent<PlayerStats>();
        if(playerStats && !playerHit && enemyHealth.getHealth() > 0){
            playerHit = true;
            playerStats.GetDamaged(damage);
            enemyHealth.OnDestruction(StrikeType.None, 1);
        }
    }
    
}
