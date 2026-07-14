using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

// Detects taps and figures out which rail they landed on.
// Uses Enhanced Touch for multi-touch on device, plus mouse clicks
// so it can be tested in the editor without a touchscreen.
public class InputManager : MonoBehaviour
{
    [Range(3, 5)]
    public int railCount = 4;

    public event Action<int, double> OnRailTapped;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += HandleFingerDown;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= HandleFingerDown;
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            RegisterTap(Mouse.current.position.ReadValue());
        }
    }

    private void HandleFingerDown(Finger finger)
    {
        RegisterTap(finger.screenPosition);
    }

    private void RegisterTap(Vector2 screenPosition)
    {
        int rail = GetRailFromScreenPosition(screenPosition);
        if (rail < 0) return;

        OnRailTapped?.Invoke(rail, AudioSettings.dspTime);
    }

    private int GetRailFromScreenPosition(Vector2 screenPosition)
    {
        float railWidth = Screen.width / (float)railCount;
        int rail = Mathf.FloorToInt(screenPosition.x / railWidth);
        return Mathf.Clamp(rail, 0, railCount - 1);
    }
}
