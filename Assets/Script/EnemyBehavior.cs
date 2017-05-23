using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

[RequireComponent(typeof(Animator))]

//Name of class must be name of file as well

public class EnemyBehavior : MonoBehaviour
{

    protected Animator animator;

    GameObject Player;
    GameObject Enemy;
    float speed = 2.0f;
    //private float direction = 0;
    Locomotion locomotion = null;
    AnimatorStateInfo state;
    

    [SerializeField]
    BoxCollider boxCol;

    [SerializeField]
    GameObject bloodParticle;
    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        
        locomotion = new Locomotion(animator);
        state = animator.GetCurrentAnimatorStateInfo(0);
        boxCol.enabled = false;
        //animator.Play("Locomotion.Born");
    }

    void Update()
    {
        if (animator)
        {
            state = animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName("Locomotion.Death")&&!state.IsName("Locomotion.Born"))
            {
                if (!state.IsName("Locomotion.Attack")&& !state.IsName("Locomotion.Attack2"))
                {
                    boxCol.enabled = false;
                    animator.SetBool("Attack", false);
                    animator.SetFloat("Speed", 1.0f);

                }



                transform.LookAt(Player.transform.position);

                Vector3 distance = Player.transform.position - transform.position;
                

                //JoystickToEvents.Do(transform, Camera.main.transform, ref speed, ref direction);
                //locomotion.Do(speed * 6, direction * 180);


                //TODO  Enemy attacklenge not magic number
                if (distance.magnitude <= 1.5f)
                {
                   
                        
                    
                    if (!animator.GetBool("Attack"))
                    {
                        PlayerBehavior.IsCrisis = true;
                        Debug.Log("Pinch!");
                        if (Random.Range(0.0f, 1.0f) >= 0.5f)
                        {
                            Invoke("KatanaEnabled", 0.5f);
                            Invoke("KatanaDisabled", 1.5f);
                            animator.Play("Attack");
                            animator.SetBool("Attack", true);
                            animator.SetFloat("Speed", 0.0f);
                        }
                        else
                        {
                            Invoke("KatanaEnabled", 0.5f);
                            Invoke("KatanaDisabled", 2.5f);
                            animator.Play("Attack2");
                            animator.SetBool("Attack", true);
                            animator.SetFloat("Speed", 0);
                        }
                        
                    }
                 
                }
                else
                {
                    
                    if (!animator.GetBool("Attack"))
                    {
                        //Debug.Log("Move!");
                        animator.SetBool("Attack", false);
                        transform.position = (transform.position + (distance.normalized * 3.0f * Time.deltaTime));
                    }
                    
                }
            }
            else
            {
                boxCol.enabled = false;
                //      katanaDamaged();
            }







        }

    }



    public void PlayerKatanaHit()
    {
        if (!animator.GetBool("Death"))
        {
            animator.SetFloat("Speed", 0.0f);
            Debug.Log("Hit");
            //animator.SetBool("Attacking", false);
            animator.SetBool("Death", true);
            animator.Play("Death");
            KilledNum.KillCounter++;
            GameObject blood = Instantiate(bloodParticle, transform.position + new Vector3(0.0f, 1f, 0.0f), Quaternion.identity);
            Destroy(blood, 0.5f);
            Destroy(this.gameObject, 3.0f);
        }
        

    }


    void KatanaEnabled()
    {
        boxCol.enabled = true;
    }

    void KatanaDisabled()
    {
        
        boxCol.enabled = false;
        

    }


    //void OnCollisionEnter(Collision collider)
    //{

    //    if (collider.gameObject.tag == "Katana")
    //    {
    //        Debug.Log("Hit");
    //        animator.SetTrigger("Death");
    //    }
    //}
}


