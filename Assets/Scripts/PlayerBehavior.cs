/// <summary>
/// 
/// </summary>

using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]

//Name of class must be name of file as well

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
    GameObject SoundManager;
    SEManager SE;
    BGMManager BGM;

    float DefaltTimeScale = 1.0f;

    static public float StopSec;


    VideoPlayer video;

    bool IsIdle;

    // Use this for initialization
    void Start()
    {
        IsIdle = false;
        video = GetComponent<VideoPlayer>();
        StopSec = 0;
        CameraObject = GameObject.Find("MainCamera");
        CameraObject.GetComponent<PostEffect>().enabled = false;
        IsSkillEnable = false;
        //  Set Frame Rate
        Application.targetFrameRate = 30;
        Time.timeScale = DefaltTimeScale;
        playerBody = GetComponent<BoxCollider>();
        Animator = GetComponent<Animator>();
        state = Animator.GetCurrentAnimatorStateInfo(0);
        //capcol = GetComponentInChildren<CapsuleCollider>();
        boxCol.enabled = false;
        locomotion = new Locomotion(Animator);
        GameObject SoundManager = GameObject.Find("SoundManager");
        SE = SoundManager.GetComponentInChildren<SEManager>();
        IsCrisis = false;

        var smObserver = Animator.GetBehaviour<StateMachineObserver>();
        smObserver.onStateExit = onStateExit;
    }

    void Update()
    {
        
        if (!KilledNum.IsStarted)
        {
            locomotion.Do(0, direction * 180);
            return;
        }
        if (Animator && Camera.main && !Animator.GetBool("Death"))
        {
            if (IsCrisis && !MakeAura)
            {
                IsCrisis = false;
                MakeAura = Instantiate(CrisisAura, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), Quaternion.identity);
                Invoke("DestroyAura", 1.0f);
                SE.SEStart(8);
            }

            JoystickToEvents.Do(transform, Camera.main.transform, ref speed, ref direction);
            locomotion.Do(speed * 6.0f, direction * 180);

            if (IsIdle&&!IsSkillEnable)
            {
               SkillManager.SkillPoint += 0.05f;
            }



                if (Input.GetButtonDown("Skill") )
            {
                if (!IsSkillEnable&& SkillManager.SkillPoint >= 5)
                {
                    video.Play();
                    Animator.speed = 2;
                    StopSec = 0;
                    Debug.Log("Skill");
                    Time.timeScale = 0.5f;
                    IsSkillEnable = true;
                    CameraObject.GetComponent<PostEffect>().enabled = true;
                }
               else if(IsSkillEnable)
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

            if ((Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Guard")) && !Animator.GetBool("Attacking"))
            {
                Animator.SetBool("Guard", true);
            }

            if (Input.GetKeyUp(KeyCode.B) || Input.GetButtonUp("Guard"))
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





            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Attack"))
            {
                if (Animator.GetBool("Attacking")) Animator.SetBool("ComboAttack", true);
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
        //animator.SetBool("Attacking", false);
        SE.SEStart(4);
        Invoke("DownSound", 0.6f);
        Animator.SetBool("Death", true);
        Animator.Play("Death");
        // Debug.Log("Player Killed");
        GameObject blood = Instantiate(bloodParticle, transform.position + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
        KatanaCollider(false);
        Destroy(blood, 0.5f);
        StartCoroutine(FadeDisp.FadeOut());
    }

    IEnumerator StartAttack()
    {
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
            yield return new WaitForSeconds(0.3f);
            KatanaCollider(true);
            AttackSound();
            yield return new WaitForSeconds(0.1f);
            KatanaCollider(false);
            yield return new WaitForSeconds(0.4f);
            Animator.SetBool("ComboAttack", false);
            Animator.SetBool("Attacking", false);
            yield break;
        }
        else
        {
            Animator.SetBool("ComboAttack", false);
            Animator.SetBool("Attacking", false);
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

    void onStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IsIdle = false;

        if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Attack"))
        {
            Debug.Log("Attack Start");
            StartCoroutine(StartAttack());
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("ComboAttack"))
        {
            Debug.Log("Combo Attack Start");
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Idle"))
        {
            Debug.Log("Idle Start");
            Animator.SetBool("Attacking", false);
            boxCol.enabled = false;
            IsIdle = true;
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Death"))
        {
            Debug.Log("Death Start");
        }
        else if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Guard"))
        {
            Debug.Log("Guard Start");
            SE.SEStart(1);
            boxCol.enabled = true;
            //Debug.Log(boxCol.enabled);
            boxCol.size = new Vector3(1.0f, 0.13f, 1.3f);
        }
    }
}
