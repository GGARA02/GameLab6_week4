using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndLight : MonoBehaviour
{
    [SerializeField] private float beforeFadeOutTime = 5f;
    [SerializeField] private float upSpeed = 10f;
    [SerializeField] private GameObject whitePanel;
    private ArrowController arrow;
    private CharacterController pcc;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            arrow = other.gameObject.GetComponent<ArrowController>();
            pcc = arrow.gameObject.GetComponent<CharacterController>();
            //플레이어 움직임 막는 코드
            StartCoroutine(EndingSequence());
        }
    }

    private IEnumerator EndingSequence()
    {
        whitePanel.SetActive(true);
        Image image = whitePanel.GetComponent<Image>();
        Color fadeColor = image.color;
        fadeColor.a = 0;
        image.color = fadeColor;
        float timeElapsed = 0;
        while (timeElapsed < beforeFadeOutTime)
        {
            pcc.Move(Vector3.up * upSpeed);
            // arrow.boostCountUp();
            fadeColor.a = Mathf.Lerp(0, 1, timeElapsed / beforeFadeOutTime);
            image.color = fadeColor;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        pcc.Move(Vector3.zero);
        arrow.transform.position = new Vector3(0, 300, 2500);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(2);
        timeElapsed = 0;
        while (timeElapsed < beforeFadeOutTime)
        {
            // pcc.Move(Vector3.up * upSpeed);
            fadeColor.a = Mathf.Lerp(1, 0, timeElapsed / beforeFadeOutTime);
            image.color = fadeColor;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        whitePanel.SetActive(false);
    }
}
