using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

/// <summary>
/// 画面切り替え  Fade処理
/// </summary>
public class FadeDisp : MonoBehaviour {
    [SerializeField]
    GameObject Black;
    static Image image;

    // Use this for initialization
    void Start () {
        image = Black.GetComponent<Image>();
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        DOTween.ToAlpha(() => image.color, color => image.color = color, 0.0f, 2.0f);
        yield break;
    }

    public static IEnumerator FadeOutToTitle()
    {
        DOTween.ToAlpha(() => image.color, color => image.color = color, 1.0f, 5.0f)
            .OnComplete(() => SceneManager.LoadScene("title"));
        yield break;
    }
}
