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

    /// <summary>
    /// 当前展示的车的Index
    /// </summary>
    public int ExhibitingCarIndex
    {
        get
        {
            return exhibitingCarIndex;
        }

        set
        {
            exhibitingCarIndex = value;
        }
    }
    #endregion

    private int exhibitingCarIndex = -1;
}
