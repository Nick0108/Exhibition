/* Create by @LinziDong
 * Create time: 2018.07.24
 * Discrition: 这是挂载在油门UI下的一个脚本，主要用于控制是否踩着油门
 *             通过外部的EventTrigger的OnPointUp和OnMouseDown来判断用户是否正在按着油门键
 *             //TODO@Zidong 这种通过拖动脚本到外部EventTrigger的实现方式真的很low很烂，但是由于项目时间紧张，先这么处理，未来一定要将其优化掉！！
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccebrateController : MonoBehaviour {

	public void ChangeAccebrateState(bool isOn)
    {
        DrivingModel.Instance.CarRuning = isOn;
    }
}
