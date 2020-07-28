using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
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

    // Update is called once per frame
    void Update()
    {
        
    }

    //Loads all of the currently equipped gear based on the adresses given in the persistent player data
    void InitEquipData(){
        if(playerData.helmetSlotAdress != ""){
            EquipItem(playerData.unlockedItemsByAdress[playerData.helmetSlotAdress]);
        }else{
            EmptySlot(SlotType.Head);
        }

        if(playerData.chestArmorSlotAdress != ""){
            EquipItem(playerData.unlockedItemsByAdress[playerData.chestArmorSlotAdress]);
        }else{
            EmptySlot(SlotType.Chest);
        }

        if(playerData.gauntletSlotAdress != ""){
            EquipItem(playerData.unlockedItemsByAdress[playerData.gauntletSlotAdress]);
        }else{
            EmptySlot(SlotType.Hands);
        }

        if(playerData.legArmorSlotAdress != ""){
            EquipItem(playerData.unlockedItemsByAdress[playerData.legArmorSlotAdress]);
        }else{
            EmptySlot(SlotType.Legs);
        }

        if(playerData.footArmorSlotAdress != ""){
            EquipItem(playerData.unlockedItemsByAdress[playerData.footArmorSlotAdress]);
        }else{
            EmptySlot(SlotType.Feet);
        }

        if(playerData.weaponSlotAdress != ""){
            EquipItem(playerData.unlockedItemsByAdress[playerData.weaponSlotAdress]);
        }else{
            EmptySlot(SlotType.Weapon);
        }

        
    }

    public void OpenSelectionMenu(int slotNumber){
        currentSelectionType = (SlotType) slotNumber;
        foreach(GameObject obj in spawnedTiles){
            Destroy(obj);
        }
       
        RectTransform lastTile = unequipTile;
        foreach(ItemStats item in playerData.unlockedItemsByAdress.Values){
            if(item.itemSlot == currentSelectionType){
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
        gameObject.SetActive(false);
        selectionCanvas.SetActive(true);
        
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
