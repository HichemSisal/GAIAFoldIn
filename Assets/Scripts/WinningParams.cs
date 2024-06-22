using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningParams : MonoBehaviour
{
  

[SerializeField]
private List<Position> winningPositions = new List<Position>();

[SerializeField]
private List<int> winningLayers = new List<int>();

    public List<Position> GetWinningPositions()
    {
        return winningPositions;
    }

    public List<int> GetWinningLayers()
    {
        return winningLayers;
    }


}

[System.Serializable]
public class Position
{
    public int x;
    public int z;

    public Position(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}