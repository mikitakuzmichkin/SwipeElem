using System;
using System.Collections;
using DefaultNamespace;
using Extensions;
using SO;
using UnityEditor;
using UnityEngine;

namespace UI
{
    public class TableView : MonoBehaviour
    {
        [SerializeField] private float _width;
        [SerializeField] private float _height;

        private int[,] _map;
        private int _rows;
        private int _columns;
        private CellSettings _cellSettings;

        private void Start()
        {
            _map = ProjectContext.GetInstance<ILevels>()[0];
            _rows = _map.GetLength(0);
            _columns = _map.GetLength(1);
            _cellSettings = ProjectContext.GetInstance<CellSettings>();
            GenerateCells(SetCell);
        }

        private void GenerateCells(Action<Vector2Int ,Vector3, Vector2> callback)
        {
            var aspectRatioCell = _cellSettings.Prefab.GetAspectRatio;
            var mainCorner = transform.position.GetCornersFromCenter(_height, _width);
            var min = Mathf.Min(_width / aspectRatioCell / _columns, _height / _rows);
            var cellSize = new Vector2(min * aspectRatioCell, min );
            var height = cellSize.y * _rows;
            var width = cellSize.x * _columns;
            var leftUpUsefulCorner = 
                new Vector3(mainCorner.leftDownCorner.x + (mainCorner.rightDownCorner.x - mainCorner.leftDownCorner.x) / 2f
                                                        - (cellSize.x * _columns / 2f),
                mainCorner.leftDownCorner.y + cellSize.y * _rows);
            var usefulCorners = leftUpUsefulCorner.GetCornersFromLeftUpCorner(height, width);

            for (int i = _rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < _columns; j++)
                {
                    var leftUpCell = new Vector3(usefulCorners.leftUpCorner.x + cellSize.x * j,
                        usefulCorners.leftUpCorner.y - cellSize.y * i);
                    var index = new Vector2Int(i, j);
                    callback?.Invoke(index, leftUpCell, cellSize);
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

        private void SetCell(Vector2Int index, Vector3 leftUpCell, Vector2 cellSize)
        {
            if(_map[index.x,index.y] == 0) return;
            
            var cell = Instantiate(_cellSettings.Prefab);
            cell.SetSprite(_cellSettings.ListCells[_map[index.x,index.y] - 1].Sprite);
            cell.SetOrder((_rows - index.x) * _columns + index.y + 1);
            cell.SetSize(cellSize.x, cellSize.y);
            cell.SetPos(leftUpCell.GetCenter(cellSize));
        }
    }
}