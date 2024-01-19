using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "Levels", menuName = "ScriptableObjects/Create Level", order = 1)]
    public class Levels : ScriptableObject
    {
        [HideInInspector] public Color[] _Colors = new[] {Color.white, Color.blue, Color.yellow};
        [HideInInspector] public int Rows;
        [HideInInspector] public int Columns;
        [HideInInspector] public int[,] _map;
    }
}