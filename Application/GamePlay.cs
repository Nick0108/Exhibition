using UnityEngine;
using System.Collections;
using System.Runtime.Remoting;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class GamePlay : ApplicationBase<GamePlay>
{
    //全局访问的数据储存
    ISingleton[] AllSingletonData = 
    {
        StaticData.Instance,
        ObjectPool.Instance,
        Sound.Instance,
        InputManager.Instance
    };

    public GameModel gameModel;

    // public Camera ExhibitionCamera; 
    //异步加载内容
    private float loadingSpeed = 1;
    private float targetValue;
    private AsyncOperation operation;
    [HideInInspector]
    public Slider loadingSlider;//异步加载UI
    [HideInInspector]
    public Text loadingText;
    private float loadMaxtime = 1;//加载最大时间
    private float timer = 0;

    //ARCore 使用变量
    private bool m_IsQuitting = false;
    public Camera FirstPersonCamera;//用于AR场景中的第一人称摄像机

    private void Start()
    {
        //在切换场景时不销毁这个游戏物体
        Object.DontDestroyOnLoad(this);//与this.gameObject效果一样
        //初始化全局单例（数据）
        foreach(ISingleton I in AllSingletonData)
        {
            I.Init();
        }
        //加上场景切换委托用于发送进入场景事件
        SceneManager.sceneLoaded += OnSceneLoaded;
        //注册启动事件
        RegisterController(Consts.E_StartUp, typeof(StartUpCommand));
        ////为启动游戏做一些处理
        SendEvent(Consts.E_StartUp);
        //开启异步加载场景2

        //获取游戏模型
        gameModel = GetModel<GameModel>();
        InputManager.Instance.gameModel = GetModel<GameModel>();
        //设置游戏状态为异步加载Exhibition场景
        gameModel.state = State.AsyncLoadExhibition;
        //添加UI组件
        this.gameObject.AddComponent<UIManager>();
        //添加工厂
        this.gameObject.AddComponent<Spawner>();
    }

    //private CarBase car;

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {

    }
    private void ExhibitionUpdate()
    {
        Touch touch;
        if (1 == Input.touchCount)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;

                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

#if UNITY_EDITOR
                Debug.Log("biubiubiu");
                Debug.Log(Input.touchCount);
#endif
                if (Physics.Raycast(ray, out hit))
                {
                    hit.transform.SendMessage("OnMouseDown");//这里做了统一处理，在电脑上和安卓端都用OnMouseDown方法来做点击事件
#if UNITY_EDITOR
                    Debug.Log(hit.transform.gameObject.name);
#endif

                }

            }
            if (touch.phase == TouchPhase.Moved)
            {
#if UNITY_EDITOR
                Debug.Log(Input.touches[0].deltaPosition.x);
#endif
                if (gameModel.CurrentCar != null)
                {
                    gameModel.CurrentCar.RotateX(-Input.touches[0].deltaPosition.x);
                }

            }
        }
        else if (2 == Input.touchCount)
        {
            InputManager.Instance.UpdateTouch();

        }
    }
    

    private bool hasSpawnVirtualCar = false;
    private void SpawnCarUpdate()
    {
        
    }

    private void ShowCarUpdate()
    {
        if (Input.touchCount <= 0)
            return;
        Touch touch;
        if (1 == Input.touchCount)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;

                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);


                if (Physics.Raycast(ray, out hit))
                {
                    hit.transform.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
                    //#if UNITY_EDITOR
                    //                    Debug.Log(hit.transform.gameObject.name);
                    //#endif

                }

            }
            if (touch.phase == TouchPhase.Moved)
            {
                //#if UNITY_EDITOR
                //                Debug.Log(Input.touches[0].deltaPosition.x);
                //#endif
                gameModel.CurrentCar.RotateX(-Input.touches[0].deltaPosition.x);
            }
            if (touch.phase == TouchPhase.Ended)
            {
                InputManager.Instance.UpdateTouch2();

            }


        }
        else if (2 == Input.touchCount)
        {
            
            
        }
    }



    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }


    private void AsyncLoadSceneUpdate()
    {

        if (operation == null || loadingSlider == null)
            return;
        timer += Time.deltaTime;
        targetValue = operation.progress;

        if (operation.progress >= 0.9f)
        {
            //operation.progress的值最大为0.9  
            targetValue = 1.0f;
        }
#if UNITY_ANDROID
            if (targetValue != loadingSlider.value)
            {
                //插值运算
                loadingSlider.value = Mathf.Lerp(loadingSlider.value, targetValue, Time.deltaTime * loadingSpeed);
                if (Mathf.Abs(loadingSlider.value - targetValue) < 0.01f)
                {
                    loadingSlider.value = targetValue;
                }

            }
#endif
#if UNITY_EDITOR
        loadingSlider.value = Mathf.Lerp(timer, loadMaxtime, Time.deltaTime * loadingSpeed);
#endif
        loadingText.text = ((int)(loadingSlider.value * 100)).ToString() + "%";

        if ((int)(loadingSlider.value * 100) == 100 && timer >= loadMaxtime)
        {
            //允许异步加载完毕后自动切换场景  
            operation.allowSceneActivation = true;

            gameModel.state = State.Exhibition;
        }
    }
    //用于给
    void OnSceneLoaded(Scene scene, LoadSceneMode Mode)
    {
        //事件参数
        SceneArgs e = new SceneArgs();
        e.SceneIndex = scene.buildIndex;
        //发布事件
        SendEvent(Consts.E_EnterScene, e);

    }
}
