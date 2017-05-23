﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleScene : MonoBehaviour
{
   
    GameObject Black;
    Image image;

    // Use this for initialization
    void Start()
    {
        Black = GameObject.Find("Black");
        image = Black.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(image.color);
        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("Pushed");

            StartCoroutine(Fading());
            
            
        }
        

    }

    IEnumerator Fading()
    {
        DOTween.ToAlpha(() => image.color, color => image.color = color, 1.0f, 1.0f)
            .OnComplete(() => SceneManager.LoadScene("a")
            );
        yield break;
    }
}
