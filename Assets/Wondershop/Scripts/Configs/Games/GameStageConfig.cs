using UnityEngine;


[CreateAssetMenu(fileName = "GameStageConfig", menuName = "Wondershop / Game Stage Config")]
public class GameStageConfig : ScriptableObject
{
    [Scene] public string scenePath;
    public int numberOfRounds;
    public GameStateConfig[] states;
}