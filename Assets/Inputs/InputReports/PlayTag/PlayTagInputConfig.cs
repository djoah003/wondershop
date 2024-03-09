using UnityEngine;

public enum Control
{
    Version1,
    Version2,
};

public class PlayTagInputConfig
{
    // info
    public readonly int Index;
    public readonly string Serial;
    
    // config
    public Control Control;
    
    // input
    public Vector2 PrevTouch = new(-1.0f, -1.0f);
    public bool PrevPress;

    public PlayTagInputConfig(int index, string serial, Control control = Control.Version1)
    {
        Index = index; Serial = serial; Control = control;
    }
}