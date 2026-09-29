using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class BackgroundFollowCam : MonoBehaviour
{
    [Header("카메라")]
    [SerializeField] private CameraOrder _cameraOrder;   // 백그라운드 카메라
    [SerializeField] private CinemachineCamera _povCam;
    [SerializeField] private CinemachineTargetGroup _group; // 비워두면 백그라운드 카메라의 Look At Target에서 찾음
    [SerializeField] private Transform _target;          // 각도 판정 + 그룹 멤버 (런타임에 SetTarget으로 지정 가능)

    [Header("전환")]
    [SerializeField] private float _camChangeTime = 3f;
    [SerializeField] private float _camCancleAngle = 90f;

    [Header("거리에 따른 값 (x = 가까울 때, y = 멀 때)")]
    [SerializeField] private Vector2 _distanceRange = new Vector2(0f, 200f);
    [SerializeField] private Vector2 _targetWeightRange = new Vector2(0f, 0.15f);
    [SerializeField] private Vector2 _orbitRadiusRange = new Vector2(5f, 100f);

    [Header("Target이 사라졌을 때 콜라이더 끄기")]
    [SerializeField] private bool isColliderOff = true;

    private const float _memberRadius = 0.5f;

    private CameraManager _cameraManager;
    private CinemachineOrbitalFollow _orbitalFollow;
    private Transform _player;
    private CinemachineTargetGroup.Target _targetMember; // 타겟이 아직 없으면 null
    private Collider _collider;
    private TorchTrigger _torchTrigger;

    private CharacterController controller;
    private Coroutine _camChangeCoroutine;
    private bool _areaCameraActive;

    void Start()
    {
        _cameraManager = FindFirstObjectByType<CameraManager>();
        _collider = GetComponent<Collider>();

        CinemachineCamera backgroundCam = _cameraOrder.GetComponent<CinemachineCamera>();
        _orbitalFollow = _cameraOrder.GetComponent<CinemachineOrbitalFollow>();

        // LookAt 프로퍼티는 Custom 토글이 꺼져 있으면 TrackingTarget(플레이어)을 돌려주므로
        // Target 구조체의 필드를 직접 읽는다
        _player = backgroundCam.Target.TrackingTarget;
        if (_group == null && backgroundCam.Target.LookAtTarget != null)
            _group = backgroundCam.Target.LookAtTarget.GetComponent<CinemachineTargetGroup>();

        if (_group == null)
        {
            return;
        }

        GetOrAddMember(_group, _player, 1f);
        _orbitalFollow.Radius = _orbitRadiusRange.y;

        // 인스펙터에 지정했거나 Start 전에 SetTarget이 불린 경우 등록
        if (_target != null)
            SetTarget(_target);

        if (TryGetComponent(out _torchTrigger))
            _torchTrigger.OnFirstWispLaunched += SetTarget;
    }

    void OnDestroy()
    {
        if (_torchTrigger != null)
            _torchTrigger.OnFirstWispLaunched -= SetTarget;
    }

    public void SetTarget(Transform target)
    {
        // Start 전이면 저장만 해두고 Start에서 등록
        if (_group == null)
        {
            _target = target;
            return;
        }

        // 이전 타겟 멤버 정리
        if (_targetMember != null && _targetMember.Object != target)
        {
            _group.Targets.Remove(_targetMember);
            _targetMember = null;
        }

        _target = target;
        if (_target == null)
            return;

        _targetMember = GetOrAddMember(_group, _target, _targetWeightRange.y);

        // 타겟이 사라져서 꺼졌던 콜라이더 복구 (다시 켜지면 OnTriggerEnter가 다시 호출됨)
        if (_collider != null)
            _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (other.TryGetComponent<CharacterController>(out var found))
            controller = found;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_group == null || controller == null || other != controller)
            return;

        // 타겟이 아직 생성되지 않았거나 파괴됨
        if (_target == null)
        {
            HandleNoTarget();
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
                _cameraManager.SetCamera(0);
                _areaCameraActive = false;
            }
        }
    }

    private void HandleNoTarget()
    {
        CancelTimer();
        if (_areaCameraActive)
            _cameraManager.SetCamera(0);
        _areaCameraActive = false;

        // TorchTrigger가 타겟을 아직 생성하지 않은 경우만 대기
        // (플레이어가 안에 있는 상태를 유지해야 타겟이 생기자마자 반응함)
        if (_torchTrigger != null && _targetMember == null)
            return;

        // 그 외(TorchTrigger 없음, 또는 있던 타겟이 파괴됨): 원래 동작
        if (_targetMember != null)
        {
            _group.Targets.Remove(_targetMember);
            _targetMember = null;
        }

        if (isColliderOff && _collider != null)
        {
            controller = null;
            _collider.enabled = false;
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

    private bool CanChangeCamera()
    {
        return controller != null
            && GetLookAtAngle() <= _camCancleAngle;
    }

    private IEnumerator SetCamTimer()
    {
        yield return new WaitForSeconds(_camChangeTime);
        _camChangeCoroutine = null;

        if (!CanChangeCamera())
            yield break;

        _cameraManager.SetCamera(_cameraOrder.order);
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
        if (_povCam == null || _target == null)
            return 180f;

        Transform povTransform = _povCam.transform;

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