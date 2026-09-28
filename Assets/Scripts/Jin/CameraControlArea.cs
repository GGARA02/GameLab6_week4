using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraControlArea : MonoBehaviour
{
    [SerializeField] private CameraOrder _cameraOrder;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private float _camChangeTime = 3f;
    private CharacterController controller;

    private Coroutine _camChangeCoroutine;
    private bool _areaCameraActive;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (other.TryGetComponent<CharacterController>(out var found))
            controller = found;
    }

    private bool CanChangeCamera()
    {
        return controller != null
            //&& controller.velocity.sqrMagnitude < 0.0025f // 속도 0.05 미만
            && GetLookAtAngle() <= 90f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (controller == null || other != controller)
            return;

        if (CanChangeCamera())
        {
            if (_camChangeCoroutine == null && !_areaCameraActive)
                _camChangeCoroutine = StartCoroutine(SetCamTimer());
        }
        else
        {
            CancelTimer();

            if (_areaCameraActive)
            {
                Debug.Log("놓쳤다");
                _cameraManager.SetCamera(0);
                _areaCameraActive = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (controller == null || other != controller)
            return;

        CancelTimer();

        _cameraManager.SetCamera(0);
        _areaCameraActive = false;
        controller = null;
    }

    private IEnumerator SetCamTimer()
    {
        yield return new WaitForSeconds(_camChangeTime);
        _camChangeCoroutine = null;

        // 대기 종료 시점에도 조건을 만족하는지 확인
        if (!CanChangeCamera())
            yield break;

        _cameraManager.SetCamera(_cameraOrder.order);
        Debug.Log("잡았다");
        _areaCameraActive = true;
    }

    private void CancelTimer()
    {
        if (_camChangeCoroutine == null)
            return;

        StopCoroutine(_camChangeCoroutine);
        _camChangeCoroutine = null;
    }

    [SerializeField] private CinemachineCamera _povCam;

    private float GetLookAtAngle()
    {
        // 계산할 수 없으면 <= 90 조건을 통과하지 않도록 처리
        if (_cameraOrder == null || _povCam == null)
            return 180f;

        CinemachineCamera backgroundCam =
            _cameraOrder.GetComponent<CinemachineCamera>();

        if (backgroundCam == null || backgroundCam.LookAt == null)
            return 180f;

        Transform povTransform = _povCam.transform;

        Vector3 toTarget =
            backgroundCam.LookAt.position - povTransform.position;

        Vector3 forward = povTransform.forward;

        // 높이 차이를 제외한 수평 각도 계산
        toTarget = Vector3.ProjectOnPlane(toTarget, Vector3.up);
        forward = Vector3.ProjectOnPlane(forward, Vector3.up);

        if (toTarget.sqrMagnitude < 0.000001f ||
            forward.sqrMagnitude < 0.000001f)
        {
            return 180f;
        }

        return Vector3.Angle(forward, toTarget);
    }
}
