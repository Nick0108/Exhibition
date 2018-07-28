using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class Sound : ISingleton
{
    #region 单例模式Singleton
    private Sound() { }
    static Sound() { }
    private static readonly Sound _instance = new Sound();
    public static Sound Instance { get { return _instance; } }
    #endregion
    public void Init() { }

    //protected override void Awake()
    //{
    //    base.Awake();
    //    ResourceDir = "Music";
    //    m_bgSound = this.gameObject.AddComponent<AudioSource>();
    //    m_bgSound.playOnAwake = false;
    //    m_bgSound.loop = true;

    //    m_effectSound=this.gameObject.AddComponent<AudioSource>();
    //}

    public string ResourceDir = "";
    AudioSource m_bgSound;
    AudioSource m_effectSound;

    //音乐大小
    public float BgVolume
    {
        get { return m_bgSound.volume; }
        set { m_bgSound.volume = value; }
    }

    //音效大小
    public float EffectVolume
    {
        get { return m_effectSound.volume; }
        set { m_effectSound.volume = value; }
    }

    //播放音乐
    public void PlayBg(string audioName)
    {
        string oldName;
        if (m_bgSound.clip == null)
            oldName = "";
        else
            oldName = m_bgSound.clip.name;
        if (oldName != audioName)
        {
            AudioClip clip = FindAudioClip(audioName);
           //播放
            if (clip != null)
            {
                m_bgSound.clip = clip;
                m_bgSound.Play();
            }
        }
    }
    //停止音乐
    public void StopBg()
    {
        m_bgSound.Stop();
        m_bgSound.clip = null;
    }
    //播放特效
    public void PlayEfect(string audioName)
    { 
        m_effectSound.PlayOneShot(FindAudioClip(audioName));
    }

    public AudioClip FindAudioClip(string audioName)
    {
        //音乐文件路径
        string path;
        if (string.IsNullOrEmpty(ResourceDir))
            path = "";
        else
            path = ResourceDir + "/" + audioName;
        //加载音乐文件
        AudioClip clip = Resources.Load<AudioClip>(path);
        return clip;
    }
}

