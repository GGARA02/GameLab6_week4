using System.Collections;
using UnityEngine;

public class ActivateEnemyAfterDelay : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private float delayTime = 3.0f; // 대기할 시간(초)

    private void OnEnable()
    {
        // 오브젝트가 켜지면 타이머 코루틴 시작
        StartCoroutine(ActivateEnemyRoutine());
    }

    private IEnumerator ActivateEnemyRoutine()
    {
        // 지정한 시간(초)만큼 대기
        yield return new WaitForSeconds(delayTime);

        // 시간이 끝나면 enemy 오브젝트 활성화
        if(enemy != null)
        {
            enemy.SetActive(true);
        }
    }
}