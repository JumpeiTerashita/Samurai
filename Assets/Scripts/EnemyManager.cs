using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵生成オブジェクト　ランダム座標
/// </summary>
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
        InvokeRepeating("EnemySpawn", 3.0f, SpawnSpan);
    }

    

    void EnemySpawn()
    {    
        Vector3 PlayerPos = Player.transform.position;
        SpawnedEnemy = Instantiate(Enemy, new Vector3(Random.Range(-10.0f,10.0f), 0, Random.Range(-10.0f, 10.0f)), Quaternion.identity);
        Animator SpawnedEnemyAnim = GameObjectManager.getAnimator(SpawnedEnemy);
        SpawnedEnemy.transform.LookAt(Player.transform.position);
        SpawnedEnemyAnim.Play("Locomotion.Born");
    }
    
    public void SetEnemySpawn(Vector3 _SetPos,float _RandomRange)
    {
        Vector3 PlayerPos = Player.transform.position;
        SpawnedEnemy = Instantiate(Enemy, new Vector3(_SetPos.x+Random.Range(-_RandomRange, _RandomRange), 0, _SetPos.z+Random.Range(-_RandomRange, _RandomRange)), Quaternion.identity);
        Animator SpawnedEnemyAnim = GameObjectManager.getAnimator(SpawnedEnemy);
        SpawnedEnemy.transform.LookAt(Player.transform.position);
        SpawnedEnemyAnim.Play("Locomotion.Born");
    }


}

