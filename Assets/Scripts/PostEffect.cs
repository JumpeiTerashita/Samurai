using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{
    [SerializeField]
    Material monotone;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
     
            Graphics.Blit(source, destination, monotone);
        
        
    }
}
