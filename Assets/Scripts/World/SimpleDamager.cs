using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDamager : MonoBehaviour
{
    //Base Class for things that hurt the player if he moves into them

    public int damageValue = 1; //How many Hearts does the player lose on hit?

    void OnTriggerEnter(Collider col){
        Debug.Log(col.name);
        PlayerStats playerStats = col.GetComponent<PlayerStats>();
        if(playerStats){
            playerStats.GetDamaged(damageValue);
            Destroy(gameObject);
        }
    }
}
