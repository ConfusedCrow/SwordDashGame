using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMover : MonoBehaviour
{
    //This class moves the worldtiles around the player to give them the impression of forward movement
    public Transform playerTransform;
    public PlayerStats playerStats;
    private List<GameObject> spawnedTiles = new List<GameObject>();
    private float spawnDistance = 20f; //How far away from the player should new tiles spawn?

    private float deSpawnDistance = 20f; //How far should tiles move behind the player before they despawn?
    private float tileWidth = 10f; //How wide is a tile?
    public GameObject[] emptyTiles; //These tiles should never contain any active gameplay elements.

    public GameObject[] dodgeHazardTiles; //These tiles should only ever hold hazards that need to be hit or dodged to proceed.

    public GameObject[] fightHazardTiles; //These tiles must always contain 1 enemy for the player to fight

    public GameObject[] pickupBonusTiles; //These tiles should always contain a useful powerup for the player to use

    public bool isInFight = false; //If isInFight is true, the world will only ever spawn empty tiles to prevent enemies from clipping into any objects

    private bool isSetup = false; //If isSetup is true, all necessary Objects were spawned and the world mover is allowed to move and spawn tiles


    public void Initiate()
    {
        //Make all necessary connections to other objects and classes
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;
        playerStats = playerTransform.GetComponent<PlayerStats>();
        spawnedTiles.Add(Instantiate(emptyTiles[Random.Range(0,emptyTiles.Length)], transform.position - new Vector3(tileWidth,0f,0f), Quaternion.identity));
        spawnedTiles.Add(Instantiate(emptyTiles[Random.Range(0,emptyTiles.Length)], transform.position, Quaternion.identity));
        spawnedTiles.Add(Instantiate(emptyTiles[Random.Range(0,emptyTiles.Length)], transform.position + new Vector3(tileWidth,0f,0f), Quaternion.identity));
        while(CheckIfNeedSpawnTile());
        GameObject.Find("LoadingScreen").SetActive(false);
        isSetup = true;
    }

  
    void Update()
    {
        if(isSetup){
            MoveExistingTiles();
            CheckIfNeedSpawnTile();
            RemoveOldTiles();
        }
    }

    void MoveExistingTiles(){
        foreach(GameObject tile in spawnedTiles){
            tile.transform.position+= new Vector3(playerStats.currentPlayerSpeed*Time.deltaTime*-0.01f,0f,0f);
        }
    }

    bool CheckIfNeedSpawnTile(){
        //Checks if there is enough space to spawn a new tile and does so, if any is available. The spawned tile is determined randomly if it is not forced to empty by IsInFight
        GameObject lastTile = spawnedTiles[spawnedTiles.Count-1];
        if((lastTile.transform.position - playerTransform.position).magnitude < spawnDistance){
            
            GameObject spawnTile;
            if(isInFight){
                spawnTile = emptyTiles[Random.Range(0,emptyTiles.Length)];
            }else{
                int randomValue = Random.Range(0,4);
                switch(randomValue){
                    case 0: spawnTile = dodgeHazardTiles[Random.Range(0,dodgeHazardTiles.Length)]; 
                    break;
                    case 1: spawnTile = fightHazardTiles[Random.Range(0,fightHazardTiles.Length)];
                            isInFight = true;
                    break;
                    case 2: spawnTile = dodgeHazardTiles[Random.Range(0,dodgeHazardTiles.Length)]; 
                            if(playerStats.GetCurrentHealth() < playerStats.GetActiveCharacter().maxHealth){
                                int spawnChance = Random.Range(0,8);
                                if(spawnChance == 0){
                                    spawnTile = pickupBonusTiles[Random.Range(0,pickupBonusTiles.Length)]; 
                                }
                            }
                    break;
                    default: spawnTile = emptyTiles[Random.Range(0,emptyTiles.Length)]; 
                    break;
                }
            }
            
            spawnedTiles.Add(Instantiate(spawnTile, lastTile.transform.position + new Vector3(tileWidth,0f,0f), Quaternion.identity));
            return true;
        }
        return false;
    }

    void RemoveOldTiles(){
        List<GameObject> destructionList = new List<GameObject>();
        
        for(int i = 0; i < spawnedTiles.Count; i++){
            if((spawnedTiles[i].transform.position.x < playerTransform.position.x) && ((playerTransform.position - spawnedTiles[i].transform.position).magnitude >= deSpawnDistance)){
               destructionList.Add(spawnedTiles[i]);
            }
        }
        foreach(GameObject tile in destructionList){
            spawnedTiles.Remove(tile);
            Destroy(tile);
        }
    }
}
