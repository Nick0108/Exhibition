using UnityEngine;
using System.Collections;

public class ResolutionAdaptive : MonoBehaviour {

   
    //float devHeight = 10f;
    float devWidth = 15f;

    // Use this for initialization
    void Start()
    {

       // float screenHeight = Screen.height;
        //摄像机的尺寸
        float orthographicSize = this.GetComponent<Camera>().orthographicSize;
        //宽高比
        float aspectRatio = Screen.width * 1.0f / Screen.height;
        //摄像机的单位宽度
        float cameraWidth = orthographicSize * 2 * aspectRatio;
        //如果设备的宽度大于摄像机的宽度的时候  调整摄像机的orthographicSize
        if (devWidth > cameraWidth)
        {
            orthographicSize = devWidth / (2 * aspectRatio);

            this.GetComponent<Camera>().orthographicSize = orthographicSize;
        }  

    }
}
