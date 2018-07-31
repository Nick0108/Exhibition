/* Create by @未知
 * Create time: 2018.06？
 * Discrition: 这是主入口，将Game类贯穿整个展示的各个阶段
 *             通过gameModel.state来判断展示的五个阶段（详见UpdateGameState）
 *             另外，场景加载逻辑也被写在这里了//TODO:@Zidong 这个需要优化
 * Modify：@Zidong（2018.07.27） 增加对驾驶模式的判断与处理（详见IsDriving的引用）
 */
using UnityEngine;
using System.Collections;
using System.Runtime.Remoting;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine.UI;
using GoogleARCore.Examples.Common;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

[RequireComponent(typeof(ObjectPool))]
[RequireComponent(typeof(Sound))]
[RequireComponent(typeof(StaticData))]
[RequireComponent(typeof(InputManager))]
public class Game : ApplicationBase<Game>{

	//全局访问功能
    [HideInInspector]
    public ObjectPool ObjectPool = null;//对象池
    [HideInInspector]
    public Sound Sound = null;//声音控制
    [HideInInspector]
    public StaticData StaticData = null;//静态数据
   
    public GameModel gameModel;
    private Touch oldTouch1;  //上次触摸点1(手指1)  
    private Touch oldTouch2;  //上次触摸点2(手指2)  

    /// <summary>
    /// 是否是驾驶模式
    /// </summary>
    private bool driving = false;
    public bool IsDriving
    {
        get { return driving; }
        set { driving = value; }
    }

    /// <summary>
    /// 是否隐藏云点和平台
    /// </summary>
    private bool hideCloudPoint = false;
    public bool IsHideCloudPoint
    {
        get { return hideCloudPoint; }
        set { hideCloudPoint = value; }
    }

    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
    /// </summary>

    // public Camera ExhibitionCamera; 
    //异步加载内容
    private float loadingSpeed = 1f;
    private float targetValue;
    private AsyncOperation operation;
    [HideInInspector]
    public Slider loadingSlider;//异步加载UI
    [HideInInspector]
    public Image loadingLogoImage;
    [HideInInspector]
    public Text loadingText;
    private float loadMaxtime = 1f;//加载最大时间
    private float timer = 0;

    //ARCore 使用变量
    private bool m_IsQuitting = false;
    public Camera FirstPersonCamera;//用于AR场景中的第一人称摄像机
    private TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon 
                                              | TrackableHitFlags.FeaturePointWithSurfaceNormal;//利用二进制位运算对结果进行选择
    private TrackableHit ARTrackhit;//这是ARCore专门用的射线碰撞类

    private 
    void Start()
    {
        //在切换场景时不销毁这个游戏物体
        Object.DontDestroyOnLoad(this);//与this.gameObject效果一样
        //加上场景切换委托用于发送进入场景事件
        SceneManager.sceneLoaded += OnSceneLoaded;
        //全局单例赋值
        ObjectPool = ObjectPool.Instance;
        Sound = Sound.Instance;
        StaticData = StaticData.Instance;
        //InputManager = InputManager.Instance;
        //注册启动事件
        RegisterController(Consts.E_StartUp, typeof(StartUpCommand));
        ////为启动游戏做一些处理
        SendEvent(Consts.E_StartUp);
        //开启异步加载场景2
        StartCoroutine(LoadSceneAsync(Consts.ExhibitionSceneIndex));
        //获取游戏模型
        gameModel = GetModel<GameModel>();
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
        //ARCore的Update，还没搞清楚具体作用
        _UpdateApplicationLifecycle();
        //有限状态机
        UpdateGameState();
    }

    /// <summary>
    /// 通过当前有限状态机判断当前状态并执行对应逻辑
    /// </summary>
    private void UpdateGameState()
    {
        if (gameModel != null)
        {
            switch (gameModel.state)
            {
                case State.AsyncLoadExhibition:
                    AsyncLoadSceneUpdate();
                    break;
                case State.Exhibition:
                    //ExhibitionUpdate();
                    break;
                case State.None:
                    break;
                case State.Spawn:
                    SpawnCarUpdate();//AR模式下，产生一辆虚拟的黑色车指示将要产生的车
                    break;
                case State.ARCarShow:
                    ShowCarUpdate();//AR模式下，产生车辆后的展示阶段
                    break;
            }

        }
    }

