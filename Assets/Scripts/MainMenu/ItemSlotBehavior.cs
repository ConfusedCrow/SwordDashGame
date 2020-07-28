using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemSlotBehavior : MonoBehaviour
{
    //Controls the look and OnClick Action of an inventory slot. Used when selecting an item to equip.

    public CustomizationMenuBehavior customsMenu;

    [SerializeField]
    private Text itemLabel;

    [SerializeField]
    private Image itemSprite;

    [SerializeField]
    private Image itemBorder;

    [SerializeField]
    private string itemAdress;

    [SerializeField]
    private ItemStats itemData;

    static Dictionary<Rarity, Color> rarityColors = new Dictionary<Rarity, Color>(){
        {Rarity.Common, Color.white},
        {Rarity.Uncommon, Color.green},
        {Rarity.Rare, Color.red},
        {Rarity.Dashing, Color.yellow}};
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetData(ItemStats item){
        itemLabel.text = item.itemName;
        itemSprite.sprite = item.menuSprite;
        itemBorder.color = rarityColors[item.itemRarity];
        itemAdress = item.itemAdress;
        itemData = item;
    }

    public void OnClick(){
        customsMenu.EquipItem(itemData);
    }
}
