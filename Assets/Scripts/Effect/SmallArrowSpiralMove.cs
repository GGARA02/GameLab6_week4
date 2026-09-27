using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SmallArrowSpiralMove : MonoBehaviour
{
    [SerializeField]
    private ArrowController player;
    [SerializeField]
    private GameObject smallArrow;
    [SerializeField]
    private float radius = 1;
    [SerializeField]
    private float speed = 100;
    [SerializeField]
    private float rotationSmoothSpeed = 10f;
    [SerializeField]
    private int poolCount = 10;
    private CharacterController cc;
    private Transform child;
    private Vector3 moveDirc = Vector3.forward;
    private List<GameObject> smallArrows;

    void Awake()
    {
        cc = player.GetComponent<CharacterController>();

        child = transform.Find("SmallArrowHolder");
        player.OnBoostUpdate += BoostCountUpdate;

        smallArrows = new List<GameObject>();
        for (int i = 0; i < poolCount; i++)
        {
            GameObject instance = Instantiate(smallArrow, child.transform);
            instance.SetActive(false);
            smallArrows.Add(instance);
        }
    }

    void Start()
    {
        if (player != null)
        {
            transform.position = player.transform.position;
        }
    }

    void Update()
    {
        FollowPlayer();

        child.Rotate(Vector3.forward * speed * Time.deltaTime);
    }

    private void FollowPlayer()
    {
        Vector3 targetPos = player.transform.position;

        transform.position = Vector3.Lerp(transform.position, targetPos, 10f * Time.deltaTime);

        if (cc.velocity.sqrMagnitude > 0.001f)
        {
            moveDirc = cc.velocity.normalized;
        }

        Quaternion lookDirc = Quaternion.LookRotation(moveDirc);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirc, rotationSmoothSpeed * Time.deltaTime);
    }

    private void BoostCountUpdate(int currentBoostCount)
    {
        currentBoostCount = Mathf.Max(0, currentBoostCount);

        // 풀이 모자라면 추가 생성
        while (smallArrows.Count < currentBoostCount)
        {
            GameObject instance = Instantiate(smallArrow, child);
            instance.SetActive(false);
            smallArrows.Add(instance);
        }

        float theta = currentBoostCount > 0 ? 360f / currentBoostCount : 0f;

        for (int i = 0; i < smallArrows.Count; i++)
        {
            bool active = i < currentBoostCount;
            smallArrows[i].SetActive(active);
            if (!active) continue;

            float rad = theta * i * Mathf.Deg2Rad;
            smallArrows[i].transform.localPosition = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0f) * radius;
            smallArrows[i].transform.localRotation = Quaternion.identity;
        }
    }
}
