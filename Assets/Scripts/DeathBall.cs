﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死の予兆オブジェクト
/// プレイヤー危険時生成
/// </summary>
public class DeathBall : MonoBehaviour {
    GameObject Player;
	// Use this for initialization
	void Start () {
        Player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(Player.transform.position.x,Player.transform.position.y+1,Player.transform.position.z);
	}
}
