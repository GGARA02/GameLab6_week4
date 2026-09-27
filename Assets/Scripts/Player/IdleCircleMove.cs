using UnityEngine;

public class IdleCircleMove : MonoBehaviour
{
    [SerializeField]
    private float idleCircleRadius = 5f;
    [SerializeField]
    private float idleCircleSpeed = 2f;
    [SerializeField]
    private float transitionSpeed = 3f;

    private CharacterController cc;

    private float currentAngle = 0f;
    private Vector3 circleCenter;
    private bool isTransitioningToIdle = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cc.velocity.sqrMagnitude < 0.001f)
        {
            if (!isTransitioningToIdle)
            {
                circleCenter = transform.position + transform.forward * idleCircleRadius;

                Vector3 offset = transform.position - circleCenter;
                currentAngle = Mathf.Atan2(offset.z, offset.x);

                isTransitioningToIdle = true;
            }
        }
        CircleAround();
    }

    private void CircleAround()
    {
        currentAngle += idleCircleSpeed * Time.deltaTime;

        float x = circleCenter.x + Mathf.Cos(currentAngle) * idleCircleRadius;
        float z = circleCenter.z + Mathf.Sin(currentAngle) * idleCircleRadius;
        Vector3 targetPos = new Vector3(x, transform.position.y, z);

        Vector3 moveDirc = (targetPos - transform.position).normalized;
        if (moveDirc != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirc);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, transitionSpeed * Time.deltaTime);
        }
        transform.position = Vector3.Lerp(transform.position, targetPos, transitionSpeed * Time.deltaTime);
    }
}
