using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class ObjectPool:Singleton<ObjectPool>
{
    public string ResourceDir = "";

    Dictionary<string, SubPool> m_pools = new Dictionary<string, SubPool>();

    //取对象
    public GameObject Spawn(string name)
    {
        if (!m_pools.ContainsKey(name))
            RegisterNew(name);
        SubPool pool = m_pools[name];
        return pool.Spawn();
        

    }
    //回收对象
    public void UnSpawn(GameObject go)
    {
        SubPool pool = null;
        foreach (SubPool p in m_pools.Values )
        {
            if (p.IsContains(go))
            {
                pool = p;
                break;
            }
        }
        pool.UnSpawn(go);
    }

    //回收所有对象
    public void UnSpawnAll()
    {
        foreach (SubPool p in m_pools.Values)
            p.UnSpawnAll();
    }

    //创建新子池子
    void RegisterNew(string name)
    {
        string path = "";
        if (string.IsNullOrEmpty(ResourceDir))
            path = name;
        else
            path = ResourceDir + "/" + name;
        GameObject prefab = Resources.Load<GameObject>(path);
        SubPool pool = new SubPool(prefab);
        m_pools.Add(pool.Name, pool);
    }
}
