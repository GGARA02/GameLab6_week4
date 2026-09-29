using UnityEngine;

public class NocturneAbyssWhaleController : MonoBehaviour
{
    [Tooltip("고래가 이동할 점들")]
    public Transform[] points;

    [Tooltip("고래의 이동 속도")]
    public float _speed = 2f;
    
    [Tooltip("고래 방향회전 속도")]
    public float _rotationSpeed = 8f;

    private int _currentPointIndex = 0;
    [SerializeField] private NocturneAbyssWhaleDeath whaleDeath;

    [SerializeField] private NocturneAbyssWhaleDeath[] whaleSteps; 
    private int currentStepIndex = -1;
    private bool isRunning;

    private NocturneAbyssWhalePointSettings _currentStats;
    private NocturneAbyssWhalePointEvantSteps _currentEvents;


    private void Update()
    {
        if (WhaleStop()) return; // 고래가 이동할 점이 없거나, 모든 점을 다 이동했으면 종료
        UpdateStats(); // 현재 이동해야할 점이 원하는 고래의 스텟 설정값을 가져옵니다.
        if(UpdateEvants()) // 현재 이동해야할 점이 원하는 고래의 이벤트 설정값을 가져옵니다.
            return;

        ChangeDirection(); // 다음으로 이동할 점을 바라보도록 방향을 바꿉니다.
        MoveWhale(); // 자기가 알고있는 점으로 이동합니다.
        CheckNextPoint(); // 현재 점에 도착했는지 체크하고, 도착했으면 다음 점으로 이동하도록 인덱스를 증가시킵니다.
    }

    private bool WhaleStop()
    {
        if(points == null || _currentPointIndex >= points.Length)// 현재 배열 이동 인덱스가 배열의 길이보다 크거나 같은가? 
        {
            whaleDeath.CompleteStep();
            return true;
            // 그러면 이동할 점이 없으므로 종료
        } 

        return false; // 아님말고
    }

    private void MoveWhale()
    {
        Transform targetPoint = points[_currentPointIndex]; // 현재 이동해야할 점을 체크

        transform.position = Vector3.MoveTowards( // 현재 위치에서 목표 위치까지 이동
            transform.position,
            targetPoint.position,
            _speed * Time.deltaTime
        );
    }

    private void ChangeDirection()
    {
        Vector3 direction =
            points[_currentPointIndex].position - transform.position; // 현재 위치에서 목표 위치를 바라볼 방향을 구함

        if (direction != Vector3.zero) // 방향이 0이 아니면 (참고로 0이면 바라볼 방향이 없으므로 회전값을 적용하지 않음)
        {
            transform.rotation = Quaternion.Slerp( // 바라보는 방향을 회전값으로 변환하여 적용 !!천천히 목이 꺾여요!!
                transform.rotation,
                Quaternion.LookRotation(direction),
                _rotationSpeed * Time.deltaTime); 
        }
    }
    private void UpdateStats()
    {
        _currentStats =
            points[_currentPointIndex] // 포인트에 있는 NocturneAbyssWhalePointSettings 컴포넌트를 읽어내림
            .GetComponent<NocturneAbyssWhalePointSettings>(); //  읽어내린 데이터를 현재 스텟으로 설정
    }



    private bool UpdateEvants()
    {
        _currentEvents =
            points[_currentPointIndex] // 포인트에 있는 NocturneAbyssWhalePointEvantSteps 컴포넌트를 읽어내림
            .GetComponent<NocturneAbyssWhalePointEvantSteps>(); //  읽어내린 데이터를 현재 이벤트로 설정
        return _currentEvents != null;
    }

    private void CheckNextPoint()
    {
        Transform targetPoint = points[_currentPointIndex]; // 현재 가지고 있는 점을 체크

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            _currentPointIndex++; // 목표 점에 도착하면 다음 점을 목표로 설정
        }
    }
    public void StartSequence()
    {
        if(isRunning) return; // 이미 실행중이면 무시
        if(whaleSteps.Length == 0) return; // 배열이 비어있으면 무시

        isRunning = true; // 실행중 상태로 변경
        currentStepIndex = 0;

        for(int i = 0; i < whaleSteps.Length; i++) // 모든 배열에 대해 이벤트 리스너를 등록
        {
            whaleSteps[i].OnStepComplete.RemoveListener(OnStepComplete); // 중복 등록 방지
            whaleSteps[i].OnStepComplete.AddListener(OnStepComplete); // 이벤트 리스너 등록 
        }

        whaleSteps[currentStepIndex].gameObject.SetActive(true); // 다음 고래를 활성화
    }
    private void OnStepComplete()
    {
        // "다음 배열로 이동하고 없으면 작동중지한다는 말에서 나옴"

        currentStepIndex++;

        if(currentStepIndex >= whaleSteps.Length)
        {
            isRunning = false;
            return;
        }

        whaleSteps[currentStepIndex].gameObject.SetActive(true);
    }
}
