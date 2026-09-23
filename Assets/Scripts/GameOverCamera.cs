using Unity.VisualScripting;
using UnityEngine;

public class GameOverCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float yOffeset;
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float radius;
    private float theta = 0f;

    void Update()
    {
        transform.LookAt(target.position);
        transform.localPosition = new Vector3(Mathf.Sin(theta) * radius, yOffeset, Mathf.Cos(theta) * radius);
        theta += Time.deltaTime * Mathf.PI * turnSpeed;
    }
}
