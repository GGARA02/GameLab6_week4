using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeIOManager : MonoBehaviour
{
    [SerializeField] private GameObject whitePanel;
    [SerializeField] private GameObject blackPanel;
    [SerializeField] private float fadeTime = 2;
    [SerializeField] SkyManager sky;
    private ArrowController arrow;
    private CharacterController cc;

    private CameraManager cam;
    void Awake()
    {
        arrow = FindFirstObjectByType<ArrowController>();
        cc = arrow.gameObject.GetComponent<CharacterController>();
        cam = FindFirstObjectByType<CameraManager>();
    }


    public void StartFadeIO()
    {
        StartCoroutine(FadeOutCoroutine(blackPanel));

    }

    private IEnumerator FadeOutCoroutine(GameObject panel)
    {
        arrow.CutChunsik();
        arrow.isCutscene = true;
        sky.CityLightDown();
        panel.SetActive(true);

        Image image = panel.GetComponent<Image>();
        Color fadeColor = image.color;
        fadeColor.a = 0;
        image.color = fadeColor;

        float fadeTimeElapsed = 0;
        while (fadeTimeElapsed < fadeTime)
        {
            fadeTimeElapsed += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(0, 1, fadeTimeElapsed / fadeTime);
            image.color = fadeColor;
            yield return null;
        }

        cc.enabled = false;

        Vector3 pos = new Vector3(0, -1800, 4100);
        arrow.ResetMovementState(pos);
        cam.SetCamera(0);
        Physics.SyncTransforms();

        yield return new WaitForSeconds(fadeTime);

        fadeTimeElapsed = 0;
        while (fadeTimeElapsed < fadeTime * 2)
        {
            fadeTimeElapsed += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1, 0, fadeTimeElapsed / (fadeTime * 2));
            image.color = fadeColor;
            arrow.gameObject.transform.position = pos;
            yield return null;
        }

        panel.SetActive(false);

        Physics.SyncTransforms();
        yield return new WaitForFixedUpdate(); // 물리 프레임 안정화 대기
        cc.enabled = true;
        arrow.isCutscene = false;
    }
}
