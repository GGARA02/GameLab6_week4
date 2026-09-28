using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] torchLights;
    [SerializeField]
    private GameObject wispPrefab;
    [SerializeField]
    private float wispSpeed = 50f;

    public System.Action<Transform> OnFirstWispLaunched;
    public System.Action<bool> OnCaveLightChanged;

    public int LightCount => torchLights.Length;

    private ReflectionProbe targetProbe;
    private ArrowController player;
    private bool[] assigned;
    private int lentCount;
    private int litCount;
    private readonly List<GameObject> flyingWisps = new List<GameObject>();

    private void Awake()
    {
        targetProbe = GetComponent<ReflectionProbe>();
        assigned = new bool[torchLights.Length];
    }

    private void Update()
    {
        if (player == null)
            return;

        for (int i = 0; i < torchLights.Length; i++)
        {
            if (assigned[i])
                continue;
            if (!player.TryLendWisp(out Pose from))
                break;

            assigned[i] = true;
            lentCount++;

            Transform wisp = LaunchWisp(i, from);
            if (lentCount == 1)
                OnFirstWispLaunched?.Invoke(wisp);
        }
    }

    public void Enter(ArrowController arrow)
    {
        player = arrow;
    }

    public void Exit()
    {
        if (player == null) return;

        StopAllCoroutines();
        foreach (GameObject wisp in flyingWisps)
            if (wisp != null) Destroy(wisp);
        flyingWisps.Clear();

        bool wasAllLit = litCount == torchLights.Length;
        for (int i = 0; i < torchLights.Length; i++)
        {
            assigned[i] = false;
            torchLights[i].SetActive(false);
        }
        litCount = 0;
        if (wasAllLit) SetCaveLight(false);

        player.ReturnWisps(lentCount);
        lentCount = 0;
        player = null;
    }

    public Transform LaunchWisp(int torchIndex, Pose from)
    {
        GameObject wisp = Instantiate(wispPrefab, from.position, from.rotation);
        flyingWisps.Add(wisp);
        StartCoroutine(Fly(wisp, torchIndex));
        return wisp.transform;
    }

    private IEnumerator Fly(GameObject wisp, int torchIndex)
    {
        Transform tr = wisp.transform;
        Vector3 target = torchLights[torchIndex].transform.position;

        while (tr.position != target)
        {
            tr.position = Vector3.MoveTowards(tr.position, target, wispSpeed * Time.deltaTime);
            yield return null;
        }

        flyingWisps.Remove(wisp);
        Destroy(wisp);
        LightOn(torchIndex);
    }

    private void LightOn(int i)
    {
        torchLights[i].SetActive(true);
        litCount++;
        if (litCount == torchLights.Length)
            SetCaveLight(true);
    }

    private void SetCaveLight(bool on)
    {
        targetProbe.intensity = on ? 1f : 0f;
        OnCaveLightChanged?.Invoke(on);
    }
}