using System.Collections.Generic;
using UnityEngine;

//블록이 생성될 때 블록 안에 건물과 불씨를 랜덤으로 배치한다.
public class CreateBlock : MonoBehaviour
{
    [Header("Building")]
    [SerializeField]
    private List<GameObject> buildingPrefabs;
    [SerializeField]
    private float yMinRange = 0.8f; //건물 높이 배율 최소
    [SerializeField]
    private float yMaxRange = 2f; //건물 높이 배율 최대
    [SerializeField]
    private float checkRadius = 40f; //건물끼리 최소 간격
    [Header("Ember")]
    [SerializeField]
    private GameObject emberPrefab;
    [SerializeField]
    private float emberRatio = 20f; //블록에 불씨가 생길 확률 (%)
    [SerializeField]
    private float yEmberMinRange = 5f; //불씨 높이 최소
    [SerializeField]
    private float yEmberMaxRange = 50f; //불씨 높이 최대
    [SerializeField]
    private float checkEmberRadius = 25f; //불씨와 다른 오브젝트의 최소 간격

    private const int MaxTryCount = 100; //배치 시도 횟수
    private const float BuildingArea = 40f; //블록 중심에서 건물이 놓일 수 있는 범위 (±)
    private const float EmberArea = 45f; //블록 중심에서 불씨가 놓일 수 있는 범위 (±)

    private readonly List<Vector3> occupiedPositions = new List<Vector3>(); //이미 배치된 위치 (높이 무시)

    void Start()
    {
        //불씨를 먼저 놓고, 건물은 불씨를 피해서 놓는다.
        if (Random.Range(0, 100) < emberRatio)
        {
            CreateEmber();
        }
        CreateBuildings();
    }

    [ContextMenu("블럭 생성")]
    public void CreateBuildings()
    {
        for (int i = 0; i < MaxTryCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-BuildingArea, BuildingArea), 0, Random.Range(-BuildingArea, BuildingArea));
            if (IsOverlapped(position, checkRadius))
            {
                continue;
            }

            GameObject building = Instantiate(buildingPrefabs[Random.Range(0, buildingPrefabs.Count)], transform);
            Vector3 scale = building.transform.localScale;
            scale.y *= Random.Range(yMinRange, yMaxRange);
            building.transform.localScale = scale;
            building.transform.localPosition = position;
            occupiedPositions.Add(position);
        }
    }

    //빈 자리를 찾으면 불씨 하나를 놓는다.
    private void CreateEmber()
    {
        for (int i = 0; i < MaxTryCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-EmberArea, EmberArea), 0, Random.Range(-EmberArea, EmberArea));
            if (IsOverlapped(position, checkEmberRadius))
            {
                continue;
            }

            GameObject ember = Instantiate(emberPrefab, transform);
            ember.transform.localPosition = new Vector3(position.x, Random.Range(yEmberMinRange, yEmberMaxRange), position.z);
            occupiedPositions.Add(position);
            return;
        }
    }

    private bool IsOverlapped(Vector3 position, float radius)
    {
        foreach (Vector3 occupied in occupiedPositions)
        {
            if (Vector3.Distance(occupied, position) < radius)
            {
                return true;
            }
        }
        return false;
    }
}