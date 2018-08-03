/* Create by @LinziDong
 * Create time: 2018.08.02
 * Discrition: 这是一个单例模式，主要是存储在AR模式下，汽车的各项参数(ps.不包含驾驶模式，驾驶模式请见DrivingModel)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ARModel
{
    #region 单例模式Singleton
    private ARModel() { }
    static ARModel() { }
    private static readonly ARModel _instance = new ARModel();
    public static ARModel Instance { get { return _instance; } }
    #endregion

    public Action<bool> ShowArCar = null;

    private bool resetingCar = false;
    /// <summary>
    /// 当重新扫描时，需要记录一下，以免手指重复触发
    /// </summary>
    public bool ResetingCar
    {
        get
        {
            return resetingCar;
        }

        set
        {
            resetingCar = value;
        }
    }

    private bool canPlaceObject = false;
    /// <summary>
    /// 当检测到可以放下AR汽车时
    /// </summary>
    public bool CanPlaceObject
    {
        get
        {
            return canPlaceObject;
        }

        set
        {
            canPlaceObject = value;
        }
    }


    private bool hasSpawnARCar = false;
    /// <summary>
    /// 是否已经生成AR的汽车模型了，每次进入AR场景都要初始化，初始化后变为true，退出AR场景时该值要变回false
    /// </summary>
    public bool HasSpawnARCar
    {
        get
        {
            return hasSpawnARCar;
        }

        set
        {
            hasSpawnARCar = value;
        }
    }

    private bool hasShowARCar = false;
    /// <summary>
    /// 是否已经展示过AR的汽车模型了，展示后为true，退出AR场景时该值要变回false
    /// </summary>
    public bool HasShowARCar
    {
        get
        {
            return hasShowARCar;
        }

        set
        {
            hasShowARCar = value;
        }
    }

    //private bool hasShowARControllerUI = false;
    ///// <summary>
    ///// 是否已经展示过AR的控制图标了，展示后为true，退出AR场景时该值要变回false
    ///// </summary>
    //public bool HasShowARControllerUI
    //{
    //    get
    //    {
    //        return hasShowARControllerUI;
    //    }

    //    set
    //    {
    //        hasShowARControllerUI = value;
    //    }
    //}

    private CarBase currentARCar = null;
    /// <summary>
    /// 当前展示的汽车（仅在AR场景）
    /// </summary>
    public CarBase CurrentARCar
    {
        get
        {
            return currentARCar;
        }
        set
        {
            currentARCar = value;
        }
    }

    

    //private bool isAllInterActiveOpen = false;
    ///// <summary>
    ///// 所有汽车可交互是否已经打开
    ///// </summary>
    //public bool IsAllInterActiveOpen
    //{
    //    get
    //    {
    //        return isAllInterActiveOpen;
    //    }

    //    set
    //    {
    //        isAllInterActiveOpen = value;
    //    }
    //}

    /// <summary>
    /// 退出AR时调用，重置所有状态值
    /// </summary>
    public void ResetARValue()
    {
        CurrentARCar = null;
        HasShowARCar = false;
        HasSpawnARCar = false;
        //HasShowARControllerUI = false;
        CanPlaceObject = false;
    }

    /// <summary>
    /// 不退出AR，但点击重新扫描
    /// </summary>
    public void ReSacnARValue()
    {
        HasShowARCar = false;
        //HasShowARControllerUI = false;
        CanPlaceObject = false;
    }

    
}
