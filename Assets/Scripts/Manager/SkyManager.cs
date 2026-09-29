using System.Collections;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    [Header("Sun")]
    [SerializeField]
    private int sunRiseMaxCount = 20; //이 횟수만큼 불씨를 먹으면 클리어
    [SerializeField]
    private float sunXRotationMin = 0f;
    [SerializeField]
    private float sunXRotationMax = 15f;
    [SerializeField]
    private float sunIntensityMin = 0.3f;
    [SerializeField]
    private float sunIntensityMax = 1f;
    [Header("Sky")]
    [SerializeField]
    private float skySunSizeMin = 0f;
    [SerializeField]
    private float skySunSizeMax = 0.04f;
    [SerializeField]
    private float skyExposureMin = 1f;
    [SerializeField]
    private float skyExposureMax = 2f;
    [SerializeField]
    private float skyAtmoThickMin = 0.75f;
    [SerializeField]
    private float skyAtmoThickMax = 3f; //대기 두께는 Max에서 시작해서 Min으로 줄어든다.
    [Header("LightUp")]
    [SerializeField]
    private float lightUpTime = 10f; //불씨 하나당 밝아지는 데 걸리는 시간
    [Header("Realease Effect")]
    [SerializeField]
    private int maxRealeaseCount = 3;
    [SerializeField]
    private GameObject realeaseEffect;
    [SerializeField]
    private Transform villageTransform;

    public System.Action OnGameClear;
    public System.Action<float> OnRealeaseFire;

    private Light sun;
    private Material sky;
    private int lightUpCount;
    private Coroutine sunRiseRoutine;

    //현재 값 (다음 보간의 시작점)
    private float xRotation;
    private float intensity;
    private float sunSize;
    private float exposure;
    private float atmosphereThickness;
    private bool isReleasing = false;

    public void Initialize()
    {
        sun = GetComponent<Light>();
        sky = new Material(RenderSettings.skybox); //원본 스카이박스 에셋이 바뀌지 않도록 복제해서 쓴다.
        RenderSettings.skybox = sky;
        lightUpCount = 0;

        xRotation = sunXRotationMin;
        intensity = sunIntensityMin;
        sunSize = skySunSizeMin;
        exposure = skyExposureMin;
        atmosphereThickness = skyAtmoThickMax;
        ApplySky();
    }

    //ArrowController.OnLightUp에 연결된다.
    [ContextMenu("불키기")]
    public void CityLightUp()
    {
        lightUpCount++;
        if (lightUpCount > sunRiseMaxCount)
        {
            return;
        }

        //차후 관리

        //float ratio = (float)lightUpCount / sunRiseMaxCount;
        //if (sunRiseRoutine != null)
        //{
        //    StopCoroutine(sunRiseRoutine);
        //}
        //sunRiseRoutine = StartCoroutine(SunRiseRoutine(ratio));

        //if (lightUpCount == sunRiseMaxCount)
        //{
        //    StartCoroutine(GameClearRoutine());
        //}
    }

    public void RealeaseLight(Transform transform, float remainBulletTime)
    {
        if (!isReleasing)
        {
            isReleasing = true;
            // StartCoroutine(RealeaseLightRoutine(transform, remainBulletTime));
        }
    }

    //스테이지 당 3개 방출
    private IEnumerator RealeaseLightRoutine(Transform transform, float remainBulletTime)
    {
        WaitForSeconds sec = new WaitForSeconds(0.5f);
        int count = Mathf.Min(lightUpCount, maxRealeaseCount);
        float targetBulletTime = Mathf.Min((lightUpCount - count) * 0.05f + 0.1f, 0.3f);
        float perGain = (remainBulletTime - targetBulletTime);
        for (int i = 0; i < count; i++)
        {
            GameObject particle = Instantiate(realeaseEffect, transform.position, Quaternion.identity);
            ParticleAttractor pa = particle.GetComponent<ParticleAttractor>();
            pa.SetTarget(villageTransform, false);
            lightUpCount--;
            OnRealeaseFire?.Invoke(-(perGain / count));
            yield return sec;
        }

        isReleasing = false;
    }

    //ratio(0~1)에 해당하는 하늘까지 lightUpTime 동안 보간
    private IEnumerator SunRiseRoutine(float ratio)
    {
        float startXRotation = xRotation;
        float startIntensity = intensity;
        float startSunSize = sunSize;
        float startExposure = exposure;
        float startAtmosphereThickness = atmosphereThickness;

        float targetXRotation = Mathf.Lerp(sunXRotationMin, sunXRotationMax, ratio);
        float targetIntensity = Mathf.Lerp(sunIntensityMin, sunIntensityMax, ratio);
        float targetSunSize = Mathf.Lerp(skySunSizeMin, skySunSizeMax, ratio);
        float targetExposure = Mathf.Lerp(skyExposureMin, skyExposureMax, ratio);
        float targetAtmosphereThickness = Mathf.Lerp(skyAtmoThickMax, skyAtmoThickMin, ratio);

        float elapsed = 0f;
        while (elapsed < lightUpTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lightUpTime;
            xRotation = Mathf.Lerp(startXRotation, targetXRotation, t);
            intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            sunSize = Mathf.Lerp(startSunSize, targetSunSize, t);
            exposure = Mathf.Lerp(startExposure, targetExposure, t);
            atmosphereThickness = Mathf.Lerp(startAtmosphereThickness, targetAtmosphereThickness, t);
            ApplySky();
            yield return null;
        }

        xRotation = targetXRotation;
        intensity = targetIntensity;
        sunSize = targetSunSize;
        exposure = targetExposure;
        atmosphereThickness = targetAtmosphereThickness;
        ApplySky();
        sunRiseRoutine = null;
    }

    //현재 값을 태양 빛과 스카이박스에 적용
    private void ApplySky()
    {
        transform.rotation = Quaternion.Euler(xRotation, 0, 0);
        sun.intensity = intensity;
        sky.SetFloat("_SunSize", sunSize);
        sky.SetFloat("_Exposure", exposure);
        sky.SetFloat("_AtmosphereThickness", atmosphereThickness);
    }

    //마지막으로 밝아지는 연출이 끝난 뒤 클리어
    private IEnumerator GameClearRoutine()
    {
        yield return new WaitForSeconds(lightUpTime);
        OnGameClear?.Invoke();
    }
}