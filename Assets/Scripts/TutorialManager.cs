using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private ArrowController arrowController;
    [SerializeField]
    private VoulmeManager arrowCamera;
    [SerializeField]
    private GameObject endCamera;
    [SerializeField]
    private SkyManager skyManager;
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI endingText;
    [SerializeField]
    private TextMeshProUGUI endingRestartText;
    [SerializeField]
    private TextMeshProUGUI gameOverText;
    private bool isClear = false;
    private bool isGameOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //이니셜 및 이벤트 구독합시다 
        arrowController.Initialize();
        arrowCamera.Initialize(arrowController);
        skyManager.Initialize();
        arrowController.OnLightUp += skyManager.CityLightUp;
        arrowController.OnHitWall += arrowCamera.HitWall;
        arrowController.OnGameOver += GameOver;
        skyManager.OnGameClear += GameClear;
        skyManager.OnGameClear += arrowController.GameClear;
        skyManager.OnGameClear += arrowCamera.GameClear;
        GameStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (isClear || isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("GridTest");
            }
        }
    }

    private void GameStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        arrowController.GameStart();
        arrowCamera.GameStart();
    }

    private void GameOver()
    {
        StartCoroutine(GameOverCorutine());
    }

    private IEnumerator GameOverCorutine()
    {
        Debug.Log("GameOver");
        endCamera.SetActive(true);
        endCamera.GetComponent<Camera>().enabled = true;
        arrowCamera.gameObject.GetComponent<Camera>().enabled = false;

        StartCoroutine(TextVisible(gameOverText, 2.0f, true));
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(TextVisible(endingRestartText, 2.0f, true));
        isGameOver = true;
    }

    private void GameClear()
    {
        StartCoroutine(GameClearCorutine());
    }

    private IEnumerator GameClearCorutine()
    {
        StartCoroutine(TextVisible(endingText, 2.0f, true));
        isClear = true;
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(TextVisible(endingRestartText, 2.0f, true));
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(TextVisible(endingRestartText, 2.0f, false));
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
