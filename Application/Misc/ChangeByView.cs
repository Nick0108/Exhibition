/* Create by @未知
 * Create time: 2018.06？
 * Discrition: 根据摄像机与挂载该脚本的UI的视角进行对比，通过控制UI的大小与朝向进行显隐
 * Modify：@Zidong（2018.07.27） 增加对AR模式的判断，由于AR下摄像机为手持模式，需要重新设计显隐方式（主要是视角与UI大小不匹配）
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeByView : MonoBehaviour {
    private float CurrentViewAngle;
    private bool isInView;

    public Sprite InViewSprite;
    public Sprite OutViewSprite;
    
    private SpriteRenderer parentSpriteRender;
    public Transform AssistPoint;
    private Transform cameraTransform;
    public float ShowAngle=60;
    public float MaxSizeAngle = 30;
    public float ARSceneSeeDistance = 0.5f;
    // Use this for initialization
    private Vector3 OriginalScale;

    public bool IsInView
    {
        get
        {
            return isInView;
        }

        set
        {
            if (IsInView!=value)
            {
                if (value)
                    parentSpriteRender.sprite = InViewSprite;
                else
                    parentSpriteRender.sprite = OutViewSprite;
                isInView = value;
            }
        }
    }

    private void Start()
    {
        parentSpriteRender = transform.parent.GetComponent<SpriteRenderer>();
        cameraTransform = Camera.main.transform;
        OriginalScale = transform.localScale;
        transform.localScale = Vector3.zero;
    }
    // Update is called once per frame
    void Update () {
        //切换场景后摄像机丢失
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        if (IsInARScence())
        {
            //TODO:@Zidong 额，好像不生效，现在一直都是UI的原始尺寸，待优化与修改
            transform.localScale = Vector3.Lerp(Vector3.zero, OriginalScale, Vector3.Distance(transform.position, Game.Instance.FirstPersonCamera.transform.position) / ARSceneSeeDistance);
            //transform.localScale = OriginalScale;
            return;
        }
        Vector3 carTothis = (transform.position - AssistPoint.position).normalized;
        Vector3 thisToCamera = (cameraTransform.position - transform.position).normalized;
        CurrentViewAngle = Mathf.Rad2Deg*Mathf.Acos(Vector3.Dot(carTothis, thisToCamera));
        IsInView = CurrentViewAngle < ShowAngle ? true : false;
        if(isInView)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, OriginalScale, (ShowAngle - CurrentViewAngle) / (ShowAngle - MaxSizeAngle));
        }
        
    }

    private bool IsInARScence()
    {
        return Game.Instance.gameModel.state == State.ARCarShow;
    }
}
