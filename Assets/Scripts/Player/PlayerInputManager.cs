using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public UnityEvent<Vector2> onMove;
    public UnityEvent<bool> onJump;
    public UnityEvent onDash;
    public UnityEvent onAttack;
    public UnityEvent<bool> onUnkuMode;
    public UnityEvent<bool> onShield;

    public void OnMove(InputValue input)
    {
        Vector2 direction = input.Get<Vector2>();
        onMove?.Invoke(direction);
    }

    public void OnJump(InputValue input)
    {
        onJump?.Invoke(input.isPressed);
    }

    public void OnDash(InputValue input)
    {
        if (input.isPressed)
        {
            onDash?.Invoke();
        }
    }

    public void OnAttack(InputValue input)
    {
        if (input.isPressed)
        {
            onAttack?.Invoke();
        }
    }

    public void OnUnkuMode(InputValue input)
    {
        onUnkuMode?.Invoke(input.isPressed);
    }

    public void OnShield(InputValue input)
    {
        onShield?.Invoke(input.isPressed);
    }
}
