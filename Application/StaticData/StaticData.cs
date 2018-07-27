using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

public class StaticData : Singleton<StaticData>
{
    Dictionary<int, CarInfo> m_Cars = new Dictionary<int, CarInfo>();

    public int CarsCount
    {
        get { return m_Cars.Count; }
    }
    protected override void Awake()
    {
        base.Awake();
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
    //void InitTowers()
    //{
    //    FileInfo file = new FileInfo(Consts.StaticDataDir + "/Tower.xml");
    //    StreamReader sr = new StreamReader(file.OpenRead(), Encoding.UTF8);

    //    XmlDocument doc = new XmlDocument();
    //    doc.Load(sr);
    //    XmlNodeList nodes;
    //    nodes = doc.SelectNodes("Tower/TowerInfo");
    //    for (int i = 0; i < nodes.Count; i++)
    //    {
    //        XmlNode node = nodes[i];
    //        TowerInfo p = new TowerInfo()
    //        {

    //            ID = int.Parse(node.Attributes["ID"].Value),
    //            PrefabName = node.Attributes["PrefabName"].Value,
    //            MaxLevel = int.Parse(node.Attributes["MaxLevel"].Value),
    //            AttackRange = float.Parse(node.Attributes["AttackRange"].Value),
    //            ShotRate = float.Parse(node.Attributes["ShotRate"].Value),
    //            UseBulletID = int.Parse(node.Attributes["UseBulletID"].Value),
    //        };
    //        //XmlNodeList priceList = node.FirstChild.ChildNodes;
    //        //XmlNodeList priceList = node.SelectSingleNode("PriceList").SelectNodes("Price");
    //        XmlNodeList priceList = node.SelectNodes("PriceList/Price");
    //        p.Price = new List<int>();
    //        for (int j = 0; j < priceList.Count; j++)
    //        {
    //            p.Price.Add(int.Parse(priceList[j].InnerText));
    //        }
    //        m_Towers.Add(p.ID, p);
    //    }
    //    sr.Close();
    //    sr.Dispose();

    //    //m_Towers.Add(0, new TowerInfo()
    //    //{
    //    //    ID = 0,
    //    //    PrefabName = "Bottle",

    //    //    MaxLevel = 3,
    //    //    Price = new List<int>() {100,280,540},

    //    //    ShotRate = 3,
    //    //    AttackRange = 3f,
    //    //    UseBulletID = 0
    //    //});
    //    //m_Towers.Add(1, new TowerInfo()
    //    //{
    //    //    ID = 1,
    //    //    PrefabName = "Shit",

    //    //    MaxLevel = 3,
    //    //    Price = new List<int>() { 120, 340, 660 },
    //    //    ShotRate = 2,
    //    //    AttackRange = 3f,
    //    //    UseBulletID = 1
    //    //});
    //    //m_Towers.Add(2, new TowerInfo()
    //    //{
    //    //    ID =2,
    //    //    PrefabName = "Fan",

    //    //    MaxLevel = 3,
    //    //    Price = new List<int>() { 160, 380, 640 },
    //    //    ShotRate = 1,
    //    //    AttackRange = 3f,
    //    //    UseBulletID = 2
    //    //});
    //}

    //void InitBullets()
    //{
    //    FileInfo file = new FileInfo(Consts.StaticDataDir + "/Bullet.xml");
    //    StreamReader sr = new StreamReader(file.OpenRead(), Encoding.UTF8);

    //    XmlDocument doc = new XmlDocument();
    //    doc.Load(sr);

    //    XmlNodeList nodes;
    //    nodes = doc.SelectNodes("/Bullet/BulletInfo");
    //    for (int i = 0; i < nodes.Count; i++)
    //    {
    //        XmlNode node = nodes[i];

    //        BulletInfo p = new BulletInfo()
    //        {
    //            ID = int.Parse(node.Attributes["ID"].Value),
    //            BaseSpeed = float.Parse(node.Attributes["BaseSpeed"].Value),
    //            BaseAttack = int.Parse(node.Attributes["BaseAttack"].Value),
    //            BaseSlowSpeed = float.Parse(node.Attributes["BaseSlowSpeed"].Value),
    //            BaseSlowTime = float.Parse(node.Attributes["BaseSlowTime"].Value)
    //        };
    //        m_Bullets.Add(p.ID, p);
    //    }

    //    sr.Close();
    //    sr.Dispose();
    //    //m_Bullets.Add(0,new BulletInfo() {ID=0,BaseSpeed = 5f,BaseAttack = 2});
    //    // m_Bullets.Add(1, new BulletInfo() { ID = 1, BaseSpeed = 5f, BaseAttack = 1,BaseSlowSpeed = 0.5f,BaseSlowTime =2});
    //    // m_Bullets.Add(2, new BulletInfo() { ID = 2, BaseSpeed = 4f, BaseAttack = 2 });
    //}

    //public MonsterInfo GetMonsterInfo(int mosterType)
    //{
    //    return m_Monsters[mosterType];
    //}
    //public CarrotInfo GetCarrotInfo(int carrotType)
    //{
    //    return m_Carrots[carrotType];
    //}

    //public TowerInfo GetTowerInfo(int towerType)
    //{
    //    return m_Towers[towerType];
    //}
    //public BulletInfo GetBulletInfo(int BulletType)
    //{
    //    return m_Bullets[BulletType];
    //}
}
