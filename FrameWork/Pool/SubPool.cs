using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class SubPool
{
    //预设
    GameObject m_prefab;
    //集合
    List<GameObject>m_objects=new List<GameObject>(); 
    //名字标识
    public string Name
    {
        get { return m_prefab.name; }
    }

    public SubPool(GameObject prefab)
    {
        m_prefab = prefab;
    }
    //取出对象
    public GameObject Spawn()
    {
        GameObject go = null;
        foreach (GameObject obj in m_objects)
        {
            //选择1，重玩关卡时如果不绑定Game上所有对象都没了直接清空
            if (obj == null)
            {
                m_objects.Clear();
                break;
            }
               
            if (!obj.activeSelf)
            {
                go = obj;
                break;
            }
        }
        if (go == null)
        {
            go = GameObject.Instantiate<GameObject>(m_prefab);
            m_objects.Add(go);
            //选择2，创建物体是将其挂到Game上保证在重载关卡时不消失

            ////Transform parent = Game.Instance.transform.Find(m_prefab.name);
            //if (parent != null)
            //{
            //    go.transform.parent = parent;
            //}
            //else
            //{
            //    GameObject parentgo=new GameObject();
            //    parentgo.name = m_prefab.name;
            //    //parentgo.transform.parent = Game.Instance.transform;
            //    parent = parentgo.transform;
            //}
            //go.transform.parent = parent;
        }
        go.SetActive(true);
        go.SendMessage("OnSpawn",SendMessageOptions.DontRequireReceiver);
        return go;
    }
    //回收对象
    public void UnSpawn(GameObject go)
    {
        if (IsContains(go))
        {
            go.SendMessage("OnUnSpawn", SendMessageOptions.DontRequireReceiver);
            go.SetActive(false);
            //选择2，创建物体是将其挂到Game上保证在重载关卡时不消失
            //在ARcore中物体的父物体会被放到锚点上，回收时放回到自己下面
            Transform parent = GamePlay.Instance.transform.Find(m_prefab.name);
            if (parent != null)
            {
                go.transform.parent = parent;
            }
            else
            {
                GameObject parentgo = new GameObject();
                parentgo.name = m_prefab.name;
                parentgo.transform.parent = GamePlay.Instance.transform;
                parent = parentgo.transform;
            }
            go.transform.parent = parent;
        }
    }

    public void UnSpawnAll()
    {
        foreach (GameObject item in m_objects)
        {
            if (item.activeSelf)
            {
                UnSpawn(item);
            }
        }
    }

    public bool IsContains(GameObject go)
    {
        return m_objects.Contains(go);
    }

}

