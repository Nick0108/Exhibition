using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnterSceneCommand : Controller
{
    public override void Execute(object data)
    { 
        SceneArgs e=data as SceneArgs;
        
        //注册视图 View
        switch (e.SceneIndex)
        {
            case Consts.ExhibitionSceneIndex:

                //Game.Instance.Sound.PlayBg("BGStart");
                //GameObject obj = GameObject.Find("CarTurnTable");
                
                break;
            case Consts.ARSceneIndex:
                Game.Instance.FirstPersonCamera = GameObject.Find("ARCore Device/First Person Camera").GetComponent<Camera>();
                //Game.Instance.Sound.PlayBg("BGStart");
                break;
        }
    }
}
