/// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]

//Name of class must be name of file as well

public class LocomotionPlayer : MonoBehaviour
{
    protected Animator animator;

    float speed = 0;
    float direction = 0;
    Locomotion locomotion = null;

    AnimatorStateInfo state;

    [SerializeField]
    BoxCollider boxCol;

    [SerializeField]
    GameObject bloodParticle;

    [SerializeField]
    Canvas DeathUI;

    Canvas DispUI = null;

    GameObject SoundManager;
    SEManager SE;
    BGMManager BGM;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        state = animator.GetCurrentAnimatorStateInfo(0);
        //capcol = GetComponentInChildren<CapsuleCollider>();
        boxCol.enabled = false;
        locomotion = new Locomotion(animator);
        GameObject SoundManager = GameObject.Find("SoundManager");
        SE = SoundManager.GetComponentInChildren<SEManager>();
    }

    void Update()
    {

        if (animator && Camera.main && !animator.GetBool("Death"))
        {
            JoystickToEvents.Do(transform, Camera.main.transform, ref speed, ref direction);
            locomotion.Do(speed *6.0f, direction * 180);

            state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("Locomotion.Idle") && !animator.GetBool("Guard"))
            {

                boxCol.enabled = false;
               // Debug.Log(boxCol.enabled);
            }

            if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("Guard"))
            {
                Debug.Log("NowGuard");
                SE.SEStart(1);
                boxCol.enabled = true;
                //Debug.Log(boxCol.enabled);
                boxCol.size = new Vector3(1.0f, 0.13f, 1.3f);
                animator.SetBool("Guard", true);

                if (state.IsName("Locomotion.CounterAttack"))
                {
                    animator.SetBool("Guard", false);

                }
            }

            if (Input.GetKeyUp(KeyCode.B) || Input.GetButtonUp("Guard"))
            {
               // Debug.Log("GuardFinished");
                boxCol.enabled = false;
                boxCol.size = new Vector3(0.3f, 0.13f, 1.3f);
                animator.SetBool("Guard", false);

            }

            if (state.IsName("Locomotion.KnockBack"))
            {
                if (Input.GetKeyUp(KeyCode.B) || Input.GetButtonUp("Guard"))
                {
                  //  Debug.Log("GuardFinished");
                    //boxCol.enabled = true;
                    //boxCol.size = new Vector3(0.3f, 0.13f, 1.3f);
                    animator.SetBool("Guard", false);
                    animator.SetTrigger("CounterAttack");
                    SE.SEStart(2);
                }

            }

            if (state.IsName("Locomotion.CounterAttack"))
            {
                boxCol.enabled = true;
                boxCol.size = new Vector3(0.3f, 0.13f, 1.3f);
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Attack"))
            {
                if (state.IsName("Locomotion.Attack"))
                {
                    Debug.Log("ComboAttacking");
                    boxCol.enabled = true;
                    animator.SetBool("ComboAttack", true);
                   // Invoke("AttackSound", 0.7f);
                }
                else if (!state.IsName("Locomotion.ComboAttack"))
                {
                    Invoke("AttackSound", 0.5f);
                    Invoke("AttackSound", 0.8f);
                    Invoke("KatanaEnabled", 0.5f);
                    
                    Invoke("KatanaDisabled", 1.2f);
                    animator.Play("Locomotion.Attack");
                }

            }



            //if (state.IsName("Locomotion.Attack"))
            //{
            //    //Debug.Log("AttackingEnd");
            //    animator.SetBool("Attacking", false);
            //    capcol.enabled = false;
            //}

            if (state.IsName("Locomotion.ComboAttack"))
            {

                animator.SetBool("ComboAttack", false);

            }
        }
        else
        {
            if (DispUI == null)
            {
                DispUI = DeathUI;
                Instantiate(DispUI);
            }
        }
    }

    void KatanaEnabled()
    {
        boxCol.enabled = true;
    }

    void KatanaDisabled()
    {
        if (!animator.GetBool("ComboAttack")&&!state.IsName("Locomotion.ComboAttack"))
        {
            boxCol.enabled = false;
        }
       
    }

    void AttackSound()
    {
        SE.SEStart(0);
    }

    void DownSound()
    {
        SE.SEStart(5);
    }

    void ReturnToTitle()
    {
        SceneManager.LoadScene("title");
    }


    public void EnemyKatanaHit()
    {
        SE.SEStart(6);
        animator.SetFloat("Speed", 0.0f);
        Debug.Log("Hit");
        //animator.SetBool("Attacking", false);
        SE.SEStart(4);
        Invoke("DownSound",0.6f);
        animator.SetBool("Death", true);
        animator.Play("Death");
        Debug.Log("Player Killed");
        GameObject blood = Instantiate(bloodParticle);
        blood.transform.position = transform.position + new Vector3(0.0f, 0.35f, 0.0f);
        Destroy(blood, 0.3f);
        Invoke("ReturnToTitle", 3.0f);
    }



}
