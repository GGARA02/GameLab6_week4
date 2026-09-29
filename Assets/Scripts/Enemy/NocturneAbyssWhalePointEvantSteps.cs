using System;
using System.Collections.Generic;
using UnityEngine;

public class NocturneAbyssWhalePointEvantSteps : MonoBehaviour
{
    public enum WhaleEvantType
    {
        MoveBackward, 
        MoveForward
    }

    [Serializable]
    public class WhaleEvant
    {
        public WhaleEvantType type;
        public float distance = 1f;
        public float duration = 1f;
    }

    [Tooltip("실행할 이벤트 목록")]
    [SerializeField] private List<WhaleEvant> evants = new List<WhaleEvant>(); // 

    [Tooltip("고래가 이벤트를 시작하기 위해 도착해야 하는 거리")]
    [SerializeField] private float arrivalDistance = 0.1f; // 고래가 이벤트를 시작하기 위해 도착해야 하는 거리

    private int currentEvantIndex; // 현재 실행 중인 이벤트의 인덱스
    private bool isEvantRunning; // 이벤트가 실행 중인지 여부

    private float evantTimer; // 이벤트 진행 시간
    private Vector3 evantStartPosition; // 이벤트 시작 위치
    private Vector3 evantTargetPosition; // 이벤트 목표 위치

    public bool UpdateEvants(Transform whale)
    {
        if(currentEvantIndex >= evants.Count)
            return false; // 모든 이벤트가 완료되었으면 더 이상 실행하지 않음

        if(!isEvantRunning) // 이벤트가 실행 중이 아니면
        {
            float distanceToPoint = Vector3.Distance(whale.position, transform.position); // 고래와 이벤트 지점 사이의 거리 계산

            if(distanceToPoint > arrivalDistance) // 고래가 이벤트 지점에 도착하지 않았으면 실행하지 않아요.
                return false;

            WhaleEvant evant = evants[currentEvantIndex]; // 현재 이벤트 가져오기

            evantTimer = 0f; 
            evantStartPosition = whale.position;

            Vector3 direction = evant.type == WhaleEvantType.MoveBackward 
                ? -whale.forward
                : whale.forward; // 이벤트 타입에 따라 이동 방향 결정

            evantTargetPosition = whale.position + direction * evant.distance; // 목표 위치 계산

            isEvantRunning = true; // 이벤트 실행 상태로 변경
        }

        WhaleEvant currentEvant = evants[currentEvantIndex]; // 현재 이벤트 가져오기

        evantTimer += Time.deltaTime; // 이벤트 진행 시간 업데이트

        float progress; // 이벤트 진행률 계산

        if(currentEvant.duration <= 0f) // 이벤트 지속 시간이 0 이하이면 즉시 완료 하는건데, 빠르게 이벤트를 만들어 내기 위해서 지속시간 계산을 넣었어요. 걍 스무스 하게 뒤로 보내고 몇초뒤에 끝내면 되니까. 
            progress = 1f;
        else
            progress = Mathf.Clamp01(evantTimer / currentEvant.duration); 

        whale.position = Vector3.Lerp( 
            evantStartPosition,
            evantTargetPosition,
            progress
        );

        if(progress >= 1f) //이벤트가 완료되었으면
        {
            currentEvantIndex++; // 다음 이벤트로 이동
            isEvantRunning = false; // 이벤트 실행 상태 초기화 어떻게 한건지는 모름. 이렇게 하면 초기화 되던데?

            if(currentEvantIndex >= evants.Count) 
                return false;// 모든 이벤트가 완료되었으면 더 이상 실행하지 않음
        }

        return true; // 이벤트가 실행 중이면 true 반환
    }
}
