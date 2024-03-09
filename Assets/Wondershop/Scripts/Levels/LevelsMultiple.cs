using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelsMultiple : MonoBehaviour, ILevels
{
    [SerializeField] [Scene] private string firstSection;
    [SerializeField] [Scene] private string finalSection;
    [SerializeField] [Scene] private string[] sections;

    // settings
    [SerializeField] private int numberOfSections;
    [SerializeField] private bool randomizeSections;
    [SerializeField] private bool sectionsByPlayers;
    [SerializeField] [Range(1, 10)] private int sectionsPerPlayer = 1;

    public List<LevelsSection> levelSections = new();

    private int _levelSectionCounter;
    private Vector3 _levelSectionOffset;
    private readonly List<string> _levelSections = new();

    /**
     * Helpers
     * 
     */
    private void LevelSection(LevelsSection levelSection, int index)
    {
        bool first = index == 0;
        // get the current length
        Vector3 length = new(0f, 0f, levelSection.Offset);
        // apply initial offset on first
        //if (first) _levelSectionOffset -= length;
        // calculate offset
        Vector3 offset = first ? _levelSectionOffset : _levelSectionOffset;
        // set the current section position
        levelSection.transform.SetPositionAndRotation(offset, Quaternion.identity);
        // increase the counter
        if (!first) _levelSectionCounter += 1;
        // increase total offset
        _levelSectionOffset += length;
    }


    /**
     * Unity life-cycle
     * 
     */
    private void OnDisable() => UnloadScenesAsync(_levelSections.ToArray(), 0);

    private static void UnloadScenesAsync(IReadOnlyList<string> scenes, int index)
    {
        if (scenes.Count - 1 < index) return;
        AsyncOperation unloadScene = SceneManager.UnloadSceneAsync(scenes[index]);
        if (unloadScene != null) unloadScene.completed += _ => { UnloadScenesAsync(scenes, index + 1); };
    }


    /**
     * Level management
     * 
     */
    private string IndexSection()
    {
        if (_levelSectionCounter < sections.Length) return sections[_levelSectionCounter];
        int index = _levelSectionCounter - _levelSectionCounter / sections.Length * sections.Length;
        return sections[index];
    }

    private string RandomSection() => sections[Random.Range(0, sections.Length)];
    private string LevelsSection() => randomizeSections ? RandomSection() : IndexSection();

    private IEnumerator LoadLevelSections(UnityAction callback)
    {
        // if number of sections is dynamic to player count
        if (sectionsByPlayers)
            numberOfSections = Mathf.Max(numberOfSections, PlayerManager.Players().Count * sectionsPerPlayer);

        // collect the list of the level sections
        if (firstSection != string.Empty) _levelSections.Add(firstSection);
        for (int i = 0; i < numberOfSections; i++) _levelSections.Add(LevelsSection());
        if (finalSection != string.Empty) _levelSections.Add(finalSection);

        // after collecting the levels, start loading them
        foreach (string levelSection in _levelSections)
        {
            AsyncOperation levelLoad = SceneManager.LoadSceneAsync(levelSection, LoadSceneMode.Additive);
            while (!levelLoad.isDone) yield return null;
        }

        // after loading the level, start moving section so that they line up
        levelSections = FindObjectsOfType<LevelsSection>()
            .OrderBy(levelSection => levelSection.LevelIndex).ToList();
        for (int i = 0; i < levelSections.Count; i++) LevelSection(levelSections[i], i);

        // finally, when done, call the given callback
        callback?.Invoke();
    }

    private IEnumerator UnloadLevelSection(UnityAction callback)
    {
        // reset level variables
        _levelSectionCounter = 0;
        _levelSectionOffset = Vector3.zero;

        // unload the sections
        foreach (string levelSection in _levelSections)
        {
            AsyncOperation levelUnload = SceneManager.UnloadSceneAsync(levelSection);
            while (!levelUnload.isDone) yield return null;
        }

        // clear the sections
        _levelSections.Clear();
        // finally, when done, call the given callback
        callback?.Invoke();
    }

    /**
     * Levels Interface
     * 
     */
    public void CreateLevel(UnityAction callback = null) => StartCoroutine(LoadLevelSections(callback));

    public void RemoveLevel(UnityAction callback = null) => StartCoroutine(UnloadLevelSection(callback));

    public string[] Analytics()
    {
        string[] levels = new string[_levelSections.Count];
        for (int i = 0; i < _levelSections.Count; i++) levels[i] = _levelSections[i];
        return levels;
    }

    public void Disable() => Destroy(this);
}