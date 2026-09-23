using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreateBlock : MonoBehaviour
{
    [SerializeField]
    private float yMinRange;
    [SerializeField]
    private float yMaxRange;
    [SerializeField]
    private float yEmberMinRange = 3;
    [SerializeField]
    private float yEmberMaxRange = 35;
    [SerializeField]
    private float checkRadius;
    [SerializeField]
    private float checkEmberRadius;
    [SerializeField]
    private float emberRatio;
    [SerializeField]
    public GameObject emberPrefab;

    public List<GameObject> buildingPrefabs;

    private List<Vector3> posVecs;
    private float emberCount;

    void Start()
    {
        posVecs = new List<Vector3>();
        if (Random.Range(0, 100) < emberRatio)
        {
            emberCount = 1;
            CreateEmberPrefab();
        }
        else
        {
            emberCount = 0;
        }
        CreateBlockPrefab();
    }



    [ContextMenu("블럭 생성")]
    public void CreateBlockPrefab()
    {

        for (int k = 0; k < 100; k++)
        {
            //int i = Random.Range(0, 500) % 5;
            //int j = Random.Range(0, 500) % 5;
            //Debug.Log(i + " " + j);

            //float x = -50 + i * 20 + 10;
            //float y = -50 + j * 20 + 10;
            //x += Random.Range(ijMinRange, ijMaxRange);
            //y += Random.Range(ijMinRange, ijMaxRange);

            float x = Random.Range(-40f, 40f);
            float y = Random.Range(-40f, 40f);
            bool isOverLap = false;
            foreach(Vector3 v in posVecs)
            {
                if (Vector3.Distance(v, new Vector3(x, 0, y)) < checkRadius)
                {
                    isOverLap = true;
                    break;
                }
            }
            if (!isOverLap)
            {
                GameObject instance = Instantiate(buildingPrefabs[Random.Range(0, buildingPrefabs.Count)], this.transform);
                instance.transform.localScale = new Vector3(instance.transform.localScale.x, instance.transform.localScale.y * Random.Range(yMinRange, yMaxRange), instance.transform.localScale.z * 1);
                instance.transform.localPosition = new Vector3(x, 0, y);
                posVecs.Add(new Vector3(x, 0, y));
            }
            //StartCoroutine(createCorutine());
        }
    }

    public void CreateEmberPrefab()
    {
        int count = 0;
        for (int i = 0; i < 100; i++)
        {
            float x = Random.Range(-45f, 45f);
            float y = Random.Range(-45f, 45f);
            bool isOverLap = false;
            foreach (Vector3 v in posVecs)
            {
                if (Vector3.Distance(v, new Vector3(x, 0, y)) < checkEmberRadius)
                {
                    isOverLap = true;
                    break;
                }
            }
            if (!isOverLap)
            {
                GameObject instance = Instantiate(emberPrefab, this.transform);
                instance.transform.localPosition = new Vector3(x, Random.Range(yEmberMinRange, yEmberMaxRange), y);
                posVecs.Add(new Vector3(x, 0, y));
                count++;
            }
            if (count >= emberCount)
            {
                return;
            }
        }
    }

    [ContextMenu("취~소!")]
    public void DestroyBuilding()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach(GameObject wall in walls)
        {
            DestroyImmediate(wall);
        }
    }


}
 