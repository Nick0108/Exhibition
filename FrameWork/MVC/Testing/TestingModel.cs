using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingModel: ISingleton
{
    private TestingModel() { }
    static TestingModel() { }
    private static readonly TestingModel _instance = new TestingModel();
    public static TestingModel Instance { get { return _instance; } }

    public void Init()
    {

    }
}
