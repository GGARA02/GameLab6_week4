using UnityEngine;
using UnityEngine.Events;
public class NocturneAbyssWhaleDeath : MonoBehaviour
{
    [SerializeField] private UnityEvent onStepComplete;

    private bool isCompleted; 

    public UnityEvent OnStepComplete => onStepComplete;

    public void CompleteStep()
    {
        if(isCompleted) return; // 이미 완료된 경우에는 중복 실행 방지
        isCompleted = true; // 완료 상태로 설정
        gameObject.SetActive(false); // 스스로 비활성화 처리
    }
}
