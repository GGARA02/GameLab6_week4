using Unity.VisualScripting;
using UnityEngine;

public class TriggerCam : MonoBehaviour
{
    private CameraManager cameraManager;
    [SerializeField]
    private CameraOrder cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraManager = FindFirstObjectByType<CameraManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            cameraManager.SetCamera(cam.order);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            cameraManager.SetCamera(0);
    }
}
