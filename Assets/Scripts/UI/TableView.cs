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
        private bool _isAnim;
        
        private Queue<List<Sequence>> _animQueue = new Queue<List<Sequence>>();

        public event Action<Vector2Int, Vector2Int> onCellMoved;

        private void Start()
        {
            var map = ProjectContext.GetInstance<ILevels>()[0];
            _rows = map.GetLength(0);
            _columns = map.GetLength(1);
            _cellSettings = ProjectContext.GetInstance<CellSettings>();
            GenerateCells(map, _rows, _columns);

            DOTween.defaultAutoPlay = AutoPlay.None;
            
            _controller = new GridController(this, new GridModel(map, _rows, _columns));
        }

        public void CellPlaceChangedAnim(Vector2Int oldPlace, Vector2Int newPlace)
        {
            List<Sequence> animList = new List<Sequence>();

            void UpdateCell(CellView cell, Vector2Int place)
            {
                if (cell != null)
                {
                    var leftUpCell = new Vector3(_leftUpUsefulCorner.x + _cellSize.x * place.y,
                        _leftUpUsefulCorner.y - _cellSize.y * place.x);
                    var newPos = leftUpCell.GetCenter(_cellSize);
                    
                    var seq = DOTween.Sequence();
                    if (cell.transform.position.x < newPos.x)
                    {
                        seq.AppendCallback(() => SetOrder(cell, place));
                    }
                    seq.Append(DOTween.To(() => cell.transform.position, cell.SetPos, newPos, _DURATION));
                    if (cell.transform.position.x >= newPos.x)
                    {
                        seq.AppendCallback(() => SetOrder(cell, place));
                    }
                    animList.Add(seq);
                }
            }
            
            var firstCell = _cellMap[oldPlace.x, oldPlace.y];
            UpdateCell(firstCell, newPlace);
            
            var secondCell = _cellMap[newPlace.x, newPlace.y];
            UpdateCell(secondCell, oldPlace);

            if (animList.Count > 0)
            {
                AddAnim(animList);
            }

            _cellMap[oldPlace.x, oldPlace.y] = secondCell;
            _cellMap[newPlace.x, newPlace.y] = firstCell;
        }

        public void CellFallAnim(List<Vector2Int> Indexes)
        {
            List<Sequence> animList = new List<Sequence>();
            foreach (var index in Indexes)
            {
                var cell = _cellMap[index.x, index.y];
                var leftUpCell = new Vector3(_leftUpUsefulCorner.x + _cellSize.x * index.y,
                    _leftUpUsefulCorner.y - _cellSize.y * (index.x + 1));
                var newPos = leftUpCell.GetCenter(_cellSize);
                animList.Add(DOTween.Sequence()
                    .AppendCallback(() => SetOrder(cell, new Vector2Int(index.x + 1, index.y)))
                    .Append(DOTween.To(() => cell.transform.position, (x) => cell.SetPos(x), newPos, _DURATION)));

                _cellMap[index.x + 1, index.y] = _cellMap[index.x, index.y];
                _cellMap[index.x, index.y] = null;
            }
            
            if (animList.Count > 0)
            {
                AddAnim(animList);
            }
        }

        private void AddAnim(List<Sequence> listSeq)
        {
            foreach (var seq in listSeq)
            {
                seq.Pause();
            }
            _animQueue.Enqueue(listSeq);
            Debug.Log("_animQueue.Enqueue");
            if (_isAnim == false)
            {
                PlayAnim();
            }
        }

        private void PlayAnim()
        {
            if (_animQueue.Count > 0)
            {
                _isAnim = true;
                Debug.Log("_animQueue.Dequeue");
                List<Sequence> list = _animQueue.Dequeue();
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == 0)
                    {
                        list[i].AppendInterval(1f).AppendCallback(PlayAnim);
                    }

                    list[i].Play();
                }
            }
            else
            {
                _isAnim = false;
            }
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
            //cell.SetOrder((rows - index.x) * columns + index.y + 1);
            SetOrder(cell, index);
            cell.SetSize(cellSize.x, cellSize.y);
            cell.SetPos(leftUpCell.GetCenter(cellSize));
            cell.onMove += CellMoved;
        }

        private void SetOrder(CellView cell, Vector2Int index)
        {
            cell.SetOrder(index.y * _rows + (_rows - index.x));
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