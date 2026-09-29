using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class BackgroundFollowCam : MonoBehaviour
{
    [Header("카메라")]
    [SerializeField] private CameraOrder _cameraOrder;   // 백그라운드 카메라
    [SerializeField] private CinemachineCamera _povCam;
    [SerializeField] private Transform _target;          // 각도 판정 + 그룹 멤버

    [Header("전환")]
    [SerializeField] private float _camChangeTime = 3f;
    [SerializeField] private float _camCancleAngle = 90f;

    [Header("거리에 따른 값 (x = 가까울 때, y = 멀 때)")]
    [SerializeField] private Vector2 _distanceRange = new Vector2(0f, 200f);
    [SerializeField] private Vector2 _targetWeightRange = new Vector2(0f, 0.15f);
    [SerializeField] private Vector2 _orbitRadiusRange = new Vector2(5f, 100f);

    private const float _memberRadius = 0.5f;

    private CameraManager _cameraManager;
    private CinemachineOrbitalFollow _orbitalFollow;
    private Transform _player;
    private CinemachineTargetGroup.Target _targetMember;

    private CharacterController controller;
    private Coroutine _camChangeCoroutine;
    private bool _areaCameraActive;

    void Start()
    {
        _cameraManager = FindFirstObjectByType<CameraManager>();

        // 백그라운드 카메라 설정에서 필요한 것들을 꺼내온다
        CinemachineCamera backgroundCam = _cameraOrder.GetComponent<CinemachineCamera>();
        _orbitalFollow = _cameraOrder.GetComponent<CinemachineOrbitalFollow>();
        _player = backgroundCam.Follow;

        CinemachineTargetGroup group = backgroundCam.LookAt.GetComponent<CinemachineTargetGroup>();
        GetOrAddMember(group, _player, 1f);
        _targetMember = GetOrAddMember(group, _target, _targetWeightRange.y);
        _orbitalFollow.Radius = _orbitRadiusRange.y;
    }

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
            && GetLookAtAngle() <= _camCancleAngle;
    }

    private void OnTriggerStay(Collider other)
    {
        if (controller == null || other != controller)
            return;

        if (_target == null)
        {
            CancelTimer();
            if (_areaCameraActive)
                _cameraManager.SetCamera(0);

            _areaCameraActive = false;
            controller = null;
            _targetMember.Weight = 0f;
            GetComponent<Collider>().enabled = false;
            return;
        }

        float distance = Vector3.Distance(_player.position, _target.position);
        float t = Mathf.InverseLerp(_distanceRange.x, _distanceRange.y, distance);

        _targetMember.Weight = Mathf.Lerp(_targetWeightRange.x, _targetWeightRange.y, t);
        _orbitalFollow.Radius = Mathf.Lerp(_orbitRadiusRange.x, _orbitRadiusRange.y, t);

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

    // POV 카메라 정면과 실제 타겟 방향 사이의 수평 각도
    private float GetLookAtAngle()
    {
        // 계산할 수 없으면 조건을 통과하지 않도록 처리
        if (_povCam == null || _target == null)
            return 180f;

        Transform povTransform = _povCam.transform;

        // 높이 차이를 제외한 수평 각도 계산
        Vector3 toTarget = Vector3.ProjectOnPlane(_target.position - povTransform.position, Vector3.up);
        Vector3 forward = Vector3.ProjectOnPlane(povTransform.forward, Vector3.up);

        if (toTarget.sqrMagnitude < 0.000001f || forward.sqrMagnitude < 0.000001f)
            return 180f;

        return Vector3.Angle(forward, toTarget);
    }

    private static CinemachineTargetGroup.Target GetOrAddMember(CinemachineTargetGroup group, Transform member, float weight)
    {
        int index = group.FindMember(member);
        if (index < 0)
        {
            group.AddMember(member, weight, _memberRadius);
            index = group.Targets.Count - 1;
        }
        else
        {
            group.Targets[index].Weight = weight;
        }
        return group.Targets[index];
    }
}