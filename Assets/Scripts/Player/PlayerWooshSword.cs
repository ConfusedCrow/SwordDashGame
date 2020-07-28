using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Plays randomly pitched sounds when called upon.
public class PlayerWooshSword : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] wooshSounds;

    private float minPitch = 0.9f;
    private float maxPitch = 1.05f;

    [SerializeField]
    private AudioSource wooshSource;

    public float baseVolumeAdjust = 0f;
    // Start is called before the first frame update
    void Start()
    {
        wooshSource.volume = baseVolumeAdjust + GameObject.FindGameObjectWithTag("PlayerState").GetComponent<PersistentPlayerState>().soundVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayWooshSound(){
        int wooshIndex = Random.Range(0, wooshSounds.Length);
        float pitch = Random.Range(minPitch, maxPitch);

        wooshSource.pitch = pitch;
        wooshSource.PlayOneShot(wooshSounds[wooshIndex]);
    }
}
