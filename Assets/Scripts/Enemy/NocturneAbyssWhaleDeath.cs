using UnityEngine;
using UnityEngine.Events;
public class NocturneAbyssWhaleDeath : MonoBehaviour
{
    [SerializeField] private UnityEvent onStepComplete;

    private bool isCompleted; 

    public UnityEvent OnStepComplete => onStepComplete;

    public void CompleteStep()
    {
        // "연출 완료 시 OnStepComplete 이벤트를 발생시키고 스스로 gameObject.SetActive(false) 처리한다는 말에서 나옴"

        if(isCompleted) return; // 이미 완료된 경우에는 중복 실행 방지

        isCompleted = true; // 완료 상태로 설정

        OnStepComplete?.Invoke(); // 이벤트 발생

        gameObject.SetActive(false); // 스스로 비활성화 처리
    }
}
