using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]

public class PostEffectController : MonoBehaviour
{
    public Shader postShader;
    Material postMaterial;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //gameObject.SetActive(false);
        if (postMaterial == null)
            postMaterial = new Material(postShader);
        RenderTexture tmp = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        Graphics.Blit(source, tmp, postMaterial);
        Graphics.Blit(tmp, destination);

        RenderTexture.ReleaseTemporary(tmp);
    }
}
