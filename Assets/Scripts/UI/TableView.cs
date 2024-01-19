using System;
using System.Collections;
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
        [SerializeField] private CellView _cell;
        [SerializeField] private Levels _level;

        private void Start()
        {
            GenerateCells(SetCell);
        }

        private void GenerateCells(Action<Vector2Int ,Vector3, Vector2> callback)
        {
            var aspectRatioCell = _cell.GetAspectRatio;
            var mainCorner = transform.position.GetCornersFromCenter(_height, _width);
            var min = Mathf.Min(_width / aspectRatioCell / _level.Columns, _height / _level.Rows);
            var cellSize = new Vector2(min * aspectRatioCell, min );
            var height = cellSize.y * _level.Rows;
            var width = cellSize.x * _level.Columns;
            var leftUpUsefulCorner = 
                new Vector3(mainCorner.leftDownCorner.x + (mainCorner.rightDownCorner.x - mainCorner.leftDownCorner.x) / 2f
                                                        - (cellSize.x * _level.Columns / 2f),
                mainCorner.leftDownCorner.y + cellSize.y * _level.Rows);
            var usefulCorners = leftUpUsefulCorner.GetCornersFromLeftUpCorner(height, width);

            for (int i = _level.Rows - 1; i >= 0; i--)
            {
                for (int j = 0; j < _level.Columns; j++)
                {
                    var leftUpCell = new Vector3(usefulCorners.leftUpCorner.x + cellSize.x * j,
                        usefulCorners.leftUpCorner.y - cellSize.y * i);
                    var index = new Vector2Int(i, j);
                    callback?.Invoke(index, leftUpCell, cellSize);
                }
            }
        }

        

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            var corner = transform.position.GetCornersFromCenter(_height, _width);
            GizmosWrapper.DrawGizmosRect(corner.leftUpCorner, corner.rightUpCorner, corner.leftDownCorner, corner.rightDownCorner);
            //GenerateCells(DrawCells);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            var corner = transform.position.GetCornersFromCenter(_height, _width);
            GizmosWrapper.DrawGizmosRect(corner.leftUpCorner, corner.rightUpCorner, corner.leftDownCorner, corner.rightDownCorner);
            //if(_level != null)
             //   GenerateCells(DrawCells);
        }

        private void DrawCells(Vector2Int index, Vector3 leftUpCell, Vector2 cellSize)
        {
            var cellCorners = leftUpCell.GetCornersFromLeftUpCorner(cellSize.y, cellSize.x);
            Gizmos.color = Color.white;
            GizmosWrapper.DrawGizmosRect(cellCorners.leftUpCorner, cellCorners.rightUpCorner, cellCorners.leftDownCorner, cellCorners.rightDownCorner);    
        }

        private void SetCell(Vector2Int index, Vector3 leftUpCell, Vector2 cellSize)
        {
            if(_level[index.x,index.y] == 0) return;
            
            var cell = Instantiate(_cell);
            
            cell.SetSize(cellSize.x, cellSize.y);
            cell.SetPos(leftUpCell.GetCenter(cellSize));
        }
    }
}