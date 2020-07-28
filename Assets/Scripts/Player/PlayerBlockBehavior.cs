using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockBehavior : MonoBehaviour
{
    private bool blockActive = false; 
    private StrikeType blockDirection = StrikeType.Middle; //Only attacks from this direction are blocked

    private PlayerStats playerStats;

    private float blockActiveTimer = 0f; //How long has the block been active. This is used to check for perfect blocks
    
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }


    void Update()
    {
        if(blockActive){
            blockActiveTimer+=Time.deltaTime;
        }
    }

    public bool OnPlayerStruck(int damage, StrikeType strikeType, BasicEnemyBehavior source){
        //Checks if the players block is active and if the block direction is equal to the direction of the hit received.
        if(!blockActive || blockDirection != strikeType){     
            playerStats.GetDamaged(damage);
            return true;
        }else{
            //Checks if the player blocked for a short enough time to stagger the enemy. The time is detemined by the Players Equipment as defined in the PlayerCharacter
            bool wasPerfectBlock = (blockActiveTimer <= playerStats.GetActiveCharacter().staggerWindow);
            source.OnBlocked(wasPerfectBlock);
            return false;
        }
    }

    public void EnableBlock(StrikeType direction){
        //Starts blocking and resets relevant statistics
        blockDirection = direction;
        blockActiveTimer = 0f;
        blockActive = true;  
    }

    public void DisableBlock(){
        blockActive = false;
    }
}
