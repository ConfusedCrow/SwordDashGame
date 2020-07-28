using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
enum PlayerActions {Jump,Slide,Attack,Block}
public class PlayerInputDetector : MonoBehaviour
{
    public Text debugText;

    public bool processTouches = true; //Should Inputs be recognized

    //Touch configuration Data
    private float swipeDistance = 30f; //How far does the player need to swipe to register an action

    //Touch data for current Touch
    private Vector2[][] touchPoints = {new Vector2[4],new Vector2[4]}; //List of the last 4 Finger Positions for two fingers
    private int[] touchPointIndex = {-1,-1}; //At which point in the touchPoints list are the two fingers right now. Used to roll over the touchpoints list and to reorder the touches for pattern recognition.
    private Animator playerAnimator;

    private bool[] waitForReset = {false,false}; //Flag that resets all of the touch points next frame

    private bool enablePCControls = true; //Should Debug PC Controls be enabled?

    private PersistentPlayerState persistentPlayerState; //Link to a players settings and configs

    

    // Start is called before the first frame update
    void Start()
    {
        debugText = GameObject.Find("DebugText").GetComponent<Text>();
        persistentPlayerState = GameObject.FindGameObjectWithTag("PlayerState").GetComponent<PersistentPlayerState>();
        playerAnimator = GetComponent<Animator>();
        for(int i = 0; i < touchPoints.Length;i++){
            ClearTouchData(i);
        }
    }

    
    void Update()
    {
        //Read Debug PC Controls
        if(enablePCControls){
            if(Input.GetKeyDown(KeyCode.S)){
                DoAction(PlayerActions.Slide, 0);
            }else if(Input.GetKeyDown(KeyCode.W)){
                DoAction(PlayerActions.Jump, 0);
            }

            if(Input.GetKeyDown(KeyCode.D)){
                if(Input.GetKey(KeyCode.UpArrow)){
                    DoAction(PlayerActions.Attack, 1);
                }else if(Input.GetKey(KeyCode.DownArrow)){
                    DoAction(PlayerActions.Attack, -1);
                }else{
                    DoAction(PlayerActions.Attack, 0);
                }
                
            }else if(Input.GetKeyDown(KeyCode.A)){
                if(Input.GetKey(KeyCode.UpArrow)){
                    DoAction(PlayerActions.Block, 1);
                }else if(Input.GetKey(KeyCode.DownArrow)){
                    DoAction(PlayerActions.Block, -1);
                }else{
                    DoAction(PlayerActions.Block, 0);
                }
                
            }
        }
        
        //Read and Process Touch Data for up to two fingers (possibly more if the lists are expanded)
        if(processTouches){
            for(int i = 0; i < touchPoints.Length;i++){
                if(Input.touchCount > i){
                    Touch touch = Input.GetTouch(i);
                    ProcessTouch(touch, i);
                }else if(touchPointIndex[i] > -1){
                    waitForReset[i] = false;
                    touchPointIndex[i] = -1;
                }
            }  
        }
    }

