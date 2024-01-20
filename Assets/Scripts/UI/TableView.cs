using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using Extensions;
using SO;
using UnityEditor;
using UnityEngine;

namespace UI
{
    public class TableView : MonoBehaviour
    {
        private const float _DURATION = 0.5f;
        
        [SerializeField] private float _width;
        [SerializeField] private float _height;
        
        private int _rows;
        private int _columns;
        private CellView[,] _cellMap;
        private CellSettings _cellSettings;
        private Vector2 _cellSize;
        private Vector3 _leftUpUsefulCorner;
        private GridController _controller;
        
        private Queue<List<Sequence>> _animQueue = new Queue<List<Sequence>>();

        public event Action<Vector2Int, Vector2Int> onCellMoved;

        private void Start()
        {
            var map = ProjectContext.GetInstance<ILevels>()[0];
            _rows = map.GetLength(0);
            _columns = map.GetLength(1);
            _cellSettings = ProjectContext.GetInstance<CellSettings>();
            GenerateCells(map, _rows, _columns);
            
            _controller = new GridController(this, new GridModel(map, _rows, _columns));
        }

        public void CellPlaceChangedAnim(Vector2Int oldPlace, Vector2Int newPlace)
        {
            List<Sequence> animList = new List<Sequence>();
            var firstCell = _cellMap[oldPlace.x, oldPlace.y];
            if (firstCell != null)
            {
                firstCell.SetOrder((_rows - newPlace.x) * _columns + newPlace.y + 1);
                var leftUpCell = new Vector3(_leftUpUsefulCorner.x + _cellSize.x * newPlace.y,
                    _leftUpUsefulCorner.y - _cellSize.y * newPlace.x);
                var newPos = leftUpCell.GetCenter(_cellSize);
                DOTween.To(() => firstCell.transform.position, (x) => firstCell.SetPos(x), newPos, _DURATION);
            }
            
            var secondCell = _cellMap[newPlace.x, newPlace.y];
            if (secondCell != null)
            {
                secondCell.SetOrder((_rows - oldPlace.x) * _columns + oldPlace.y + 1);
                var leftUpCell = new Vector3(_leftUpUsefulCorner.x + _cellSize.x * oldPlace.y,
                    _leftUpUsefulCorner.y - _cellSize.y * oldPlace.x);
                var newPos = leftUpCell.GetCenter(_cellSize);
                DOTween.To(() => secondCell.transform.position, (x) => secondCell.SetPos(x), newPos, _DURATION);
            }

            _cellMap[oldPlace.x, oldPlace.y] = secondCell;
            _cellMap[newPlace.x, newPlace.y] = firstCell;
        }

        private void GenerateCells(int[,] map, int rows, int columns)
        {
            var aspectRatioCell = _cellSettings.Prefab.GetAspectRatio;
            _cellMap = new CellView[rows,columns];
            var mainCorner = transform.position.GetCornersFromCenter(_height, _width);
            var min = Mathf.Min(_width / aspectRatioCell / columns, _height / rows);
            _cellSize = new Vector2(min * aspectRatioCell, min );
            var height = _cellSize.y * rows;
            var width = _cellSize.x * columns;
            _leftUpUsefulCorner = 
                new Vector3(mainCorner.leftDownCorner.x + (mainCorner.rightDownCorner.x - mainCorner.leftDownCorner.x) / 2f
                                                        - (_cellSize.x * columns / 2f),
                mainCorner.leftDownCorner.y + _cellSize.y * rows);
            var usefulCorners = _leftUpUsefulCorner.GetCornersFromLeftUpCorner(height, width);

            for (int i = rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < columns; j++)
                {
                    var leftUpCell = new Vector3(usefulCorners.leftUpCorner.x + _cellSize.x * j,
                        usefulCorners.leftUpCorner.y - _cellSize.y * i);
                    var index = new Vector2Int(i, j);
                    SetCell(map, rows, columns, index, leftUpCell, _cellSize);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            var corner = transform.position.GetCornersFromCenter(_height, _width);
            GizmosWrapper.DrawGizmosRect(corner.leftUpCorner, corner.rightUpCorner, corner.leftDownCorner, corner.rightDownCorner);
        }

        private void DrawCells(Vector2Int index, Vector3 leftUpCell, Vector2 cellSize)
        {
            var cellCorners = leftUpCell.GetCornersFromLeftUpCorner(cellSize.y, cellSize.x);
            Gizmos.color = Color.white;
            GizmosWrapper.DrawGizmosRect(cellCorners.leftUpCorner, cellCorners.rightUpCorner, cellCorners.leftDownCorner, cellCorners.rightDownCorner);    
        }

        private void SetCell(int[,] map, int rows, int columns, Vector2Int index, Vector3 leftUpCell, Vector2 cellSize)
        {
            if(map[index.x,index.y] == 0) return;
            
            var cell = Instantiate(_cellSettings.Prefab);
            _cellMap[index.x, index.y] = cell;
            cell.SetSprite(_cellSettings.ListCells[map[index.x,index.y] - 1].Sprite);
            cell.SetOrder((rows - index.x) * columns + index.y + 1);
            cell.SetSize(cellSize.x, cellSize.y);
            cell.SetPos(leftUpCell.GetCenter(cellSize));
            cell.onMove += CellMoved;
        }

        private void CellMoved(CellView cell, EMove direction)
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    if (_cellMap[i, j] == cell)
                    {
                        switch (direction)
                        {
                            case EMove.None:
                                return;
                            case EMove.Up:
                                if (i > 0 && _cellMap[i - 1, j] != null)
                                {
                                    onCellMoved?.Invoke(new Vector2Int(i, j), new Vector2Int(i - 1, j));
                                }
                                return;
                            case EMove.Down:
                                if (i < _rows - 1)
                                {
                                    onCellMoved?.Invoke(new Vector2Int(i, j), new Vector2Int(i + 1, j));
                                }
                                return;
                            case EMove.Right:
                                if (j < _columns - 1)
                                {
                                    onCellMoved?.Invoke(new Vector2Int(i, j), new Vector2Int(i, j + 1));
                                }
                                return;
                            case EMove.Left:
                                if (j > 0)
                                {
                                    onCellMoved?.Invoke(new Vector2Int(i, j), new Vector2Int(i, j - 1));
                                }
                                return;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                        }
                    }
                }
            }
        }
    }
}