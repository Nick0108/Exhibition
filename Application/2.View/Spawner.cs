/* Create by @未知
 * Create time: 2018.06？
 * Discrition: 根据handleEvent传入的参数产生对应的汽车
 * Modify：@Zidong（2018.07.31） 注释掉已经不再用的方法与判断
 */
using UnityEngine;
using System.Collections;
using GoogleARCore;
public class Spawner : View
{
    GameModel gameModel;
    public override string Name
    {
        get { return Consts.V_Spawner; }
    }
  
    public override void RegisterEvents()
    {
        AttationEvents.Add(Consts.E_SpawnCar);
        //AttationEvents.Add(Consts.E_SpawnCarAt);
        //AttationEvents.Add(Consts.E_SpawnCarAtHit);
        //AttationEvents.Add(Consts.E_EnterScene);
    }

    public override void HandleEvent(string eventName, object data)
    {
        switch (eventName)
        {

            //case Consts.E_SpawnCarAtHit://AR场景根据射线产生的碰撞信息来产生汽车
            //    SpawnCarAtHitArgs e0 = data as SpawnCarAtHitArgs;
            //    SpawnCar(e0.CarID, e0.Hit);
            //    DrivingModel.Instance.HideCloudPoint = true;
            //    break;
            
            case Consts.E_SpawnCar://用在展示汽车模型时调用，AR模式下isRealBody为false来产生汽车阴影
                SpawnCarArgs e2 = data as SpawnCarArgs;
                SpawnCar(e2.CarID,e2.isInARScene);
                break;
            //case Consts.E_EnterScene://进入场景
            //    SceneArgs e3 = data as SceneArgs;
            //    switch (e3.SceneIndex)
            //    {
            //        case Consts.ExhibitionSceneIndex:
            //            //gameModel = GetModel<GameModel>();
            //            break;
            //    }
            //    break;
            //case Consts.E_SpawnCarAt://在世界坐标系中产生汽车
            //    SpawnCarAtArgs e1 = data as SpawnCarAtArgs;
            //    SpawnCar(e1.CarID, e1.position);
            //    break;
        }
    }
    public void Start()
    {
        gameModel = GetModel<GameModel>();
        SendEvent(Consts.E_RegisterView, this);
    }
    ///// <summary>
    ///// AR场景根据射线产生的碰撞信息来产生汽车
    ///// </summary>
    ///// <param name="CarID"></param>
    ///// <param name="hit"></param>
    //public void SpawnCar(int CarID, TrackableHit hit)
    //{
    //    CarInfo carInfo = StaticData.Instance.GetCarInfo(CarID);
    //    if (carInfo != null)
    //    {
    //        GameObject Car = Game.Instance.ObjectPool.Spawn(carInfo.Name);
    //        var anchor = hit.Trackable.CreateAnchor(hit.Pose);
    //        Car.transform.parent = anchor.transform;

    //        //gameModel.ShowedCarList.Add(Car.GetComponent<CarBase>());
    //        gameModel.SelectARCarIndex = gameModel.ShowedCarList.Count - 1;//产生一个新的之后自动选择该汽车
    //        //状态重置
    //        Car.transform.localPosition = Vector3.zero;
    //        Car.transform.localRotation = Quaternion.identity;
    //        //缩小汽车
    //        Car.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    //        Car.GetComponent<CarBase>().originalScale = Car.transform.localScale;
    //        Car.GetComponent<CarBase>().SetShowRealBody(true);
    //        //gameModel.state = State.Show;
    //        //return Car;
    //    }
    //    Debug.Log("SpawnCar(int CarID, TrackableHit hit)");
    //    // return null;
    //}

    /// <summary>
    /// 用在展示汽车模型和AR时调用，AR模式下isRealBody为false来产生汽车阴影
    /// </summary>
    /// <param name="CarID"></param>
    /// <param name="isRealBody">是否是黑色幻影的Car影</param>
    public void SpawnCar(int CarID, bool isInARScene = false)
    {
        if (isInARScene)
        {
            //AR场景不走对象池，因为不可复用
            CarInfo carInfo = StaticData.Instance.GetCarInfo(CarID);
            if (carInfo != null)
            {
                GameObject Car = Instantiate(Resources.Load<GameObject>(carInfo.Path));
                //gameModel.SelectARCarIndex = gameModel.ShowedCarList.Count - 1;//产生一个新的之后自动选择该汽车
                Car.GetComponent<CarBase>().ResetCarToSpawn();
                ARModel.Instance.CurrentARCar = Car.GetComponent<CarBase>();
                Debug.Log(string.Format("SpawnCar(int CarID,bool isInARScene={0})", isInARScene));
            }
        }
        else
        {
            SpawnCar(CarID);
        }
    }

    private void SpawnCar(int CarID)
    {
        CarInfo carInfo = StaticData.Instance.GetCarInfo(CarID);
        if (carInfo != null)
        {
            Game.Instance.ObjectPool.Spawn(carInfo.Name);
        }
        Debug.Log("SpawnCar(int CarID)");
        // return null;
    }

    //public void SpawnCar(int CarID ,Vector3 position)
    //{
    //    CarInfo carInfo = StaticData.Instance.GetCarInfo(CarID);
    //    if (carInfo != null)
    //    {
    //        GameObject Car = Game.Instance.ObjectPool.Spawn(carInfo.Name);
    //        Car.transform.position = position;
    //       // return Car;
    //    }
    //    Debug.Log("SpawnCar(int CarID ,Vector3 position)");
    //    //return null;
    //}


}
