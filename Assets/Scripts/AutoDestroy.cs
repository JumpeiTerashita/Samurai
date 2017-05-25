using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {


    [SerializeField]
    float DestroyLimit = 1.0f;
	// Use this for initialization
	void Start () {
        StartCoroutine(Destroy());
	}
	
	

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(DestroyLimit);
        Destroy(this.gameObject);
    }
}
