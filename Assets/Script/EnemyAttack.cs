using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    GameObject Player;

    GameObject SoundManager;
    SEManager SE;
    bool oneHit;

    [SerializeField]
    GameObject Enemy;

    [SerializeField]
    GameObject WeaponSpark;
    GameObject SparkParticle;
    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player");
        SoundManager = GameObject.Find("SoundManager");
        SE = SoundManager.GetComponentInChildren<SEManager>();

        oneHit = false;
    }


    void OnTriggerEnter(Collider col)
    {
        bool IsDeath_Player = GameObjectManager.getAnimator(Player).GetBool("Death");
        bool IsGuard_Player = GameObjectManager.getAnimator(Player).GetBool("Guard");
        bool IsCounterAttacking = GameObjectManager.getAnimatorStateInfo(Player).IsName("CounterAttack");

        if (!IsDeath_Player)
        {



            if (!IsGuard_Player && !IsCounterAttacking && col.tag == "Hero")
            {
                Debug.Log("プレイヤーに当たった");
                col.GetComponent<PlayerBehavior>().EnemyKatanaHit();
                CameraMove.ShakeCamera();
            }
            else if (IsGuard_Player && col.tag == "Hero")
            {
                SE.SEStart(3);
                GameObjectManager.getAnimator(Player).SetTrigger("KnockBack");
                CameraMove.ShakeCamera();
                SparkParticle = Instantiate(WeaponSpark);
                SparkParticle.transform.position = transform.position;
                Invoke("KillSpark", 0.2f);
            }




        }

       
    }

    void KillSpark()
    {
        Destroy(SparkParticle.gameObject);
    }

}
