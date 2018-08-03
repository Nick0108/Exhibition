/* Create by @LinziDong
 * Create time: 2018.07.30
 * Discrition: 这是展示时（包括AR）的一个单例模式数据类
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhibitionModel
{
    #region 单例模式Singleton
    private ExhibitionModel() { }
    static ExhibitionModel() { }
    private static readonly ExhibitionModel _instance = new ExhibitionModel();
    public static ExhibitionModel Instance { get { return _instance; } }
    #endregion

    public Vector3 RealSize = new Vector3(1f, 1f, 1f);
    public Vector3 ModelSize = new Vector3(0.1f, 0.1f, 0.1f);

    private Vector3 currentSelectSize;
    /// <summary>
    /// 当前初始化模型时的size
    /// </summary>
    public Vector3 CurrentSelectSize
    {
        get
        {
            return currentSelectSize;
        }

        set
        {
            currentSelectSize = value;
        }
    }


    private bool hasInitExhibition = false;
    /// <summary>
    /// 是否已经初始化过展示模式下的模型了（整个游戏有且仅有初始化一次）
    /// </summary>
    public bool HasInitExhibition
    {
        get
        {
            return hasInitExhibition;
        }

        set
        {
            hasInitExhibition = value;
        }
    }

    

    

    private int exhibitingCarIndex = 0;
    /// <summary>
    /// 当前展示的车的Index
    /// </summary>
    public int ExhibitingCarIndex
    {
        get
        {
            return exhibitingCarIndex;
        }

        private set
        {
            exhibitingCarIndex = value;
        }
    }

    private List<CarBase> showedCarList = new List<CarBase>();
    /// <summary>
    /// 生成的所有汽车的CarBase
    /// </summary>
    public List<CarBase> ShowedCarList
    {
        get
        {
            return showedCarList;
        }

        set
        {
            showedCarList = value;
        }
    }

    /// <summary>
    /// 当前展示的汽车
    /// </summary>
    public CarBase CurrentCar
    {
        get
        {
            if (ShowedCarList.Count > 0 && ExhibitingCarIndex < ShowedCarList.Count)
            {
                return ShowedCarList[ExhibitingCarIndex];
            }
            else
            {
                //我觉得虽然有了IsValidInputIndex应该没问题了，但是还有ShowedCarList的变动不可忽略
                //TODO:@Zidong 要进一步加强对ShowedCarList的增删控制
                Debug.LogError(string.Format("该汽车的ExhibitingCarIndex({0})不能使用,ShowedCarList.Count={1}", ExhibitingCarIndex, ShowedCarList.Count));
                return null;
            }
        }
    }

    private int currentARCarId = -1;
    /// <summary>
    /// 当前展示的汽车的id（仅用于AR场景）
    /// </summary>
    public int CurrentARCarId
    {
        get
        {
            return currentARCarId;
        }

        set
        {
            currentARCarId = value;
        }
    }

    public CarBase GetCar(int index)
    {
        if (ShowedCarList.Count > 0)
        {
            return ShowedCarList[index];
        }
        else
        {
            return null;
        }
    }

    public void ChangeCurrentCar(int index)
    {
        if (IsValidInputIndex(index))
        {
            ExhibitingCarIndex = index;
        }
    }

    private bool IsValidInputIndex(int index)
    {
        if (ExhibitingCarIndex == index)
        {
            Debug.LogWarning(string.Format("输入的index({0})与当前ExhibitingCarIndex一致", index));
            return false;
        }
        if(index >= ShowedCarList.Count)
        {
            Debug.LogError(string.Format("输入的index（{0}）超出当前ShowedCarList.Count（{1}）", index, ShowedCarList.Count));
            return false;
        }
        if (index < 0)
        {
            Debug.LogError(string.Format("输入的index（{0}）小于0", index));
            return false;
        }
        return true;
    }

    public int GetAddIndex(int originalIndex, int MaxCount, bool isAdd = true)
    {
        if(MaxCount <= 0)
        {
            Debug.LogError("MaxCount为0");
            return -1;
        }
        int tIndex = originalIndex;
        tIndex += isAdd ? 1 : -1;
        if (tIndex < 0)
        {
            tIndex = MaxCount - 1;
        }
        else
        {
            tIndex %= MaxCount;
        }
        return tIndex;
    }

    public int GetAddIndex(bool isAdd = true)
    {
        if (ShowedCarList.Count <= 0)
        {
            Debug.LogError("ShowedCarList.Count<=0");
            return -1;
        }
        int tIndex = ExhibitingCarIndex;
        tIndex += isAdd ? 1 : -1;
        if (tIndex < 0)
        {
            tIndex = ShowedCarList.Count - 1;
        }
        else
        {
            tIndex %= ShowedCarList.Count;
        }
        return tIndex;
    }
}
