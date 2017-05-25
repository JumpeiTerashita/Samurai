﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {

    [SerializeField]
    AudioClip[] _runBGM = new AudioClip[4];

    AudioClip _stopBGM = null;               
    AudioSource audioPlayer;                         
    private int Num = -1;                           
    // Use this for initialization
    void Start()
    {
        audioPlayer = this.GetComponent<AudioSource>(); 
    }

   

    //SoundStart関数
    public void SoundStart(int SoundNum)
    {
        audioPlayer.clip = _runBGM[SoundNum];   
    }

    //SoundStop関数
    public void SoundStop()
    {
        audioPlayer.clip = _stopBGM;   
        audioPlayer.Play();             
    }

}

