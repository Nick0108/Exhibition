using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

public class StaticData : ISingleton
{
    #region 单例模式Singleton
    private StaticData() { }
    static StaticData() { }
    private static readonly StaticData _instance = new StaticData();
    public static StaticData Instance { get { return _instance; } }
    #endregion

    Dictionary<int, CarInfo> m_Cars = new Dictionary<int, CarInfo>();

    public int CarsCount
    {
        get { return m_Cars.Count; }
    }
    public void Init()
    {
        InitCar();
    }
    void InitCar()
    {
        m_Cars.Add(0, new CarInfo() { ID=0,Name="BMWi8", });
        m_Cars.Add(1, new CarInfo() { ID = 1, Name = "BMWi3", });
    }
    public CarInfo GetCarInfo(int CarID)
    {
        return m_Cars[CarID];
    }
}
