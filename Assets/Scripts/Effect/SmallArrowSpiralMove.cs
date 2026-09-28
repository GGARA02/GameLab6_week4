using System.Collections.Generic;
using UnityEngine;

public class SmallArrowSpiralMove : MonoBehaviour
{
    [SerializeField]
    private ArrowController player;
    [SerializeField]
    private GameObject smallArrow;
    [SerializeField]
    private float length = 1f;
    [SerializeField]
    private float maxLengthRatio = 2.0f;
    [SerializeField]
    private float maxVelocity = 5.0f;
    [SerializeField]
    private int poolCount = 10;
    [SerializeField]
    private float rotationSmoothSpeed = 10f;
    [SerializeField]
    private float smoothTime = 0.1f;
    [SerializeField]
    private float smoothTimeStep = 0.05f;
    [SerializeField]
    private AnimationCurve yCurve;
    [SerializeField]
    private float height;
    [SerializeField]
    private float frequency;
    [SerializeField]
    private int wispFrequency;

    private Vector3 offset;

    private CharacterController cc;
    private List<GameObject> smallArrows = new List<GameObject>();
    private List<Vector3> velocities = new List<Vector3>();
    private Vector3 moveDirc = Vector3.forward;
    private int activeCount;
    private float currentLength;

    void Awake()
    {
        cc = player.GetComponent<CharacterController>();
        currentLength = length;
        player.OnBoostUpdate += BoostCountUpdate;
        offset = transform.localPosition;
        for (int i = 0; i < poolCount; i++)
            AddArrow(); ;
    }

    void Start()
    {
        transform.position = player.transform.position + offset;
    }

    void LateUpdate()
    {
        FollowPlayer();
        UpdateArrows();
    }

    private void AddArrow()
    {
        GameObject instance = Instantiate(smallArrow);
        instance.SetActive(false);
        smallArrows.Add(instance);
        velocities.Add(Vector3.zero);
    }

    private void FollowPlayer()
    {
        transform.position = player.transform.position + offset;

        Vector3 flatVel = Vector3.ProjectOnPlane(cc.velocity, Vector3.up);
        if (flatVel.sqrMagnitude > 0.001f)
            moveDirc = flatVel.normalized;

        float t = Mathf.InverseLerp(0f, maxVelocity, cc.velocity.magnitude);
        currentLength = length * Mathf.Lerp(1f, maxLengthRatio, t);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirc), rotationSmoothSpeed * Time.deltaTime);
    }

    private Vector3 GetTarget(int i)
    {
        float t = Time.time * frequency + (float)i / wispFrequency;
        float y = yCurve.Evaluate(t) * height;
        return transform.TransformPoint(new Vector3(0f, y, -(i + 1) * currentLength));
    }

    private void UpdateArrows()
    {
        for (int i = 0; i < activeCount; i++)
        {
            Transform tr = smallArrows[i].transform;

            Vector3 v = velocities[i];
            tr.position = Vector3.SmoothDamp(tr.position, GetTarget(i), ref v, smoothTime + i * smoothTimeStep);
            velocities[i] = v;

            tr.rotation = Quaternion.Slerp(tr.rotation, transform.rotation, rotationSmoothSpeed * Time.deltaTime);
        }
    }

    private void BoostCountUpdate(int currentBoostCount)
    {
        activeCount = Mathf.Max(0, currentBoostCount);

        while (smallArrows.Count < activeCount)
            AddArrow();

        for (int i = 0; i < smallArrows.Count; i++)
        {
            GameObject arrow = smallArrows[i];
            bool active = i < activeCount;

            if (active && !arrow.activeSelf)
            {
                arrow.transform.SetPositionAndRotation(GetTarget(i), transform.rotation);
                velocities[i] = Vector3.zero;
            }

            arrow.SetActive(active);
        }
    }

    public Pose GetArrowPose(int index)
    {
        Transform tr = smallArrows[index].transform;
        return new Pose(tr.position, tr.rotation);
    }
}