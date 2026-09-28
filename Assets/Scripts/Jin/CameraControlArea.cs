using System.Collections;
using UnityEngine;

public class CameraControlArea : MonoBehaviour
{
    [SerializeField] private CameraOrder _cameraOrder;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private float _camChangeTime = 3f;
    private CharacterController controller;

    private bool _coroutineRunning = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out controller)) { }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && controller.velocity == Vector3.zero)
        {
            if(!_coroutineRunning)
            {
                StartCoroutine(SetCamTimer());
            }
        }
        else if(other.CompareTag("Player") && controller.velocity != Vector3.zero)
        {
            _cameraManager.SetCamera(_cameraOrder.order);
            StopCoroutine(SetCamTimer());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _cameraManager.SetCamera(_cameraOrder.order);
            StopCoroutine(SetCamTimer());
        }
    }

    private IEnumerator SetCamTimer()
    {
        _coroutineRunning = true;
        yield return new WaitForSeconds(_camChangeTime);

        _cameraManager.SetCamera(0);
        _coroutineRunning = false;
    }
}
