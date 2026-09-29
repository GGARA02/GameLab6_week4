using UnityEngine;

public class WispFloating : MonoBehaviour
{
    public float period = 4f; // 한 번 왕복하는 시간(초)
    private float elapsed;

    private void Update()
    {
        elapsed += Time.deltaTime;

        float angle = elapsed / period * 2f * Mathf.PI;
        float targetY = 4.5f + Mathf.Sin(angle) * 0.5f;

        transform.Translate(
            Vector3.up * (targetY - transform.position.y),
            Space.World
        );
    }
}
