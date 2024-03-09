using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

[StructLayout(LayoutKind.Explicit, Size = 32)]
internal struct LeftJoyConInputReport : IInputStateTypeInfo
{
    public FourCC format => new FourCC('H', 'I', 'D');
    [FieldOffset(0)] private readonly byte reportId;

    [InputControl(name = "buttonSouth", displayName = "Down", bit = 0)]
    [InputControl(name = "buttonEast", displayName = "Right", bit = 1)]
    [InputControl(name = "buttonWest", displayName = "Left", bit = 2)]
    [InputControl(name = "buttonNorth", displayName = "Up", bit = 3)]
    [FieldOffset(1)]
    private readonly byte buttons1;

    [InputControl(name = "dpad", format = "BIT", layout = "Dpad", sizeInBits = 4, defaultState = 8)]
    [InputControl(name = "dpad/up", format = "BIT", layout = "DiscreteButton",
        parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7", bit = 0, sizeInBits = 4)]
    [InputControl(name = "dpad/left", format = "BIT", layout = "DiscreteButton", parameters = "minValue=5,maxValue=7",
        bit = 0, sizeInBits = 4)]
    [InputControl(name = "dpad/right", format = "BIT", layout = "DiscreteButton", parameters = "minValue=1,maxValue=3",
        bit = 0, sizeInBits = 4)]
    [InputControl(name = "dpad/down", format = "BIT", layout = "DiscreteButton", parameters = "minValue=3, maxValue=5",
        bit = 0, sizeInBits = 4)]
    [FieldOffset(3)]
    private readonly byte leftStickX;
}