using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraMove : MonoBehaviour
{
    GameObject Player;
    GameObject heroine;
    static GameObject Camera;
    GameObject Katana;
    [SerializeField]
    float RotateSpeed;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player");
        heroine = GameObject.Find("Heroine");
        Camera = GameObject.Find("CameraSetPos");

        transform.position = heroine.transform.position;
        transform.LookAt(heroine.transform);
        StartCoroutine(StartCameraMove());
    }

    // Update is called once per frame
    void Update()
    {
        if (!KilledNum.IsStarted) return;

        

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
       // Debug.Log("Shake!");
        Camera.transform.DOShakePosition(0.1f, 0.5f, 20);
    }

    IEnumerator StartCameraMove()
    {
        transform.LookAt(heroine.transform);
        yield return new WaitForSeconds(2f);
        transform.DOLocalPath(new Vector3[] { heroine.transform.position, new Vector3(6, 2, 95), new Vector3(8, 1, 60), new Vector3(0, 0, 0) }, 2f, PathType.CatmullRom);
        yield return new WaitForSeconds(2f);
        KilledNum.IsStarted = true;
        yield break;
    }
}