    void ProcessTouch(Touch touch, int fingerIndex){
        //Process and interpret the given Touchdata
        if(waitForReset[fingerIndex]){
            return;
        }
        //Save Touchdata to List
        SaveCurrentTouch(touch, fingerIndex);
        //Sort Data and get the swipe direction, averaged across the last 4 frames
        Vector2[] sortedTouchData = GetSortedTouchPoints(touchPoints[fingerIndex], touchPointIndex[fingerIndex]);
        Vector2 averagedDirVector = Vector2.zero;
        int touchesProcessed = 0;
        
        for(int i = 1; i < sortedTouchData.Length;i++){
            if(sortedTouchData[i].x != -999f){
                touchesProcessed++;
                averagedDirVector += sortedTouchData[i-1] - sortedTouchData[i];
            }
        }
        
        //return if no movement occured
        if(touchesProcessed <= 0){
            return;
        }

        averagedDirVector = averagedDirVector / touchesProcessed;
        if(averagedDirVector.magnitude < swipeDistance){
            return;
        }
        //Enough movement occured to read an action, reset finger data on next frame once player releases their touch
        waitForReset[fingerIndex] = true;

        //Check if the move or the attack side was touched
        bool touchedMoveSide = false;
        if((!persistentPlayerState.swapInputSides && sortedTouchData[0].x < Screen.width / 2) || (persistentPlayerState.swapInputSides && sortedTouchData[0].x > Screen.width / 2)){
            touchedMoveSide = true;
        }

        //Read the vertical direction first. This is used for both moving and to determine the attack and block directions, depending on which side was touched.
        Vector2 normedVector = averagedDirVector.normalized;
        short verticalDirection = 0;
        if(normedVector.y > 0.4f){
            verticalDirection = 1;
        }else if(normedVector.y < -0.4f){
            verticalDirection = -1;
        }

        //If the move side was touched, start jumping or sliding and end processing.
        if(touchedMoveSide){
            if(verticalDirection == 1){
                DoAction(PlayerActions.Jump, -999);
            }else if(verticalDirection == -1){
                DoAction(PlayerActions.Slide, -999);
            }
            return;
        }

        
        //Read side swipe direction and start attacking / blocking accordingly
        if(normedVector.x > 0f){
            DoAction(PlayerActions.Attack, verticalDirection);
        }else if(normedVector.x < 0f){
            if(persistentPlayerState.flipBlockVerticalInput){
                verticalDirection*=-1;
            }
            DoAction(PlayerActions.Block, verticalDirection);
        }

    }

    //Save finger position to relevant list. 
    void SaveCurrentTouch(Touch touch, int fingerIndex){
        if(touchPointIndex[fingerIndex]++ == -1){
            ClearTouchData(fingerIndex);
        }else if(touchPointIndex[fingerIndex] >= touchPoints[fingerIndex].Length){
            touchPointIndex[fingerIndex] = 0;
        }
        touchPoints[fingerIndex][touchPointIndex[fingerIndex]] = touch.position;              
    }

    //Returns a list of the last 4 touchpoints, sorted in chronological order, starting from the current frame at [0]
    Vector2[] GetSortedTouchPoints(Vector2[] tourchPointsToSort, int startIndex){
        Vector2[] output = new Vector2[tourchPointsToSort.Length];
        int readIndex = startIndex+1;
        for(int i = 0; i < tourchPointsToSort.Length; i++){
            if(--readIndex < 0){
                readIndex = tourchPointsToSort.Length-1;
            }
            output[i] = tourchPointsToSort[readIndex];
        }
        return output;
    }

    //Clears all touch data for selected finger
    void ClearTouchData(int fingerIndex){  
        for(int i = 0; i < touchPoints[fingerIndex].Length; i++){
                touchPoints[fingerIndex][i] = new Vector2(-999f,-999f);
        }     
    }

    //Starts the called upon action in the given direction.
    void DoAction(PlayerActions action, int actionDirection){
        switch(action){
            case PlayerActions.Jump: 
                debugText.text = "Jump!";
                if(playerAnimator.GetBool("Jump")){
                    playerAnimator.SetBool("ResetAnim", true);
                }
                playerAnimator.SetBool("Jump",true);
                playerAnimator.SetBool("Slide",false);
                break;
            case PlayerActions.Slide: 
                debugText.text = "Slide!";
                if(playerAnimator.GetBool("Slide")){
                    playerAnimator.SetBool("ResetAnim", true);
                }
                playerAnimator.SetBool("Slide",true);
                playerAnimator.SetBool("Jump",false);
                break;
            case PlayerActions.Attack: 
                debugText.text = "Attack! Direction: " + actionDirection;
                playerAnimator.SetInteger("AttackDirection", actionDirection);
                playerAnimator.SetBool("Attack",true);

                break;
            case PlayerActions.Block: 
                debugText.text = "Block! Direction: " + actionDirection;
                playerAnimator.SetInteger("AttackDirection", actionDirection);
                playerAnimator.SetBool("Block",true);
                break;
        }
    }
}
