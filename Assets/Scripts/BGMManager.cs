using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGMの管理クラス
/// </summary>
public class BGMManager : SingleTon<BGMManager>
{

    [SerializeField]
    AudioClip[] _runBGM = new AudioClip[4];

    AudioClip _stopBGM = null;               
    AudioSource audioPlayer;                                                
    // Use this for initialization
    void Start()
    {
        audioPlayer = this.GetComponent<AudioSource>(); 
    }

   

    //SoundStart関数
    public void SoundStart(int SoundNum)
    {
        audioPlayer.clip = _runBGM[SoundNum];
        audioPlayer.Play();
    }

    //SoundStop関数
    public void SoundStop()
    {
        audioPlayer.clip = _stopBGM;   
        audioPlayer.Play();             
    }

}


