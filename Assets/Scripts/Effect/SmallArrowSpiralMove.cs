using UnityEngine;

public class SmallArrowSpiralMove : MonoBehaviour
{
    [SerializeField]
    private ArrowController player;
    [SerializeField]
    private float speed = 100;
    [SerializeField]
    private float rotationSmoothSpeed = 10f;

    private CharacterController cc;
    private Transform child;
    private Vector3 moveDirc = Vector3.forward;

    void Awake()
    {
        cc = player.GetComponent<CharacterController>();

        child = transform.Find("SmallArrowHolder");
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
}
