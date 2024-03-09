using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

internal struct PlayTagInputReport : IInputStateTypeInfo
{
    public FourCC format => new FourCC('P', 'L', 'A', 'Y');

    [InputControl(name = "button", displayName = "Virtual Button", layout = "Button")]
    public bool Button;

    [InputControl(name = "stick", displayName = "Virtual Stick", layout = "Stick")]
    public Vector2 Stick;
}