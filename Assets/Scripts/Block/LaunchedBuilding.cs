using System.Collections;
using UnityEngine;

public class LaunchedBuilding : MonoBehaviour
{
    [SerializeField] private GameObject attackIndicator;
    [SerializeField] private float attackDelay = 3f;
    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        attackIndicator.transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
        Vector3 startScale = attackIndicator.transform.localScale;
        attackIndicator.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        float timeElapsed = 0;
        while (timeElapsed < attackDelay)
        {
            float indicatorScale = Mathf.Lerp(0.1f, startScale.x, timeElapsed / attackDelay);
            Vector3 vec = new Vector3(indicatorScale, indicatorScale, indicatorScale);
            attackIndicator.transform.localScale = vec;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        timeElapsed = 0;
        float startPosY = transform.position.y;
        while (timeElapsed < 0.5f)
        {
            float posY = Mathf.Lerp(startPosY, 0, timeElapsed / 0.5f);
            Vector3 pos = new Vector3(transform.position.x, posY, transform.position.z);
            transform.position = pos;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        attackIndicator.SetActive(false);


    }
}
