﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//[ExecuteInEditMode]
public class BMWi8Controller : CarBase
{


    //public Vector3 TargetRotation = new Vector3(36.5f, -30.4f, 60);
    //private GameObject LightButton;
    private ExplodePartManager explodePartManager;

    // Use this for initialization
    new void Awake()
    {
        base.Awake();
        explodePartManager = this.GetComponent<ExplodePartManager>();
        //originalScale = transform.localScale;
        //Reset();
    }

    // Update is called once per frame
    private new void Update()
    {
        base.Update();
        BonnetUpdate();
        TrunkUpdate();
    }
    public override void BonnetUpdate()
    {
        if (IsBonnetOpen)
        {
            if (CurrentBonnetAngle >= m_BonnetRotateAngle)
                return;
            CurrentBonnetAngle += m_BonnetRotateSpeed * Time.deltaTime;
        }
        else
        {
            if (CurrentBonnetAngle <= 0)
                return;
            CurrentBonnetAngle -= m_BonnetRotateSpeed * Time.deltaTime;
        }
        CurrentBonnetAngle = Mathf.Clamp(CurrentBonnetAngle, 0, m_BonnetRotateAngle);
        m_Bonnet.localRotation = Quaternion.Euler(CurrentBonnetAngle, 0, 0);
    }

    public override void TrunkUpdate()
    {

        if (IsTrunkOpen)
        {
            if (CurrentTrunkAngle >= m_TrunkRotateAngle)
                return;
            CurrentTrunkAngle += m_TrunkRotateSpeed * Time.deltaTime;
        }
        else
        {
            if (CurrentTrunkAngle <= 0)
                return;
            CurrentTrunkAngle -= m_TrunkRotateSpeed * Time.deltaTime;
        }
        CurrentTrunkAngle = Mathf.Clamp(CurrentTrunkAngle, 0, m_TrunkRotateAngle);
        m_Trunk.localRotation = Quaternion.Euler(CurrentTrunkAngle, 0, 0);
    }


    public void ChangeDoor(int DoorNum)
    {
        if (Doors.Length > DoorNum && DoorNum >= 0)
        {
            Doors[DoorNum].IsDoorOpen = !Doors[DoorNum].IsDoorOpen;
        }

    }


    //public override void ChangeTrunk()
    //{
    //    IsTrunkOpen = !IsTrunkOpen;
    //}
    //public override void ChangeBonnet()
    //{
    //    IsBonnetOpen = !IsBonnetOpen;
    //}


    public void ChangeLight(int lightNum)
    {
        LightNum light = (LightNum)lightNum;
        switch (light)
        {
            case LightNum.LeftBack:
                IsLeftBackLightOpen = !IsLeftBackLightOpen;
                break;
            case LightNum.LeftFornt:
                IsLeftFrontLightOpen = !IsLeftFrontLightOpen;
                break;
            case LightNum.RightBack:
                IsRightBackLightOpen = !IsRightBackLightOpen;
                break;
            case LightNum.RightFront:
                IsRightFrontLightOpen = !IsRightFrontLightOpen;
                break;

        }
    }
    public override void Reset()
    {
        for (int i = 0; i < Doors.Length; i++)
            Doors[i].IsDoorOpen = false;
        IsBonnetOpen = false;
        IsTrunkOpen = false;
        if (explodePartManager != null)
            explodePartManager.Reset();
        ChangeShellColor(0);
    }
    public override void Disintegrate()
    {
        //Debug.Log("Disintegrate" + "TODO");
        if (explodePartManager != null)
            explodePartManager.Disintegrate();

    }
    public override void GoForward()
    {
        transform.localPosition += transform.forward * DrivingModel.Instance.GetCarVolecity() / 600.0f * transform.localScale.z;
        transform.Rotate(DrivingModel.Instance.Direction);
        Wheel_1.transform.Rotate(-GetWheelSpeed(), 0, 0);
        Wheel_2.transform.Rotate(GetWheelSpeed(), 0, 0);
        Wheel_3.transform.Rotate(-GetWheelSpeed(), 0, 0);
        Wheel_4.transform.Rotate(GetWheelSpeed(), 0, 0);
    }

    public override void GoBackward()
    {
        transform.localPosition -= transform.forward * DrivingModel.Instance.GetCarVolecity() / 600.0f * transform.localScale.z;
        transform.Rotate(DrivingModel.Instance.Direction);
        Wheel_1.transform.Rotate(GetWheelSpeed(), 0, 0);
        Wheel_2.transform.Rotate(-GetWheelSpeed(), 0, 0);
        Wheel_3.transform.Rotate(GetWheelSpeed(), 0, 0);
        Wheel_4.transform.Rotate(-GetWheelSpeed(), 0, 0);
    }
}
