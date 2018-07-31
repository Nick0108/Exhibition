using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum LightNum
{
    LeftFornt,
    RightFront,
    LeftBack,
    RightBack,
}
[System.Serializable]
public enum DoorNum
{
    LeftFornt,
    RightFront,
    LeftBack,
    RightBack,

}
[System.Serializable]
public class DoorInfo
{
    public Transform DoorRoot;//车门的旋转点
    private bool isDoorOpen;//车门是否开着
    [HideInInspector]
    public bool isDoorChange;//车门是否在开启或者关闭
    [HideInInspector]
    public Quaternion targetQuaternion;//车门旋转目标角度
    public Vector3 finalRotation;//为了方便编辑，使用Vector3来表示车门的旋转角度
    public bool IsDoorOpen { get { return isDoorOpen; }//使用属性来确保调整一个参数同时调整其他几个参数同步
        set { isDoorOpen = value;
            isDoorChange = true;
            targetQuaternion = isDoorOpen ? Quaternion.Euler(finalRotation) : Quaternion.identity;
        } }
}
public abstract class CarBase :ReusableObject{
    public int CarID;
    //车门
    public float m_doorRotateSpeed = 20;//车门旋转速度
    public DoorInfo[] Doors;
    //引擎盖可以和车门合并，他们的旋转方式时相同的
    public Transform m_Bonnet;
    public float CurrentBonnetAngle = 0;
    public float m_BonnetRotateAngle = 60;
    public float m_BonnetRotateSpeed = 20;
    public bool IsBonnetOpen = false;

    //后备箱可以和车门合并，他们的旋转方式时相同的
    public Transform m_Trunk;
    public float CurrentTrunkAngle = 0;
    public float m_TrunkRotateAngle = 60;
    public float m_TrunkRotateSpeed = 20;
    public bool IsTrunkOpen = false;

    //控制缩放
    public float rotationSmooth = 2f;
    public float scaleSmooth = 0.01f;
    public Vector3 originalScale;
    private float currentScraling = 1f;
    public float MaxScale = 10f;
    public float MinScale = 0.5f;
    public bool TurnMaxScale = false;
    private bool lastIsMaxScale = false;
    protected float AutoScaleSpeed = 10.0f;

    //引擎盖和后备箱的旋转方式由子类实现
    public virtual void BonnetUpdate()
    {
    }
    
    public virtual void TrunkUpdate()
    {
    }
    //灯光
    public GameObject RightFrontLight;
    public GameObject LeftFrontLight;
    public GameObject RightBackLight;
    public GameObject LeftBackLight;

    private bool isRightBackLightOpen = false;
    private bool isRightFrontLightOpen = false;
    private bool isLeftBackLightOpen = false;
    private bool isLeftFrontLightOpen = false;
    //汽车颜色
    public enum ShellColor
    {
        Brown,
        Orange,
        Gray,
        White,
        Red,
        Blue,
        Black,
            
    }
    public Material cheke_bai;//汽车外壳材质
    public Color Brown_0;
    public Color Orange_1;
    public Color Gray_2;
    public Color White_3;
    public Color Red_4;
    public Color Blue_5;
    public Color Black_6;
    private Color oldColor;
    private Color newColor;
    private bool isChangeColor=false;
    public float ColorSmooth = 1;

    public float WheelSpeedPara = 50.0f;
    public Transform Wheel_1;
    public Transform Wheel_2;
    public Transform Wheel_3;
    public Transform Wheel_4;


    //public float AlphaSmooth = 1;
    public float MinAlpha = 0.3f;//汽车透明化的透明程度的最小值
    private bool isHyalinize = false;
    protected bool IsRightBackLightOpen
    {
        get
        {
            return isRightBackLightOpen;
        }

        set
        {
            isRightBackLightOpen = value;
            RightBackLight.SetActive(value);
        }
    }

    protected bool IsRightFrontLightOpen
    {
        get
        {
            return isRightFrontLightOpen;
        }

        set
        {
            isRightFrontLightOpen = value;
            RightFrontLight.SetActive(value);
        }
    }

    protected bool IsLeftBackLightOpen
    {
        get
        {
            return isLeftBackLightOpen;
        }

        set
        {
            isLeftBackLightOpen = value;
            LeftBackLight.SetActive(value);
        }
    }

    protected bool IsLeftFrontLightOpen
    {
        get
        {
            return isLeftFrontLightOpen;
        }

        set
        {
            isLeftFrontLightOpen = value;
            LeftFrontLight.SetActive(value);
        }
    }

    private GameObject VirtualBody;
    private GameObject RealBody;
    private bool isShowRealBody = false;
    /// <summary>
    /// 是否显示车的本体，还是显示阴影
    /// </summary>
    public bool IsShowRealBody
    {
        get
        {
            return isShowRealBody;
        }
        private set
        {
            isShowRealBody = value;
        }
    }

    public void SetShowRealBody(bool isActive)
    {
        IsShowRealBody = isActive;
        RealBody.SetActive(isActive);
        VirtualBody.SetActive(!isActive);
    }

    public void HideShowCar(bool isHide)
    {
        if (isHide)
        {
            RealBody.SetActive(false);
            VirtualBody.SetActive(false);
        }
        else
        {
            RealBody.SetActive(isShowRealBody);
            VirtualBody.SetActive(!isShowRealBody);
        }
        
    }
   
