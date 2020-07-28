using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Manages all of the players statistics, such as Health, Speed, and Distance travelled.
public class PlayerStats : MonoBehaviour
{
    float speedIndex = 0.0f; //Key for the players speed buildup curve

    public float currentPlayerSpeed = 0f; //How fast is the player currently going?

    float speedMod = 1.0f; //Additional speed modifier that gets applied on top of the value read from the Curve
    public int currentPlayerHealth = 3;

    int currentPlayerArmor = 0; //Player Armor gets drained before player health but cannot be restored
    public bool isDead = false; //If the player is dead, all Inputs and Behaviors are disabled
    
    public bool canMove = false; //If true, the players speed will be increased during each Update-loop

    public float totalDistanceMoved = 0f; //How far did the player move during his current run.

    private Animator playerAnimator;

    [SerializeField]
    private PlayerCharacter activeChar;

    private Image[] heartSlots;

    private Image[] armorSlots;

    private Text distanceText;

    private GameObject gameOverScreen;

    private Text distanceEndText;


    void LoadReferences(){
        activeChar = GetComponent<PlayerCharacter>();
        Transform healthBarTransform = GameObject.Find("Lifebar").transform;
        heartSlots = new Image[healthBarTransform.childCount];
        for(int i = 0; i < heartSlots.Length; i++){
            heartSlots[i] = healthBarTransform.GetChild(i).GetComponent<Image>();
        }

        Transform armorBarTransform = GameObject.Find("Armorbar").transform;
        armorSlots = new Image[armorBarTransform.childCount];
        for(int i = 0; i < armorSlots.Length; i++){
            armorSlots[i] = armorBarTransform.GetChild(i).GetComponent<Image>();
        }
        
        distanceText = GameObject.Find("DistanceText").GetComponent<Text>();
        gameOverScreen = GameObject.Find("GameOverScreen");
        gameOverScreen.SetActive(false);
        
        playerAnimator = GetComponent<Animator>();

    }

    public void Initiate()
    {
        LoadReferences();
        currentPlayerHealth = activeChar.maxHealth;
        currentPlayerArmor = activeChar.initialArmor;
        speedMod = activeChar.totalSpeedMod;

        SetupHealthUI();
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove){
           CalculateSpeed();
        }
        
    }

    private void CalculateSpeed(){
        //Calculates the players movement speed and updates the UI accordingly. The players speed is used to move the world and any enemies towards the player
         if(speedIndex < 1.0f){
                speedIndex+=(Time.deltaTime*0.1f);
                currentPlayerSpeed = activeChar.speedCurve.Evaluate(speedIndex)*speedMod;
            }
            playerAnimator.SetFloat("PlayerSpeed", currentPlayerSpeed * 0.001f);
            totalDistanceMoved+=currentPlayerSpeed*Time.deltaTime;
            distanceText.text = Mathf.Floor(totalDistanceMoved / 100) + " m";
    }   


    //Plays hit animation, and resets the players speed to 0. Also reduces the players armor or health (if no armor is left). If health reaches zero, OnDeath is called.
    public void GetDamaged(int damageValue){
        if(isDead){
            return;
        }
        canMove = false;
        speedIndex = 0.0f;
        currentPlayerSpeed = 0f;
        playerAnimator.SetBool("isHit", true);
        if(currentPlayerArmor > 0){
            currentPlayerArmor-=1;
            armorSlots[currentPlayerArmor].gameObject.SetActive(false);
        }else{
            heartSlots[currentPlayerHealth-1].sprite = activeChar.deadHeartSprite;
            if((currentPlayerHealth-=damageValue) <= 0){
                isDead = true;
                playerAnimator.SetBool("isDead", true);
                OnDeath();
            }
        }
        
        
        
    }

    public void GetHealed(int healValue){
        if(currentPlayerHealth+healValue > activeChar.maxHealth){
            return;
        }

        currentPlayerHealth+=healValue;
        heartSlots[currentPlayerHealth-1].sprite = activeChar.healthyHeartSprite;
    }

    public int GetCurrentHealth(){
        return currentPlayerHealth;
    }

    //Stops processing the input data and displays the gameover screen.
    void OnDeath(){
        canMove=false;
        GetComponent<PlayerInputDetector>().processTouches = false;
        gameOverScreen.SetActive(true);
        GameObject.Find("DeathDistanceText").GetComponent<Text>().text = "Distance travelled: " + Mathf.Floor(totalDistanceMoved / 100) + " m";
        GameObject.FindGameObjectWithTag("PlayerState").GetComponent<PersistentPlayerState>().SaveLevelProgress(totalDistanceMoved);
    }

    //Fills all of the displayed hearts and armor pieces with the sprites associated with the currently active player character and disables all of the icons that are above the current maxHealth / initialArmor
    void SetupHealthUI(){
        for(int i=0;i<heartSlots.Length;i++){
            if(i<activeChar.maxHealth){
                heartSlots[i].sprite = activeChar.healthyHeartSprite;
            }else{
                heartSlots[i].gameObject.SetActive(false);
            }
        }

        for(int i=0;i<armorSlots.Length;i++){
            if(i<activeChar.initialArmor){
                armorSlots[i].sprite = activeChar.armorSprite;
            }else{
                armorSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public PlayerCharacter GetActiveCharacter(){
        return activeChar;
    }
}
