using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private CameraManager cameraManager;
    [SerializeField]
    private ArrowController arrowController;
    [SerializeField]
    private VoulmeManager volumeManager;
    [SerializeField]
    private SkyManager skyManager;
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private FadeIOManager fadeIOmanager;
    [Header("UI")]
    [SerializeField]
    public List<TextMeshProUGUI> startUIs; //시작 시 순서대로 보여줄 안내 문구
    [SerializeField]
    private TextMeshProUGUI endingText;
    [SerializeField]
    private TextMeshProUGUI endingRestartText;
    [SerializeField]
    private TextMeshProUGUI gameOverText;

    private Coroutine startUICoru;
    private bool isClear = false;
    private bool isGameOver = false;

    //모든 매니저의 초기화와 이벤트 연결을 여기서 한 번에 한다.
    void Awake()
    {
        inputManager.Initialize();
        cameraManager.Initialize();
        arrowController.Initialize();
        arrowController.inputInit(inputManager);
        volumeManager.Initialize(arrowController);
        skyManager.Initialize();
        // gridManager.Initialize();

        arrowController.OnArrowStateChange += cameraManager.OnArrowStateChanged;
        arrowController.OnLightUp += skyManager.CityLightUp;
        arrowController.OnHitWall += volumeManager.HitWall;
        arrowController.OnGameOver += GameOver;
        arrowController.OnRealeasePressed += skyManager.RealeaseLight;
        skyManager.OnGameClear += GameClear;
        skyManager.OnRealeaseFire += arrowController.remainBulletTimeGain;

        GameStart();
    }

    void Update()
    {
        //System 맵은 게임오버·클리어 때만 켜진다.
        if ((isClear || isGameOver) && inputManager.InteractPressed)
        {
            //현재 씬을 다시 로드 (씬 이름을 코드에 박지 않는다)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnDestroy()
    {
        inputManager.Dispose();
    }

    private void GameStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        arrowController.GameStart();
        volumeManager.GameStart();
        // startUICoru = StartCoroutine(GameStartUICorutine());
    }

    private void GameOver()
    {
        //조작 입력을 끄고 재시작 입력만 받는다.
        inputManager.PlayerDisable();
        inputManager.SystemEnable();

        StopCoroutine(startUICoru);
        foreach (TextMeshProUGUI text in startUIs)
        {
            text.gameObject.SetActive(false);
        }
        StartCoroutine(GameOverCorutine());
    }

    private IEnumerator GameOverCorutine()
    {
        StartCoroutine(TextVisible(gameOverText, 2.0f, true));
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(TextVisible(endingRestartText, 2.0f, true));
        isGameOver = true; //재시작 문구가 뜨기 시작한 뒤부터 재시작 가능
    }

    private void GameClear()
    {
        arrowController.GameClear();
        volumeManager.GameClear();
        gridManager.GameClear();

        //조작 입력을 끄고 재시작 입력만 받는다.
        inputManager.PlayerDisable();
        inputManager.SystemEnable();

        StopCoroutine(startUICoru);
        foreach (TextMeshProUGUI text in startUIs)
        {
            text.gameObject.SetActive(false);
        }
        StartCoroutine(GameClearCorutine());
    }

    private IEnumerator GameClearCorutine()
    {
        StartCoroutine(TextVisible(endingText, 2.0f, true));
        isClear = true;
        //재시작 문구를 계속 깜빡인다.
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(TextVisible(endingRestartText, 2.0f, true));
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(TextVisible(endingRestartText, 2.0f, false));
        }
    }

    private IEnumerator GameStartUICorutine()
    {
        for (int i = 0; i < startUIs.Count; i++)
        {
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(TextVisible(startUIs[i], 2.0f, true));
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(TextVisible(startUIs[i], 2.0f, false));
        }
    }

    //텍스트를 time초 동안 페이드 인(isVisible = true) 또는 페이드 아웃
    private IEnumerator TextVisible(TextMeshProUGUI text, float time, bool isVisible)
    {
        float count = 0;
        if (isVisible)
        {
            text.gameObject.SetActive(true);
            text.alpha = 0f;
            while (count <= time)
            {
                count += Time.unscaledDeltaTime;
                text.alpha = Mathf.Lerp(0f, 1f, count / time);
                yield return null;
            }
            text.alpha = 1f;
        }
        else
        {
            text.alpha = 1f;
            while (count <= time)
            {
                count += Time.unscaledDeltaTime;
                text.alpha = Mathf.Lerp(1f, 0f, count / time);
                yield return null;
            }
            text.alpha = 1f;
            text.gameObject.SetActive(false);
        }
    }
}