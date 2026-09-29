using UnityEngine;

public class WispFloating : MonoBehaviour
{
    public float period = 4f;       // 왕복 시간(초)
    public float amplitude = 0.5f;    // 중심에서 움직이는 최대 거리

    private float startY;
    private float elapsed;

    private void Start()
    {
        startY = transform.position.y;
    }

    private void Update()
    {
        if (period <= 0f) return;

        elapsed += Time.deltaTime;

        float angle = elapsed / period * 2f * Mathf.PI;
        float targetY = startY + Mathf.Sin(angle) * amplitude;

        transform.Translate(
            Vector3.up * (targetY - transform.position.y),
            Space.World
        );
    }
}
