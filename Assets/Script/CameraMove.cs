using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraMove : MonoBehaviour
{
    GameObject Player;
    static GameObject Camera;
    GameObject Katana;
    [SerializeField]
    float RotateSpeed;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player");
        
        Camera = GameObject.Find("CameraSetPos");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.transform.position;

       // float Camerahigh = transform.position.y + Input.GetAxisRaw("Vertical2");

        //transform.position = new Vector3(transform.position.x, Camerahigh, transform.position.z);

        transform.RotateAround(transform.position, Vector3.up, RotateSpeed * Input.GetAxisRaw("Horizontal2"));

        if (Input.GetButtonDown("CameraSet"))
        {
            transform.DOLocalRotateQuaternion(Player.transform.rotation, 1.0f);
        }

    }

    static public void ShakeCamera()
    {
        Debug.Log("Shake!");
        Camera.transform.DOShakePosition(0.1f, 0.5f, 20);
    }
}
