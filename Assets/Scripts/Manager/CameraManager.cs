using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

//상태 하나에 대한 FOV 연출 값
[Serializable]
public struct FovSetting
{
    public float fov;
    public float duration; //목표 FOV까지 걸리는 시간 (실제 시간 기준)

    public FovSetting(float fov, float duration)
    {
        this.fov = fov;
        this.duration = duration;
    }
}

public class CameraManager : MonoBehaviour
{
    [Header("상태별 FOV (0번 카메라)")]
    [SerializeField]
    private FovSetting _defaultFov = new FovSetting(30f, 0.3f);
    [SerializeField]
    private FovSetting _dashFov = new FovSetting(20f, 0.4f);
    [SerializeField]
    private FovSetting _hyperDashFov = new FovSetting(50f, 0.2f);
    [SerializeField]
    private FovSetting _bulletTimeFov = new FovSetting(25f, 0.1f);

    //0번 카메라가 플레이어를 따라가는 POV 카메라다
    private const int _PovIndex = 0;
    private const int _currentCamPriority = 10;
    private const int _otherCamPriority = 0;

    private CinemachineBrain _brain;
    private CameraOrder _currentCamera;

    [SerializeField]
    private List<CameraOrder> _cameraOrders; //order 오름차순, 인덱스 = order

    public void Initialize()
    {
        //꺼져 있는 카메라도 포함해서 모으고 order 순으로 정렬한다.
        _cameraOrders = new List<CameraOrder>(FindObjectsByType<CameraOrder>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        _cameraOrders.Sort((a, b) => a.order.CompareTo(b.order));
        _cameraOrders[_PovIndex].SetFov(_defaultFov.fov);

        _currentCamera = _cameraOrders[_PovIndex];

        _brain = FindAnyObjectByType<CinemachineBrain>();
    }

    //ArrowController.OnArrowStateChange에 연결된다.
    public void OnArrowStateChanged(ArrowState state)
    {
        FovSetting fovSetting = GetFovSetting(state);
        ChangeFov(_PovIndex, fovSetting.fov, fovSetting.duration);
    }

    public void ChangeFov(int index, float fov, float duration)
    {
        _cameraOrders[index].ChangeFov(fov, duration);
    }

    //카메라 변환
    public void SetCamera(int index)
    {
        // currentCam의 priority = 10
        // _cameraOrders[index].priority = 0;

        _currentCamera.SetPriority(_otherCamPriority);
        _cameraOrders[index].SetPriority(_currentCamPriority);

        _currentCamera = _cameraOrders[index];
    }

    private FovSetting GetFovSetting(ArrowState state)
    {
        switch (state)
        {
            case ArrowState.Dash:
                return _dashFov;
            case ArrowState.HyperDash:
                return _hyperDashFov;
            case ArrowState.BulletTime:
                return _bulletTimeFov;
            default:
                return _defaultFov;
        }
    }
}