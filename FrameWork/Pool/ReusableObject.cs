using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public abstract class ReusableObject : MonoBehaviour ,IReusable{

    public abstract void OnSpawn();
    public abstract void OnUnSpawn();

}
