using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelsSingles : MonoBehaviour, ILevels
{
    [SerializeField] [Scene] private string[] levels;
    [SerializeField] private bool randomizeLevels;

    private int _levelIndex;
    private ILevels _levelsImplementation;
    private int _currentIndex;
    private string currentLevel => levels[_currentIndex];


    /**
     * Unity life-cycle
     *
     */
    private void OnDestroy()
    {
        // make sure to unload the level at least OnDestroy
        // if level removed has not been called previously.
        try
        {
            SceneManager.UnloadSceneAsync(currentLevel);
        }
        catch (Exception)
        {
            // ignored
        }
    }


    /**
     * Level management
     *
     */
    private string LevelByIndex()
    {
        if (_currentIndex < levels.Length) return levels[_currentIndex];
        int index = _currentIndex - _currentIndex / levels.Length * levels.Length;
        _currentIndex = index;
        return levels[index];
    }

    private int RandomIndex() => Random.Range(0, levels.Length);

    private int RoundIndex()
    {
        int roundIndex = Math.Max(GameManager.currentRound, _levelIndex);
        if (roundIndex < levels.Length - 1) return roundIndex;
        // get the current roundIndex accounting rotating index
        return roundIndex - levels.Length * (roundIndex / levels.Length);
    }

    private string LevelSection()
    {
        _currentIndex = randomizeLevels ? RandomIndex() : RoundIndex();
        return LevelByIndex();
    }

    private IEnumerator LoadLevel(UnityAction callback)
    {
        string level = LevelSection();
        AsyncOperation levelLoad = SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
        while (!levelLoad.isDone) yield return null;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(System.IO.Path.GetFileNameWithoutExtension(level)));
        callback?.Invoke();
    }

    private IEnumerator UnloadLevel(UnityAction callback)
    {
        AsyncOperation levelUnload = SceneManager.UnloadSceneAsync(currentLevel);
        while (!levelUnload.isDone) yield return null;
        callback?.Invoke();
        _levelIndex += 1;
    }

    /**
     * Levels Interface
     *
     */
    public void CreateLevel(UnityAction callback = null)
    {
        if (enabled) StartCoroutine(LoadLevel(callback));
        else callback?.Invoke();
    }

    public void RemoveLevel(UnityAction callback = null)
    {
        bool validLevel = SceneManager.GetSceneByName(currentLevel).IsValid();
        if (enabled && validLevel) StartCoroutine(UnloadLevel(callback));
        else callback?.Invoke();
    }

    public string[] Analytics() => new[] { "" };

    public void Disable() => enabled = false;
}