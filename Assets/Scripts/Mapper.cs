using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapper : MonoBehaviour {
    GameObject Player;
    Vector2 CenterPos = new Vector2(96.6f, 225.4f);
    // Use this for initialization
    void Start () {
        Player = GameObject.Find("Player");
        transform.position = CenterPos;
        Debug.Log(transform.position);
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(CenterPos.x+ Player.transform.position.x/3, CenterPos.y+ Player.transform.position.z/3);
        
    }
}
