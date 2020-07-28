using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
public class LoadWorldData : MonoBehaviour
{

    //Loads the Adressables for the PlayerCharacter, the Worldspawner and all of the players Equipment as needed.
    private Vector3 playerCharSpawnPos = new Vector3(-5.85f,0.9f,-4.19f);
    private Vector3 worldSpawnerSpawnPos = new Vector3(-2.13f,0.31f,-4.31f);

    private int dataSuccessfullyLoaded = 0;
    // Start is called before the first frame update
    async void Start()
    {
        await LoadRequiredAssets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task LoadRequiredAssets(){
        //Load required Data from Adressables
        PersistentPlayerState persistentPlayer = GameObject.FindGameObjectWithTag("PlayerState").GetComponent<PersistentPlayerState>();
        AssetReference playerCharRef = new AssetReference(persistentPlayer.playerCharacterAdress);
        GameObject player = await playerCharRef.InstantiateAsync(playerCharSpawnPos, Quaternion.identity).Task;
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerCharacter playerCharacter = player.GetComponent<PlayerCharacter>();

        PlayerVisualsManager playerVisuals = player.GetComponent<PlayerVisualsManager>();
        
        AssetReference worldRef = new AssetReference(persistentPlayer.worldSpawnerAdress);
        GameObject world = await worldRef.InstantiateAsync(worldSpawnerSpawnPos, Quaternion.identity).Task;

        if(persistentPlayer.helmetSlotAdress != ""){
            AssetReference helmetRef = new AssetReference(persistentPlayer.helmetSlotAdress);
            ItemStats helmetItem = await helmetRef.LoadAssetAsync<ItemStats>().Task;
            InitItemData(helmetItem, playerCharacter, playerVisuals);
        }

        if(persistentPlayer.chestArmorSlotAdress != ""){
            AssetReference chestRef = new AssetReference(persistentPlayer.chestArmorSlotAdress);
            ItemStats chestItem = await chestRef.LoadAssetAsync<ItemStats>().Task;
            InitItemData(chestItem, playerCharacter, playerVisuals);
        }

        if(persistentPlayer.gauntletSlotAdress != ""){
            AssetReference handRef = new AssetReference(persistentPlayer.gauntletSlotAdress);
            ItemStats gauntletItem = await handRef.LoadAssetAsync<ItemStats>().Task;
            InitItemData(gauntletItem, playerCharacter, playerVisuals);
        }

        if(persistentPlayer.legArmorSlotAdress != ""){
            AssetReference legRef = new AssetReference(persistentPlayer.legArmorSlotAdress);
            ItemStats legItem = await legRef.LoadAssetAsync<ItemStats>().Task;
            InitItemData(legItem, playerCharacter, playerVisuals);
        }
        
        if(persistentPlayer.footArmorSlotAdress != ""){
            AssetReference footRef = new AssetReference(persistentPlayer.footArmorSlotAdress);
            ItemStats footItem = await footRef.LoadAssetAsync<ItemStats>().Task;
            InitItemData(footItem, playerCharacter, playerVisuals);
        }

        if(persistentPlayer.weaponSlotAdress != ""){
            AssetReference weaponRef = new AssetReference(persistentPlayer.weaponSlotAdress);
            ItemStats weaponItem = await weaponRef.LoadAssetAsync<ItemStats>().Task;
            InitItemData(weaponItem, playerCharacter, playerVisuals);
        }

        //Once everything is loaded, setup the game and run
        playerStats.Initiate();
        world.GetComponent<WorldMover>().Initiate();

    }

    //Modifies the players stats according to the items data
    void InitItemData(ItemStats item, PlayerCharacter playerCharacter, PlayerVisualsManager playerVisuals){
        playerCharacter.maxHealth += item.healthMod;
        playerCharacter.totalSpeedMod += item.speedMod;
        playerCharacter.staggerWindow += item.staggerMod;
        playerCharacter.initialArmor += item.armorMod;
        playerCharacter.attackDamage += item.damageMod;

        playerVisuals.DisplayEquipment(item);
    }
}
