using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public abstract class View:MonoBehaviour
{
    //视图的标识
    public abstract string Name { get; }

    //关心的事件列表
    [HideInInspector]
    public List<string> AttationEvents=new List<string>();
    //注册关心的事件
    public abstract void RegisterEvents();
    
    //事件处理函数
    public abstract void HandleEvent(string eventName, object data);

    //获取模型
    protected T GetModel<T>()
        where T : Model
    {
        return MVC.GetModel<T>()as T;
    }

    //发送消息
    protected void SendEvent(string eventNmae, object data = null)
    {
        MVC.SendEvent(eventNmae, data);
    }
}

