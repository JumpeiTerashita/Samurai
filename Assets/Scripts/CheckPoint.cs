using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    GameObject enemyManageObj;

    GameObject Hime;
    BoxCollider check;
	// Use this for initialization
	void Start () {
        enemyManageObj = GameObject.Find("EnemyManager");
        Hime = GameObject.Find("Heroine");
        check = GetComponent<BoxCollider>();
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hero")
        {
            
            InvokeRepeating("heroineGardnerSpawn", 0.0f, 5.0f);
        }
    }

    void heroineGardnerSpawn()
    {
        Debug.Log("OK!!!");
        enemyManageObj.GetComponent<EnemyManager>().SetEnemySpawn(Hime.transform.position, 5f);
    }
}
