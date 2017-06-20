using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SEの管理クラス
/// Singletonパターン
/// </summary>
public class SEManager : SingleTon<SEManager> {


    [SerializeField]
    AudioClip[] _runSE = new AudioClip[20];

    AudioSource audioPlayer;                           
    private int Num = -1;
    // Use this for initialization
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
   

    public void SEStart(int SoundNum)
    {
        audioPlayer.PlayOneShot(_runSE[SoundNum]);
    }


}
