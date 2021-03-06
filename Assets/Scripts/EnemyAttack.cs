﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の攻撃モーション中
/// 衝突判定・ノックバック
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    GameObject Player;

    GameObject SoundManager;
    SEManager SE;

    [SerializeField]
    GameObject Enemy;

    Animator EnemyAnim;

    float HitStopTime;
    
    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player");
        SoundManager = GameObject.Find("SoundManager");
        SE = SoundManager.GetComponentInChildren<SEManager>();
        EnemyAnim = GameObjectManager.getAnimator(Enemy);
        HitStopTime = MotionManager.Instance.GetComponent<MotionManager>().HitStopTime;
    }


    void OnTriggerEnter(Collider col)
    {


        if (!EnemyAnim.GetBool("Attack")) {  return; }

        //  Playerのアニメーションステート取得
        //  TODO    StateMachineObserverを使った処理に出来ないか？

        bool IsDeath_Player = GameObjectManager.getAnimator(Player).GetBool("Death");
        bool IsGuard_Player = GameObjectManager.getAnimator(Player).GetBool("Guard");
        bool IsCounterAttacking = GameObjectManager.getAnimatorStateInfo(Player).IsName("CounterAttack");
        bool IsPlayerRolling = PlayerBehavior.IsRolling;
        if (!IsDeath_Player&&!IsPlayerRolling)
        {
            if (!IsGuard_Player && !IsCounterAttacking && col.tag == "Hero")
            {
               Debug.Log("プレイヤーに当たった");
                if (Enemy.GetComponent<EnemyBehavior>() != null) StartCoroutine(Enemy.GetComponent<EnemyBehavior>().AttackHitStop(HitStopTime));
                else StartCoroutine(Enemy.GetComponent<BossBehavior>().AttackHitStop(HitStopTime));
                col.GetComponent<PlayerBehavior>().Damged();
                CameraMove.ShakeCamera();
            }
            else if (IsGuard_Player && col.tag == "Hero")
            {
                SE.SEStart(3);
                if (Enemy.GetComponent<EnemyBehavior>() != null) StartCoroutine(Enemy.GetComponent<EnemyBehavior>().AttackHitStop(HitStopTime));
                else StartCoroutine(Enemy.GetComponent<BossBehavior>().AttackHitStop(HitStopTime));
                GameObjectManager.getAnimator(Player).SetTrigger("KnockBack");
                CameraMove.ShakeCamera();
                col.GetComponent<PlayerBehavior>().MakeWeaponSpark(transform.position);
                
            }
        }
       
    }

    

}
