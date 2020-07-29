using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
[System.Serializable]
class EquipSlotVisuals{
    public Image itemIcon;

    public Transform effectParent;

    public Image[] healthIndicator;

    public Image[] armorIndicator;

    public Image[] speedIndicator;

    //Loads the references to the stat indicators based on their parent transform
    public void AutoFillIndicators(){
        
        Transform speedParent = effectParent.GetChild(0);    
        speedIndicator = new Image[speedParent.childCount];
        for(int i = 0; i < speedIndicator.Length;i++){
            speedIndicator[i] = speedParent.GetChild(i).GetComponent<Image>();
        }

        Transform healthParent = effectParent.GetChild(1);
        healthIndicator = new Image[healthParent.childCount];
        for(int i = 0; i < healthIndicator.Length;i++){
            healthIndicator[i] = healthParent.GetChild(i).GetComponent<Image>();
        }

        Transform armorParent = effectParent.GetChild(2);
        armorIndicator = new Image[armorParent.childCount];
        for(int i = 0; i < armorIndicator.Length;i++){
            armorIndicator[i] = armorParent.GetChild(i).GetComponent<Image>();
        }
    }
}

public class CustomizationMenuBehavior : MonoBehaviour
{

    private PlayerVisualsManager playerVisuals;
    private PersistentPlayerState playerData;

    [SerializeField]
    private GameObject selectionCanvas;

    [SerializeField]
    private EquipSlotVisuals[] visualsByType;

    [SerializeField]
    private Sprite positiveSpeedSprite;

    [SerializeField]
    private Sprite negativeSpeedSprite;

    [SerializeField]
    private Sprite positiveHealthSprite;

    [SerializeField]
    private Sprite negativeHealthSprite;

    [SerializeField]
    private Sprite armorSprite;

    [SerializeField]
    private GameObject itemSlotPrefab;

    [SerializeField]
    private RectTransform unequipTile; //Reference to the "Unequip" Entry in the selection menu. Other tiles are spawned from here.

    private RectTransform slotParentTransform;
    private List<GameObject> spawnedTiles = new List<GameObject>();

    private Vector2 spawnSpacing = new Vector2(0.5f, 9f);


    [SerializeField]
    private Text selectionMenuHeader;

    [SerializeField]
    private Sprite emptySprite;

    private SlotType currentSelectionType = SlotType.Head;

    // Start is called before the first frame update
    void Start()
    {
        playerData = GameObject.FindGameObjectWithTag("PlayerState").GetComponent<PersistentPlayerState>();
        playerVisuals = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerVisualsManager>();
        slotParentTransform = unequipTile.transform.parent.GetComponent<RectTransform>();
        foreach(EquipSlotVisuals vis in visualsByType){
            vis.AutoFillIndicators();
        }
        InitEquipData();
    }

    void InitEquipData(){ //Loads all the items needed in the player preview
        LoadLookFromAdress(playerData.helmetSlotAdress, SlotType.Head);
        LoadLookFromAdress(playerData.chestArmorSlotAdress, SlotType.Chest);
        LoadLookFromAdress(playerData.gauntletSlotAdress, SlotType.Hands);
        LoadLookFromAdress(playerData.legArmorSlotAdress, SlotType.Legs);
        LoadLookFromAdress(playerData.footArmorSlotAdress, SlotType.Feet);
        LoadLookFromAdress(playerData.weaponSlotAdress, SlotType.Weapon);
    }

    void LoadLookFromAdress(string adress, SlotType slot){
        if(adress != ""){
            AssetReference assetReference = new AssetReference(adress);
            assetReference.LoadAssetAsync<ItemStats>().Completed += output =>
            {
                EquipItem(output.Result);
            };  
        }else{
            EmptySlot(slot);
        }
    }

    public void OpenSelectionMenu(int slotNumber){
        currentSelectionType = (SlotType) slotNumber;
        foreach(GameObject obj in spawnedTiles){
            Destroy(obj);
        }
        FillSelectionSlots();
        
        gameObject.SetActive(false);
        selectionCanvas.SetActive(true);
        
    }


