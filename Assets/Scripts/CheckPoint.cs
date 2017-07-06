using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 指定ポイント通過時   姫の周りから敵沸かせる
/// </summary>
public class CheckPoint : MonoBehaviour {

    GameObject EnemySpawner;
    GameObject EnemySpawner2;

    GameObject Hime;
	// Use this for initialization
	void Start () {
        EnemySpawner = GameObject.Find("EnemySpawner");
        EnemySpawner2 = GameObject.Find("EnemySpawner2");
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hero")
        {
            EnemySpawner.transform.position = new Vector3(8, 0, 84);
            EnemySpawner.GetComponent<EnemySpawner>().SetSpawnSpan(12);
            Destroy(EnemySpawner2.gameObject);
        }
    }

    
}
