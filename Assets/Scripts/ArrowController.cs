using Unity.Cinemachine;
using UnityEngine;

public enum ArrowState
{
    None,
    BulletTime,
    Dash,
    HyperDash
}

public class ArrowController : MonoBehaviour
{
    //TODO : 쉐이더 그래프로 깜빡임 효과
    [Header("Default")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float invincibleTime; //벽 피격 후 무적 시간
    [Header("Dash")]
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashCoolTime; //대시 유지 시간
    [Header("HyperDash")]
    [SerializeField]
    private float hyperDashSpeed;
    [SerializeField]
    private float hyperDashCoolTime; //하이퍼대시 유지 시간
    [Header("BulletTime")]
    [SerializeField]
    private float startBulletTime; //시작 게이지 (체력 겸 불릿타임 자원)
    [SerializeField]
    private float maxBulletTime;
    [SerializeField]
    private float bulletTimeSpeed;
    [SerializeField]
    private float bulletTimeScale; //불릿타임 중 Time.timeScale
    [SerializeField]
    private float bulletTimeDiscount; //초당 기본 게이지 소모량
    [SerializeField]
    private float bulletTimeDuringDiscount; //불릿타임 중 추가 소모 배율
    [SerializeField]
    private float bulletTimeLightUp; //불씨 획득 시 회복량
    [SerializeField]
    private float bulletWallHit; //벽 피격 시 감소량
    [Header("Difficult")]
    [SerializeField]
    private float difficultSpeedUp; //초당 속도 증가량
    [Header("Trail")]
    [SerializeField]
    private TrailRenderer trail;
    [SerializeField]
    private ParticleSystem particle;

    private ArrowState arrowState;
    private float currentSpeed;
    private float remainCoolTime; //남은 쿨타임은 조작불가능 시간과 동일하다.
    private float remainInvincibleTime;
    private float remainBulletTime; //0이 되면 게임오버
    private bool isActive = false; //false면 조작과 게이지 소모가 멈춘다.

    private InputManager input;
    private Transform brainTransform; //이동 기준이 되는 메인 카메라(시네머신 브레인)
    private CharacterController characterController;

    public System.Action OnHitWall; //플레이어 히트처리
    public System.Action<ArrowState> OnArrowStateChange; //카메라에서 상태별 연출을 위한 이벤트
    public System.Action OnLightUp; //불씨를 밝히자
    public System.Action OnGameOver;

    private void Update()
    {
        if (isActive)
        {
            HandleStateLogic();
            Move();
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Wall") && remainInvincibleTime <= 0)
    //    {
    //        //OnHitWall?.Invoke();
    //        //remainBulletTime -= bulletWallHit;
    //        //remainBulletTime = Mathf.Clamp(remainBulletTime, 0f, maxBulletTime);
    //        //remainInvincibleTime = invincibleTime;
    //    }
    //    else if (other.CompareTag("Ember"))
    //    {
    //        OnLightUp?.Invoke();

    //        //불씨를 먹으면 대시, 대시 중에 또 먹으면 하이퍼대시
    //        if (arrowState == ArrowState.None)
    //        {
    //            ChangeArrowState(ArrowState.Dash);
    //            Time.timeScale = 1;
    //            currentSpeed = dashSpeed;
    //            remainCoolTime = dashCoolTime;
    //        }
    //        else if (arrowState == ArrowState.HyperDash || arrowState == ArrowState.Dash)
    //        {
    //            ChangeArrowState(ArrowState.HyperDash);
    //            Time.timeScale = 1;
    //            currentSpeed = hyperDashSpeed;
    //            remainCoolTime = hyperDashCoolTime;
    //        }

    //        remainBulletTime += bulletTimeLightUp;
    //        remainBulletTime = Mathf.Clamp(remainBulletTime, 0f, maxBulletTime);
    //        Destroy(other.gameObject);
    //    }
    //}

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Ember"))
        {
            OnLightUp?.Invoke();

            //불씨를 먹으면 대시, 대시 중에 또 먹으면 하이퍼대시
            if (arrowState == ArrowState.None)
            {
                ChangeArrowState(ArrowState.Dash);
                Time.timeScale = 1;
                currentSpeed = dashSpeed;
                remainCoolTime = dashCoolTime;
            }
            else if (arrowState == ArrowState.HyperDash || arrowState == ArrowState.Dash)
            {
                ChangeArrowState(ArrowState.HyperDash);
                Time.timeScale = 1;
                currentSpeed = hyperDashSpeed;
                remainCoolTime = hyperDashCoolTime;
            }

            remainBulletTime += bulletTimeLightUp;
            remainBulletTime = Mathf.Clamp(remainBulletTime, 0f, maxBulletTime);
            Destroy(hit.gameObject);
        }
    }

    public void Initialize()
    {
        currentSpeed = speed;
        remainCoolTime = 0;
        remainInvincibleTime = 0;
        remainBulletTime = startBulletTime;
        arrowState = ArrowState.None;
        brainTransform = FindFirstObjectByType<CinemachineBrain>().transform;
        characterController = GetComponent<CharacterController>();
    }

    public void inputInit(InputManager inputManager)
    {
        input = inputManager;
    }

    public void GameStart()
    {
        isActive = true;
    }

    public void GameOver()
    {
        ChangeArrowState(ArrowState.None);
        Time.timeScale = 1f;
        isActive = false;
    }

    //클리어 시 조작만 멈춘다. (클리어 연출은 추후)
    public void GameClear()
    {
        ChangeArrowState(ArrowState.None);
        Time.timeScale = 1f;
        isActive = false;
    }

    private void ChangeArrowState(ArrowState state)
    {
        arrowState = state;
        OnArrowStateChange?.Invoke(arrowState);
    }

    private void HandleStateLogic() //상태 변환 및 그에 따른 변수도 조금 바꿔주자
    {
        if (arrowState == ArrowState.None)
        {
            if (input.BulletTimePressed)
            {
                ChangeArrowState(ArrowState.BulletTime);
                currentSpeed = bulletTimeSpeed;
                Time.timeScale = bulletTimeScale;
            }
            //바로 위에서 넣은 bulletTimeSpeed를 덮어쓴다. (불릿타임 속도가 적용되지 않음)
            currentSpeed = speed;
        }
        else if (arrowState == ArrowState.Dash)
        {
            //대시 시간이 끝나면 기본 상태로
            if (remainCoolTime <= 0)
            {
                ChangeArrowState(ArrowState.None);
                currentSpeed = speed;
                remainCoolTime = 0;
            }
            else
            {
                currentSpeed = dashSpeed;
            }
        }
        else if (arrowState == ArrowState.BulletTime)
        {
            //키를 떼거나 게이지가 바닥나면 해제
            if (input.BulletTimeReleased || remainBulletTime <= 0)
            {
                ChangeArrowState(ArrowState.None);
                currentSpeed = speed;
                Time.timeScale = 1;
            }
            remainBulletTime -= Time.deltaTime * bulletTimeDiscount * bulletTimeDuringDiscount;
            if (remainBulletTime <= 0)
            {
                remainBulletTime = 0;
            }
        }
        else if (arrowState == ArrowState.HyperDash)
        {
            //하이퍼대시 시간이 끝나면 기본 상태로
            if (remainCoolTime <= 0)
            {
                ChangeArrowState(ArrowState.None);
                currentSpeed = speed;
                remainCoolTime = 0;
            }
            else
            {
                currentSpeed = hyperDashSpeed;
            }
        }
    }

    private void Move()
    {
        if (remainCoolTime > 0)
        {
            remainCoolTime -= Time.deltaTime;
        }
        if (remainInvincibleTime > 0)
        {
            remainInvincibleTime -= Time.deltaTime;
        }
        remainBulletTime -= Time.deltaTime * bulletTimeDiscount;

        //시간이 지날수록 난이도 상승 (대시 속도들도 같은 비율로)
        speed += difficultSpeedUp * Time.deltaTime;
        dashSpeed += difficultSpeedUp * Time.deltaTime * dashSpeed / speed;
        hyperDashSpeed += difficultSpeedUp * Time.deltaTime * hyperDashSpeed / speed;

        //게이지가 많을수록 빠르게 (0.33 ~ 0.66배)
        float trailRangeRatio = remainBulletTime / maxBulletTime;
        trailRangeRatio = Mathf.Clamp(trailRangeRatio, 0.33f, 0.66f);

        //카메라가 보는 방향 기준 WASD 이동, 바닥과 벽은 CharacterController가 콜라이더로 막는다.
        Vector2 moveInput = input.Move.normalized;
        Vector3 move = (brainTransform.forward * moveInput.y + brainTransform.right * moveInput.x)
                       * Time.deltaTime * currentSpeed * trailRangeRatio;
        characterController.Move(move);

        trail.time = remainBulletTime; //남은 게이지만큼 트레일 길이

        if (remainBulletTime <= 0)
        {
            OnGameOver?.Invoke();
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            GameOver();
        }
    }
}