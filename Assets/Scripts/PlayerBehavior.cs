using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using MotionValues;

[RequireComponent(typeof(Animator))]

//  TODO    PlayerBehaviorクラス   非ゴッドクラス化（責任分散）

/// <summary>
/// Playerの挙動管理
/// </summary>
public class PlayerBehavior : MonoBehaviour
{
    protected Animator Animator;

    float speed = 0;
    float direction = 0;
    Locomotion locomotion = null;
    static public bool IsCrisis;
    static public bool IsSkillEnable;

    [SerializeField]
    float GainSkillPoint = 0.1f;

    [SerializeField]
    GameObject CrisisAura;
    GameObject MakeAura = null;

    AnimatorStateInfo state;

    BoxCollider PlayerBodyCollider;

    [SerializeField]
    BoxCollider WeaponCollider;

    [SerializeField]
    GameObject bloodParticle;

    [SerializeField]
    Canvas DeathUI;

    [SerializeField]
    GameObject Flash;

    [SerializeField]
    GameObject WeaponSpark;

    Canvas DispUI = null;

    GameObject CameraObject;

    SEManager SE;
    BGMManager BGM;

    float DefaltTimeScale = 1.0f;

    static public float StopSec;


    VideoPlayer video;

    bool IsIdle;
    bool IsAttackCoroutineRunning;
    bool IsCounterAttackCoroutineRunning;
    static public bool IsRolling;
    bool IsHitStopEnabled;

    MotionTiming AttackTimings;
    MotionTiming CounterAttackTimings;

    // Use this for initialization
    void Start()
    {
        IsHitStopEnabled = false;
        IsAttackCoroutineRunning = false;
        IsCounterAttackCoroutineRunning = false;
        IsIdle = false;
        video = GetComponent<VideoPlayer>();
        StopSec = 0;
        CameraObject = GameObject.Find("MainCamera");
        CameraObject.GetComponent<PostEffect>().enabled = false;
        IsSkillEnable = false;
        Application.targetFrameRate = 30;
        Time.timeScale = DefaltTimeScale;
        PlayerBodyCollider = GetComponent<BoxCollider>();
        Animator = GetComponent<Animator>();
        state = Animator.GetCurrentAnimatorStateInfo(0);
        WeaponCollider.enabled = false;
        locomotion = new Locomotion(Animator);
        SE = SEManager.Instance.GetComponent<SEManager>();
        IsCrisis = false;
        IsRolling = false;
        var smObserver = Animator.GetBehaviour<StateMachineObserver>();
        smObserver.onStateExit = onStateExit;

        AttackTimings = MotionManager.Instance.GetComponent<MotionManager>().GetMotionValue("PlayerAttack");
        CounterAttackTimings = MotionManager.Instance.GetComponent<MotionManager>().GetMotionValue("PlayerCounterAttack");
    }

