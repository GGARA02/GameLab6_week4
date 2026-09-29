using System.Collections;
using UnityEngine;

public class VillageLightEffect : MonoBehaviour
{
    [SerializeField] private GameObject[] spotLights;
    [SerializeField] private GameObject whale;
    [SerializeField] private float delayTime = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        whale.SetActive(false);
        SetLightsAngle(70);
        StartCoroutine(LightingSequence());
    }

    private void SetLightsAngle(float angle)
    {
        Vector3 rotVec = new(0, 0, 0);
        foreach (GameObject light in spotLights)
        {
            rotVec.x = angle;
            light.transform.rotation = Quaternion.Euler(rotVec);
        }
    }

    private IEnumerator SetlightsAngleSequence(float angle, float time)
    {
        float startAngle = spotLights[0].transform.eulerAngles.x;
        float timeElapsed = 0;
        while (timeElapsed < time)
        {
            foreach (GameObject light in spotLights)
            {
                light.transform.rotation = Quaternion.Euler(Mathf.Lerp(startAngle, angle, timeElapsed / time), 0, 0);
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SetReflectionIntensity(float intensity, float time)
    {
        float timeElapsed = 0;
        float startIntensity = RenderSettings.reflectionIntensity;
        while (timeElapsed < time)
        {
            RenderSettings.reflectionIntensity = Mathf.Lerp(startIntensity, intensity, timeElapsed / time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator LightingSequence()
    {
        WaitForSeconds sec = new WaitForSeconds(delayTime);
        while (true)
        {
            StartCoroutine(SetlightsAngleSequence(65, delayTime / 2));
            yield return sec;
            StartCoroutine(SetlightsAngleSequence(60, delayTime / 2));
            yield return sec;
            whale.SetActive(true);
            StartCoroutine(SetlightsAngleSequence(70, 3));
            StartCoroutine(SetReflectionIntensity(0.2f, 3));
            yield return new WaitForSeconds(6f);
            whale.SetActive(false);
            StartCoroutine(SetlightsAngleSequence(40, 2));
            StartCoroutine(SetReflectionIntensity(1f, 2));
            yield return sec;
            SetLightsAngle(70);
            yield return new WaitForSeconds(2f);
        }
    }
}
