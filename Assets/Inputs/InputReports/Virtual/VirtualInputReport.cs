using System.Numerics;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

internal struct VirtualInputReport : IInputStateTypeInfo
{
    public FourCC format => new('F', 'A', 'K', 'E');

    [InputControl(name = "button", displayName = "Virtual Button", layout = "Button")]
    public bool Button;

    [InputControl(name = "stick", displayName = "Virtual Stick", layout = "Stick")]
    public Vector2 Stick;
}