using UnityEngine;

public class NocturneAbyssWhaleOnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
            other.gameObject.SetActive(true);
    }
}
