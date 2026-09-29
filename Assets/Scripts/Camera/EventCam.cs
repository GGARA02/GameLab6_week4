using System;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class EventCam : MonoBehaviour
{
    [SerializeField]
    private bool isOnEnable;
    [SerializeField]
    private bool isOnDisable;
    [SerializeField]
    private bool isOnDestroy;
    [SerializeField]
    private bool isTriggerEnter;
    [SerializeField]
    private bool isTriggerExit;
    [SerializeField]
    private CameraOrder cam;
    [SerializeField]
    private float duration;

    private CameraManager cameraManager;

    void Start()
    {
        cameraManager = FindFirstObjectByType<CameraManager>();
    }
    private void OnEnable()
    {
        if (isOnEnable)
            cameraManager.SetCamTemp(cam.order, duration);
    }
    private void OnDisable()
    {
        if (isOnDisable)
            cameraManager.SetCamTemp(cam.order, duration);
    }

    private void OnDestroy()
    {
        if (isOnDestroy)
            cameraManager.SetCamTemp(cam.order, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isTriggerEnter)
            cameraManager.SetCamTemp(cam.order, duration);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isTriggerExit)
            cameraManager.SetCamTemp(cam.order, duration);
    }
}
