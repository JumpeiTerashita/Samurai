using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]

/// <summary>
/// 敵生成オブジェクト　指定座標
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    GameObject Player;
    [SerializeField]
    GameObject Enemy;

    [SerializeField]
    float SpawnSpan = 5.0f;

    Vector3 SpawnPoint;

    bool isStarted = false;
    bool isRunning = false;
    bool isAlreadySpawn = false;

    // Use this for initialization
    void Start()
    {
        SpawnPoint = transform.position;
        SpawnPoint.y = 0;
        Player = GameObject.Find("Player");
    }

    void Update()
    {
        if (!KilledNum.IsStarted) return;
        if (!isStarted)
        {
            isStarted = true;
            StartCoroutine(SpawnLoop());
        }   
    }

    IEnumerator SpawnLoop()
    {
        if (isRunning) yield break;
        isRunning = true;
        //  Debug.Log("I'm Running");
        if (isAlreadySpawn)
        {
            isAlreadySpawn = false;
        }
        else
        {
            GameObject SpawnedEnemy = Instantiate(Enemy, SpawnPoint, Quaternion.identity);
            SpawnedEnemy.transform.LookAt(Player.transform.position);
            Animator SpawnedEnemyAnim = GameObjectManager.getAnimator(SpawnedEnemy);
            SpawnedEnemyAnim.Play("Locomotion.Born");
        }

        yield return new WaitForSeconds(SpawnSpan);


        isRunning = false;
        StartCoroutine(SpawnLoop());
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Enemy") isAlreadySpawn = true;
    }

    }
