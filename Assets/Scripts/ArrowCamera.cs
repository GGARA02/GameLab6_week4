using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ArrowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float smoothTime;

    [SerializeField]
    private Vector3 offset;
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
    private float time = 0.1f;
    [SerializeField]
    private float defaultFOV;
    [SerializeField]
    private float dashFOV;
    [SerializeField]
    private float hyperDashFOV;
    [SerializeField]
    private float bulletTimeFOV;
    [SerializeField]
    private float detectDistance;
    [SerializeField]
    private Transform endPoint;

    private bool isActive;
    private bool isClear = false;


    private Vector3 currentCameraSpeed;
    private float currentFov;

    private Coroutine fovChangeCoroutine;
    private Coroutine volumeChangeCoroutine;
    private Coroutine onHitCorutine;
    private Volume volume;
    private ArrowController arrowController;
    private RaycastHit[] wallHIt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void LateUpdate()
    {
        if (isActive)
        {
            Move();
            DetectWall();
        }
        if (isClear)
        {
            transform.LookAt(target.position);
        }
    }

    public void Initialize(ArrowController controller)
    {
        cam = GetComponent<Camera>();
        arrowController = controller;
        volume = defaultVolume;
        volume.weight = 1f;

        currentCameraSpeed = Vector3.zero;
        controller.OnArrowStateChange += ArrowStateFollowCam;
        currentFov = defaultFOV;
        wallHIt = new RaycastHit[4];
        isActive = false;
    }

    private void ArrowStateFollowCam(ArrowState arrowState)
    {
        switch (arrowState)
        {
            case ArrowState.None:
                if (fovChangeCoroutine != null) StopCoroutine(fovChangeCoroutine);
                fovChangeCoroutine = StartCoroutine(FovChangeSmooth(currentFov, defaultFOV, 0.3f));

                if (volumeChangeCoroutine != null) StopCoroutine(volumeChangeCoroutine);
                volumeChangeCoroutine = StartCoroutine(VolumeChangeSmooth(arrowState));
                break;
            case ArrowState.Dash:
                if (fovChangeCoroutine != null) StopCoroutine(fovChangeCoroutine);
                fovChangeCoroutine = StartCoroutine(FovChangeSmooth(currentFov, dashFOV, 0.4f));

                if (volumeChangeCoroutine != null) StopCoroutine(volumeChangeCoroutine);
                volumeChangeCoroutine = StartCoroutine(VolumeChangeSmooth(arrowState));
                break;
            case ArrowState.HyperDash:
                if (fovChangeCoroutine != null) StopCoroutine(fovChangeCoroutine);
                fovChangeCoroutine = StartCoroutine(FovChangeSmooth(currentFov, hyperDashFOV, 0.2f));

                if (volumeChangeCoroutine != null) StopCoroutine(volumeChangeCoroutine);
                volumeChangeCoroutine = StartCoroutine(VolumeChangeSmooth(arrowState));
                break;
            case ArrowState.BulletTime:
                if (fovChangeCoroutine != null) StopCoroutine(fovChangeCoroutine);
                fovChangeCoroutine = StartCoroutine(FovChangeSmooth(currentFov, bulletTimeFOV, 0.1f));

                if (volumeChangeCoroutine != null) StopCoroutine(volumeChangeCoroutine);
                volumeChangeCoroutine = StartCoroutine(VolumeChangeSmooth(arrowState));
                break;
            default:

                break;
        }
    }

    IEnumerator FovChangeSmooth(float originFov, float targetFov, float time)
    {
        var count = 0f;
        while (count < time)
        {
            count += Time.deltaTime;
            currentFov = Mathf.Lerp(originFov, targetFov, count / time);

            yield return null;
        }
        currentFov = targetFov;
        fovChangeCoroutine = null;
    }

    public void HitWall()
    {
        if (onHitCorutine != null)
        {
            StopCoroutine(onHitCorutine);
        }
        StartCoroutine(HitWallCorutine(2f));
    }

    private IEnumerator HitWallCorutine(float hitTime)
    {
        float count = 0f;
        while (count < (hitTime / 2))
        {
            count += Time.deltaTime;
            HitVolume.weight = Mathf.Lerp(0, 1, count / (hitTime / 2));
            yield return null;
        }
        Debug.Log("데미지 이펙트");
        count = 0;
        while (count < (hitTime / 2))
        {
            count += Time.deltaTime;
            HitVolume.weight = Mathf.Lerp(1, 0, count / (hitTime / 2));
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
                    count += Time.deltaTime;
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
            count += Time.deltaTime;
            volume.weight = Mathf.Lerp(0, 1, count / time);
            yield return null;
        }
    }

    private void DetectWall()
    {
        Vector3 worldPos = target.position + target.forward * detectDistance;
        Debug.DrawRay(transform.position, (worldPos - transform.position).normalized * Vector3.Distance(worldPos, transform.position), Color.red);
        int hitCount = Physics.RaycastNonAlloc(transform.position, 
                    worldPos - transform.position,
                    wallHIt, 
                    Vector3.Distance(worldPos, transform.position),
                    LayerMask.GetMask("Wall"));
        for (int i = 0; i < hitCount; i++)
        {
            Debug.Log("Hit");
            if (wallHIt[i].collider.gameObject.TryGetComponent<WallMatChanger>(out WallMatChanger wallMatChanger))
            {
                wallMatChanger.Invisible();
            }
            //wallHIt[i].collider.gameObject.GetComponent<WallMatChanger>().Invisible();
        }

        //for (int i = 0; i < 4; i++)
        //{
        //    for (int j = 0; j < 4; j++)
        //    {
        //        betweenWalls[i][j] = null;
        //    }
        //}
        //for (int i = 0; i < 4; i++)
        //{
        //    Vector3 rayTarget = target.position + new Vector3(wallDetetedOffset[i][0] * wallDetectRange, wallDetetedOffset[i][1] * wallDetectRange, 0);
        //    RaycastHit[] wallHit = new RaycastHit[4];
        //    int rayCount = Physics.RaycastNonAlloc(rayTarget,
        //                        (rayTarget - transform.position),
        //                        wallHit,
        //                        Vector3.Distance(rayTarget, transform.position),
        //                        LayerMask.GetMask("Wall"));
        //    for (int j = 0; j < rayCount; j++)
        //    {
        //        betweenWalls[i][j] = wallHit[j].collider;
        //    }
        //} 
        //for (int i = 0; i < 4; i++)
        //{

        //    for (int j = 0; j < betweenWalls[i].Length; j++)
        //    {
        //        if (System.Array.IndexOf(betweenWallsPast[0], betweenWalls[i][j]) < 0 
        //            && System.Array.IndexOf(betweenWallsPast[1], betweenWalls[i][j]) < 0
        //            && System.Array.IndexOf(betweenWallsPast[2], betweenWalls[i][j]) < 0
        //            && System.Array.IndexOf(betweenWallsPast[3], betweenWalls[i][j]) < 0)
        //        {
        //            betweenWalls[i][j].gameObject.GetComponent<WallMatChanger>().AlphaChange(wallDetectAlpha, wallChangeTime);
        //            Debug.Log("투명화 : " + betweenWalls[i][j].GetComponent<Collider>().gameObject);
        //        }
        //    }
        //}
        //for (int i = 0; i < 4; i++)
        //{
        //    for (int j = 0; j < betweenWallsPast[i].Length; j++)
        //    {
        //        if (System.Array.IndexOf(betweenWalls[0], betweenWallsPast[i][j]) < 0
        //            && System.Array.IndexOf(betweenWalls[1], betweenWallsPast[i][j]) < 0
        //            && System.Array.IndexOf(betweenWalls[2], betweenWallsPast[i][j]) < 0
        //            && System.Array.IndexOf(betweenWalls[3], betweenWallsPast[i][j]) < 0)
        //        {
        //            betweenWallsPast[i][j].gameObject.GetComponent<WallMatChanger>().AlphaChange(255, wallChangeTime);
        //            Debug.Log("투명화 끝 : " + betweenWallsPast[i][j].gameObject);

        //            break; 
        //        }
        //    }
        //}
        //for (int i = 0; i < 4; i++)
        //{
        //    for (int j = 0; j < 4; j++)
        //    {
        //        betweenWallsPast[i][j] = betweenWalls[i][j];
        //    }
        //}
        // 시발시발시발 이걸 다 날려야된다.
    }

    private void Move()
    {
        Vector3 targetWorldPos = target.TransformPoint(offset);
        Vector3 newPosition = Vector3.SmoothDamp(transform.position, targetWorldPos, ref currentCameraSpeed, smoothTime);
        //newPosition.x = Mathf.Round(newPosition.x / 0.1f) * 0.1f;
        //newPosition.y = Mathf.Round(newPosition.y / 0.1f) * 0.1f;
        //newPosition.z = Mathf.Round(newPosition.z / 0.1f) * 0.1f;
        transform.position = newPosition;
        Vector3 lookTarget = target.transform.position;
        //lookTarget.x = Mathf.Round(newPosition.x / 0.1f) * 0.1f;
        //lookTarget.y = Mathf.Round(newPosition.y / 0.1f) * 0.1f;
        //lookTarget.z = Mathf.Round(newPosition.z / 0.1f) * 0.1f;
        //Debug.Log(lookTarget);
        transform.LookAt(lookTarget);
        cam.fieldOfView = currentFov;
    }

    public void GameClear()
    {
        isActive = false;
        isClear = true;
        //transform.position = endPoint.position;
        transform.position = new Vector3(target.position.x + 100, 50, target.position.z + 100);
    }

    public void GameStart()
    {
        isActive = true;
        transform.position = target.TransformPoint(offset);
        transform.LookAt(target.transform.position);
    }
}
