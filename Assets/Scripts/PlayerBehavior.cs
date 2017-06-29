

using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

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
    GameObject CrisisAura;
    GameObject MakeAura = null;

    AnimatorStateInfo state;

    BoxCollider playerBody;

    [SerializeField]
    BoxCollider boxCol;

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
    bool IsAttaceCorutineRunning;
    static public bool IsRolling;

    // Use this for initialization
    void Start()
    {
        IsAttaceCorutineRunning = false;
        IsIdle = false;
        video = GetComponent<VideoPlayer>();
        StopSec = 0;
        CameraObject = GameObject.Find("MainCamera");
        CameraObject.GetComponent<PostEffect>().enabled = false;
        IsSkillEnable = false;
        Application.targetFrameRate = 30;
        Time.timeScale = DefaltTimeScale;
        playerBody = GetComponent<BoxCollider>();
        Animator = GetComponent<Animator>();
        state = Animator.GetCurrentAnimatorStateInfo(0);
        boxCol.enabled = false;
        locomotion = new Locomotion(Animator);
        SE = SEManager.Instance.GetComponent<SEManager>();
        IsCrisis = false;
        IsRolling = false;
        var smObserver = Animator.GetBehaviour<StateMachineObserver>();
        smObserver.onStateExit = onStateExit;
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
            if (IsCrisis && !MakeAura)
            {
                IsCrisis = false;
                MakeAura = Instantiate(CrisisAura, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), Quaternion.identity);
                Invoke("DestroyAura", 1.0f);
                SE.SEStart(8);  //  TODO    SEStart マジックナンバーでのindex指定   分かりづらい
            }

            JoystickToEvents.Do(transform, Camera.main.transform, ref speed, ref direction);
            locomotion.Do(speed * 6.0f, direction * 180);

            if (IsRolling) return;

            if (IsIdle && !IsSkillEnable)
            {
                SkillManager.SkillPoint += 0.05f;
            }



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

            if (Input.GetButtonDown("Rolling") && !IsRolling)
            {
                KatanaCollider(false);
                IsRolling = true;
                Animator.SetTrigger("Rolling");
                SE.SEStart(12);
            }

            if (Input.GetButtonDown("Guard") && !Animator.GetBool("Attacking"))
            {
                Animator.SetBool("Guard", true);
            }

            if ( Input.GetButtonUp("Guard"))
            {
                // Debug.Log("GuardFinished");
                boxCol.enabled = false;
                boxCol.size = new Vector3(0.3f, 0.13f, 1.3f);
                Animator.SetBool("Guard", false);
                state = Animator.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("Locomotion.KnockBack"))
                {
                    Animator.SetTrigger("CounterAttack");
                    Instantiate(Flash, boxCol.transform.position, Quaternion.identity);
                    SE.SEStart(2);
                    StartCoroutine(StartCounterAttack());

                }
            }





            if (Input.GetButtonDown("Attack"))
            {
                if (Animator.GetBool("Attacking")&&IsAttaceCorutineRunning) Animator.SetBool("ComboAttack", true);
                else Animator.Play("Attack");
            }

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

    void PlayerInvicible(bool _Is)
    {
        playerBody.enabled = !_Is;
    }

    void KatanaCollider(bool _Is)
    {
        boxCol.enabled = _Is;
    }



    void AttackSound()
    {
        SE.SEStart(0);
    }

    void DownSound()
    {
        SE.SEStart(5);
    }

    void DestroyAura()
    {
        Destroy(MakeAura.gameObject);
    }

    public void MakeWeaponSpark(Vector3 _MakePosition)
    {
        Instantiate(WeaponSpark, _MakePosition, Quaternion.identity);
    }


    public void EnemyKatanaHit()
    {
        SE.SEStart(6);
        Animator.SetFloat("Speed", 0.0f);
        // Debug.Log("Hit");
        SE.SEStart(4);
        Invoke("DownSound", 0.6f);
        Animator.SetBool("Death", true);
        Animator.Play("Death");
        GameObject blood = Instantiate(bloodParticle, transform.position + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
        KatanaCollider(false);
        Destroy(blood, 0.5f);
        StartCoroutine(FadeDisp.FadeOutToTitle());
    }


    //  各モーション  コルーチン処理

    IEnumerator StartAttack()
    {
        if (IsAttaceCorutineRunning) yield break;
        IsAttaceCorutineRunning = true;
        Animator.SetBool("Attacking", true);
        yield return new WaitForSeconds(0.2f);
        KatanaCollider(true);
        yield return new WaitForSeconds(0.3f);
        AttackSound();
        yield return new WaitForSeconds(0.3f);
        AttackSound();
        yield return new WaitForSeconds(0.2f);
        KatanaCollider(false);

        if (Animator.GetBool("ComboAttack"))
        {
            Animator.SetBool("ComboAttack", false);
            yield return new WaitForSeconds(0.3f);
            KatanaCollider(true);
            AttackSound();
            yield return new WaitForSeconds(0.1f);
            KatanaCollider(false);
            yield return new WaitForSeconds(0.4f);
            Animator.SetBool("Attacking", false);
            IsAttaceCorutineRunning = false;
            yield break;
        }
        else
        {
            Animator.SetBool("ComboAttack", false);
            Animator.SetBool("Attacking", false);
            IsAttaceCorutineRunning = false;
            yield break;
        }

    }

    IEnumerator StartCounterAttack()
    {
        PlayerInvicible(true);
        yield return new WaitForSeconds(0.3f);
        KatanaCollider(true);
        yield return new WaitForSeconds(0.35f);
        AttackSound();
        yield return new WaitForSeconds(0.22f);
        AttackSound();
        yield return new WaitForSeconds(0.18f);
        KatanaCollider(false);
        PlayerInvicible(false);
        yield return new WaitForSeconds(0.35f);
        yield break;
    }

    bool IsAnimFinished()
    {
        var AnimState = Animator.GetCurrentAnimatorStateInfo(1);
        if (AnimState.normalizedTime <= 1.0f) return false;
        return true;
    }

    /// <summary>
    /// アニメーション終了時動作
    /// </summary>
    void onStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IsIdle = false;

        if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Attack"))
        {
            Debug.Log("Attack Start");
            StartCoroutine(StartAttack());
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Rolling"))
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * 1000, ForceMode.Force);
            // Animator.SetFloat("Speed", 100.0f);
            KatanaCollider(false);
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
            boxCol.enabled = false;
            IsRolling = false;
            IsIdle = true;
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Death"))
        {
            Debug.Log("Death Start");
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("SitUp"))
        {
            transform.position = transform.position - transform.forward*3f;
         
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Guard"))
        {
            Debug.Log("Guard Start");
            SE.SEStart(1);
            boxCol.enabled = true;
            boxCol.size = new Vector3(1.0f, 0.13f, 1.3f);
        }
    }
}