    void Update()
    {

        if (!KilledNum.IsStarted)
        {
            Animator.SetFloat("Speed", 0.0f);
            locomotion.Do(0, 0);
            return;
        }
        if (Animator && Camera.main && !Animator.GetBool("Death"))
        {
            JoystickToEvents.Do(transform, Camera.main.transform, ref speed, ref direction);
            locomotion.Do(speed * 6.0f, direction * 180);

            if (IsCrisis && !MakeAura)
            {
                IsCrisis = false;
                MakeAura = Instantiate(CrisisAura, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), Quaternion.identity);
                SE.SEStart(8);  //  TODO    SEStart マジックナンバーでのindex指定   分かりづらい
            }

            if (IsRolling) return;

            if (IsIdle && !IsSkillEnable) SkillManager.SkillPoint += GainSkillPoint;
          

            if (Input.GetButtonDown("Rolling") && !IsRolling) Rolling();

            if (Input.GetButtonDown("Guard") && !Animator.GetBool("Attacking")) Guard();

            if (Input.GetButtonUp("Guard")) GuardFinished();
               
            Skill();

            if (Input.GetButtonDown("Attack")) Attack();

        }
        else
        {
            if (DispUI == null)
            {
                DispUI = DeathUI;
                Instantiate(DispUI);
                Time.timeScale = DefaltTimeScale;
                IsSkillEnable = false;
                CameraObject.GetComponent<PostEffect>().enabled = false;
            }
        }
    }

    void Rolling()
    {
        AttackCollider(false);
        IsRolling = true;
        Animator.SetTrigger("Rolling");
        SE.SEStart(12);
    }

    void Guard()
    {
        Animator.SetBool("Guard", true);
    }

    void GuardFinished()
    {
        AttackCollider(false);
        WeaponCollider.size = new Vector3(0.3f, 0.13f, 1.3f);
        Animator.SetBool("Guard", false);
        state = Animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Locomotion.KnockBack"))
        {
            Animator.SetTrigger("CounterAttack");
            Instantiate(Flash, WeaponCollider.transform.position, Quaternion.identity);
            SE.SEStart(2);
            StartCoroutine(StartCounterAttack());
        }
    }

    void Attack()
    {
        if (Animator.GetBool("Attacking") && IsAttackCoroutineRunning) Animator.SetBool("ComboAttack", true);
        else Animator.SetTrigger("Attack");
    }

    void Skill()
    {
        if (Input.GetButtonDown("Skill"))
        {
            if (!IsSkillEnable && SkillManager.SkillPoint >= 5)
            {
                SE.SEStart(9);
                video.Play();
                Animator.speed = 2;
                StopSec = 0;
                Debug.Log("Skill");
                Time.timeScale = 0.5f;
                IsSkillEnable = true;
                CameraObject.GetComponent<PostEffect>().enabled = true;
            }
            else if (IsSkillEnable)
            {
                Animator.speed = 1;
                StopSec = 0;
                Debug.Log("Skill Over");
                Time.timeScale = DefaltTimeScale;
                IsSkillEnable = false;
                CameraObject.GetComponent<PostEffect>().enabled = false;
            }
        }

        if (IsSkillEnable && SkillManager.SkillPoint > 0) StopSec += Time.deltaTime;

        if (IsSkillEnable && SkillManager.SkillPoint <= 0)
        {
            Animator.speed = 1;
            StopSec = 0;
            Debug.Log("Skill Over");
            Time.timeScale = DefaltTimeScale;
            IsSkillEnable = false;
            CameraObject.GetComponent<PostEffect>().enabled = false;
        }
    }

    void PlayerInvicible(bool _Is)
    {
        PlayerBodyCollider.enabled = !_Is;
    }

    void AttackCollider(bool _Is)
    {
        WeaponCollider.enabled = _Is;
    }



    void AttackSound()
    {
        SE.SEStart(0);
    }

    void DownSound()
    {
        SE.SEStart(5);
    }


    public void MakeWeaponSpark(Vector3 _MakePosition)
    {
        Instantiate(WeaponSpark, _MakePosition, Quaternion.identity);
    }


    public void Damged()
    {
        SE.SEStart(6);
        Animator.SetFloat("Speed", 0.0f);
        // Debug.Log("Hit");
        SE.SEStart(4);
        Invoke("DownSound", 0.6f);
        Animator.SetBool("Death", true);
        Animator.Play("Death");
        GameObject blood = Instantiate(bloodParticle, transform.position + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
        AttackCollider(false);
        Destroy(blood, 0.5f);
        StartCoroutine(FadeDisp.FadeOutToTitle());
    }

    public IEnumerator AttackHitStop(float time)
    {
        if (IsHitStopEnabled) yield break;
        IsHitStopEnabled = true;
        float DefaultSpeed = Animator.speed;
        Animator.speed = 0.1f;
        yield return new WaitForSeconds(time);
        Animator.speed = DefaultSpeed;
        IsHitStopEnabled = false;
        yield break;
    }

    //  各モーション  コルーチン処理

    IEnumerator StartAttack()
    {
       
        if (IsAttackCoroutineRunning) yield break;
        IsAttackCoroutineRunning = true;
        Animator.SetBool("Attacking", true);
        yield return new WaitForSeconds(AttackTimings.MotionPeriods[0]);
        AttackCollider(true);
        yield return new WaitForSeconds(AttackTimings.MotionPeriods[1]);
        AttackSound();
        yield return new WaitForSeconds(AttackTimings.MotionPeriods[2]);
        AttackSound();
        yield return new WaitForSeconds(AttackTimings.MotionPeriods[3]);
        AttackCollider(false);

        if (Animator.GetBool("ComboAttack"))
        {
            Animator.SetBool("ComboAttack", false);
            yield return new WaitForSeconds(AttackTimings.MotionPeriods[4]);
            AttackCollider(true);
            AttackSound();
            yield return new WaitForSeconds(AttackTimings.MotionPeriods[5]);
            AttackCollider(false);
            yield return new WaitForSeconds(AttackTimings.MotionPeriods[6]);

        }
        else Animator.SetBool("ComboAttack", false);

        Animator.SetBool("Attacking", false);
        IsAttackCoroutineRunning = false;
        yield break;

    }

    IEnumerator StartCounterAttack()
    {
        if (IsCounterAttackCoroutineRunning) yield break;
        IsCounterAttackCoroutineRunning = true;
        PlayerInvicible(true);
        yield return new WaitForSeconds(CounterAttackTimings.MotionPeriods[0]);
        AttackCollider(true);
        yield return new WaitForSeconds(CounterAttackTimings.MotionPeriods[1]);
        AttackSound();
        yield return new WaitForSeconds(CounterAttackTimings.MotionPeriods[2]);
        AttackSound();
        yield return new WaitForSeconds(CounterAttackTimings.MotionPeriods[3]);
        AttackCollider(false);
        PlayerInvicible(false);
        yield return new WaitForSeconds(CounterAttackTimings.MotionPeriods[4]);
        IsCounterAttackCoroutineRunning = false;
        yield break;
    }

    /// <summary>
    /// アニメーション終了時動作
    /// </summary>
    void onStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IsIdle = false;

        if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Attack"))
        {
            StartCoroutine(StartAttack());
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Rolling"))
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * 1000, ForceMode.Force);
            AttackCollider(false);
            Debug.Log("Rolling Start");
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("ComboAttack"))
        {
            Debug.Log("Combo Attack Start");
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Idle"))
        {
            Debug.Log("Idle Start");
            PlayerInvicible(false);
            Animator.SetBool("Attacking", false);
            WeaponCollider.enabled = false;
            IsRolling = false;
            IsIdle = true;
            if (IsAttackCoroutineRunning)
            {
                StopCoroutine(StartAttack());
                Animator.SetBool("ComboAttack", false);
                Animator.SetBool("Attacking", false);
                IsAttackCoroutineRunning = false;
            }

            if (IsCounterAttackCoroutineRunning)
            {
                StopCoroutine(StartCounterAttack());
                IsCounterAttackCoroutineRunning = false;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Death"))
        {
            Debug.Log("Death Start");
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("SitUp"))
        {
            transform.position = transform.position - transform.forward * 3f;

        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Guard"))
        {
            Debug.Log("Guard Start");
            SE.SEStart(1);
            WeaponCollider.enabled = true;
            WeaponCollider.size = new Vector3(1.0f, 0.13f, 1.3f);
        }
    }
}
