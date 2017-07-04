using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

namespace Enemy
{
    [System.Serializable]
    public class EnemySetting
    {
        public int AIPattern;
        public float StayLimit;
        public float AttackLength;
        public float SenseLength;
        public float ArriveLength;
        public float MoveSpeed;
        public float MoveSpeed_High;

        public EnemySetting()
        {
            AIPattern = 0;
            StayLimit = 3;
            AttackLength = 1.5f;
            SenseLength = 10.0f;
            ArriveLength = 1.0f;
            MoveSpeed = 1.0f;
            MoveSpeed_High = 3.0f;
        }
    }
}

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

    [SerializeField] EnemySetting NowSetting;

    // Use this for initialization
    void Start()
    {
        SpawnPoint = transform.position;
        SpawnPoint.y = 0;
        Player = GameObject.Find("Player");
        if (NowSetting == null)
        {
            NowSetting = new EnemySetting();
        }

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
            SpawnPoint = transform.position;
            SpawnPoint.y = 0;
            GameObject SpawnedEnemy = Instantiate(Enemy, SpawnPoint, Quaternion.identity);
            SetEnemySetting(SpawnedEnemy);
            SpawnedEnemy.transform.LookAt(Player.transform.position);
            SpawnedEnemy.GetComponent<DestinationManager>().SetDestination(Player.transform.position);
            Animator SpawnedEnemyAnim = GameObjectManager.getAnimator(SpawnedEnemy);
            SpawnedEnemyAnim.Play("Locomotion.Born");
        }

        yield return new WaitForSeconds(SpawnSpan);


        isRunning = false;
        StartCoroutine(SpawnLoop());
    }

    void SetEnemySetting(GameObject _Target)
    {
        var TargetBehavior = _Target.GetComponent<EnemyBehavior>();
        TargetBehavior.SetSettings(NowSetting);
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Enemy") isAlreadySpawn = true;
    }

    public void SetSpawnSpan(float _sec)
    {
        SpawnSpan = _sec;
    }

}
