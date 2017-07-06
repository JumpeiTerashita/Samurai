using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MotionValues;

[RequireComponent(typeof(Animator))]


/// <summary>
/// ボス   挙動管理
/// </summary>
public class BossBehavior : MonoBehaviour
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
    BoxCollider boxCol2;

    [SerializeField]
    GameObject bloodParticle;

    [SerializeField]
    Canvas ClearUI;

    bool IsDead;
    bool IsArrived;

    bool CanMove;

    SEManager SE;

    Vector3 destination;

    float StayTime;

    [SerializeField]
    float MoveSpeed = 1.0f;

    [SerializeField]
    float SenseLength = 10.0f;

    [SerializeField]
    float AttackLength = 3.0f;

    [SerializeField]
    float StayLimit;

    [SerializeField]
    int Life = 6;

    MotionTiming Attack1Timings;
    MotionTiming Attack2Timings;
    MotionTiming DamageReactTimings;

    bool IsAttackCoroutineRunning;
    bool IsPunchCoroutineRunning;
    bool IsHitStiopEnabled;
    // Use this for initialization
    void Start()
    {
        IsHitStiopEnabled = false;
        Player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        IsDead = false;
        IsArrived = false;
        locomotion = new Locomotion(animator);
        state = animator.GetCurrentAnimatorStateInfo(0);
        boxCol.enabled = false;
        boxCol2.enabled = false;
        GameObject SoundManager = GameObject.Find("SoundManager");
        SE = SoundManager.GetComponentInChildren<SEManager>();
        CanMove = false;
        CharaCon = GetComponent<CharacterController>();
        GetComponent<DestinationManager>().SetDestination(Player.transform.position);
        destination = GetComponent<DestinationManager>().GetDestination();
        var smObserver = animator.GetBehaviour<StateMachineObserver>();
        smObserver.onStateExit = onStateExit;
        StayTime = 0;

        Attack1Timings = MotionManager.Instance.GetComponent<MotionManager>().GetMotionValue("BossAttack");
        Attack2Timings = MotionManager.Instance.GetComponent<MotionManager>().GetMotionValue("BossAttack2");
        DamageReactTimings = MotionManager.Instance.GetComponent<MotionManager>().GetMotionValue("BossDamageReact");
        IsAttackCoroutineRunning = false;
        IsPunchCoroutineRunning = false;
    }

    void Update()
    {
        state = animator.GetCurrentAnimatorStateInfo(0);
        IsDead = state.IsName("Locomotion.Death");

        if (!CanMove || IsDead || !KilledNum.IsStarted) return;

        if (animator)
        {
            bool IsBorning = state.IsName("Locomotion.Born");
            if (IsBorning) return;

            if (!JudgeNowAttacking())
            {
                AttackCollider(false);
                animator.SetBool("Attack", false);
                SetSpeed(0);
            }

            Vector3 DistanceToPlayer = Player.transform.position - transform.position;
           
            if (DistanceToPlayer.magnitude <= AttackLength)
            {
                if (JudgeNowAttacking()) return;
                if (animator.GetBool("Damaging")) return;
                IsArrived = true;

                //  攻撃開始
                PlayerBehavior.IsCrisis = true;
                transform.DOLookAt(Player.transform.position, 0.5f);
                transform.Rotate(-transform.eulerAngles.x, 0, -transform.eulerAngles.z);
                if (Random.Range(0.0f, 1.0f) >= 0.5f) animator.Play("Attack");
                else animator.Play("Punch");
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
            SetSpeed(MoveSpeed*2.0f);
            destination = Player.transform.position;
        }
        transform.DOLookAt(destination, 1.0f);
        Vector3 velocity = DistanceToDestination.normalized * Time.deltaTime * 3f * animator.GetFloat("Speed");
        velocity.y += Physics.gravity.y * Time.deltaTime;
        //Debug.Log(velocity);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            CharaCon.Move(velocity);
        }
        //transform.position = new Vector3(transform.position.x+velocity.x,transform.position.y,transform.position.z+velocity.z);
        transform.Rotate(-transform.eulerAngles.x, 0, -transform.eulerAngles.z);
        if (Vector3.Distance(destination, transform.position) < 0.5f)
        {
            IsArrived = true;
            SetSpeed(0);
        }
    }

    void Stay()
    {
        //  一定時間(StayLimit)経過後、目的地変更
        StayTime += Time.deltaTime;
        if (StayLimit <= StayTime)
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
        if (state.IsName("Locomotion.Attack") || state.IsName("Locomotion.Attack2") || state.IsName("Locomotion.Punch") || state.IsName("Locomotion.NextPunch")) return true;
        if (animator.GetBool("Attack")) return true;
        return false;
    }

    void SetSpeed(float _Speed)
    {
        animator.SetFloat("Speed", _Speed);
    }

    void AttackSound()
    {
        SE.SEStart(15);
    }

    void AttackCollider(bool _Is)
    {
        boxCol.enabled = _Is;
        boxCol2.enabled = _Is;
    }

    IEnumerator StartAttack()
    {
        if (IsAttackCoroutineRunning) yield break;
        IsAttackCoroutineRunning = true;
        yield return new WaitForSeconds(Attack1Timings.MotionPeriods[0]);
        transform.DOLookAt(Player.transform.position, 0.2f);
        yield return new WaitForSeconds(Attack1Timings.MotionPeriods[1]);
        AttackCollider(true);
        yield return new WaitForSeconds(Attack1Timings.MotionPeriods[2]);
        AttackSound();
        yield return new WaitForSeconds(Attack1Timings.MotionPeriods[3]);
        AttackCollider(false);
        IsAttackCoroutineRunning = false;
        yield break;
    }

    IEnumerator StartPunch()
    {
        if (IsPunchCoroutineRunning) yield break;
        IsPunchCoroutineRunning = true;
        yield return new WaitForSeconds(Attack2Timings.MotionPeriods[0]);
        boxCol2.enabled = true;
        yield return new WaitForSeconds(Attack2Timings.MotionPeriods[1]);
        AttackSound();
        yield return new WaitForSeconds(Attack2Timings.MotionPeriods[2]);
        boxCol2.enabled = false;
        transform.DOLookAt(Player.transform.position, 1.0f);
        yield return new WaitForSeconds(Attack2Timings.MotionPeriods[3]);
        boxCol.enabled = true;
        yield return new WaitForSeconds(Attack2Timings.MotionPeriods[4]);
        AttackSound();
        yield return new WaitForSeconds(Attack2Timings.MotionPeriods[5]);
        AttackCollider(false);
        IsPunchCoroutineRunning = false;
        yield break;
    }

    public IEnumerator AttackHitStop(float time)
    {
        if (IsHitStiopEnabled) yield break;
        IsHitStiopEnabled = true;
        float DefaultSpeed = animator.speed;
        animator.speed = 0.1f;
        yield return new WaitForSeconds(time);
        animator.speed = DefaultSpeed;
        IsHitStiopEnabled = false;
        yield break;
    }

    IEnumerator Damaging()
    {
        yield return new WaitForSeconds(DamageReactTimings.MotionPeriods[0]);
        CharaCon.enabled = false;
        yield return new WaitForSeconds(DamageReactTimings.MotionPeriods[1]);
        animator.SetBool("Damaging", false);
        CharaCon.enabled = true;
        yield break;
    }

    void Damage()
    {
        if (!animator.GetBool("Damaging"))
        {
            animator.SetBool("Damaging", true);
            animator.Play("Damaged");
            GameObject blood = Instantiate(bloodParticle, transform.position + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
            Destroy(blood, 0.5f);
            Life--;
            Debug.Log("Boss Life Changed to : " + Life);
            if (Life <= 0) Death();
        }
    }

    void Death()
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
            KilledNum.KillCounter++;
            SkillManager.SkillPoint += 0.5f;
            GameObject blood = Instantiate(bloodParticle, transform.position + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
            Destroy(blood, 0.5f);
            Canvas DispUI = Instantiate(ClearUI);
            DispUI.GetComponentInChildren<Text>().text = "生還";
            KilledNum.IsStarted = false;
            Time.timeScale = 1.0f;
            StartCoroutine(FadeDisp.FadeOutToTitle());
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
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Punch"))
        {
            AttackCollider(false);
            StartCoroutine(StartPunch());
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Damaged"))
        {
            //   Debug.Log("Enemy : Death Start");
            animator.SetBool("Attack", false);
            AttackCollider(false);
            StartCoroutine(Damaging());
            if (IsAttackCoroutineRunning)
            {
                IsAttackCoroutineRunning = false;
                StopCoroutine(StartAttack());
            }
            if (IsPunchCoroutineRunning)
            {
                IsPunchCoroutineRunning = false;
                StopCoroutine(StartPunch());
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Idle"))
        {
            CanMove = true;
            //  Debug.Log("Enemy : Idle Start");
            animator.SetBool("Attack", false);
            AttackCollider(false);
            if (IsAttackCoroutineRunning)
            {
                IsAttackCoroutineRunning = false;
                StopCoroutine(StartAttack());
            }
            if (IsPunchCoroutineRunning)
            {
                IsPunchCoroutineRunning = false;
                StopCoroutine(StartPunch());
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Death"))
        {
            //   Debug.Log("Enemy : Death Start");
            animator.SetBool("Attack", false);
            AttackCollider(false);
        }

    }



    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Katana")
        {
            bool IsGuard_Player = GameObjectManager.getAnimator(Player).GetBool("Guard");
            if (IsGuard_Player) return;
            StartCoroutine(AttackHitStop(0.1f));
            Damage();
        }
    }
}


