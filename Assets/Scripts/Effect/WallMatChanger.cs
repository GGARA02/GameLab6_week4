using System.Collections;
using UnityEngine;

//카메라와 플레이어 사이를 가리는 벽을 잠시 투명하게 바꾼다. (VoulmeManager.DetectWall에서 호출)
public class WallMatChanger : MonoBehaviour
{
    [SerializeField]
    private Material defaultMat;
    [SerializeField]
    private Material invisibleMat;
    [SerializeField]
    private float invisibleTime = 2f; //투명하게 유지되는 시간

    private Renderer rend;
    private Coroutine invisibleRoutine;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    //이미 투명한 동안에는 다시 불려도 무시한다.
    public void Invisible()
    {
        if (invisibleRoutine != null)
        {
            return;
        }
        invisibleRoutine = StartCoroutine(InvisibleRoutine());
    }

    private IEnumerator InvisibleRoutine()
    {
        rend.sharedMaterial = invisibleMat;
        yield return new WaitForSeconds(invisibleTime);
        rend.sharedMaterial = defaultMat;
        invisibleRoutine = null;
    }
}