    private void ExhibitionUpdate()
    {
        if (IsDriving)
        {
            return;
        }
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
                if(gameModel.CurrentCar != null)
                {
                    gameModel.CurrentCar.RotateX(-Input.touches[0].deltaPosition.x);
                }
                
            }
        }
        else if (2 == Input.touchCount)
        {
            Touch newTouch1 = Input.GetTouch(0);
            Touch newTouch2 = Input.GetTouch(1);

            //第2点刚开始接触屏幕, 只记录，不做处理  
            if (newTouch2.phase == TouchPhase.Began)
            {
                oldTouch2 = newTouch2;
                oldTouch1 = newTouch1;
                return;
            }

            //计算老的两点距离和新的两点间距离，变大要放大模型，变小要缩放模型  
            float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
            float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);

            //两个距离之差，为正表示放大手势， 为负表示缩小手势  
            float offset = newDistance - oldDistance;

            if (gameModel.CurrentCar != null)
            {
                gameModel.CurrentCar.ChangeScale(offset);
            }
            //记住最新的触摸点，下次使用  
            oldTouch1 = newTouch1;
            oldTouch2 = newTouch2;

        }
    }
    //VirtualCar 是用一个黑色透明的汽车模型来向用户展示将要产生汽车的位置，本来是要做成长按生成VirtualCar松开后产生真汽车吧VirtualCar隐藏掉，
    //但是ARCore中的锚点根据相机位置会不断修正位置，不能通过改变锚点来来修改汽车位置，而且不能保证人们在使用时手机不会晃动（晃动会导致射线位置偏移，导致显示效果模型闪烁移动），
    //所以干脆就只通过旋转手机通过屏幕中心点射出射线来添加VirtualCar指导客户汽车将产生的位置
    private bool hasSpawnCar = false;
    [HideInInspector]
    public bool hasShowARCar = false;
    /// <summary>
    /// AR模式下，产生一辆虚拟的黑色车指示将要产生的车
    /// </summary>
    private void SpawnCarUpdate()
    {
        if (IsDriving)
        {
            return;
        }
        if (IsHideCloudPoint)
        {
            IsHideCloudPoint = false;
        }
        if (!hasSpawnCar)
        {
            SendEvent(Consts.E_SpawnCar, Consts.V_Spawner, new SpawnCarArgs(gameModel.currentSpawnCarID, true));
            gameModel.CurrentCar.HideShowCar(true);
            hasSpawnCar = true;
        }
        if (gameModel.CurrentCar == null)
            return;
        gameModel.CurrentCar.HideShowCar(true);//默认先隐藏汽车
        if (Frame.Raycast(Screen.width / 2, Screen.height / 2, raycastFilter, out ARTrackhit))
        {
            if ((ARTrackhit.Trackable is DetectedPlane) &&
                            Vector3.Dot(FirstPersonCamera.transform.position - ARTrackhit.Pose.position,
                                ARTrackhit.Pose.rotation * Vector3.up) < 0)
            {
                //当射线角度必须是0-180度，在背面不做处理
                gameModel.CurrentCar.HideShowCar(true);
            }
            else
            {
                if(!(ARTrackhit.Trackable is DetectedPlane))
                {
                    gameModel.CurrentCar.HideShowCar(true);
                    return;
                }
                if (!hasShowARCar)
                {
                    if (Input.touchCount > 0)//任意点击行为
                    {
                        gameModel.CurrentCar.HideShowCar(false);
                        gameModel.CurrentCar.SetShowRealBody(true);//这里本来想把虚拟车和真车直接放一起，通过开关显示虚拟车还是真车，目前这种模式下就不需要了
                        hasShowARCar = true;
                        gameModel.state = State.ARCarShow;//更改游戏状态为展示状态，Update中将调用ShowCarUpdate
                    }
                    else
                    {
                        gameModel.CurrentCar.HideShowCar(false);
                        gameModel.CurrentCar.SetShowRealBody(false);
                        gameModel.CurrentCar.transform.position = ARTrackhit.Pose.position;
                    }
                }
                else
                {
                    gameModel.CurrentCar.HideShowCar(false);
                    gameModel.CurrentCar.SetShowRealBody(true);
                    gameModel.CurrentCar.transform.position = ARTrackhit.Pose.position;
                }
            }

        }
    }
    /// <summary>
    /// AR模式下，产生车辆后的展示阶段
    /// </summary>
    private void ShowCarUpdate()
    {
        if(IsDriving)
        {
            return;
        }
        if (!IsHideCloudPoint)
        {
            IsHideCloudPoint = true;
        }
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
                    hit.transform.SendMessage("OnMouseDown",SendMessageOptions.DontRequireReceiver);
                }
            }
            if (touch.phase == TouchPhase.Moved)
            {
                gameModel.CurrentCar.RotateX(-Input.touches[0].deltaPosition.x);
            }
            if (touch.phase == TouchPhase.Ended)
            {

            }


        }
        else if (2 == Input.touchCount)
        {
            Touch newTouch1 = Input.GetTouch(0);
            Touch newTouch2 = Input.GetTouch(1);

            //第2点刚开始接触屏幕, 只记录，不做处理  
            if (newTouch2.phase == TouchPhase.Began)
            {
                oldTouch2 = newTouch2;
                oldTouch1 = newTouch1;
                return;
            }
            if(newTouch2.phase==TouchPhase.Moved||newTouch1.phase==TouchPhase.Moved)
            {
                //计算老的两点距离和新的两点间距离，变大要放大模型，变小要缩放模型  
                float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
                float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);

                //两个距离之差，为正表示放大手势， 为负表示缩小手势  
                float offset = newDistance - oldDistance;

                if (gameModel.CurrentCar != null)
                {
                    gameModel.CurrentCar.ChangeScale(offset);
                }
                //记住最新的触摸点，下次使用  
                oldTouch1 = newTouch1;
                oldTouch2 = newTouch2;
            }
        }
    }

    #region ARCore的demo自带代码
    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
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
    #endregion

    #region 场景加载（含异步处理）
    //TODO:@Zidong 接手的项目之前是这样写的，没有直接利用operation.progress的值，或者可以尝试优化一下？因为现在读条到100%后还要等一小段时间
    public void LoadScene(int level)
    {
        //--退出上一场景
        //事件参数
        SceneArgs e = new SceneArgs();
        e.SceneIndex = SceneManager.GetActiveScene().buildIndex;
        //发布
        SendEvent(Consts.E_ExitScene, e);

        //--加载新场景
        SceneManager.LoadScene(level, LoadSceneMode.Single);

    }
    IEnumerator LoadSceneAsync(int SceneIndex)
    {
        SceneArgs e = new SceneArgs();
        e.SceneIndex = SceneManager.GetActiveScene().buildIndex;
        SendEvent(Consts.E_ExitScene, e);

        //--加载新场景
        //SceneManager.LoadScene(SceneIndex, LoadSceneMode.Single);
        operation = SceneManager.LoadSceneAsync(SceneIndex);
        //阻止当加载完成自动切换  
        operation.allowSceneActivation = false;

        yield return operation;
    }
    private void AsyncLoadSceneUpdate()
    {
        
        if (operation == null||loadingSlider==null|| loadingLogoImage == null)
            return;
        timer += Time.deltaTime;
        targetValue = operation.progress;

        if (operation.progress >= 0.9f)
        {
            //operation.progress的值最大为0.9  
            targetValue = 1.0f;
        }
//#if UNITY_ANDROID
        if (targetValue != loadingSlider.value)
        {
            //插值运算
            loadingSlider.value = loadingLogoImage.fillAmount = Mathf.Lerp(loadingSlider.value, targetValue, Time.deltaTime * loadingSpeed);
            //loadingSlider.value = loadingLogoImage.fillAmount = Mathf.Lerp(0, loadMaxtime, Time.deltaTime * loadingSpeed);
            if (Mathf.Abs(loadingSlider.value - targetValue) < 0.01f)
            {
                loadingSlider.value = targetValue;
            }

        }
//#else
        //loadingSlider.value = Mathf.Lerp(timer, loadMaxtime, Time.deltaTime * loadingSpeed);
//#endif
        loadingText.text = ((int)(loadingSlider.value * 100)).ToString() + "%";

        if ((int)(loadingSlider.value * 100) == 100&&timer>=loadMaxtime)
        {
            //允许异步加载完毕后自动切换场景  
            operation.allowSceneActivation = true;
            loadingSlider.handleRect.gameObject.SetActive(false);
            loadingText.gameObject.SetActive(false);
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
    #endregion
}
