using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{
    [SerializeField]
    Material monotone;
    float _stopTime;
    void Update()
    {
        // Debug.Log(_stopTime);
        _stopTime = Mathf.Clamp(PlayerBehavior.StopSec, 0, 1);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        monotone.SetFloat("_StopTime", _stopTime);
        Graphics.Blit(source, destination, monotone);
    }
}
