/// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]

//Name of class must be name of file as well

public class PlayerBehavior : MonoBehaviour
{
    protected Animator animator;

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
    // Use this for initialization
    void Start()
    {
        CameraObject = GameObject.Find("MainCamera");
        CameraObject.GetComponent<PostEffect>().enabled = false;
        IsSkillEnable = false;
        //  Set Frame Rate
        Application.targetFrameRate = 30;
        Time.timeScale = DefaltTimeScale;
        playerBody = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        state = animator.GetCurrentAnimatorStateInfo(0);
        //capcol = GetComponentInChildren<CapsuleCollider>();
        boxCol.enabled = false;
        locomotion = new Locomotion(animator);
        GameObject SoundManager = GameObject.Find("SoundManager");
        SE = SoundManager.GetComponentInChildren<SEManager>();
        IsCrisis = false;
    }

    void Update()
    {
        if (!KilledNum.IsStarted)
        {
            locomotion.Do(0, direction * 180);
            return;
        }
        if (animator && Camera.main && !animator.GetBool("Death"))
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

            state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("Locomotion.Idle") && !animator.GetBool("Guard"))
            {
                animator.SetBool("Attacking", false);
                boxCol.enabled = false;
                // Debug.Log(boxCol.enabled);
            }

            if (Input.GetButtonDown("Skill"))
            {
                Debug.Log("Skill");
                Time.timeScale = 0.5f;
                IsSkillEnable = true;
                CameraObject.GetComponent<PostEffect>().enabled = true;
            }

            if (Input.GetButtonUp("Skill"))
            {
                Debug.Log("Skill Over");
                Time.timeScale = DefaltTimeScale;
                IsSkillEnable = false;
                CameraObject.GetComponent<PostEffect>().enabled = false;
            }

            if ((Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Guard")) && !animator.GetBool("Attacking"))
            {
                Debug.Log("NowGuard");
                SE.SEStart(1);
                boxCol.enabled = true;
                //Debug.Log(boxCol.enabled);
                boxCol.size = new Vector3(1.0f, 0.13f, 1.3f);
                animator.SetBool("Guard", true);
            }

            if (Input.GetKeyUp(KeyCode.B) || Input.GetButtonUp("Guard"))
            {
                // Debug.Log("GuardFinished");
                boxCol.enabled = false;
                boxCol.size = new Vector3(0.3f, 0.13f, 1.3f);
                animator.SetBool("Guard", false);

                if (state.IsName("Locomotion.KnockBack"))
                {
                    animator.SetTrigger("CounterAttack");
                    Instantiate(Flash, boxCol.transform.position, Quaternion.identity);
                    SE.SEStart(2);
                    StartCoroutine(StartCounterAttack());

                }
            }





            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Attack"))
            {
                if (state.IsName("Locomotion.Attack"))
                {
                    Debug.Log("ComboAttacking");

                    animator.SetBool("ComboAttack", true);
                    animator.SetBool("Attacking", true);
                    // Invoke("AttackSound", 0.7f);
                }
                else if (!state.IsName("Locomotion.ComboAttack"))
                {

                    StartCoroutine(StartAttack());
                }
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
        animator.SetFloat("Speed", 0.0f);
        Debug.Log("Hit");
        //animator.SetBool("Attacking", false);
        SE.SEStart(4);
        Invoke("DownSound", 0.6f);
        animator.SetBool("Death", true);
        animator.Play("Death");
        Debug.Log("Player Killed");
        GameObject blood = Instantiate(bloodParticle, transform.position + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
        KatanaCollider(false);
        Destroy(blood, 0.5f);
        StartCoroutine(FadeDisp.FadeOut());
    }

    IEnumerator StartAttack()
    {
        animator.SetBool("Attacking", true);
        animator.Play("Locomotion.Attack");
        yield return new WaitForSeconds(0.2f);
        KatanaCollider(true);
        yield return new WaitForSeconds(0.3f);
        AttackSound();
        yield return new WaitForSeconds(0.3f);
        AttackSound();
        yield return new WaitForSeconds(0.2f);
        KatanaCollider(false);

        if (animator.GetBool("ComboAttack"))
        {
            yield return new WaitForSeconds(0.3f);
            KatanaCollider(true);
            AttackSound();
            yield return new WaitForSeconds(0.1f);
            KatanaCollider(false);
            yield return new WaitForSeconds(0.4f);
            animator.SetBool("ComboAttack", false);
            animator.SetBool("Attacking", false);
            yield break;
        }
        else
        {
            animator.SetBool("ComboAttack", false);
            animator.SetBool("Attacking", false);
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
        var AnimState = animator.GetCurrentAnimatorStateInfo(1);
        if (AnimState.normalizedTime <= 1.0f) return false;
        return true;
    }

}
