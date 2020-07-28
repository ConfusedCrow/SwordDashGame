using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Applies visual data from equipped items to the players body.
[System.Serializable]
struct PlayerVisualSlots{
    public MeshRenderer headSlot;
    public MeshRenderer[] chestRenderSlots;
    public MeshRenderer[] handRenderSlots;
    public MeshRenderer[] LegRenderSlots;
    public MeshRenderer[] FootRenderSlots;

    public MeshRenderer weaponSlot;
}

public class PlayerVisualsManager : MonoBehaviour
{
    
    [SerializeField]
    private PlayerVisualSlots bodyVisuals;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayEquipment(ItemStats equipment){

        //Finds all Renderers needed to display a specific bodypart.
        MeshRenderer[] activeRenderers = null;
        switch(equipment.itemSlot){
            case SlotType.Head: activeRenderers = new MeshRenderer[]{bodyVisuals.headSlot}; break;
            case SlotType.Chest: activeRenderers = bodyVisuals.chestRenderSlots; break;
            case SlotType.Hands: activeRenderers = bodyVisuals.handRenderSlots; break;
            case SlotType.Legs: activeRenderers = bodyVisuals.LegRenderSlots; break;
            case SlotType.Feet: activeRenderers = bodyVisuals.FootRenderSlots; break;
            case SlotType.Weapon: activeRenderers = new MeshRenderer[]{bodyVisuals.weaponSlot}; break;
        }
        if(activeRenderers == null){
            Debug.LogError("SlotType for Item: " + equipment.itemName + " came up empty...");
            return;
        }
        //Empties out the slot before creating a new copy with the updated texture
        RevertAndClearSlot(equipment.itemSlot);
        CreateMeshCopiesAndApply(activeRenderers, equipment.itemMats, equipment.hidesBodypart);

    }


    //Copies all of the meshes that make up a specific slot and applies a new material to this copy. The copy will get the suffix "Armor" to prevent confusion between the original and the copy
    void CreateMeshCopiesAndApply(MeshRenderer[] renderersToCopy, Material[] materialsToApply, bool hideOriginalSlot){
        for(int i=0; i < renderersToCopy.Length;i++){
            if(materialsToApply[i] == null){
                continue;
            }
            GameObject bodyCopy = Instantiate(renderersToCopy[i].gameObject,renderersToCopy[i].transform.parent);
            bodyCopy.transform.position += new Vector3(0.0f,0.0f,-0.001f); //Move Armor forward to prevent Z-Clipping
            bodyCopy.GetComponent<MeshRenderer>().material = materialsToApply[i];
            bodyCopy.name = renderersToCopy[i].name + "Armor";
            if(hideOriginalSlot){
                renderersToCopy[i].gameObject.SetActive(false);
            }
        }
    }

    //Completly clears any Copies created with CreateMeshCopiesAndApply for the selected equipment slot
    public void RevertAndClearSlot(SlotType slot){
        MeshRenderer[] activeRenderers = null;
        switch(slot){
            case SlotType.Head: activeRenderers = new MeshRenderer[]{bodyVisuals.headSlot}; break;
            case SlotType.Chest: activeRenderers = bodyVisuals.chestRenderSlots; break;
            case SlotType.Hands: activeRenderers = bodyVisuals.handRenderSlots; break;
            case SlotType.Legs: activeRenderers = bodyVisuals.LegRenderSlots; break;
            case SlotType.Feet: activeRenderers = bodyVisuals.FootRenderSlots; break;
            case SlotType.Weapon: activeRenderers = new MeshRenderer[]{bodyVisuals.weaponSlot}; break;
        }

        foreach(MeshRenderer rend in activeRenderers){
            rend.gameObject.SetActive(true);
            Transform relatedArmor = rend.transform.parent.Find(rend.name + "Armor");
            if(relatedArmor != null){
                Destroy(relatedArmor.gameObject);
            }
            
        }
    }

    
}
