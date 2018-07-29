using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager:MonoBehaviour  {
   
    public string message;
    public int messageArg;
    public float UnuseTime = 0.1f;
    public AudioSource AudioSource = null;
    public AudioClip[] AudioClips = null;
    public float[] SoundDelayTime = new float[] { 0.0f, 0.0f };

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
            if (AudioSource != null)
            {
                if (AudioClips.Length > 0 && isopen)
                {
                    AudioSource.clip = AudioClips[0];
                }
                else if(AudioClips.Length >1 && !isopen)
                {
                    AudioSource.clip = AudioClips[1];
                }
                else
                {
                    AudioSource.clip = null;
                    return;
                }
                AudioSource.PlayDelayed(SoundDelayTime[isopen?0:1]);
            }
        }
      
    }
    private void Update()
    {
        timer += Time.deltaTime;
    }
}
