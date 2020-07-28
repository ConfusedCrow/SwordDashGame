using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup_Base : MonoBehaviour
{
    //Base Class for Objects that get activated if the player runs into them.

    [SerializeField]
    private GameObject pickupEffectPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnImpactWithPlayer(GameObject playerObject){

    }

    void OnTriggerEnter(Collider col){
        if(col.CompareTag("Player")){
            OnImpactWithPlayer(col.gameObject);
            Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
