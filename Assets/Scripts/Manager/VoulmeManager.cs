using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public class VoulmeManager : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField]
    private Volume defaultVolume;
    [SerializeField]
    private Volume dashVolume;
    [SerializeField]
    private Volume hyperDashVolume;
    [SerializeField]
    private Volume bulletTimeVolume;
    [SerializeField]
    private Volume HitVolume;
    [SerializeField]
    private float time = 0.4f; // 상태 볼륨 전환 시간
    [SerializeField]
    private float hitTime = 2f; // 피격 볼륨 전체 시간 (절반 켜짐, 절반 꺼짐)
    [Header("Wall")]
    [SerializeField]
    private float detectDistance = 1f;

    private bool isActive;
    private Transform target;
    private Transform brainTransform;
    private Volume volume;
    private Coroutine volumeChangeCoroutine;
    private Coroutine onHitCorutine;
    private RaycastHit[] wallHit;
    private int wallLayerMask;

    void LateUpdate()
    {
        if (isActive)
        {
            DetectWall();
        }
    }

    public void Initialize(ArrowController controller)
    {
        target = controller.transform;
        brainTransform = FindFirstObjectByType<CinemachineBrain>().transform;
        volume = defaultVolume;
        volume.weight = 1f;
        wallHit = new RaycastHit[4];
        wallLayerMask = LayerMask.GetMask("Wall");
        isActive = false;
        controller.OnArrowStateChange += ChangeVolume;
    }

    public void GameStart()
    {
        isActive = true;
    }

    public void GameClear()
    {
        isActive = false;
    }

    private void ChangeVolume(ArrowState arrowState)
    {
        if (volumeChangeCoroutine != null) StopCoroutine(volumeChangeCoroutine);
        volumeChangeCoroutine = StartCoroutine(VolumeChangeSmooth(arrowState));
    }

    public void HitWall()
    {
        if (onHitCorutine != null) StopCoroutine(onHitCorutine);
        onHitCorutine = StartCoroutine(HitWallCorutine(hitTime));
    }

    private IEnumerator HitWallCorutine(float duration)
    {
        float half = duration / 2;
        float count = 0f;
        while (count < half)
        {
            count += Time.unscaledDeltaTime;
            HitVolume.weight = Mathf.Lerp(0, 1, count / half);
            yield return null;
        }
        count = 0f;
        while (count < half)
        {
            count += Time.unscaledDeltaTime;
            HitVolume.weight = Mathf.Lerp(1, 0, count / half);
            yield return null;
        }
        HitVolume.weight = 0;
        onHitCorutine = null;
    }

    private IEnumerator VolumeChangeSmooth(ArrowState arrowState)
    {
        float count = 0f;
        //대시를 할때는 즉각적으로한다.
        //부드럽게 전환이 필요한것은 대시가 꺼질때와 불렛타임 온오프일때 정도란다.
        switch (arrowState)
        {
            case ArrowState.None:
                while (count < time)
                {
                    count += Time.unscaledDeltaTime;
                    volume.weight = Mathf.Lerp(1, 0, count / time);
                    yield return null;
                }
                count = 0;
                volume = defaultVolume;
                break;
            case ArrowState.Dash:
                volume.weight = 0f;
                volume = dashVolume;
                break;
            case ArrowState.HyperDash:
                volume.weight = 0f;
                volume = hyperDashVolume;
                break;
            case ArrowState.BulletTime:
                volume.weight = 0f;
                volume = bulletTimeVolume;
                break;
            default:
                volume = defaultVolume;
                break;
        }
        while (count < time)
        {
            count += Time.unscaledDeltaTime;
            volume.weight = Mathf.Lerp(0, 1, count / time);
            yield return null;
        }
        volumeChangeCoroutine = null;
    }

    //현재 브레인(실제 카메라)과 플레이어 사이를 가리는 벽을 찾는다.
    private void DetectWall()
    {
        Vector3 origin = brainTransform.position;
        Vector3 toTarget = target.position + target.forward * detectDistance - origin;
        int hitCount = Physics.RaycastNonAlloc(origin, toTarget, wallHit, toTarget.magnitude, wallLayerMask);
        for (int i = 0; i < hitCount; i++)
        {
            if (wallHit[i].collider.TryGetComponent(out WallMatChanger wallMatChanger))
            {
                wallMatChanger.Invisible();
            }
        }
    }
}