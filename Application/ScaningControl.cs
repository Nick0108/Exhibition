/* Create by @LinziDong
 * Create time: 2018.08.02
 * Discrition: 这是一个独立控制器，用来控制扫描时的UI效果
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaningControl : MonoBehaviour {

    public Image GreenOKImage;
    public GameObject OKButton;
    public Text SearchingText;

    public Image ScaningLoading;
    public Text ScaningText;
    public Image ScanCenter;

    private Vector3 rotateSpeed = new Vector3(0, 0, -1.0f);
    private Color RedColor = new Color(255f, 0, 0);
    private Color GreenColor = new Color(0, 255f, 0);

    private float colorChangeTimer = 0.0f;
    //public bool testingON = false;

    private void Start()
    {
        
    }

    void OnEnable ()
    {

    }

    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update ()
    {
        ScaningLoading.rectTransform.Rotate(rotateSpeed);
        RefreshScanView(ARModel.Instance.CanPlaceObject);

        //if (testingON)
        //{
        //    ARModel.Instance.CanPlaceObject = true;
        //}
        //else
        //{
        //    ARModel.Instance.CanPlaceObject = false;
        //}
    }

    private void RefreshScanView(bool isCanPlace)
    {
        if (isCanPlace)
        {

            colorChangeTimer += (Time.deltaTime * 2);
        }
        else
        {
            colorChangeTimer -= (Time.deltaTime * 2);
        }
        colorChangeTimer = Mathf.Clamp01(colorChangeTimer);
        ScaningLoading.color = Color.Lerp(RedColor, GreenColor, colorChangeTimer);
        ScaningText.color = Color.Lerp(RedColor, GreenColor, colorChangeTimer);
        ScanCenter.color = Color.Lerp(RedColor, GreenColor, colorChangeTimer);
        GreenOKImage.color = new Color(GreenOKImage.color.r, GreenOKImage.color.g, GreenOKImage.color.b, colorChangeTimer);
        float tSize = Mathf.Clamp(colorChangeTimer, 0.1f, 1f);
        GreenOKImage.rectTransform.localScale = new Vector3(1 / tSize, 1 / tSize, 1 / tSize);
        SearchingText.gameObject.SetActive(!isCanPlace);
        OKButton.gameObject.SetActive(isCanPlace);
        ScaningText.gameObject.SetActive(!isCanPlace);
        ScanCenter.gameObject.SetActive(true);
        GreenOKImage.gameObject.SetActive(isCanPlace);
    }
}
