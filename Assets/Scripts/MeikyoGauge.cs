using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MeikyoGauge : MonoBehaviour
{
    public static float MeikyoValue;

    float ColorBase = 0;

    [SerializeField]
    float MeikyoMax = 5.0f;
    [SerializeField]
    Text Meikyo;
    [SerializeField]
    GameObject MeikyoImage;
    [SerializeField]
    GameObject Meikyos;
    [SerializeField]
    Text[] MeikyoMask = new Text[4];

    bool IsMeikyo;

    

    // Use this for initialization
    void Start()
    {
       
        //Copy = Instantiate(Meikyo, Meikyo.transform.position, Quaternion.identity);
        //Copy.enabled = false;
        IsMeikyo = false;
        MeikyoValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Meikyos.SetActive(KilledNum.IsStarted);
        MeikyoValue = Mathf.Clamp(MeikyoValue, 0, MeikyoMax);

        if (MeikyoValue >= 5 && !IsMeikyo)
        {
            StartCoroutine(Meikyo_Enshutsu(0f));
            StartCoroutine(Meikyo_Enshutsu(0.5f));
            StartCoroutine(Meikyo_Enshutsu(1.0f));

            IsMeikyo = true;
        }
        else if (MeikyoValue < 5) IsMeikyo = false;

        if (IsMeikyo)
        {
            //  明鏡止水中
            for (int i = 0; i < 4; i++)
            {
                MeikyoMask[i].color = new Color(0, 0, 0, 1);
            }

            Meikyo.color = new Color(1, 0, 0, 1);
          
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                MeikyoMask[i].color = new Color(1, 1, 1, 1);
            }
            Meikyo.color = new Color(MeikyoValue / 5, MeikyoValue / 5, MeikyoValue / 5, 1);
        }
      

        if (PlayerBehavior.IsSkillEnable && MeikyoValue > 0)
        {
            MeikyoValue -= 0.025f;
            if (MeikyoValue <= 0) IsMeikyo = false; return;
        }
    }

    IEnumerator Meikyo_Enshutsu(float _WaitSec)
    {
        yield return new WaitForSeconds(_WaitSec);
        GameObject Copy = Instantiate(MeikyoImage, Meikyo.transform.position, Quaternion.identity);
        Copy.transform.parent = Meikyo.transform;
        Copy.transform.localPosition = new Vector3(-14, 1, 1);
        Copy.transform.localScale = new Vector3(3.5f, 2f, 1);
        Image CopyIm = Copy.GetComponent<Image>();
        Copy.transform.DOScale(new Vector3(7f, 4f, 2f), 2f);
        DOTween.ToAlpha(() => CopyIm.color, color => CopyIm.color = color, 0, 2.0f);
        yield return new WaitForSeconds(2f);
        Destroy(Copy.gameObject);
        yield break;
    }
}
