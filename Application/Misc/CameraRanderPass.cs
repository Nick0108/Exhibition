using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PassType
{
    Reverse,
    Depth
}
public class CameraRanderPass : MonoBehaviour
{

    public PassType showType = PassType.Reverse;
    public Material Mat;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, Mat, (int)showType);
    }
}