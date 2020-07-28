using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StrikeType {Down, Middle, Up, None};

public class DestructableObject : MonoBehaviour
{
    public StrikeType[] immunityList = {};
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnDestruction(StrikeType hitType, int damageValue){
        foreach(StrikeType strike in immunityList){
            if(hitType == strike){
                return;
            }
        }
        Destroy(gameObject);
    }
}
