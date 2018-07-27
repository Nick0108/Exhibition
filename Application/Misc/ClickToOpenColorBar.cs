using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClickToOpenColorBar : MonoBehaviour {
    private GameObject ColorBar;
    private HorizontalLayoutGroup layoutGroup;
    private bool isColorBarShow = false;
    public float minSpacing = -1.83f;
    public float smooth = 1;
    public float maxSpacing = 0;
    private float targetSpacing = 0;
    private bool isChanging = false;
    public float Unusetime = 0.1f;
    private float timer = 0;
	// Use this for initialization
	void Start () {
        ColorBar = transform.Find("ColorBar").gameObject;
        ColorBar.SetActive(isColorBarShow);
        layoutGroup = ColorBar.GetComponent<HorizontalLayoutGroup>();
	}

    // Update is called once per frame
    private void OnMouseDown()
    {
        if(timer>Unusetime)
        {
            isColorBarShow = !isColorBarShow;
            if (isColorBarShow)
            {
                ColorBar.SetActive(true);
                targetSpacing = maxSpacing;
            }
            else
                targetSpacing = minSpacing;
            isChanging = true;
            timer = 0;
        }
       
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(isChanging)
        {
            float spacing;
            if (isColorBarShow)
                spacing = layoutGroup.spacing + smooth * Time.deltaTime;
            else
                spacing = layoutGroup.spacing - smooth * Time.deltaTime;

            if ((isColorBarShow&&spacing>targetSpacing)||(!isColorBarShow && spacing < targetSpacing))
            {
                layoutGroup.spacing = targetSpacing;
                isChanging = false;
                if(!isColorBarShow)
                    ColorBar.SetActive(false);
                return;
            }
            layoutGroup.spacing = spacing;
        }
    }
}
