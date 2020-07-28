using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class WorldMenuBehavior : MonoBehaviour
{

    [SerializeField]
    private Text[] recordTextOfWorlds;

    private string[] worldAdressByID = {"DungeonWorld", "DebugWorld"};

    PersistentPlayerState playerState;
    // Start is called before the first frame update
    void Start()
    {
        playerState = GameObject.FindGameObjectWithTag("PlayerState").GetComponent<PersistentPlayerState>();
        for(int i = 0; i < recordTextOfWorlds.Length; i++){
            recordTextOfWorlds[i].text = Mathf.Floor(playerState.GetProgressOfLevel(i) / 100) + "M";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRun(int worldID){
        playerState.worldSpawnerAdress = worldAdressByID[worldID];
        playerState.worldSaveSlot = worldID;
        SceneManager.LoadScene(2);
    }
}
