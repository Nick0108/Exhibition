using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GoogleARCore;
public enum State
{
    AsyncLoadExhibition,
    Exhibition,
    None,
    Spawn,
    ARCarShow,
}
public class GameModel : Model
{

    public State state = State.None;
    
    //所有关卡
    public override string Name
    {
        get
        {
            return Consts.M_GameModel;
        }
    }


    //public List<GameObject> CanSpawnCars = new List<GameObject>();


    //public int SelectARCarIndex = 0;
    //public Directory<int, List<CaseInsensitiveCompare> ShowedCarList = new Directory<int, CarBase[]>();
    //public GameObject VirtualBodyCar;
    //public bool HasFindGround = false;

    //已经产生的汽车
    
    //public int currentSpawnCarID = 0;
    //public CarBase CurrentCar
    //{
    //    get
    //    {
    //        if (ShowedCarList.Count > 0)
    //        {
    //            return ShowedCarList[ExhibitionModel.Instance.ExhibitingCarIndex];
    //        }
    //        else
    //        {
    //            Debug.LogError("该汽车的SelectARCarIndex不能使用");
    //            return null;
    //        }
    //    }
    //}

    //public void Init()
    //{
        
    //}
    //public CarBase GetCar(int index)
    //{
    //    if (ShowedCarList.Count > 0)
    //        return ShowedCarList[index];
    //    else
    //        return null;
    //}
}
