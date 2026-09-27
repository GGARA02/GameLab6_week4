using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraOrder : MonoBehaviour
{
    public int order; //카메라 순서 (0번 = 플레이어를 따라가는 POV 카메라)
    [SerializeField]
    private int defaultPriority; //시작할 때 적용할 우선순위

    private CinemachineCamera cinemachineCamera;
    private Coroutine fovRoutine;

    //CameraManager.Initialize가 이 오브젝트의 Awake보다 먼저 불릴 수 있어서, 처음 쓸 때 가져온다.
    private CinemachineCamera CinemachineCamera
    {
        get
        {
            if (cinemachineCamera == null)
            {
                cinemachineCamera = GetComponent<CinemachineCamera>();
            }
            return cinemachineCamera;
        }
    }

    void Start()
    {
        SetPriority(defaultPriority); //시작할 때 기본 우선순위로
    }

    public void SetPriority(int priority)
    {
        CinemachineCamera.Priority.Value = priority;
    }

    //보간 없이 즉시 FOV 변경
    public void SetFov(float fov)
    {
        StopFovRoutine();
        CinemachineCamera.Lens.FieldOfView = fov;
    }

    //duration초 동안 targetFov까지 보간 (실제 시간 기준)
    public void ChangeFov(float targetFov, float duration)
    {
        StopFovRoutine();
        fovRoutine = StartCoroutine(ChangeFovRoutine(targetFov, duration));
    }

    private void StopFovRoutine()
    {
        if (fovRoutine != null)
        {
            StopCoroutine(fovRoutine);
            fovRoutine = null;
        }
    }

    private IEnumerator ChangeFovRoutine(float targetFov, float duration)
    {
        float elapsed = 0f;
        float startFov = CinemachineCamera.Lens.FieldOfView;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            CinemachineCamera.Lens.FieldOfView = Mathf.Lerp(startFov, targetFov, elapsed / duration);
            yield return null;
        }
        CinemachineCamera.Lens.FieldOfView = targetFov;
        fovRoutine = null;
    }
}