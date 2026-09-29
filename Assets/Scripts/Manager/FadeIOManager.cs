using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeIOManager : MonoBehaviour
{
    [SerializeField] private GameObject whitePanel;
    [SerializeField] private GameObject blackPanel;
    [SerializeField] private float fadeTime = 2;
    private ArrowController arrow;
    private CharacterController cc;
    void Awake()
    {
        arrow = FindFirstObjectByType<ArrowController>();
        cc = arrow.gameObject.GetComponent<CharacterController>();
    }


    public void StartFadeIO()
    {
        StartCoroutine(FadeOutCoroutine(blackPanel));

    }

    private IEnumerator FadeOutCoroutine(GameObject panel)
    {
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
        Vector3 pos = new Vector3(0, -1800, 4100);
        arrow.gameObject.transform.position = pos;
        yield return new WaitForSeconds(fadeTime);
        cc.Move(Vector3.down);
        fadeTimeElapsed = 0;
        while (fadeTimeElapsed < fadeTime * 2)
        {
            fadeTimeElapsed += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1, 0, fadeTimeElapsed / (fadeTime * 2));
            image.color = fadeColor;
            yield return null;

        }
        cc.Move(Vector3.zero);
        panel.SetActive(false);
    }
}
