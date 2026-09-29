using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gamePadPanel;
    [SerializeField] private GameObject rStick;
    [SerializeField] private GameObject lStick;
    [SerializeField] private GameObject ltrt;
    [SerializeField] private GameObject lbrb;
    private ArrowController arrow;
    private bool isViewedLBRB = false;
    private float timeElapsed = 0;

    void Awake()
    {
        arrow = FindFirstObjectByType<ArrowController>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GameStartUICoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > 0.5f)
        {
            timeElapsed = 0;
            if (arrow.GetBoostCount() > 0 && !isViewedLBRB)
            {
                isViewedLBRB = true;
                StartCoroutine(LBRBViewUICoroutine());
            }
        }
    }

    private IEnumerator GameStartUICoroutine()
    {

        rStick.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        rStick.SetActive(false);
        yield return new WaitForSeconds(2.5f);
        lStick.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        lStick.SetActive(false);
        yield return new WaitForSeconds(2.5f);
        ltrt.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        ltrt.SetActive(false);
    }

    public void ViewLBRB()
    {
        StartCoroutine(LBRBViewUICoroutine());
    }

    private IEnumerator LBRBViewUICoroutine()
    {
        lbrb.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        lbrb.SetActive(false);
    }

}
