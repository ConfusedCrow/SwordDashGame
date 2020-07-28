using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    //PlayerCharacter contains each of the Players Stats so that other scripts can easily access and modify them as needed.
    public string characterName = "DebugChar";
    public int maxHealth = 3;

    public int initialArmor = 0;
    public AnimationCurve speedCurve; //Playerspeed over time

    public float totalSpeedMod = 1.0f; //Additional modifier that gets applied to the speed at which the envionment moves.

    public float staggerWindow = 0.5f; //How many seconds can a player block before an attack hits to perfect stagger?

    public int attackDamage = 1;
    
    public Sprite healthyHeartSprite;
    public Sprite deadHeartSprite;

    public Sprite armorSprite;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
