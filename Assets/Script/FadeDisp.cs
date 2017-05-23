﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class FadeDisp : MonoBehaviour {
    GameObject Black;
    static Image image;

    

    // Use this for initialization
    void Start () {
        Black = GameObject.Find("Black");
        image = Black.GetComponent<Image>();
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        //Debug.Log(image.color);
        


    }

    IEnumerator FadeIn()
    {
        DOTween.ToAlpha(() => image.color, color => image.color = color, 0.0f, 2.0f);
        yield break;
    }

    public static IEnumerator FadeOut()
    {
        DOTween.ToAlpha(() => image.color, color => image.color = color, 1.0f, 5.0f)
            .OnComplete(() => SceneManager.LoadScene("title"));
        yield break;
    }
}