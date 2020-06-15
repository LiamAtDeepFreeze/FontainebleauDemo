using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSwitcher : MonoBehaviour
{
    public Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        CameraState.AddCamera(_camera);   
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        _camera = GetComponent<Camera>();
    }
#endif
}

public static class CameraState
{
    public static Camera currentCamera;

    public static void AddCamera(Camera newCamera)
    {
        if (currentCamera == null)
        {
            currentCamera = newCamera;
            return;
        }
        
        GameObject.Destroy(currentCamera.gameObject);
        currentCamera = newCamera;
    }
}
