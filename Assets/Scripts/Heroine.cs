using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

/// <summary>
/// 姫の衝突
/// => ゲームクリア処理
/// </summary>
public class Heroine : MonoBehaviour
{


    CapsuleCollider Cap;

    [SerializeField]
    GameObject Thunder;

    [SerializeField]
    GameObject Aura;


    [SerializeField]
    GameObject Boss;

    Animator Anim;

    Vector3 HimePos;

    SEManager SE;


    void Start()
    {
        Cap = GetComponent<CapsuleCollider>();
        Anim = GetComponent<Animator>();
        Anim.SetBool("Touched", false);
        HimePos = this.transform.position;
        Debug.Log("HimePos:" + HimePos);
        SE = SEManager.Instance.GetComponent<SEManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hero" && !Anim.GetBool("Touched"))
        {
            Anim.Play("Death");
            Anim.SetBool("Touched", true);
            StartCoroutine(HimeDeath());
            //Canvas DispUI = Instantiate(ClearUI);
            //DispUI.GetComponentInChildren<Text>().text = "生還";
            //KilledNum.IsStarted = false;
            //Time.timeScale = 1.0f;
            //StartCoroutine(FadeDisp.FadeOutToTitle());
        }
    }

    IEnumerator HimeDeath()
    {
        CameraMove.HimeIsDead = true;
        Cap.enabled = false;
        HimePos.y += 0.1f;
        GameObject ThuInst = Instantiate(Thunder);
        ThuInst.transform.position = HimePos;

        SE.SEStart(11);
       
        yield return new WaitForSeconds(1.0f);

        GameObject AuraInst = Instantiate(Aura);
        AuraInst.transform.position = HimePos;
     

        yield return new WaitForSeconds(2.5f);
        GameObject BossInst = Instantiate(Boss);
       
        BossInst.transform.LookAt(GameObject.Find("Player").transform.position);
        BossInst.transform.position = HimePos;
        BossInst.GetComponent<Animator>().Play("Locomotion.Spawn");
        SE.SEStart(13);
        yield return new WaitForSeconds(1.0f);
        SE.SEStart(14);
        Destroy(gameObject);
        yield break;
    }
}
