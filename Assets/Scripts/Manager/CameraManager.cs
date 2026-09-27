using System;
using System.Collections.Generic;
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
    private FovSetting defaultFov = new FovSetting(30f, 0.3f);
    [SerializeField]
    private FovSetting dashFov = new FovSetting(20f, 0.4f);
    [SerializeField]
    private FovSetting hyperDashFov = new FovSetting(50f, 0.2f);
    [SerializeField]
    private FovSetting bulletTimeFov = new FovSetting(25f, 0.1f);

    //0번 카메라가 플레이어를 따라가는 POV 카메라다
    private const int PovIndex = 0;

    private List<CameraOrder> cameraOrders; //order 오름차순, 인덱스 = order

    public void Initialize()
    {
        //꺼져 있는 카메라도 포함해서 모으고 order 순으로 정렬한다.
        cameraOrders = new List<CameraOrder>(FindObjectsByType<CameraOrder>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        cameraOrders.Sort((a, b) => a.order.CompareTo(b.order));
        cameraOrders[PovIndex].SetFov(defaultFov.fov);
    }

    //ArrowController.OnArrowStateChange에 연결된다.
    public void OnArrowStateChanged(ArrowState state)
    {
        FovSetting fovSetting = GetFovSetting(state);
        ChangeFov(PovIndex, fovSetting.fov, fovSetting.duration);
    }

    public void ChangeFov(int index, float fov, float duration)
    {
        cameraOrders[index].ChangeFov(fov, duration);
    }

    //우선순위가 가장 높은 카메라가 화면을 맡는다. (시네머신 브레인이 블렌드 처리)
    public void ChangePriority(int index, int priority)
    {
        cameraOrders[index].SetPriority(priority);
    }

    private FovSetting GetFovSetting(ArrowState state)
    {
        switch (state)
        {
            case ArrowState.Dash:
                return dashFov;
            case ArrowState.HyperDash:
                return hyperDashFov;
            case ArrowState.BulletTime:
                return bulletTimeFov;
            default:
                return defaultFov;
        }
    }
}