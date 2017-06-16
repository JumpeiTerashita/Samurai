using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillManager : MonoBehaviour
{
    public static float SkillPoint;


    [SerializeField]
    float SkillMax = 5.0f;
    [SerializeField]
    float ConsumeSkillPoint= 0.05f;

    [SerializeField]
    Text BaseTxt;
    [SerializeField]
    GameObject DecorationImage;
    [SerializeField]
    GameObject TxtBox;
    [SerializeField]
    Text[] TxtFlame = new Text[4];

    bool IsSkillEnable;



    // Use this for initialization
    void Start()
    {
        IsSkillEnable = false;
        SkillPoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TxtBox.SetActive(KilledNum.IsStarted);
        SkillPoint = Mathf.Clamp(SkillPoint, 0, SkillMax);

        if (SkillPoint >= 5 && !IsSkillEnable)
        {
            StartCoroutine(Meikyo_Enshutsu(0f));
            StartCoroutine(Meikyo_Enshutsu(0.15f));
            StartCoroutine(Meikyo_Enshutsu(0.3f));

            IsSkillEnable = true;
        }
        else if (SkillPoint < 5) IsSkillEnable = false;

        if (IsSkillEnable)
        {
            //  明鏡止水中
            for (int i = 0; i < 4; i++)
            {
                TxtFlame[i].color = new Color(0, 0, 0, 1);
            }

            BaseTxt.color = new Color(1, 0, 0, 1);

        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                TxtFlame[i].color = new Color(1, 1, 1, 1);
            }
            BaseTxt.color = new Color(SkillPoint / (int)SkillMax, SkillPoint / (int)SkillMax, SkillPoint / 5, (int)SkillMax);
        }


        if (PlayerBehavior.IsSkillEnable && SkillPoint > 0)
        {
            SkillPoint -= ConsumeSkillPoint;
            if (SkillPoint <= 0) IsSkillEnable = false; return;
        }
    }

    IEnumerator Meikyo_Enshutsu(float _WaitSec)
    {
        yield return new WaitForSeconds(_WaitSec);
        GameObject Copy = Instantiate(DecorationImage, BaseTxt.transform.position, Quaternion.identity);
        Copy.transform.parent = BaseTxt.transform;
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
