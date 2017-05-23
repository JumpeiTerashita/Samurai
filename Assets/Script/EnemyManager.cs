using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    GameObject Enemy;
    GameObject Player;
    GameObject SpawnedEnemy;

    [SerializeField]
    float SpawnSpan = 5.0f;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player");
        InvokeRepeating("EnemySpawn", 0.0f, SpawnSpan);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void EnemySpawn()
    {
        Vector3 PlayerPos = Player.transform.position;
        SpawnedEnemy = Instantiate(Enemy, new Vector3(Random.Range(-10.0f,10.0f), 0, Random.Range(-10.0f, 10.0f)), Quaternion.identity);
        Animator SpawnedEnemyAnim = GameObjectManager.getAnimator(SpawnedEnemy);
        SpawnedEnemy.transform.LookAt(Player.transform.position);
        SpawnedEnemyAnim.Play("Locomotion.Born");
    }




}

