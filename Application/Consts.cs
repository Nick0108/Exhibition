using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public static class Consts
{
    //接近变量
    public const float CLOSEDISTANCE = 0.01f;
    //场景索引
    public const int InitSceneIndex = 0;
    public const int ExhibitionSceneIndex = 1;
    public const int ARSceneIndex = 2;
    //Resources路径
    public const string PrefabPath = @"Prefabs/";
    public const string RP_BMWi8refab = @"Prefabs/BMWi8";
    public const string RP_CarPrefab = @"Prefabs/BMWi3";
    //model
    public const string M_GameModel= "M_GameModel";

    //View
    //public const string V_AsyncLoadScene = "V_AsyncLoadScene";
    public const string V_UIManager = "V_UIManager";
    public const string V_BMWi8 = "V_BMWi8";
    public const string V_Spawner = " V_Spawner";
    public const string V_CarTurnTable = "V_CarTurnTable";
    //事件
    public const string E_RegisterView = "E_RegisterView";//Command
    public const string E_StartUp = "E_StartUp";//
    public const string E_SpawnCarAtHit = "E_SpawnCarAtHit";//Args  int CarID ; TrackableHit hit
    public const string E_SpawnCarAt = "E_SpawnCarAt";//Args int CarID;Vector3 position;
    public const string E_SpawnCar = "E_SpawnCar";//Args int CarID;
    public const string E_ExitScene = "E_ExitScene";//Args int SceneIndex
    public const string E_EnterScene = "E_EnterScene";
    public const string E_TurnDirection = "E_TurnDirection";//Args bool isRight;

}

