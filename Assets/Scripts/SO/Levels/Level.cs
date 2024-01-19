using System;
using DefaultNamespace;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Create Level", order = 1)]
    [Serializable]
    public class Level : ScriptableObject
    {
        [HideInInspector] public Color[] _Colors = new[] {Color.white, Color.blue, Color.yellow};
        [HideInInspector] public int Rows;
        [HideInInspector] public int Columns;
        private int[] _map;
        private int _rows;
        private int _columns;

        public void CreateMap()
        {
            _map = new int[Rows*Columns];
            _rows = Rows;
            _columns = Columns;
        }

        public void ResizeMap()
        {
            var newArray = new int[Rows, Columns];
            int minRows = Math.Min(Rows, _rows);
            int minCols = Math.Min(Columns, _columns);
            for(int i = 0; i < minRows; i++)
            for(int j = 0; j < minCols; j++)
                newArray[i, j] = this[i, j];
            
            CreateMap();
            for(int i = 0; i < Rows; i++)
            for(int j = 0; j < Columns; j++)
                this[i, j] = newArray[i, j];
        }

        public int[,] GetMap()
        {
            var newArray = new int[Rows, Columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    newArray[i, j] = this[i, j];
                }
            }

            return newArray;
        }

        public bool IsMapNull()
        {
            return _map == null;
        }
        
        public int this[int i, int j]
        {
            get => _map[i * Columns + j];
            set => _map[i * Columns + j] = value;
        }
    }
}