    private void FillSelectionSlots(){ //Fills the selection menu with icons based on the current type
       string slotLabel = currentSelectionType.ToString();
       RectTransform lastTile = unequipTile;
       
       Addressables.LoadAssetsAsync<ItemStats>(slotLabel, null).Completed += objects =>
        {
            foreach (ItemStats item in objects.Result){
                 if(playerData.GetIsItemUnlocked(item.itemAdress)){
                     //Spawns a new slot if the item is of the current category and moves it to the next open position either to the right or below the current row
                    ItemSlotBehavior addedSlot = Instantiate(itemSlotPrefab, slotParentTransform).GetComponent<ItemSlotBehavior>();
                    RectTransform addedSlotTransform = addedSlot.GetComponent<RectTransform>();
                    float xOffset = lastTile.GetComponent<RectTransform>().anchoredPosition.x + addedSlotTransform.rect.width + spawnSpacing.x;
                    float yOffset = lastTile.GetComponent<RectTransform>().anchoredPosition.y;
                    if(xOffset > slotParentTransform.rect.width/2f){
                        xOffset = unequipTile.anchoredPosition.x;
                        yOffset = addedSlotTransform.sizeDelta.y + (spawnSpacing.y + addedSlotTransform.rect.height)*-1f;
                    }
                    addedSlotTransform.anchoredPosition = new Vector2(xOffset, yOffset);
                    addedSlot.SetData(item);
                    addedSlot.customsMenu = this;
                    lastTile = addedSlotTransform;
                    spawnedTiles.Add(addedSlot.gameObject);
                }
            }    
        };             
    }

    public void EquipItem(ItemStats item){ 
        if(item == null){
            return;
        }
        EquipSlotVisuals relatedVisuals = visualsByType[(int)item.itemSlot];
        switch(item.itemSlot){
            case SlotType.Head: playerData.helmetSlotAdress = item.itemAdress;
            break;
            case SlotType.Chest: playerData.chestArmorSlotAdress = item.itemAdress;
            break;
            case SlotType.Hands: playerData.gauntletSlotAdress = item.itemAdress;
            break;
            case SlotType.Legs: playerData.legArmorSlotAdress = item.itemAdress;
            break;
            case SlotType.Feet: playerData.footArmorSlotAdress = item.itemAdress;
            break;
            case SlotType.Weapon: playerData.weaponSlotAdress = item.itemAdress;
            break;
        }

        relatedVisuals.itemIcon.sprite = item.menuSprite;
        //Displays the items stats on the main equip screen
        SetSlotIcons(relatedVisuals.healthIndicator, item.healthMod, positiveHealthSprite, negativeHealthSprite);
        SetSlotIcons(relatedVisuals.armorIndicator, item.armorMod, armorSprite, null);
        SetSlotIcons(relatedVisuals.speedIndicator, Mathf.FloorToInt(item.speedMod*10), positiveSpeedSprite, negativeSpeedSprite);
        playerVisuals.DisplayEquipment(item);
        //Save Selection to the Savefile and return to the main equip screen.
        playerData.SaveDataToPrefs();
    }

    public void ExitToCustomizationCanvas(){
        selectionCanvas.SetActive(false);
        gameObject.SetActive(true);
    }

    public void EmptyCurrentSlotAndExit(){
        EmptySlot(currentSelectionType);
        selectionCanvas.SetActive(false);
        gameObject.SetActive(true);
        playerVisuals.RevertAndClearSlot(currentSelectionType);
         switch(currentSelectionType){
            case SlotType.Head: playerData.helmetSlotAdress = "";
            break;
            case SlotType.Chest: playerData.chestArmorSlotAdress = "";
            break;
            case SlotType.Hands: playerData.gauntletSlotAdress = "";
            break;
            case SlotType.Legs: playerData.legArmorSlotAdress = "";
            break;
            case SlotType.Feet: playerData.footArmorSlotAdress = "";
            break;
            case SlotType.Weapon: playerData.weaponSlotAdress = "";
            break;
        }
        playerData.SaveDataToPrefs();
    }

    public void EmptySlot(SlotType slot){
        EquipSlotVisuals relatedVisuals = visualsByType[(int)slot];
        relatedVisuals.itemIcon.sprite = emptySprite;
        SetSlotIcons(relatedVisuals.healthIndicator, 0, positiveHealthSprite, negativeHealthSprite);
        SetSlotIcons(relatedVisuals.armorIndicator, 0, armorSprite, null);
        SetSlotIcons(relatedVisuals.speedIndicator, 0, positiveSpeedSprite, negativeSpeedSprite);
    }


    void SetSlotIcons(Image[] iconSlots, int value, Sprite positiveIcon, Sprite negativeIcon){
        int absValue = Mathf.Abs(value);
        for(int i = 0; i < iconSlots.Length;i++){
            if(i >= absValue){
                iconSlots[i].gameObject.SetActive(false);
            }else{
                iconSlots[i].gameObject.SetActive(true);
                if(value > 0 ){
                    iconSlots[i].sprite = positiveIcon;
                }else{
                    iconSlots[i].sprite = negativeIcon;
                }
            }
        }
    }
}
