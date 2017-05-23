using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heroine : MonoBehaviour {
    protected Animator animator;

    GameObject Player;
    GameObject Enemy;
    float speed = 2.0f;
    //private float direction = 0;
    Locomotion locomotion = null;
    AnimatorStateInfo state;

    // Use this for initialization
    void Start () {
        Player = GameObject.Find("Player");
        animator = GetComponent<Animator>();

        locomotion = new Locomotion(animator);
        state = animator.GetCurrentAnimatorStateInfo(0);
  
    }
	
	// Update is called once per frame
	void Update () {
        if (animator)
        {
            state = animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName("Locomotion.Death") && !state.IsName("Locomotion.Born"))
            {
                if (!state.IsName("Locomotion.Attack") && !state.IsName("Locomotion.Attack2"))
                {
                 
                    animator.SetBool("Attack", false);
                }



                transform.LookAt(Player.transform.position);

                Vector3 distance = Player.transform.position - transform.position;
                //float direction = Mathf.Atan2(distance.x, distance.z);


                animator.SetFloat("Speed", 0.1f);


                //JoystickToEvents.Do(transform, Camera.main.transform, ref speed, ref direction);
                //locomotion.Do(speed * 6, direction * 180);


                //TODO  Enemy attacklenge not magic number
                if (distance.magnitude <= 1.5f)
                {
                    if (!animator.GetBool("Attack"))
                    {

                        if (Random.Range(0.0f, 1.0f) >= 0.5f)
                        {
                            Invoke("KatanaEnabled", 0.5f);
                            animator.Play("Attack");
                            animator.SetBool("Attack", true);

                        }
                        else
                        {
                            Invoke("KatanaEnabled", 0.5f);
                            Invoke("KatanaDisabled", 2.5f);
                            animator.Play("Attack2");
                            animator.SetBool("Attack", true);

                        }

                    }






                }
                else
                {

                    transform.position = (transform.position + (distance.normalized * 1.0f * Time.deltaTime));
                }



            }
           

        }

    }
}
