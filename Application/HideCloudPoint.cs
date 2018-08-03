/* Create by @LinziDong
 * Create time: 2018.07.27
 * Discrition: 这是一个用于隐藏AR模式下的云点与识别平台的脚本
 *             //TODO:@Zidong 其实后来发现可以用GoogleARCore.ARCoreSession中的Awake中来缓存，
 *                     然后用GoogleARCore.ARCoreSession.SessionConfig
 *                     的EnableCloudAnchor和EnablePlaneFinding来控制，后面有空再尝试
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCloudPoint : MonoBehaviour {

    public GameObject Plane;
    public GameObject CloudPoint;

    private bool LastShowCloud = false;
	// Use this for initialization
	void Start () {
        //Game.Instance.gameModel.state = State.Spawn;
    }
	
	// Update is called once per frame
	void Update () {
        if(LastShowCloud != Game.Instance.IsHideCloudPoint)
        {
            if (Game.Instance.IsHideCloudPoint)
            {
                Plane.SetActive(false);
                CloudPoint.SetActive(false);
            }
            else
            {
                Plane.SetActive(true);
                CloudPoint.SetActive(true);
            }
            LastShowCloud = Game.Instance.IsHideCloudPoint;
        }
	}
}
