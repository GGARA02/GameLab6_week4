using System.Collections;
using UnityEngine;

public class BuildingAttack : MonoBehaviour
{
    [SerializeField] private GameObject[] buildings;
    [SerializeField] private int buildingCount = 10;
    private GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;
            StartCoroutine(LaunchBuilding());
        }
    }

    private IEnumerator LaunchBuilding()
    {
        float timeElapsed = 0;
        while (timeElapsed < 1f)
        {
            RenderSettings.reflectionIntensity = Mathf.Lerp(0.5f, 0, timeElapsed / 1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        timeElapsed = 0;
        while (timeElapsed < 1f)
        {
            RenderSettings.reflectionIntensity = Mathf.Lerp(0, 0.5f, timeElapsed / 1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        WaitForSeconds sec = new WaitForSeconds(0.1f);
        for (int i = 0; i < buildingCount; i++)
        {
            float xPos = Random.Range(-200, 200);
            float zPos = player.transform.position.z - Random.Range(900, 1000);
            Vector3 pos = new Vector3(xPos, -1500, zPos);
            Instantiate(buildings[Random.Range(0, 3)], pos, Quaternion.identity);
            yield return sec;
        }
    }
}
