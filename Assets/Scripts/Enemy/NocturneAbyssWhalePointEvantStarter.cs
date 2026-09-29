using UnityEngine;

public class NocturneAbyssWhalePointEvantStarter : MonoBehaviour
{
    [SerializeField] private GameObject targetWhale;
    [SerializeField] private string playerTag = "Player"; // 태그 있길래 태그로 하는걸로 만듬

    private bool hasStarted;

    private void OnTriggerEnter(Collider other)
    {
        if(hasStarted) return; // 어차피 한 번만 쓸텐데 이거면 되지 않겠음? 애가 처음 닿았을 때만 실행되도록 한거임. 이 데이터는 아마 계속 남아있을거임. 그래서 한 번만 실행되도록 하는거임.
        if(!other.CompareTag(playerTag)) return; // 플레이어가 아니면 무시하도록 한거.

        hasStarted = true; // 이제 시작했으니 true로 바꿔서 다시 실행되지 않도록 함.

        targetWhale.SetActive(true); // 연결된 고래를 활성화
    }
}
