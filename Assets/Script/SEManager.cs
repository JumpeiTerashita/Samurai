using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour {
    [SerializeField]
    AudioClip[] _runSE = new AudioClip[20];

    AudioSource audioPlayer;                           
    private int Num = -1;
    // Use this for initialization
    void Start()
    {
        audioPlayer = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
   

    public void SEStart(int SoundNum)
    {
        audioPlayer.PlayOneShot(_runSE[SoundNum]);
    }


}
