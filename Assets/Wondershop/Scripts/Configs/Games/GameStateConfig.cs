using UnityEngine;


[CreateAssetMenu(fileName = "GameStateConfig", menuName = "Wondershop / Game State Config")]
public class GameStateConfig : ScriptableObject
{
    [Scene] public string scenePath;
}