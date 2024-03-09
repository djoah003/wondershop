using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;


[InputControlLayout(displayName = "PlayTag", stateType = typeof(PlayTagInputReport))]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class PlayTagInputDevice : InputDevice
{
    public StickControl stick { get; private set; }
    public ButtonControl button { get; private set; }

    public new static IEnumerable<PlayTagInputDevice> all => AllPlayTagInputDevices;
    private static readonly List<PlayTagInputDevice> AllPlayTagInputDevices = new();

    protected override void OnAdded()
    {
        base.OnAdded();
        AllPlayTagInputDevices.Add(this);
    }

    protected override void OnRemoved()
    {
        base.OnRemoved();
        AllPlayTagInputDevices.Remove(this);
    }

    protected override void FinishSetup()
    {
        base.FinishSetup();
        stick = GetChildControl<StickControl>("stick");
        button = GetChildControl<ButtonControl>("button");
    }

    static PlayTagInputDevice() => InputSystem.RegisterLayout<PlayTagInputDevice>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeInPlayer()
    {
    }
}