using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

[InputControlLayout(stateType = typeof(LeftJoyConInputReport))]
#if UNITY_EDITOR
[InitializeOnLoad] // Make sure static constructor is called during startup.
#endif
public class LeftJoyConGamepadHid : Gamepad
{
    static LeftJoyConGamepadHid()
    {
        InputSystem.RegisterLayout<LeftJoyConGamepadHid>(
            matches: new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x57E) // Nintendo
                .WithCapability("productId", 0x2006)); // L
    }

    // In the Player, to trigger the calling of the static constructor,
    // create an empty method annotated with RuntimeInitializeOnLoadMethod.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
    }
}