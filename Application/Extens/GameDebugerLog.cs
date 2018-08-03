using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDebugerLog : MonoBehaviour
{
////#if UNITY_EDITOR
//    #region 单例模式Singleton
//    private GameDebugerLog() { }
//    static GameDebugerLog() { }
//    private static readonly GameDebugerLog _instance = new GameDebugerLog();
//    public static GameDebugerLog Instance { get { return _instance; } }
//    #endregion

//    public GameObject TipOG;
//    private GameObject Tips;
//    private float timer;

//    public void AddTipDebugLog(string log)
//    {
        
//        if (Tips == null)
//        {
//            if (TipOG == null)
//            {
//                TipOG = Resources.Load<GameObject>("Prefabs/DebugLog");
//            }
//            var GO = Instantiate(TipOG);
//            Tips = GO;
//            GO.transform.Find("LogText").GetComponent<Text>().text = log;
//        }
//        else
//        {
//            Text tText = Tips.transform.Find("LogText").GetComponent<Text>();
//            tText.text += string.Format("\n"+log);
//        }
        
//    }
//    //#endif
}
