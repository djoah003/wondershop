using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;


[InputControlLayout(displayName = "Virtual", stateType = typeof(VirtualInputReport))]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class VirtualInputDevice : InputDevice
{
    public StickControl stick { get; private set; }
    public ButtonControl button { get; private set; }

    public new static IEnumerable<VirtualInputDevice> all => AllVirtualInputDevices;
    private static readonly List<VirtualInputDevice> AllVirtualInputDevices = new();

    protected override void OnAdded()
    {
        base.OnAdded();
        AllVirtualInputDevices.Add(this);
    }

    protected override void OnRemoved()
    {
        base.OnRemoved();
        AllVirtualInputDevices.Remove(this);
    }

    protected override void FinishSetup()
    {
        base.FinishSetup();
        stick = GetChildControl<StickControl>("stick");
        button = GetChildControl<ButtonControl>("button");
    }

    static VirtualInputDevice() => InputSystem.RegisterLayout<VirtualInputDevice>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeInPlayer()
    {
    }
}