    protected void Awake()
    {
        VirtualBody = transform.Find("VirtualBody").gameObject;
        RealBody = transform.Find("RealBody").gameObject;
    }
    public virtual void ChangeScale(float offset)
    {
        if (Game.Instance.IsDriving)
        {
            return;
        }
        float scaleFactor = offset * scaleSmooth;
        currentScraling += scaleFactor;
        currentScraling = Mathf.Clamp(currentScraling, MinScale, MaxScale);

        transform.localScale = currentScraling * originalScale;
    }
    public virtual void RotateX(float offset)
    {
        if (Game.Instance.IsDriving)
        {
            return;
        }
        transform.Rotate(0, offset * rotationSmooth, 0);
    }
    public virtual void ChangeShellColor(int ColorNum)
    {
        ShellColor color = (ShellColor)ColorNum;
        switch(color)
        {
            case ShellColor.Brown:
                newColor = Brown_0;
                break;
            case ShellColor.Orange:
                newColor = Orange_1;
                break;
            case ShellColor.Gray:
                newColor = Gray_2;
                break;
            case ShellColor.White:
                newColor = White_3;
                break;
            case ShellColor.Red:
                newColor = Red_4;
                break;
            case ShellColor.Blue:
                newColor = Blue_5;
                break;
            case ShellColor.Black:
                newColor = Black_6;
                break;
        }
        newColor.a = cheke_bai.GetColor("_BaseColor").a;//保证在透明化时改变汽车颜色不会变回不透明的
        oldColor = cheke_bai.GetColor("_BaseColor");
        
        isChangeColor = true;
    }
    public virtual void Hyalinize()
    {
        isHyalinize = !isHyalinize;
        newColor.a = isHyalinize ? MinAlpha : 1f;
        isChangeColor = true;
    }
    public virtual void Update()
    {
        DoorUpdate();
        ShellColorUpdate();
        UpdateCarPosition();
        AutoTurnScale();
    }
    public virtual void DoorUpdate()
    {
        Transform temp;
        for (int i=0;i<Doors.Length;i++)
        {
            if (Doors[i].isDoorChange)
            {
                temp = Doors[i].DoorRoot;
                temp.localRotation = Quaternion.Lerp(temp.localRotation, Doors[i].targetQuaternion, Time.deltaTime * m_doorRotateSpeed);
            }
        }

    }
    public virtual void ShellColorUpdate()
    {
        if (isChangeColor)
        {
            Color color = Color.Lerp(oldColor, newColor, Time.deltaTime * ColorSmooth);

            cheke_bai.SetColor("_BaseColor", color);
            oldColor = color;
            if (color == newColor)
                isChangeColor = false;
        }
    }
    public virtual void Reset() { }
  
    public virtual void Disintegrate(){}

    public override void OnSpawn()
    {
        Game.Instance.gameModel.ShowedCarList.Add(this);
    }
    public override void OnUnSpawn()
    {
        Game.Instance.gameModel.ShowedCarList.Remove(this);
    }

    public virtual void GoForward()
    {
        //Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        transform.localPosition += transform.forward * DrivingModel.Instance.GetCarVolecity() / 600.0f * transform.localScale.z;
        transform.Rotate(DrivingModel.Instance.Direction);
        Wheel_1.transform.Rotate(-GetWheelSpeed(), 0, 0);
        Wheel_2.transform.Rotate(GetWheelSpeed(), 0, 0);
        Wheel_3.transform.Rotate(-GetWheelSpeed(), 0, 0);
        Wheel_4.transform.Rotate(GetWheelSpeed(), 0, 0);
    }

    public virtual void GoBackward()
    {
        transform.localPosition -= transform.forward * DrivingModel.Instance.GetCarVolecity() / 600.0f * transform.localScale.z;
        transform.Rotate(DrivingModel.Instance.Direction);
        Wheel_1.transform.Rotate(GetWheelSpeed(), 0, 0);
        Wheel_2.transform.Rotate(-GetWheelSpeed(), 0, 0);
        Wheel_3.transform.Rotate(GetWheelSpeed(), 0, 0);
        Wheel_4.transform.Rotate(-GetWheelSpeed(), 0, 0);
    }

    private void UpdateCarPosition()
    {
        if (DrivingModel.Instance.GetCarVolecity() > 0)
        {
            CarBase car = Game.Instance.gameModel.CurrentCar;
            if (car != null)
            {
                if (DrivingModel.Instance.isCarForward)
                {
                    car.GoForward();
                }
                else
                {
                    car.GoBackward();
                }
            }
        }
    }

    private void AutoTurnScale()
    {
        if(TurnMaxScale!= lastIsMaxScale)
        {
            if (TurnMaxScale)
            {
                float scaleFactor = AutoScaleSpeed * scaleSmooth;
                currentScraling += scaleFactor;
                currentScraling = Mathf.Clamp(currentScraling, MinScale, MaxScale);
                if(currentScraling == MaxScale)
                {
                    lastIsMaxScale = true;
                }
            }
            else
            {
                float scaleFactor = AutoScaleSpeed * scaleSmooth;
                currentScraling -= scaleFactor;
                currentScraling = Mathf.Clamp(currentScraling, MinScale, MaxScale);
                if (currentScraling == MinScale)
                {
                    lastIsMaxScale = false;
                }
            }
            transform.localScale = currentScraling * originalScale;
        }
    }

    protected float GetWheelSpeed()
    {
        return WheelSpeedPara * (DrivingModel.Instance.GetCarVolecity() / DrivingModel.MAX_VOLECITY);
    }
}
