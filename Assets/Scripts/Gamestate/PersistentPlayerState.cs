using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

[System.Serializable]
public class ProgressData{
    public List<string> unlockedItems = new List<string>(){"Item_ShoulderStrapChest", "Item_CoolGloves", "Item_HeavyLegs", "Item_SwiftElvenBoots", "Item_CoolShadesHelmet", "Item_CowboyHatHelmet", "Item_FullplateHelmet", "Item_LeatherHelmet", "Item_OpenBrainHelmet", "Item_RangerhatHelmet", "Item_SpikyLeatherHelmet", "Item_WelderMaskHelmet", "Item_TophatHelmet","Item_BeardHelmet","Item_WeaponAxe","Item_WeaponBlueDagger", "Item_WeaponButcher", "Item_WeaponCurved", "Item_WeaponDagger"};

    public float[] progressByLevel = new float[]{0,0};

    public int[] chestsByRarityLeft = new int[]{0,0,0,0};


}

public class PersistentPlayerState : MonoBehaviour
{

    //PersistentPlayerState hosts and saves all of the players persistent data, such as the players settings and game progress.
    //One instance of this component gets created in the inital loading screen.
    public float soundVolume = 1.0f;
    public float musicVolume = 1.0f;

    public bool swapInputSides = false;
    public bool flipBlockVerticalInput = false;

    public string playerCharacterAdress = "DebugPlayer";

    public string worldSpawnerAdress = "DebugWorld";
    public int worldSaveSlot = 0;

    public string weaponSlotAdress = "";

    public string helmetSlotAdress = "";

    public string chestArmorSlotAdress = "";

    public string gauntletSlotAdress = "";

    public string legArmorSlotAdress = "";

    public string footArmorSlotAdress = "";


    public ProgressData progressData;

    public Dictionary<string, ItemStats> unlockedItemsByAdress = new Dictionary<string, ItemStats>();
    private string savePath = "/progressData.json";

    public bool DEBUG_ResetPreference = false;
    // Start is called before the first frame update
    async void Start()
    {
        DontDestroyOnLoad(gameObject);
        if(File.Exists(Application.persistentDataPath + savePath)){
            progressData = JsonUtility.FromJson<ProgressData>(File.ReadAllText(Application.persistentDataPath + savePath));
        }else{
            progressData = new ProgressData();
            SaveProgressData();
        }
        if(DEBUG_ResetPreference){
            SaveDataToPrefs();
        }else{
            GetDataFromPrefs();
        }   
        await LoadItemStatsFromProgress();
        SceneManager.LoadSceneAsync(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetDataFromPrefs(){ //Loads the players Settings and Customization Config from the PlayerPrefs for quick access.
        soundVolume = PlayerPrefs.GetFloat("EffectVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        swapInputSides = bool.Parse(PlayerPrefs.GetString("SwapSides", "false"));
        flipBlockVerticalInput = bool.Parse(PlayerPrefs.GetString("FlipBlock", "false"));
    
        playerCharacterAdress = PlayerPrefs.GetString("PlayerCharacterAdress", "DebugPlayer");
        helmetSlotAdress = PlayerPrefs.GetString("HelmetAdress");
        chestArmorSlotAdress = PlayerPrefs.GetString("ChestAdress");
        gauntletSlotAdress = PlayerPrefs.GetString("HandsAdress");
        legArmorSlotAdress = PlayerPrefs.GetString("LegsAdress");
        footArmorSlotAdress = PlayerPrefs.GetString("FeetAdress");
        weaponSlotAdress = PlayerPrefs.GetString("WeaponAdress");
        
    }

    public void SaveDataToPrefs(){//Saves the players Settings and Customization Config tp the PlayerPrefs.
        PlayerPrefs.SetFloat("EffectVolume", soundVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetString("SwapSides", swapInputSides.ToString());
        PlayerPrefs.SetString("FlipBlock", flipBlockVerticalInput.ToString());
        
        //Customization Data
        PlayerPrefs.SetString("PlayerCharacterAdress", playerCharacterAdress);
        PlayerPrefs.SetString("HelmetAdress", helmetSlotAdress);
        PlayerPrefs.SetString("ChestAdress", chestArmorSlotAdress);
        PlayerPrefs.SetString("HandsAdress", gauntletSlotAdress);
        PlayerPrefs.SetString("LegsAdress", legArmorSlotAdress);
        PlayerPrefs.SetString("FeetAdress", footArmorSlotAdress);
        PlayerPrefs.SetString("WeaponAdress", weaponSlotAdress);
    }

    public void SaveProgressData(){ //Saves the players progress as a json to the persistentDataPath. This allows the player to easily back up their save files.
        string progressString = JsonUtility.ToJson(progressData);
        File.WriteAllText(Application.persistentDataPath + savePath, progressString);
    }

    async Task LoadItemStatsFromProgress(){ //Loads the players progress as a json from the persistentDataPath.
        foreach(string item in progressData.unlockedItems){
            AssetReference itemRef = new AssetReference(item);
            ItemStats itemData = await itemRef.LoadAssetAsync<ItemStats>().Task;
            if(itemData != null){
                unlockedItemsByAdress.Add(item, itemData);
            }
        }
    }

    
    public void SaveLevelProgress(float distance){ //Saves the players highest distance in the currently active level
        if(progressData.progressByLevel[worldSaveSlot] < distance){
            progressData.progressByLevel[worldSaveSlot] = distance;
            SaveProgressData();
        }
    }

    public float GetProgressOfLevel(int levelID){
        return progressData.progressByLevel[levelID];
    }

    public float GetProgressOfCurrentLevel(){
        return GetProgressOfLevel(worldSaveSlot);
    }
}
