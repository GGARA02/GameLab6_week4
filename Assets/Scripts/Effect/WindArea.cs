using System;
using UnityEngine;

[Serializable]
public struct WindSetting
{
    public Vector3 dir;
    public float speed;
    public WindSetting(Vector3 dir, float speed)
    {
        this.dir = dir.normalized;
        this.speed = speed;
    }
}

public class WindArea : MonoBehaviour
{

    [SerializeField]
    private WindSetting windSetting = new WindSetting();
    public WindSetting Wind => windSetting;
}
