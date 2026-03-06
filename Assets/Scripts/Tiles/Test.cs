using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Test", menuName = "Scriptable Objects/Test")]
public class Test : ScriptableObject
{

    public List<List<Vector2>> elevations = new List<List<Vector2>>();
    public List<Vector2> currentHeight = new List<Vector2>();
    public List<Vector2> checkedTiles = new List<Vector2>();
    public List<Vector2> safeTiles = new List<Vector2>();
    public List<Vector2> unSafeTiles = new List<Vector2>();
    public List<Vector2> impassableTiles = new List<Vector2>();
}
