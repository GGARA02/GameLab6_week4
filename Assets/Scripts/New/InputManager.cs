using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    //Player
    //카메라의 움직임(Look)은 시네머신에서 처리한다.
    public Vector2 Move => inputActions?.Player.Move.ReadValue<Vector2>() ?? Vector2.zero;
    public bool BulletTimePressed => inputActions?.Player.BulletTime.WasPressedThisFrame() ?? false;
    public bool BulletTimeReleased => inputActions?.Player.BulletTime.WasReleasedThisFrame() ?? false;

    //System
    public bool InteractPressed => inputActions?.System.Interact.WasPressedThisFrame() ?? false;

    public void Initialize()
    {
        inputActions = new InputSystem_Actions();
        PlayerEnable();
    }

    public void PlayerEnable()
    {
        inputActions.Player.Enable();
    }

    public void PlayerDisable()
    {
        inputActions.Player.Disable();
    }

    public void UIEnable()
    {
        inputActions.UI.Enable();
    }

    public void UIDisable()
    {
        inputActions.UI.Disable();
    }

    public void SystemEnable()
    {
        inputActions.System.Enable();
    }

    public void SystemDisable()
    {
        inputActions.System.Disable();
    }

    public void Dispose()
    {
        if (inputActions == null)
        {
            return;
        }
        inputActions.Disable();
        inputActions.Dispose();
        inputActions = null;
    }
}