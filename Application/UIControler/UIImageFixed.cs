using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageFixed : MonoBehaviour
{

    public static void FixedScreenByWidth(Image image)
    {
        int targetHeight = 0;
        int targetWidth = 0;
        float tImageHeight = (float)image.mainTexture.height;
        float tImageWidth = (float)image.mainTexture.width;
        float tImageProportion = tImageWidth / tImageHeight;

        float tScreenProportion = (float)Screen.width / (float)Screen.height;
        if(tImageProportion > tScreenProportion)
        {
            targetHeight = Screen.height;
            targetWidth = Mathf.CeilToInt(Screen.height * tImageProportion);
        }
        else
        {
            targetWidth = Screen.width;
            targetHeight = Mathf.CeilToInt(Screen.width / tImageProportion);
        }
        image.rectTransform.sizeDelta =  new Vector2(targetWidth, targetHeight);
    }
	
}
