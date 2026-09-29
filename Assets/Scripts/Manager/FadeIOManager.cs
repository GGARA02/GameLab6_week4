using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeIOManager : MonoBehaviour
{
    [SerializeField] private GameObject whitePanel;
    [SerializeField] private GameObject blackPanel;
    [SerializeField] private float fadeTime = 2;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartFadeIO(bool isBlack)
    {
        if (isBlack)
        {
            StartCoroutine(FadeOutCoroutine(blackPanel));
        }
        else
        {
            StartCoroutine(FadeOutCoroutine(whitePanel));
        }

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
        yield return new WaitForSeconds(fadeTime);
        fadeTimeElapsed = 0;
        while (fadeTimeElapsed < fadeTime * 2)
        {
            fadeTimeElapsed += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1, 0, fadeTimeElapsed / (fadeTime * 2));
            image.color = fadeColor;
            yield return null;

        }
        panel.SetActive(false);
    }
}
