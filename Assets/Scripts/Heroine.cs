using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heroine : MonoBehaviour {
    CapsuleCollider Cap;

    [SerializeField]
    Canvas ClearUI;

    void Start()
    {
        Cap = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Hero")
        {
            Canvas DispUI = Instantiate(ClearUI);
            DispUI.GetComponentInChildren<Text>().text = "生還";
            KilledNum.IsStarted = false;
            StartCoroutine(FadeDisp.FadeOut());
        }
    }
}
