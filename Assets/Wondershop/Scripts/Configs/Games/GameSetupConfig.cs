using UnityEngine;

[CreateAssetMenu(fileName = "GameSetupConfig", menuName = "Wondershop / Game Setup Config ")]
public class GameSetupConfig : ScriptableObject
{
    [Scene] public string scenePath;
    public GameStageConfig[] stages;
}