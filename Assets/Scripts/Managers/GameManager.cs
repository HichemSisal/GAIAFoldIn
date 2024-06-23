using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The instance of the game manager.
    /// </summary>
    public static GameManager Instance;

    [SerializeField]
    private Transform dynamicPivot; // dynamic pivot of the block

    private GameObject currentLevelClone; // current level clone

    [SerializeField]
    private Transform levelParent; // parent of the level

    [SerializeField]
    private LevelScriptableObject levels; // levels of the game

    /// <summary>
    /// Event handler for level completed.
    /// </summary>
    public EventHandler OnLevelCompleted;

    private int currentLevelIndex = 0; // index of the current level

    private Transform mainCameraTransform; // main camera transform

    private int levelCount; // count of the levels

    /// <summary>
    /// Event handler for full rewind.
    /// </summary>
    public EventHandler OnFullRewind;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mainCameraTransform = Camera.main.transform;
        levelCount = GetLevelCount();
        LoadCurrentLevelIndex();
        LoadLevel();
    }

    /// <summary>
    /// Gets the count of the levels.
    /// </summary>
    /// <returns>The count of the levels.</returns>
    private int GetLevelCount()
    {
        return levels.levels.Count;
    }

    /// <summary>
    /// Loads the current level.
    /// </summary>
    public void LoadLevel()
    {
        if (currentLevelClone != null)
        {
            Destroy(currentLevelClone);
        }

        if (currentLevelIndex >= levelCount)
        {
            currentLevelIndex = 0;
        }

        currentLevelClone = Instantiate(levels.levels[currentLevelIndex].level, levelParent);
        mainCameraTransform.position = levels.levels[currentLevelIndex].cameraPosition;
        mainCameraTransform.rotation = Quaternion.Euler(levels.levels[currentLevelIndex].cameraRotation);
        levelCount = levels.levels.Count;
    }

    /// <summary>
    /// Gets the dynamic pivot transform.
    /// </summary>
    /// <returns>The dynamic pivot transform.</returns>
    public Transform GetDynamicPivot()
    {
        return dynamicPivot;
    }

    /// <summary>
    /// Gets the transform of the current level.
    /// </summary>
    /// <returns>The transform of the current level.</returns>
    public Transform GetLevelTransform()
    {
        return currentLevelClone.transform;
    }

    /// <summary>
    /// Called when a level is completed.
    /// </summary>
    public void LevelCompleted()
    {
        currentLevelIndex++;
        SaveCurrentLevelIndex();
        LoadLevel();
        OnLevelCompleted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Saves the current level index.
    /// </summary>
    private void SaveCurrentLevelIndex()
    {
        PlayerPrefs.SetInt("Level", currentLevelIndex);
    }

    /// <summary>
    /// Loads the current level index.
    /// </summary>
    private void LoadCurrentLevelIndex()
    {
        currentLevelIndex = PlayerPrefs.GetInt("Level");
    }

    /// <summary>
    /// Triggers a full rewind.
    /// </summary>
    public void FullRewind()
    {
        OnFullRewind?.Invoke(this, EventArgs.Empty);
    }
}
