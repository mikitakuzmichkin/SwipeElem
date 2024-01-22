using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using Extensions;
using Models;
using SO;
using UnityEditor;
using UnityEngine;

namespace UI
{
    public class TableView : MonoBehaviour
    {
        private const float _DURATION_CHANGE_PLACE = 0.5f;
        private const float _DURATION_CHANGE_FALL = 0.5f;
        private const float _DURATION_BOOM = 1.5f;
        
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
        private int _levelIndex;
        private Action _callBackAfterAnim;
        
        private Queue<List<Sequence>> _animQueue = new Queue<List<Sequence>>();

        public event Action<Vector2Int, Vector2Int> onCellMoved;

        private void Start()
        {
            _levelIndex = 0;
            DOTween.defaultAutoPlay = AutoPlay.None;

            _controller = new GridController(this);
            _cellSettings = ProjectContext.GetInstance<CellSettings>();
            
            InitNewLevel();
        }

        private void InitNewLevel()
        {
            var map = ProjectContext.GetInstance<ILevels>()[_levelIndex];
            _rows = map.GetLength(0);
            _columns = map.GetLength(1);
            GenerateCells(map, _rows, _columns);
            _controller.Init(new GridModel(map, _rows, _columns));
            _callBackAfterAnim = null;
            Debug.Log("InitNewLevel");
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
                    seq.Append(DOTween.To(() => cell.transform.position, cell.SetPos, newPos, _DURATION_CHANGE_PLACE));
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

        public void CellFallAnim(List<CellModelFall> cellModelFalls)
        {
            List<Sequence> animList = new List<Sequence>();
            foreach (var modelFall in cellModelFalls)
            {
                var cell = _cellMap[modelFall.Index.x, modelFall.Index.y];
                var leftUpCell = new Vector3(_leftUpUsefulCorner.x + _cellSize.x * modelFall.Index.y,
                    _leftUpUsefulCorner.y - _cellSize.y * (modelFall.Index.x + modelFall.FallStep));
                var newPos = leftUpCell.GetCenter(_cellSize);
                animList.Add(DOTween.Sequence()
                    .AppendCallback(() => SetOrder(cell, new Vector2Int(modelFall.Index.x + modelFall.FallStep, modelFall.Index.y)))
                    .Append(DOTween.To(() => cell.transform.position, (x) => cell.SetPos(x), newPos, _DURATION_CHANGE_FALL * modelFall.FallStep))
                    .SetSpeedBased());

                _cellMap[modelFall.Index.x + modelFall.FallStep, modelFall.Index.y] = _cellMap[modelFall.Index.x, modelFall.Index.y];
                _cellMap[modelFall.Index.x, modelFall.Index.y] = null;
            }
            
            if (animList.Count > 0)
            {
                AddAnim(animList);
            }
        }

        public void CellBoomAnim(List<Vector2Int> Indexes)
        {
            List<Sequence> animList = new List<Sequence>();
            foreach (var index in Indexes)
            {
                var cell = _cellMap[index.x, index.y];
                animList.Add(DOTween.Sequence()
                    .Append(DOTween.To(() => cell.BoomIndex, (x) => cell.BoomIndex = x, cell.BoomIndexMax ,
                        _DURATION_BOOM)).SetEase(Ease.Linear)
                    .AppendCallback(() => cell.Destroy()));
                
                _cellMap[index.x, index.y] = null;
            }
            
            if (animList.Count > 0)
            {
                AddAnim(animList);
            }
        }

        public void SetNextLevel()
        {
            NextLevel();
            if (_cellMap != null)
            {
                for (int i = 0; i < _rows; i++)
                {
                    for (int j = 0; j < _columns; j++)
                    {
                        if (_cellMap[i, j] != null)
                        { 
                            _cellMap[i,j].Destroy();
                        }
                    }
                }
            }
            InitNewLevel();
        }

        public void SetNextLevelAnim()
        {
            NextLevel();
            _callBackAfterAnim = InitNewLevel;
        }

        private void NextLevel()
        {
            Debug.Log("NextLevel");
            var levels = ProjectContext.GetInstance<ILevels>();
            _levelIndex++;
            if (_levelIndex >= levels.Count)
            {
                _levelIndex = 0;
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
                        list[i].AppendCallback(PlayAnim);
                    }

                    list[i].Play();
                }
            }
            else
            {
                _isAnim = false;
                _callBackAfterAnim?.Invoke();
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
            var cellVariant = _cellSettings.ListCells[map[index.x, index.y] - 1];
            cell.SetSprite(cellVariant.Sprite);
            cell.SetAnim(cellVariant.IdleAnim, cellVariant.BoomAnim);
            SetOrder(cell, index);
            cell.SetSize(cellSize.x, cellSize.y);
            cell.SetPos(leftUpCell.GetCenter(cellSize));
            cell.Init();
            cell.onMove += CellMoved;
        }

        private void SetOrder(CellView cell, Vector2Int index)
        {
            cell.SetOrder(index.y * _rows + (_rows - index.x));
        }

        private void CellMoved(CellView cell, EMove direction)
        {
            if(_isAnim) return;
            
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