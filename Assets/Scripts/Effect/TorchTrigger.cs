using UnityEngine;

public class TorchTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] torchLights;
    private ReflectionProbe targetProbe;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetProbe = GetComponent<ReflectionProbe>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (GameObject light in torchLights)
            {
                light.SetActive(true);
            }
            targetProbe.intensity = 1;
            targetProbe.RenderProbe();

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (GameObject light in torchLights)
            {
                light.SetActive(false);
            }
            targetProbe.intensity = 0;
            targetProbe.RenderProbe();
        }
    }
}
