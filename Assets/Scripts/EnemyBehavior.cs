using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Enemy;
using MotionValues;

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
    //private float direction = 0;
    AnimatorStateInfo state;

    CharacterController CharaCon;

    [SerializeField]
    BoxCollider WeaponCollider;

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

    [SerializeField]
    float AttackLength = 1.5f;

    [SerializeField]
    float SenseLength = 10.0f;

    [SerializeField]
    float ArriveLength = 1.0f;

    [SerializeField]
    float MoveSpeed = 1.0f;
    [SerializeField]
    float MoveSpeed_High = 3.0f;

    MotionTiming Attack1Timings;
    MotionTiming Attack2Timings;
    bool IsHitStopEnabled;
    // Use this for initialization
    void Start()
    {
        IsHitStopEnabled = false;
        Player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        IsDead = false;
        IsArrived = false;
        state = animator.GetCurrentAnimatorStateInfo(0);
        WeaponCollider.enabled = false;
        GameObject SoundManager = GameObject.Find("SoundManager");
        SE = SoundManager.GetComponentInChildren<SEManager>();
        CharaCon = GetComponent<CharacterController>();
        destination = GetComponent<DestinationManager>().GetDestination();
        var smObserver = animator.GetBehaviour<StateMachineObserver>();
        smObserver.onStateExit = onStateExit;
        StayTime = 0;
        Attack1Timings = MotionManager.Instance.GetComponent<MotionManager>().GetMotionValue("EnemyAttack");
        Attack2Timings = MotionManager.Instance.GetComponent<MotionManager>().GetMotionValue("EnemyAttack2");
        
        //animator.Play("Locomotion.Born");
    }

    void Update()
    {
        state = animator.GetCurrentAnimatorStateInfo(0);
        IsDead = state.IsName("Locomotion.Death");

        if (IsDead || !KilledNum.IsStarted) return;

        if (animator)
        {

            transform.Rotate(-transform.eulerAngles.x, 0, -transform.eulerAngles.z);



            bool IsBorning = state.IsName("Locomotion.Born");
            if (IsBorning) return;

            if (!JudgeNowAttacking())
            {
                WeaponCollider.enabled = false;
                animator.SetBool("Attack", false);
                SetSpeed(0);
            }

            Vector3 DistanceToPlayer = Player.transform.position - transform.position;

            if (DistanceToPlayer.magnitude <= AttackLength)
            {
                if (JudgeNowAttacking()) return;
                IsArrived = true;
                StayTime = StayLimit;
                //  攻撃開始
                PlayerBehavior.IsCrisis = true;
                transform.LookAt(Player.transform.position);
                transform.Rotate(-transform.eulerAngles.x, 0, -transform.eulerAngles.z);
                if (Random.Range(0.0f, 1.0f) >= 0.5f) animator.Play("Attack");
                else animator.Play("Attack2");
                animator.SetBool("Attack", true);
                SetSpeed(0);
            }
            else
            {
                //  攻撃範囲外
                if (!JudgeNowAttacking() && !IsArrived)
                {
                    Move();
                }
                else if (IsArrived) Stay();

            }
        }
    }

    void Move()
    {
        Vector3 DistanceToPlayer = Player.transform.position - transform.position;
        Vector3 DistanceToDestination = destination - transform.position;

        SetSpeed(MoveSpeed);
        animator.SetBool("Attack", false);
        AttackCollider(false);
        if (DistanceToPlayer.magnitude <= SenseLength)
        {
            SetSpeed(MoveSpeed * 2.0f);
            if (AIpattern == 1) SetSpeed(MoveSpeed_High);
            destination = Player.transform.position;
        }
        else if (AIpattern == 1)
        {
            SetSpeed(0);
            IsArrived = true;
            StayLimit = 1;
            return;
        }
        transform.DOLookAt(destination, 1.0f);
        Vector3 velocity = DistanceToDestination.normalized * Time.deltaTime * 1.5f * animator.GetFloat("Speed");
        velocity.y += Physics.gravity.y * Time.deltaTime;
        CharaCon.Move(velocity);
        transform.Rotate(-transform.eulerAngles.x, 0, -transform.eulerAngles.z);
        if (Vector3.Distance(destination, transform.position) < ArriveLength)
        {
            IsArrived = true;
            SetSpeed(0);
        }
    }

    void Stay()
    {
        //  一定時間(StayLimit)経過後、目的地変更
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

    bool JudgeNowAttacking()
    {
        state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Locomotion.Attack") || state.IsName("Locomotion.Attack2")) return true;
        if (animator.GetBool("Attack")) return true;
        return false;
    }

    void SetSpeed(float _Speed)
    {
        animator.SetFloat("Speed", _Speed);
    }

    void AttackSound()
    {
        SE.SEStart(0);
    }

    void AttackCollider(bool _Is)
    {
        WeaponCollider.enabled = _Is;
    }

    public void SetAiPattern(int _AiPattern)
    {
        AIpattern = _AiPattern;
    }

    public void SetSettings(Enemy.EnemySetting EnemySet)
    {
        StayLimit = EnemySet.StayLimit;
        AIpattern = EnemySet.AIPattern;
        AttackLength = EnemySet.ArriveLength;
        SenseLength = EnemySet.SenseLength;
        ArriveLength = EnemySet.ArriveLength;
        MoveSpeed = EnemySet.MoveSpeed;
        MoveSpeed_High = EnemySet.MoveSpeed_High;
    }

    IEnumerator StartAttack()
    {
        yield return new WaitForSeconds(Attack1Timings.MotionPeriods[0]);
        AttackCollider(true);
        yield return new WaitForSeconds(Attack1Timings.MotionPeriods[1]);
        AttackSound();
        yield return new WaitForSeconds(Attack1Timings.MotionPeriods[2]);
        AttackSound();
        AttackCollider(false);
        yield break;
    }

    IEnumerator StartAttack2()
    {
        yield return new WaitForSeconds(Attack2Timings.MotionPeriods[0]);
        AttackCollider(true);
        yield return new WaitForSeconds(Attack2Timings.MotionPeriods[1]);
        AttackSound();
        yield return new WaitForSeconds(Attack2Timings.MotionPeriods[2]);
        AttackSound();
        yield return new WaitForSeconds(Attack2Timings.MotionPeriods[3]);
        AttackSound();
        AttackCollider(false);
        yield break;
    }

    public IEnumerator AttackHitStop(float time)
    {
        if (IsHitStopEnabled) yield break;
        IsHitStopEnabled = true;
        float DefaultSpeed = animator.speed;
        animator.speed = 0.1f;
        yield return new WaitForSeconds(time);
        animator.speed = DefaultSpeed;
        IsHitStopEnabled = false;
        yield break;
    }

    public void Damaged()
    {
        if (!animator.GetBool("Death") && !IsDead)
        {
            
            IsDead = true;
            AttackCollider(false);
            animator.SetFloat("Speed", 0.0f);
            // Debug.Log("Hit");
            //animator.SetBool("Attacking", false);
            animator.SetBool("Death", true);
            animator.Play("Death");
            transform.LookAt(Player.transform.position);
            transform.Rotate(-transform.eulerAngles.x, 0, -transform.eulerAngles.z);
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
            AttackCollider(false);
            StartCoroutine(StartAttack());
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Attack2"))
        {
            //Debug.Log("Enemy : Attack2j Start");
            AttackCollider(false);
            StartCoroutine(StartAttack2());
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Idle"))
        {
            //  Debug.Log("Enemy : Idle Start");
            animator.SetBool("Attack", false);
            AttackCollider(false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Death"))
        {
            //   Debug.Log("Enemy : Death Start");
            animator.SetBool("Attack", false);
            AttackCollider(false);
            transform.Rotate(-transform.eulerAngles.x, 0, -transform.eulerAngles.z);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Enemy")
        //{
        //    Vector3 Distance = transform.position - collision.transform.position;

        //    transform.position = new Vector3(transform.position.x + Distance.x * 0.5f, 0, transform.position.z + Distance.z * 0.5f);
        //}
    }

}


