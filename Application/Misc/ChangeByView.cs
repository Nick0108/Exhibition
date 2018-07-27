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
        if(cameraTransform==null)
            cameraTransform = Camera.main.transform;
        Vector3 carTothis = (transform.position - AssistPoint.position).normalized;
        Vector3 thisToCamera = (cameraTransform.position - transform.position).normalized;
        CurrentViewAngle = Mathf.Rad2Deg*Mathf.Acos(Vector3.Dot(carTothis, thisToCamera));
        IsInView = CurrentViewAngle < ShowAngle ? true : false;
        if(isInView)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, OriginalScale, (ShowAngle - CurrentViewAngle) / (ShowAngle - MaxSizeAngle));
        }
}
}
