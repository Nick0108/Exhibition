using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CarInfo
{
    public int ID;
    public string Name;
    private string path;

    public string Path
    {
        get
        {
            return Consts.PrefabPath + Name;
        }
    }
}