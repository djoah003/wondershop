using UnityEngine;

public class LevelsSection : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private ScriptableEventForGameObject onLevelSectionSpawned;
    public int LevelIndex { get; private set; }
    public float Offset => boxCollider.bounds.size.z;
    public Vector3 Middle => boxCollider.bounds.center;

    private void Awake()
    {
        LevelIndex = FindObjectsOfType<LevelsSection>().Length;
        onLevelSectionSpawned.TriggerEvent(gameObject);
    }
}