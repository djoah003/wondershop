using UnityEngine.Events;

public interface ILevels
{
    /// <summary>
    /// Generates or loads the next level
    /// </summary>
    public void CreateLevel(UnityAction callback = null);

    /// <summary>
    /// Removes the current level, be it scene or prefab
    /// </summary>
    public void RemoveLevel(UnityAction callback = null);

    /// <summary>
    /// Returns list of string of level information
    /// </summary>
    public string[] Analytics();

    /// <summary>
    /// Custom disable function
    /// </summary>
    public void Disable();
}