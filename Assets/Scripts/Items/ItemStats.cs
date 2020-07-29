using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType {Head, Chest, Hands, Legs, Feet, Weapon}

public enum Rarity {Common, Uncommon, Rare, Dashing}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemScriptableObject", order = 1)]
public class ItemStats : ScriptableObject
{
    //Contains all the relevant information that defines an item
    public string itemName = "Unnamed Item"; //Display name of item, used in the Customization Menu

    public string itemAdress = ""; 
    
    public SlotType itemSlot = SlotType.Head; //Which slot is occupied by this item?

    public Rarity itemRarity = Rarity.Common; //Rarity used to determin the Inventory Border Color
    
    public Material[] itemMats; //Sprites are assigned in the following order (if applicable): main part, upper right, upper left, lower right, lower left

    public Sprite menuSprite; //Sprite that gets loaded inside the customization screen

    public bool hidesBodypart = false; //Should the underlying body be hidden by this item?

    public float speedMod = 0.0f;

    public int healthMod = 0;

    public int armorMod = 0;
    
    public int damageMod = 0;

    public float staggerMod = 0.0f;


}
