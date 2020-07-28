using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : DestructableObject
{

    private BasicEnemyBehavior enemyBehavior;

    [SerializeField]
    private int currentHealth = 1;

    public StrikeType[] vulnerabilityList = {}; //List of all Strike Directions that deal double damage

    void Start()
    {
        enemyBehavior = GetComponent<BasicEnemyBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnDestruction(StrikeType hitType, int damageValue){
        //Gets called every time the player hits this enemy.
        if(!enemyBehavior.isStaggered){
            return;
        }
        foreach(StrikeType strike in immunityList){
            if(hitType == strike){
                return;
            }
        }
        int modifiedDamageValue = damageValue;
        foreach(StrikeType strike in vulnerabilityList){
            if(hitType == strike){
                modifiedDamageValue *=2;
            }
        }
        if((currentHealth-=modifiedDamageValue)<=0){
            enemyBehavior.OnDeath();
        }else{
            GetComponent<Animator>().SetBool("Damaged", true);
        }
        
    }

    public int getHealth(){
        return currentHealth;
    }

    
}
