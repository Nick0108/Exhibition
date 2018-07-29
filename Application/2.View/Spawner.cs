using UnityEngine;
using System.Collections;
//using GoogleARCore;
public class Spawner : View
{


    #region 常量
    #endregion

    #region 字段
    GameModel gameModel;
    
    
    #endregion

    #region 属性
    public override string Name
    {
        get { return Consts.V_Spawner; }
    }
    #endregion

    #region 事件
    #endregion
  
    public override void RegisterEvents()
    {
        //
        AttationEvents.Add(Consts.E_SpawnCar);
        AttationEvents.Add(Consts.E_SpawnCarAt);
        AttationEvents.Add(Consts.E_SpawnCarAtHit);
        //AttationEvents.Add(Consts.E_EnterScene);
    }

    public override void HandleEvent(string eventName, object data)
    {
        switch (eventName)
        {

            case Consts.E_SpawnCarAtHit://AR场景根据射线产生的碰撞信息来产生汽车
                SpawnCarAtHitArgs e0 = data as SpawnCarAtHitArgs;
                SpawnCar(e0.CarID);
                break;
            case Consts.E_SpawnCarAt://在世界坐标系中产生汽车
                SpawnCarAtArgs e1 = data as SpawnCarAtArgs;
                SpawnCar(e1.CarID, e1.position);
                break;
            case Consts.E_SpawnCar://只是造个车
                SpawnCarArgs e2 = data as SpawnCarArgs;
                SpawnCar(e2.CarID,e2.isRealBody);
                break;
            case Consts.E_EnterScene://进入场景
                SceneArgs e3 = data as SceneArgs;
                switch (e3.SceneIndex)
                {
                    case Consts.ExhibitionSceneIndex:
                        //gameModel = GetModel<GameModel>();
                        break;
                }
                break;
        }
    }
    public void Start()
    {
        gameModel = GetModel<GameModel>();
        SendEvent(Consts.E_RegisterView, this);
    }
    //public void SpawnCar(int CarID,TrackableHit hit)
    //{
    //    CarInfo carInfo = StaticData.Instance.GetCarInfo(CarID);
    //    if ( carInfo!=null)
    //    {
    //        GameObject Car = Game.Instance.ObjectPool.Spawn(carInfo.Name);
    //        var anchor = hit.Trackable.CreateAnchor(hit.Pose);
    //        Car.transform.parent = anchor.transform;
            
    //        //gameModel.ShowedCarList.Add(Car.GetComponent<CarBase>());
    //        gameModel.SelectCarIndex = gameModel.ShowedCarList.Count - 1;//产生一个新的之后自动选择该汽车
    //        //状态重置
    //        Car.transform.localPosition = Vector3.zero;
    //        Car.transform.localRotation = Quaternion.identity;
    //        //缩小汽车
    //        Car.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    //        Car.GetComponent<CarBase>().originalScale = Car.transform.localScale;
    //        Car.GetComponent<CarBase>().IsShowRealBody=false ;
    //        //gameModel.state = State.Show;
    //        //return Car;
    //    }
    //   // return null;
    //}
    public void SpawnCar(int CarID ,Vector3 position)
    {
        CarInfo carInfo = StaticData.Instance.GetCarInfo(CarID);
        if (carInfo != null)
        {
            GameObject Car = Game.Instance.ObjectPool.Spawn(carInfo.Name);
            Car.transform.position = position;
           // return Car;
        }
        //return null;
    }
    public void SpawnCar(int CarID)
    {
        CarInfo carInfo = StaticData.Instance.GetCarInfo(CarID);
        if (carInfo != null)
        {
           Game.Instance.ObjectPool.Spawn(carInfo.Name);
        }
       // return null;
    }

    public void SpawnCar(int CarID,bool isRealBody=true)
    {
        if (isRealBody)
            SpawnCar(CarID);
        else
        {
            CarInfo carInfo = StaticData.Instance.GetCarInfo(CarID);
            if (carInfo != null)
            {
                GameObject Car = Game.Instance.ObjectPool.Spawn(carInfo.Name + "VirtualBody");
                gameModel.VirtualBodyCar = Car;
                Car.transform.localPosition = Vector3.zero;
                Car.transform.localRotation = Quaternion.identity;
                //缩小汽车
                Car.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //gameModel.state = State.Show;
                //return Car;
            }
        }
           
            // return null;

    }

}
