using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//플레이어 주변에 15x15 블록을 깔고, 플레이어가 움직이면 한 줄씩 지우고 새로 만든다. (무한 맵)
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private float premiumRatio = 30f; //프리미엄 블록이 나올 확률 (%)
    [SerializeField]
    private Transform target; //기준이 되는 플레이어
    [SerializeField]
    private List<GameObject> premiumPrefabs;
    [SerializeField, FormerlySerializedAs("defalutPrefabs")]
    private GameObject defaultPrefab;
    [SerializeField, FormerlySerializedAs("startPrefabs")]
    private GameObject startPrefab; //가운데 시작 블록

    private const int GridSize = 11;
    private const int LastIndex = GridSize - 1;
    private const int CenterIndex = GridSize / 2;
    private const float CellSize = 100f;
    private const float HalfCell = CellSize / 2;
    private const float OriginOffset = -CenterIndex * CellSize; //가운데 블록이 (0, 0)에 오도록 하는 시작 위치

    private readonly GameObject[][] blocks = new GameObject[GridSize][]; //[x][z]
    private float currentX = 0; //그리드 중심
    private float currentZ = 0;
    private bool isActive = true;

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        //플레이어가 중심에서 반 칸 이상 벗어나면 그 방향으로 한 줄 이동
        if (target.position.x > currentX + HalfCell)
        {
            ShiftX(1);
        }
        else if (target.position.x < currentX - HalfCell)
        {
            ShiftX(-1);
        }
        else if (target.position.z > currentZ + HalfCell)
        {
            ShiftZ(1);
        }
        else if (target.position.z < currentZ - HalfCell)
        {
            ShiftZ(-1);
        }
    }

    public void Initialize()
    {
        for (int x = 0; x < GridSize; x++)
        {
            blocks[x] = new GameObject[GridSize];
            for (int z = 0; z < GridSize; z++)
            {
                if (x == CenterIndex && z == CenterIndex)
                {
                    blocks[x][z] = Instantiate(startPrefab, Vector3.zero, Quaternion.identity);
                    blocks[x][z].name = "StartPoint";
                }
                else
                {
                    blocks[x][z] = CreateRandomBlock(x, z);
                    blocks[x][z].name = x + ", " + z;
                }
            }
        }
    }

    public void GameClear()
    {
        isActive = false;
    }

    //x 방향으로 한 줄 이동: 반대쪽 줄을 지우고, 배열을 한 칸씩 밀고, 진행 방향 끝에 새 줄을 만든다.
    private void ShiftX(int direction)
    {
        currentX += direction * CellSize;
        int removeX = direction > 0 ? 0 : LastIndex;
        int addX = direction > 0 ? LastIndex : 0;

        for (int z = 0; z < GridSize; z++)
        {
            Destroy(blocks[removeX][z]);
        }

        if (direction > 0)
        {
            for (int x = 1; x < GridSize; x++)
            {
                for (int z = 0; z < GridSize; z++)
                {
                    blocks[x - 1][z] = blocks[x][z];
                }
            }
        }
        else
        {
            for (int x = LastIndex - 1; x >= 0; x--)
            {
                for (int z = 0; z < GridSize; z++)
                {
                    blocks[x + 1][z] = blocks[x][z];
                }
            }
        }

        for (int z = 0; z < GridSize; z++)
        {
            blocks[addX][z] = CreateRandomBlock(addX, z);
        }
    }

    //z 방향으로 한 줄 이동 (ShiftX와 같은 방식)
    private void ShiftZ(int direction)
    {
        currentZ += direction * CellSize;
        int removeZ = direction > 0 ? 0 : LastIndex;
        int addZ = direction > 0 ? LastIndex : 0;

        for (int x = 0; x < GridSize; x++)
        {
            Destroy(blocks[x][removeZ]);
        }

        if (direction > 0)
        {
            for (int z = 1; z < GridSize; z++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    blocks[x][z - 1] = blocks[x][z];
                }
            }
        }
        else
        {
            for (int z = LastIndex - 1; z >= 0; z--)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    blocks[x][z + 1] = blocks[x][z];
                }
            }
        }

        for (int x = 0; x < GridSize; x++)
        {
            blocks[x][addZ] = CreateRandomBlock(x, addZ);
        }
    }

    //(x, z) 칸에 랜덤 블록을 랜덤 방향(90도 단위)으로 만든다.
    private GameObject CreateRandomBlock(int x, int z)
    {
        Vector3 position = new Vector3(currentX + OriginOffset + x * CellSize, 0, currentZ + OriginOffset + z * CellSize);
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);
        return Instantiate(SelectRandomPrefab(), position, rotation);
    }

    private GameObject SelectRandomPrefab()
    {
        if (Random.Range(0, 100) < premiumRatio)
        {
            return premiumPrefabs[Random.Range(0, premiumPrefabs.Count)];
        }
        return defaultPrefab;
    }
}