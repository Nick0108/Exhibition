using UnityEngine;
using System.Collections;

public interface IReusable
{
    //产生物体
    void OnSpawn();
    //回收物体
    void OnUnSpawn();
}
