using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class WallMatChanger : MonoBehaviour
{

    private Renderer rend;
    private bool isInvisible = false;
    private bool isLightUp = false;
    [SerializeField]
    private Material defaultMat;
    [SerializeField]
    private Material invisibleMat;
    [SerializeField]
    private Material lightUpMat;
    [SerializeField]
    private Material superMat;

    private Coroutine isWaiting;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public void Lightup()
    {
        isLightUp = true;
        UpdateMat();
    }

    public void Invisible()
    {
        if (isWaiting == null)
        {
            isInvisible = true;
            UpdateMat();
            isWaiting = StartCoroutine(Wait());
        }
    }

    private void UpdateMat()
    {
        Debug.Log("업데이트중 ");
        if (isInvisible)
        {
            if (isLightUp)
            {
                rend.sharedMaterial = superMat;
            }
            else
            {
                rend.sharedMaterial = invisibleMat;
            }
        }
        else
        {
            if (isLightUp)
            {
                rend.sharedMaterial = lightUpMat;
            }
            else
            {
                rend.sharedMaterial = defaultMat;
            }
        }
    }
    
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2.0f);
        isInvisible = false;
        UpdateMat();
        isWaiting = null;
    }
}
