using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeikyoGauge : MonoBehaviour {
    public static float MeikyoValue;
   
    float ColorBase = 0;

    [SerializeField]
    float MeikyoMax = 5.0f;
    [SerializeField]
    Text Meikyo;
    [SerializeField]
    Text MeikyoMask;
	// Use this for initialization
	void Start () {
        MeikyoValue = 0;
    }
	
	// Update is called once per frame
	void Update () {
        Meikyo.enabled = KilledNum.IsStarted;
        MeikyoMask.enabled = KilledNum.IsStarted;
        MeikyoValue = Mathf.Clamp(MeikyoValue,0,MeikyoMax);
        Meikyo.color = new Color(MeikyoValue/5, MeikyoValue/5, MeikyoValue/5, 1);

        if (PlayerBehavior.IsSkillEnable && MeikyoValue > 0)
        {
            MeikyoValue-= 0.025f;
            if (MeikyoValue <= 0) return;
           
        }
    }

    
}
