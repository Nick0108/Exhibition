using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager:MonoBehaviour  {
   
    public string message;
    public int messageArg;
    public float UnuseTime = 0.1f;
    private float timer;
    private bool isopen=false;

    public bool isChangeSprite;
    public Sprite OpenSprite;
    public Sprite CloseSprite;
    private SpriteRenderer parentSpriteRenderer;
    private void Start()
    {
        if (isChangeSprite)
        parentSpriteRenderer = GetComponentInParent<SpriteRenderer>();
    }
    // Use this for initialization
    private void OnMouseDown()
    {
       if (timer>UnuseTime)
        {
            SendMessageUpwards(message, messageArg, SendMessageOptions.RequireReceiver);
            isopen = !isopen;
            if (isChangeSprite)
                parentSpriteRenderer.sprite = isopen ? CloseSprite : OpenSprite;
            timer = 0;
        }
      
    }
    private void Update()
    {
        timer += Time.deltaTime;
    }
}
