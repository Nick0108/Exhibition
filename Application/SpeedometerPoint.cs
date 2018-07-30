/* Create by @LinziDong
 * Create time: 2018.07.24
 * Discrition: 这个一个挂载在速度计盘上面的一个脚本，作用就是每帧根据当前速度（DrivingModel.Instance.GetCarVolecity()），
 *             来实时改变当前的速度指针，和以文本实时显示当前的速度。
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpeedometerPoint : MonoBehaviour
{
    RectTransform speedometerPointTrans;
    Text speedometerPointTransText;
    private const float BEGIN_ROTATE_Z = 2f;
    private const float END_ROTATE_Z = 230.0f;

    

    private void Awake()
    {
        speedometerPointTransText = this.transform.Find("TextSpeedometer").GetComponent<Text>();
        speedometerPointTrans = this.transform.Find("speedometerPointTransParent/speedometerPointTrans").GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!Game.Instance.IsDriving)
        {
            return;
        }
        SpeedResistance();
        RefreshSpeedometerPointView();
    }

    private void RefreshSpeedometerPointView()
    {
        float tZ = (END_ROTATE_Z - BEGIN_ROTATE_Z) * DrivingModel.Instance.GetCarVolecity() / 100.0f + BEGIN_ROTATE_Z;
        if (tZ > speedometerPointTrans.localRotation.eulerAngles.z)
        {
            speedometerPointTrans.Rotate(0, 0, 2);
        }
        else
        {
            speedometerPointTrans.Rotate(0, 0, -2);
        }
        //Debug.Log("速率：" + DrivingModel.Instance.GetCarVolecity());
        speedometerPointTransText.text = Mathf.FloorToInt(DrivingModel.Instance.GetCarVolecity()).ToString();
    }

    private void SpeedResistance()
    {
        DrivingModel.Instance.CarVolecityResistance();

    }
}
