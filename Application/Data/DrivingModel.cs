/* Create by @LinziDong
 * Create time: 2018.07.24
 * Discrition: 这是一个单例模式，主要是存储在驾驶模式下，汽车的各项参数
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingModel
{
    #region 单例模式Singleton
    private DrivingModel() { }
    static DrivingModel() { }
    private static readonly DrivingModel _instance = new DrivingModel();
    public static DrivingModel Instance { get { return _instance; } }
    #endregion

    public const float MAX_VOLECITY = 100.0f;
    public const float MIN_VOLECITY = 0.0f;
    public const float ACCEBRATE_PARA = 100.0f;
    public const float RESISTANCE_PARE = 0.5f;
    public const float STOP_POWER = 10.0f;

    public bool isCarForward = true;
    private bool carRuning = false;
    /// <summary>
    /// 汽车是否正在跑动中
    /// </summary>
    public bool CarRuning
    {
        get
        {
            return carRuning;
        }

        set
        {
            carRuning = value;
        }
    }
    public Vector3 Direction;

    public bool HideCloudPoint = false;


    private float carVolecity = 0;

    /// <summary>
    /// 改变汽车速度（km/h）
    /// </summary>
    /// <param name="value">范围0~1</param>
    /// <returns></returns>
    public float ChangeCarVolecity(float value)
    {
        if(value == 0)
        {
            CarVolecityResistance();
        }
        else
        {
            carVolecity = ACCEBRATE_PARA * value;
            carVolecity = Mathf.Clamp(carVolecity, MIN_VOLECITY, MAX_VOLECITY);
        }
        return carVolecity;
    }

    /// <summary>
    /// 刹车，将车的速度减为0
    /// </summary>
    public void StopCarVolecity()
    {
        if (carVolecity != 0)
        {
            carVolecity = 0;
        }
    }

    /// <summary>
    /// 倒车切换，将车的行驶方向颠倒
    /// </summary>
    public void ChangeCarLookward()
    {
        isCarForward = !isCarForward;
    }

    /// <summary>
    /// 转盘，改变车的行驶方向，可以查看Direction的引用，主要在Carbase里行驶时对其进行调用
    /// </summary>
    /// <param name="value">范围0~1,0~0.5为向左，0.5~1为向右</param>
    public void ChangeCarDirection(float value)
    {
        Direction = new Vector3(0,(value - 0.5f)/0.5f, 0);
    }

    /// <summary>
    /// 获得汽车速度（单位km/h）
    /// </summary>
    /// <returns></returns>
    public float GetCarVolecity()
    {
        return carVolecity;
    }

    /// <summary>
    /// 汽车阻力，只在非驾驶时发生作用
    /// </summary>
    public void CarVolecityResistance()
    {
        if (CarRuning) return;
        if(carVolecity > 0)
        {
            carVolecity -= RESISTANCE_PARE;
            if (carVolecity < 0)
            {
                carVolecity = 0;
            }
        }
    }
}
