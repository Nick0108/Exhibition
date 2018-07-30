/* Create by @LinziDong
 * Create time: 2018.07.24
 * Discrition: 这是一个用于缓存以及获取Image的小脚本
 *             //TODO:@Zidong 未来可以进一步扩展
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour {

    public Sprite[] Sprites;
    [SerializeField]
    private int curIndex = 0;

    /// <summary>
    /// 通过Index获取缓存的Image
    /// </summary>
    /// <param name="pIndex"></param>
    /// <returns></returns>
    public Sprite GetSpriteByIndex(int pIndex)
    {
        if (Sprites.Length > pIndex)
        {
            return Sprites[pIndex];
        }
        else
        {
            Debug.LogError("获取图片index出错");
            return null;
        }
    }

    /// <summary>
    /// 依照既定顺序获取缓存的Image
    /// </summary>
    /// <returns></returns>
    public Sprite GetSpriteByOrder()
    {
        curIndex++;
        if(curIndex >= Sprites.Length)
        {
            curIndex = 0;
        }
        return Sprites[curIndex];
    }
}
