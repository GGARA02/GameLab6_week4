using UnityEngine;

public class WispEffect : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem enablePs;
    [SerializeField]
    private ParticleSystem disablePs;

    private void OnEnable()
    {
        Instantiate(enablePs, transform.position, Quaternion.identity);
    }

    private void OnDisable()
    {
        Instantiate(disablePs, transform.position, Quaternion.identity);
    }
}
