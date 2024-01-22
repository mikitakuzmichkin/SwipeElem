using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Models
{
    public class GridModel
    {
        private int[,] _map;
        private int _rows;
        private int _columns;

        public event Action<Vector2Int, Vector2Int> onCellChangePlace;
        public event Action<List<CellModelFall>> onCellFall;
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
            
            var win = false;
            var boom = false;
            do
            {
                bool fall = false;
                do
                {
                    fall = CheckFall();
                } while (fall);
                
                boom = CheckBoom();
                win = CheckWin();
            } while (boom && !win);
        }

        private bool CheckFall()
        {
            List<CellModelFall> fallIndexes = new List<CellModelFall>();
            for (int j = 0; j < _columns; j++)
            {
                bool isFall = false;
                int count = 0;
                for (int i = _rows - 1; i >= 0; i--)
                {
                    if (_map[i, j] == 0)
                    {
                        isFall = true;
                        count++;
                    }
                    else
                    {
                        if (isFall)
                        {
                            fallIndexes.Add(new CellModelFall()
                            {
                                Index = new Vector2Int(i, j),
                                FallStep = count
                            });
                        }
                    }
                }
            }

            foreach (var cellFall in fallIndexes)
            {
                _map[cellFall.Index.x + cellFall.FallStep, cellFall.Index.y] = _map[cellFall.Index.x, cellFall.Index.y];
                _map[cellFall.Index.x, cellFall.Index.y] = 0;
            }

            if (fallIndexes.Count > 0)
            {
                onCellFall?.Invoke(fallIndexes.OrderByDescending(c => c.FallStep).ToList());
            }
            
            return fallIndexes.Count > 0;
        }
        
        //Will cells consisting of two horizontal and three vertical in the shape of a letter "Г" be considered valid?
        //I thought not. Because the assignment specifies special cases with three vertically and three horizontally
        private bool CheckBoom()
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

            boomIndexes = boomIndexes.Distinct().ToList();
            
            foreach (var index in boomIndexes)
            {
                _map[index.x, index.y] = 0;
            }

            if (boomIndexes.Count > 0)
            {
                onCellBoom?.Invoke(boomIndexes);
            }

            return boomIndexes.Count > 0;
        }

        private bool CheckWin()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    if (_map[i, j] > 0)
                    {
                        return false;
                    }
                }
            }

            onWin?.Invoke();
            return true;
        }
    }
}