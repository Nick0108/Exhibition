using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusOnCamera : MonoBehaviour {

    private Transform m_camera;
    private void Start()
    {
        m_camera = Camera.main.transform;
    }
    // Update is called once per frame
    void Update () {
        transform.LookAt(Camera.main.transform);
	}
}
