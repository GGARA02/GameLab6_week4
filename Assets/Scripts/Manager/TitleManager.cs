using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI start;
    private bool canPress = false;

    private void Awake()
    {
        CreateBlock[] createBlocks = FindObjectsByType<CreateBlock>(FindObjectsSortMode.None);
        for (int i = 0; i < createBlocks.Length; i++)
        {
            createBlocks[i].enabled = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GameStartCorutine());
    }

    void Update()
    {
        if (canPress)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("LightUp");
            }
        }
    }

    private IEnumerator GameStartCorutine()
    {
        StartCoroutine(TextVisible(title, 2.0f, true));
        canPress = true;
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(TextVisible(start, 2.0f, true));
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(TextVisible(start, 2.0f, false));
        }

    }
    private IEnumerator TextVisible(TextMeshProUGUI text, float time, bool isVisible)
    {
        float count = 0;
        if (isVisible)
        {
            text.gameObject.SetActive(true);
            text.alpha = 0f;
            while (count <= time)
            {
                count += Time.deltaTime;
                text.alpha = Mathf.Lerp(0f, 1f, count / time);
                yield return null;
            }
            text.alpha = 1f;
        }
        else
        {
            text.alpha = 255f;
            while (count <= time)
            {
                count += Time.deltaTime;
                text.alpha = Mathf.Lerp(1f, 0f, count / time);
                yield return null;
            }
            text.alpha = 1f;
            text.gameObject.SetActive(false);
        }
    }
}
