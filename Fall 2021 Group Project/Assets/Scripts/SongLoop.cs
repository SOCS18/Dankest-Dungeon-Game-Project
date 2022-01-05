using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongLoop : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip loopingSong;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource.PlayOneShot(loopingSong);
        audioSource.PlayScheduled(AudioSettings.dspTime + loopingSong.length);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
