using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

 
    [SerializeField]
     private Transform dynamicPivot;
     private GameObject currentLevelClone;

    [SerializeField]
    private Transform levelParent;

    [SerializeField]
    private LevelScriptableObject levels;

    public EventHandler OnLevelCompleted;

    private int currentLevelIndex = 0; // index of the current level

    private Transform mainCameraTransform;

    private int levelCount;

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

    private int GetLevelCount()
    {
       return  levels.levels.Count;
    }


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

    


    public Transform GetDynamicPivot()
    {
        return dynamicPivot;
    }

    public Transform GetLevelTransform()
    {
        return currentLevelClone.transform;
    }


    public void LevelCompleted()
    {
        currentLevelIndex++;
        SaveCurrentLevelIndex();
        LoadLevel();
        OnLevelCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void SaveCurrentLevelIndex()
    {
        PlayerPrefs.SetInt("Level", currentLevelIndex);
    }

    private void LoadCurrentLevelIndex()
    {
        currentLevelIndex = PlayerPrefs.GetInt("Level");
    }

    public void FullRewind()
    {
        OnFullRewind?.Invoke(this, EventArgs.Empty);
    }
    
}
