using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class GridModel
    {
        private int[,] _map;
        private int _rows;
        private int _columns;

        public event Action<Vector2Int, Vector2Int> onCellChangePlace;
        public event Action<List<Vector2Int>> onCellFall;
        public event Action<List<Vector2Int>> onCellBoom;
        public event Action onWin;

        public GridModel(int[,] map, int rows, int columns)
        {
            _map = map;
            _rows = rows;
            _columns = columns;
        }

        public void ChangePlace(Vector2Int oldPlace, Vector2Int newPlace)
        {
            var temp = _map[oldPlace.x, oldPlace.y];
            _map[oldPlace.x, oldPlace.y] = _map[newPlace.x, newPlace.y];
            _map[newPlace.x, newPlace.y] = temp;
            
            onCellChangePlace?.Invoke(oldPlace, newPlace);

            bool fall = false;
            do
            {
                fall = CheckFall();
                CheckBoom();
            } while (fall);
        }

        private bool CheckFall()
        {
            List<Vector2Int> fallIndexes = new List<Vector2Int>();
            for (int j = 0; j < _columns; j++)
            {
                bool isFall = false;
                for (int i = _rows - 1; i >= 0; i--)
                {
                    if (_map[i, j] == 0)
                    {
                        isFall = true;
                    }
                    else
                    {
                        if (isFall)
                        {
                            fallIndexes.Add(new Vector2Int(i,j));
                        }
                    }
                }
            }

            foreach (var index in fallIndexes)
            {
                _map[index.x + 1, index.y] = _map[index.x, index.y];
                _map[index.x, index.y] = 0;
            }

            if (fallIndexes.Count > 0)
            {
                onCellFall?.Invoke(fallIndexes);
            }
            
            return fallIndexes.Count > 0;
        }
        
        //Will cells consisting of two horizontal and three vertical in the shape of a letter "Г" be considered valid?
        //I thought not. Because the assignment specifies special cases with three vertically and three horizontally
        private void CheckBoom()
        {
            List<Vector2Int> boomIndexes = new List<Vector2Int>();
            for (int i = 0; i < _rows; i++)
            {
                List<Vector2Int> colIndexes = new List<Vector2Int>();
                for (int j = 1; j < _columns; j++)
                {
                    if (_map[i,j] == _map[i,j - 1] && _map[i,j] != 0)
                    {
                        if (colIndexes.Count == 0)
                        {
                            colIndexes.Add(new Vector2Int(i,j - 1));
                        }
                        colIndexes.Add(new Vector2Int(i,j));
                    }
                    else
                    {
                        if (colIndexes.Count >= 3)
                        {
                            boomIndexes.AddRange(colIndexes);
                        }
                        colIndexes.Clear();
                    }
                }
                if (colIndexes.Count >= 3)
                {
                    boomIndexes.AddRange(colIndexes);
                }
            }
            
            for (int j = 0; j < _columns; j++)
            {
                List<Vector2Int> rowIndexes = new List<Vector2Int>();
                for (int i = 1; i < _rows; i++)
                {
                    if (_map[i,j] == _map[i - 1,j] && _map[i,j] != 0)
                    {
                        if (rowIndexes.Count == 0)
                        {
                            rowIndexes.Add(new Vector2Int(i - 1,j));
                        }
                        rowIndexes.Add(new Vector2Int(i,j));
                    }
                    else
                    {
                        if (rowIndexes.Count >= 3)
                        {
                            boomIndexes.AddRange(rowIndexes);
                        }
                        rowIndexes.Clear();
                    }
                }
                if (rowIndexes.Count >= 3)
                {
                    boomIndexes.AddRange(rowIndexes);
                }
            }
            
            
            foreach (var index in boomIndexes)
            {
                _map[index.x, index.y] = 0;
            }

            if (boomIndexes.Count > 0)
            {
                onCellBoom?.Invoke(boomIndexes);
            }
        }

        private void CheckWin()
        {
            bool isWin = true;
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    if (_map[i, j] > 0)
                    {
                        isWin = false;
                        return;
                    }
                }
            }
        }
    }
}