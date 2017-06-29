using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//using DG.Tweening;

[RequireComponent(typeof(Animator))]

//Name of class must be name of file as well

/// <summary>
/// 敵   挙動管理
/// </summary>
public class EnemyBehavior : MonoBehaviour
{

    protected Animator animator;

    GameObject Player;
    GameObject Enemy;
    float speed = 2.0f;
    //private float direction = 0;
    Locomotion locomotion = null;
    AnimatorStateInfo state;

    CharacterController CharaCon;

    [SerializeField]
    BoxCollider boxCol;

    [SerializeField]
    GameObject bloodParticle;

    bool IsDead;
    bool IsArrived;

    SEManager SE;

    Vector3 destination;

    float StayTime;
    [SerializeField]
    float StayLimit = 3;

    [SerializeField]
    int AIpattern = 0;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        IsDead = false;
        IsArrived = false;
        locomotion = new Locomotion(animator);
        state = animator.GetCurrentAnimatorStateInfo(0);
        boxCol.enabled = false;
        GameObject SoundManager = GameObject.Find("SoundManager");
        SE = SoundManager.GetComponentInChildren<SEManager>();
        CharaCon = GetComponent<CharacterController>();
        destination = GetComponent<DestinationManager>().GetDestination();
        var smObserver = animator.GetBehaviour<StateMachineObserver>();
        smObserver.onStateExit = onStateExit;
        StayTime = 0;
        //animator.Play("Locomotion.Born");
    }

    void Update()
    {
     
        Vector3 DistanceToPlayer = Player.transform.position - transform.position;
        Vector3 DistanceToDestination = destination - transform.position;


        if (IsDead || !KilledNum.IsStarted) return;

        if (animator)
        {
            state = animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName("Locomotion.Death") && !state.IsName("Locomotion.Born"))
            {
                if (!state.IsName("Locomotion.Attack") && !state.IsName("Locomotion.Attack2"))
                {
                    boxCol.enabled = false;
                    animator.SetBool("Attack", false);
                    animator.SetFloat("Speed", 0.0f);
                }

                //  TODO    Enemy   行動パターン追加（巡回）
                //  TODO    Enemy   Player感知範囲指定    非マジックナンバー化
               


                if (DistanceToPlayer.magnitude <= 1.5f)
                {
                    if (!animator.GetBool("Attack"))
                    {
                        PlayerBehavior.IsCrisis = true;
                        // Debug.Log("Pinch!");
                        transform.LookAt(Player.transform.position);
                        transform.Rotate(-transform.eulerAngles.x, 0, -transform.eulerAngles.z);
                        if (Random.Range(0.0f, 1.0f) >= 0.5f)
                        {
                            animator.Play("Attack");
                            animator.SetBool("Attack", true);
                            animator.SetFloat("Speed", 0.0f);

                        }
                        else
                        {
                            animator.Play("Attack2");
                            animator.SetBool("Attack", true);
                            animator.SetFloat("Speed", 0);

                        }

                    }

                }
                else
                {


                    //  Enemy 移動
                    if (!animator.GetBool("Attack") && !IsArrived)
                    {
                        animator.SetFloat("Speed", 1.0f);
                        animator.SetBool("Attack", false);
                        if (DistanceToPlayer.magnitude <= 10.0f)
                        {
                            animator.SetFloat("Speed", 2.0f);

                            if (AIpattern == 1) animator.SetFloat("Speed", 3.0f);

                            destination = Player.transform.position;
                        }
                        else if (AIpattern == 1)
                        {
                            animator.SetFloat("Speed", 0.0f);
                            IsArrived = true;
                            StayLimit = 1;
                            return;
                        }

                        transform.DOLookAt(destination, 1.0f);
                        Vector3 velocity = DistanceToDestination.normalized * Time.deltaTime * 1.5f *animator.GetFloat("Speed");
                        velocity.y += Physics.gravity.y * Time.deltaTime;
                        CharaCon.Move(velocity);
                        transform.Rotate(-transform.eulerAngles.x, 0, -transform.eulerAngles.z);
                        if (Vector3.Distance(destination, transform.position) < 1)
                        {
                            IsArrived = true;
                    
                            animator.SetFloat("Speed", 0.0f);
                        }
                    }
                    else if (IsArrived)
                    {
                        StayTime += Time.deltaTime;
                        if (StayLimit < StayTime)
                        {
                           
                            IsArrived = false;
                            GetComponent<DestinationManager>().SetDestination(Player.transform.position);
                            destination = GetComponent<DestinationManager>().GetDestination();
                            StayTime = 0;
                            return;
                        }
                    }

                }
            }
            else
            {
                boxCol.enabled = false;
            }
        }

    }

    void AttackSound()
    {
        SE.SEStart(0);
    }

    void KatanaCollider(bool _Is)
    {
        boxCol.enabled = _Is;
    }

    IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(0.35f);
        KatanaCollider(true);
        yield return new WaitForSeconds(0.02f);
        AttackSound();
        yield return new WaitForSeconds(0.73f);
        AttackSound();
        KatanaCollider(false);
        yield break;
    }

    IEnumerator StartAttack2()
    {
        yield return new WaitForSeconds(0.40f);
        KatanaCollider(true);
        yield return new WaitForSeconds(0.05f);
        AttackSound();
        yield return new WaitForSeconds(0.75f);
        AttackSound();
        yield return new WaitForSeconds(0.85f);
        AttackSound();
        KatanaCollider(false);
        yield break;
    }

    public void PlayerKatanaHit()
    {
        if (!animator.GetBool("Death") && !IsDead)
        {
            IsDead = true;
            KatanaCollider(false);
            animator.SetFloat("Speed", 0.0f);
            // Debug.Log("Hit");
            //animator.SetBool("Attacking", false);
            animator.SetBool("Death", true);
            animator.Play("Death");
            KilledNum.KillCounter++;
            SkillManager.SkillPoint += 0.5f;
            GameObject blood = Instantiate(bloodParticle, transform.position + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
            Destroy(blood, 0.5f);
            Destroy(this.gameObject, 3.0f);
        }


    }

    void onStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Attack"))
        {
            // Debug.Log("Enemy : Attack Start");
            KatanaCollider(false);
            StartCoroutine(StartAttack());
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Attack2"))
        {
            //Debug.Log("Enemy : Attack2j Start");
            KatanaCollider(false);
            StartCoroutine(StartAttack2());
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Idle"))
        {
            //  Debug.Log("Enemy : Idle Start");
            animator.SetBool("Attack", false);
            KatanaCollider(false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Death"))
        {
            //   Debug.Log("Enemy : Death Start");
            animator.SetBool("Attack", false);
            KatanaCollider(false);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Vector3 Distance = transform.position - collision.transform.position;

            transform.position = new Vector3(transform.position.x + Distance.x * 0.5f, 0, transform.position.z + Distance.z * 0.5f);
        }
    }

}


