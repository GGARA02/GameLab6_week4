using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private float distance;
    [SerializeField]
    private float premiumRatio;
    [SerializeField]
    private Transform target;
    [SerializeField]
    public List<GameObject> premiumPrefabs;
    [SerializeField]
    public GameObject defalutPrefabs;
    [SerializeField]
    public GameObject startPrefabs;

    private GameObject[][] blocks = new GameObject[15][];
    
    private float defalutXY = -700;
    private float currentX = 0;
    private float currentY = 0;
    private bool isActive = true;

    void Update()
    {
        if (isActive)
        {
            if (target.position.x > currentX + 50)
            {
                currentX += 100;
                for (int i = 0; i < 15; i++)
                {
                    Destroy(blocks[0][i]);
                }
                for (int i = 1; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        blocks[i - 1][j] = blocks[i][j];
                    }
                }
                for (int i = 0; i < 15; i++)
                {
                    blocks[14][i] =
                        Instantiate(SelectRandomPrefabs(),
                        new Vector3(currentX - defalutXY, 0, currentY + defalutXY + i * 100),
                        Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
                }
            }
            else if (target.position.x < currentX - 50)
            {
                currentX -= 100;
                for (int i = 0; i < 15; i++)
                {
                    Debug.Log("Destory : " + blocks[14][i]);
                    Destroy(blocks[14][i]);
                }
                for (int i = 13; i >= 0; i--)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        blocks[i + 1][j] = blocks[i][j];
                    }
                }
                for (int i = 0; i < 15; i++)
                {
                    blocks[0][i] =
                        Instantiate(SelectRandomPrefabs(),
                        new Vector3(currentX + defalutXY, 0, currentY + defalutXY + i * 100),
                        Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
                }
            }
            else if (target.position.z > currentY + 50)
            {
                currentY += 100;
                for (int i = 0; i < 15; i++)
                {
                    Destroy(blocks[i][0]);
                }
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 1; j < 15; j++)
                    {
                        blocks[i][j - 1] = blocks[i][j];
                    }
                }
                for (int i = 0; i < 15; i++)
                {
                    blocks[i][14] =
                        Instantiate(SelectRandomPrefabs(),
                        new Vector3(currentX + defalutXY + i * 100, 0, currentY - defalutXY),
                        Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
                }
            }
            else if (target.position.z < currentY - 50)
            {
                currentY -= 100;
                for (int i = 0; i < 15; i++)
                {
                    Destroy(blocks[i][14]);
                }

                for (int j = 13; j >= 0; j--)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        blocks[i][j + 1] = blocks[i][j];
                    }
                }
                for (int i = 0; i < 15; i++)
                {
                    blocks[i][0] =
                             Instantiate(SelectRandomPrefabs(),
                            new Vector3(currentX + defalutXY + i * 100, 0, currentY + defalutXY),
                            Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
                }
            }
        }
    }

    //우리는 항상 77에 있다. //NOT콩콩
    public void Initialize()
    {
        float startXPoint = currentX + defalutXY;
        float startYPoint = currentY + defalutXY;
        for (int i = 0; i < 15; i++)
        {
            blocks[i] = new GameObject[15];
        }
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                if (i == 7 && j == 7)
                {
                    blocks[i][j] = Instantiate(startPrefabs, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                    blocks[i][j].name = "StartPoint";
                }
                else
                {
                    blocks[i][j] = Instantiate(SelectRandomPrefabs(), new Vector3(startXPoint + i * 100, 0, startYPoint + j * 100), Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
                    blocks[i][j].name = "" + i + ", " + j;
                }
            }
        }
    }

    private void OutGrid()
    {

    }

    private GameObject SelectRandomPrefabs()
    {
        int rand = Random.Range(0, 100);
        if (rand < premiumRatio)
        {
            return premiumPrefabs[Random.Range(0, premiumPrefabs.Count)];
        }
        else
        {
            return defalutPrefabs;
        }
    }
    public void GameClear()
    {
        isActive = false;
    }
}
