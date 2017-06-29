using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// カメラ移動処理
/// </summary>
public class CameraMove : MonoBehaviour
{
    GameObject Player;
    GameObject heroine;
    static GameObject Camera;
   
    [SerializeField]
    float RotateSpeed;

    static public bool HimeIsDead;

    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("Player");
        heroine = GameObject.Find("Heroine");
        Camera = GameObject.Find("CameraSetPos");


        HimeIsDead = false;
        transform.position = heroine.transform.position;
        transform.LookAt(heroine.transform);
        StartCoroutine(StartCameraMove());
    }

    // Update is called once per frame
    void Update()
    {
        

        if (!KilledNum.IsStarted) return;

        transform.position = Player.transform.position;

        //float Camerahigh = transform.position.y + Input.GetAxisRaw("Vertical2");

        //transform.position = new Vector3(transform.position.x, Camerahigh, transform.position.z);

        transform.RotateAround(transform.position, Vector3.up, RotateSpeed * Input.GetAxisRaw("Horizontal2"));

        float turnX = Input.GetAxisRaw("Vertical2");
        float turnY = Input.GetAxisRaw("Horizontal2");
        // 現在の回転角度を0～360から-180～180に変換
        float rotateX = (transform.eulerAngles.x > 180) ? transform.eulerAngles.x - 360 : transform.eulerAngles.x;
        float rotateY = (transform.eulerAngles.y > 180) ? transform.eulerAngles.y - 360 : transform.eulerAngles.y;
        // 現在の回転角度に入力(turn)を加味した回転角度をMathf.Clamp()を使いminAngleからMaxAngle内に収まるようにする
        float angleX = Mathf.Clamp(rotateX + turnX *RotateSpeed, -30, 30);
        float angleY = Mathf.Clamp(rotateY + turnY * RotateSpeed, -180, 180);
        // 回転角度を-180～180から0～360に変換
        angleX = (angleX < 0) ? angleX + 360 : angleX;
        angleY = (angleY < 0) ? angleY + 360 : angleY;
        // 回転角度をオブジェクトに適用
        transform.rotation = Quaternion.Euler(angleX, angleY, 0);

        // transform.RotateAround(transform.position, Vector3.left, RotateSpeed * Input.GetAxisRaw("Vertical2"));

        if (HimeIsDead)
        {
            HimeIsDead = false;
            StartCoroutine(HeroineDeathMove());
        }

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


   IEnumerator HeroineDeathMove()
    {
        BGMManager.Instance.GetComponent<BGMManager>().SoundStop();
        Camera.transform.position = Player.transform.position;
        Camera.transform.LookAt(heroine.transform);
        KilledNum.IsStarted = false;
        Player.GetComponent<Animator>().Play("Down");
        yield return new WaitForSeconds(1f);
        Camera.transform.LookAt(heroine.transform);
        transform.DOMove(Player.transform.position, 1.0f);
        yield return new WaitForSeconds(2f);
        Camera.transform.LookAt(heroine.transform);
        transform.DOMove(Player.transform.position,1.0f);

        Camera.transform.DOLookAt(new Vector3(heroine.transform.position.x, heroine.transform.position.y+2, heroine.transform.position.z),4.0f);
        yield return new WaitForSeconds(3f);
        BGMManager.Instance.GetComponent<BGMManager>().SoundStart(1);
        KilledNum.IsStarted = true;
        yield break;
    }

    /// <summary>
    /// ゲーム開始時  カメラ移動演出
    /// </summary>
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
