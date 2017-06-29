using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 姫の衝突
/// => ゲームクリア処理
/// </summary>
public class Heroine : MonoBehaviour {
    CapsuleCollider Cap;

    [SerializeField]
    GameObject Aura;

    [SerializeField]
    Canvas ClearUI;

    [SerializeField]
    GameObject Boss;

    Animator Anim;

    Vector3 HimePos;

    void Start()
    {
        Cap = GetComponent<CapsuleCollider>();
        Anim = GetComponent<Animator>();
        Anim.SetBool("Touched", false);
        HimePos = this.transform.position;
        Debug.Log("HimePos:"+HimePos);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hero"&&!Anim.GetBool("Touched"))
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
        Cap.enabled = false;
        yield return new WaitForSeconds(1.0f);
        HimePos.y += 0.1f;
        GameObject AuraInst = Instantiate(Aura);
        AuraInst.transform.position = HimePos;
        AuraInst.transform.DOScale(2.0f,2.5f);
        Debug.Log("AuraPos:"+AuraInst.transform.position);
        yield return new WaitForSeconds(2.5f);
        GameObject BossInst = Instantiate(Boss);
        BossInst.GetComponent<Animator>().Play("Locomotion.Spawn");
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
        yield break;
    }
}
