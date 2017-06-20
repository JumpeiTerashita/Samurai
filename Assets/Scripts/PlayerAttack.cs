using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Playerの攻撃モーション中
/// 衝突判定等
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    GameObject Player;
    GameObject SoundManager;

    TrailRenderer WeaponTrail;
    SEManager SE;
    Animator PlayerAnim;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player");
        PlayerAnim = GameObjectManager.getAnimator(Player);
        SoundManager = GameObject.Find("SoundManager");
        SE = SoundManager.GetComponentInChildren<SEManager>();
        WeaponTrail = GetComponentInChildren<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        WeaponTrail.enabled = PlayerAnim.GetBool("Attacking");
    }

    void OnTriggerEnter(Collider col)
    {
        bool IsGuard = GameObjectManager.getAnimator(Player).GetBool("Guard");

        if (col.tag != "Enemy") return;


        bool EnemyIsDead = GameObjectManager.getAnimator(col.gameObject).GetBool("Death");

        if (EnemyIsDead) return;

        if (!IsGuard && col.tag == "Enemy")
        {
          //  Debug.Log("敵に当たった");
            SE.SEStart(7);
            col.GetComponent<EnemyBehavior>().PlayerKatanaHit();
        }
    }
}
