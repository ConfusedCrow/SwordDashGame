using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuCanvas;

    [SerializeField]
    private GameObject settingsCanvas;

    [SerializeField]
    private GameObject runCanvas;

    [SerializeField]
    private GameObject customizationCanvas;

    [SerializeField]
    private GameObject selectCanvas;

    [SerializeField]
    private GameObject creditsCanvas;

    
    public AudioSource bgMusicSource;
    
    public AudioSource buttonClickSource;

    
    // Start is called before the first frame update
    void Start()
    {
        
        OpenMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButtonClick(){
        buttonClickSource.Play();
    }    


    public void OpenMainMenu(){
        PlayButtonClick();
        settingsCanvas.SetActive(false);
        runCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
        customizationCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        selectCanvas.SetActive(false);
    }

    public void OpenSettingsMenu(){
        PlayButtonClick();
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void OpenRunMenu(){
        PlayButtonClick();
        mainMenuCanvas.SetActive(false);
        customizationCanvas.SetActive(false);
        runCanvas.SetActive(true);
    }

    public void OpenCredits(){
        PlayButtonClick();
        mainMenuCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
    }

    public void OpenCustomization(){
        PlayButtonClick();
        runCanvas.SetActive(false);
        selectCanvas.SetActive(false);
        customizationCanvas.SetActive(true);
    }

    
}
