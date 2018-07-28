using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using GoogleARCore;
public enum State
{
    AsyncLoadExhibition,
    Exhibition,
    None,
    Spawn,
    Show,
}
public class GameModel : Model
{

    public State state = State.None;
    public bool HasFindGround = false;
    
    //所有关卡
    public override string Name
    {
        get
        {
            return Consts.M_GameModel;
        }
    }
    

   // public List<GameObject> CanSpawnCars = new List<GameObject>();
    public int currentSpawnCarID = 0;
    //已经产生的汽车
    public int SelectCarIndex = -1;
    //public Directory<int, List<CaseInsensitiveCompare> ShowedCarList = new Directory<int, CarBase[]>();
   public List<CarBase> ShowedCarList = new List<CarBase>();
    public GameObject VirtualBodyCar;
    public CarBase CurrentCar
    {
        get
        {
            if (ShowedCarList.Count > 0)
                return ShowedCarList[SelectCarIndex];
            else
                return null;
        }
    }
    public void Init()
    {
        
    }
}
