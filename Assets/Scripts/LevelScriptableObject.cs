using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Levels", menuName = "ScriptableObjects/LevelSciptableObject", order = 1)]
public class LevelScriptableObject : ScriptableObject
{
   public List<Level> levels = new List<Level>();
}


//
[System.Serializable]
public class Level
{
    public GameObject level;
    public Vector3 cameraPosition;
    public Vector3 cameraRotation; 
}