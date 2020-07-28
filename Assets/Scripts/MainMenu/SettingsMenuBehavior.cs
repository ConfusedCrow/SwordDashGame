using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingsMenuBehavior : MonoBehaviour
{
    private PersistentPlayerState persistentPlayerState;

    [SerializeField]
    private Slider effectVolumeSlider;

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Text swapSidesButtonText;

    [SerializeField]
    private Text invertBlockYText;

    [SerializeField]
    private MainMenuBehavior mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        persistentPlayerState =  GameObject.FindGameObjectWithTag("PlayerState").GetComponent<PersistentPlayerState>();
        LoadSettings();
        mainMenu.bgMusicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSettings(){
        persistentPlayerState.GetDataFromPrefs();
        musicVolumeSlider.value = persistentPlayerState.musicVolume;
        mainMenu.bgMusicSource.volume = musicVolumeSlider.value;
        effectVolumeSlider.value = persistentPlayerState.soundVolume;
        mainMenu.buttonClickSource.volume = effectVolumeSlider.value;
        if(persistentPlayerState.swapInputSides){
            swapSidesButtonText.text = "Inverted";
        }
        if(persistentPlayerState.flipBlockVerticalInput){
            invertBlockYText.text = "Inverted";
        }
        
    }

    public void SaveSettings(){
        persistentPlayerState.SaveDataToPrefs();
    }

    public void MusicSliderMoved(){
        persistentPlayerState.musicVolume = musicVolumeSlider.value;
        mainMenu.bgMusicSource.volume = persistentPlayerState.musicVolume;
    }

    public void EffectSliderMoved(){
        persistentPlayerState.soundVolume = effectVolumeSlider.value;
        mainMenu.buttonClickSource.volume = persistentPlayerState.soundVolume;
    }

    public void SwapInputButtonPressed(){
        mainMenu.PlayButtonClick();
        persistentPlayerState.swapInputSides = !persistentPlayerState.swapInputSides;
        if(persistentPlayerState.swapInputSides){
            swapSidesButtonText.text = "<b>Inverted</b>";
        }else{
            swapSidesButtonText.text = "<b>Normal</b>";
        }
    }

    public void InvertAttackYButtonPressed(){
        mainMenu.PlayButtonClick();
        persistentPlayerState.flipBlockVerticalInput = !persistentPlayerState.flipBlockVerticalInput;
        if(persistentPlayerState.flipBlockVerticalInput){
            invertBlockYText.text = "<b>Inverted</b>";
        }else{
            invertBlockYText.text = "<b>Normal</b>";
        }
    }
}
