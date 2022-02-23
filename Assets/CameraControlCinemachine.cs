using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlCinemachine : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [HideInInspector] LayerMask mainCameraLayerMask;
    [SerializeField] Camera camera1;
    [HideInInspector] LayerMask camera1LayerMask;
    [SerializeField] Camera camera2;
    [HideInInspector] LayerMask camera2LayerMask;
    [SerializeField] Camera camera3;
    [HideInInspector] LayerMask camera3LayerMask;
    [SerializeField] Camera camera4;
    [HideInInspector] LayerMask camera4LayerMask;

    [SerializeField] Camera cameraMiniMap;
    [HideInInspector] LayerMask cameraMiniMapLayerMask;


    private void Start()
    {
        // Store culling masks 
        mainCameraLayerMask = mainCamera.cullingMask;
        camera1LayerMask = camera1.cullingMask;
        camera2LayerMask = camera2.cullingMask;
        camera3LayerMask = camera3.cullingMask;
        camera4LayerMask = camera4.cullingMask;
        cameraMiniMapLayerMask = cameraMiniMap.cullingMask;
    }

    public void ChangeCameraLayout(int players)
    {
        // Set Layout
        if (players == 2)
        {
            TwoPlayers();
        }
        else if (players == 3)
        {
            ThreePlayers();
        }
        else if (players == 4)
        {
            FourPlayers();
        }
    }

    private void TwoPlayers()
    {
        EnableCamera(camera1, camera1LayerMask);
        camera1.rect = new Rect(0.0f, 0.5f, 0.89f, 0.5f);
        EnableCamera(camera2, camera2LayerMask);
        camera2.rect = new Rect(0.11f, 0.0f, 0.89f, 0.5f);

        DisableCamera(mainCamera);
        DisableCamera(camera3);
        DisableCamera(camera4);
        DisableCamera(cameraMiniMap);
        
    }

    private void ThreePlayers()
    {
        EnableCamera(camera1, camera1LayerMask);
        camera1.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
        EnableCamera(camera2, camera2LayerMask);
        camera2.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        EnableCamera(camera3, camera3LayerMask);
        camera3.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
        EnableCamera(cameraMiniMap, cameraMiniMapLayerMask);
        cameraMiniMap.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);

        DisableCamera(mainCamera);
        DisableCamera(camera4);
    }

    private void FourPlayers()
    {
        EnableCamera(camera1, camera1LayerMask);
        camera1.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
        EnableCamera(camera2, camera2LayerMask);
        camera2.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        EnableCamera(camera3, camera3LayerMask);
        camera3.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
        EnableCamera(camera4, camera4LayerMask);
        camera4.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);

        DisableCamera(mainCamera);
        DisableCamera(cameraMiniMap);
    }

    // method to disable all cameras.
    private void DisableAllCameras()
    {
        DisableCamera(mainCamera);
        DisableCamera(camera1);
        DisableCamera(camera2);
        DisableCamera(camera3);
        DisableCamera(camera4);
        DisableCamera(cameraMiniMap);
    }

    //method to disable a camera
    private void DisableCamera(Camera cam)
    {
        cam.cullingMask = 0;
        StartCoroutine(DisableCameraCoroutine(cam));
    }

    // Method to enamble camera
    private void EnableCamera(Camera cam, LayerMask mask)
    {
        cam.gameObject.SetActive(true);
        cam.cullingMask = mask;
    }

    // Method to disable camera on next update
    private IEnumerator DisableCameraCoroutine(Camera cam)
    {
        yield return null;
        cam.gameObject.SetActive(false);
    }
}
