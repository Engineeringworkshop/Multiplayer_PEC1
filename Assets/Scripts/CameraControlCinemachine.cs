using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlCinemachine : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera camera1;
    [SerializeField] Camera camera2;
    [SerializeField] Camera camera3;
    [SerializeField] Camera camera4;

    [SerializeField] Camera cameraMiniMap;

    public void ChangeCameraLayout(int players)
    {
        // Set camera layout
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
        EnableCamera(camera1);
        camera1.rect = new Rect(0.0f, 0.5f, 0.89f, 0.5f);
        EnableCamera(camera2);
        camera2.rect = new Rect(0.11f, 0.0f, 0.89f, 0.5f);

        DisableCamera2(mainCamera);
        DisableCamera2(camera3);
        DisableCamera2(camera4);
        DisableCamera2(cameraMiniMap);
        
    }

    private void ThreePlayers()
    {
        EnableCamera(camera1);
        camera1.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
        EnableCamera(camera2);
        camera2.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        EnableCamera(camera3);
        camera3.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
        EnableCamera(cameraMiniMap);
        cameraMiniMap.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);

        DisableCamera2(mainCamera);
        DisableCamera2(camera4);
    }

    private void FourPlayers()
    {
        EnableCamera(camera1);
        camera1.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
        EnableCamera(camera2);
        camera2.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        EnableCamera(camera3);
        camera3.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
        EnableCamera(camera4);
        camera4.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);

        DisableCamera2(mainCamera);
        DisableCamera2(cameraMiniMap);
    }

    //method to disable a camera
    private void DisableCamera(Camera cam)
    {
        StartCoroutine(DisableCameraCoroutine(cam));
    }

    // Method to enamble camera
    private void EnableCamera(Camera cam)
    {
        cam.gameObject.SetActive(true);
    }

    private void DisableCamera2(Camera cam)
    {
        cam.gameObject.SetActive(false);
    }


    // Method to disable camera on next update
    private IEnumerator DisableCameraCoroutine(Camera cam)
    {
        yield return null;
        cam.gameObject.SetActive(false);
    }
}
