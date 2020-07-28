using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUnpauseGame : MonoBehaviour
{
    //Pauses/Unpauses the game after the pause button is pressed. Timescale gets set to 0 while the game is paused

    private float lastUnpausedTime = 0.0f;

    [SerializeField]
    private GameObject pauseCanvas;

    private bool gamePaused = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnpauseGame(){
        if(!gamePaused){
            return;
        }
        pauseCanvas.SetActive(false);
        gamePaused = false;
        Time.timeScale = lastUnpausedTime;
    }

    public void PauseGame(){
        if(gamePaused){
            return;
        }
        gamePaused = true;
        lastUnpausedTime = Time.timeScale;
        pauseCanvas.SetActive(true);
        Time.timeScale = 0.0f;
    }
}
