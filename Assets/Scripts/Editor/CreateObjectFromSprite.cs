using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class CreateObjectFromSprite : MonoBehaviour
{
    //Creates a Cutout Material from a selected Texture2D
    [MenuItem("Assets/Create/QuickCreate/Create Material", false, 100)]
    static void CreateMaterialFromSelection(){
        //Load predefined Sprite Material and copy it.
        Material srcMat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Art Assets/SpriteParent.mat", typeof(Material));
        if (srcMat == null)
        {
            Debug.LogError("Sprite Parent Material \"SpriteParent\" missing! Was it removed from Assets/ArtAssets?)");
            return;
        }

        foreach (object selectedObject in Selection.objects)
        {
            
            Material mat = new Material(srcMat);
            //Set Texture to Sprite
            Texture2D selectedSprite = (Texture2D)selectedObject;
            mat.mainTexture = selectedSprite;
            //Create Path to new Asset
            string folderPath = AssetDatabase.GetAssetPath(selectedSprite.GetInstanceID());
            //Remove Filename from Path
            folderPath = folderPath.Replace(selectedSprite.name + ".png", "");

            //Create Asset in Database
            AssetDatabase.CreateAsset(mat, folderPath + "/Mat_" + selectedSprite.name + ".mat");
            Debug.Log("Material created at: " + folderPath + "/Mat_" + selectedSprite.name + ".mat");
        }
    }

    [MenuItem("Assets/Create/QuickCreate/Create Material", true)]
    static bool ValidateCreateMaterialFromSelection(){
        foreach (object selectedObject in Selection.objects)
        {
            if(!selectedObject.GetType().ToString().Equals("UnityEngine.Texture2D")){
                return false;
            }
        }
        return true;
    }


    [MenuItem("Assets/Create/QuickCreate/Create Quad Prefab", false, 100)]
    static void CreateQuadPrefabFromMaterial(MenuCommand menuCommand){
        foreach (object selectedObject in Selection.objects)
        {
            Material srcMat = (Material)selectedObject;

            //Create Path to new Asset
            string folderPath = AssetDatabase.GetAssetPath(srcMat.GetInstanceID());

            //Remove Filename from Path
            folderPath = folderPath.Replace(srcMat.name + ".mat", "");
            //Reroute to Prefab Folder
            folderPath = folderPath.Replace("Art Assets", "Prefabs");

            string objectName = srcMat.name.Replace(".mat", "");
            objectName = srcMat.name.Replace("Mat_", "");
            //Create GameObject
            GameObject quadObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quadObject.GetComponent<MeshRenderer>().material = srcMat;

            //Make sure destination folder exists
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/" + folderPath))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/" + folderPath);
            }
            //Save gameobject to destination
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(quadObject, folderPath + objectName + ".prefab");
            Debug.Log("Quad-Prefab created at: " + AssetDatabase.GetAssetPath(prefab));

            //Clean up gameobject from scene to prevent unintended clutter
            DestroyImmediate(quadObject);
        }
        
    }

    [MenuItem("Assets/Create/QuickCreate/Create Quad Prefab", true)]
    static bool ValidateCreateQuadPrefabFromMaterial(){
         foreach (object selectedObject in Selection.objects)
        {
            if(!selectedObject.GetType().ToString().Equals("UnityEngine.Material")){
                return false;
            }
        }
        return true;
    }
}
