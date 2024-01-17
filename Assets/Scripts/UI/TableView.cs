using System;
using Extensions;
using UnityEditor;
using UnityEngine;

namespace UI
{
    public class TableView : MonoBehaviour
    {
        [SerializeField] private int _rows;
        [SerializeField] private int _columns;
        [SerializeField] private float _width;
        [SerializeField] private float _height;
        [SerializeField] private CellView _cell;

        private void Start()
        {
            GenerateCells((x,z) => _cell.SetSize(z.x, z.y));
        }

        private void GenerateCells(Action<Vector3, Vector2> callback)
        {
            var aspectRatioCell = _cell.GetAspectRatio;
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

            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    var leftUpCell = new Vector3(usefulCorners.leftUpCorner.x + cellSize.x * j,
                        usefulCorners.leftUpCorner.y - cellSize.y * i);
                    callback?.Invoke(leftUpCell, cellSize);
                }
            }
        }

        

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            var corner = transform.position.GetCornersFromCenter(_height, _width);
            GizmosWrapper.DrawGizmosRect(corner.leftUpCorner, corner.rightUpCorner, corner.leftDownCorner, corner.rightDownCorner);
            GenerateCells(DrawCells);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            var corner = transform.position.GetCornersFromCenter(_height, _width);
            GizmosWrapper.DrawGizmosRect(corner.leftUpCorner, corner.rightUpCorner, corner.leftDownCorner, corner.rightDownCorner);
            GenerateCells(DrawCells);
        }

        private void DrawCells(Vector3 leftUpCell, Vector2 cellSize)
        {
            var cellCorners = leftUpCell.GetCornersFromLeftUpCorner(cellSize.y, cellSize.x);
            Gizmos.color = Color.white;
            GizmosWrapper.DrawGizmosRect(cellCorners.leftUpCorner, cellCorners.rightUpCorner, cellCorners.leftDownCorner, cellCorners.rightDownCorner);    
        }
        
    }
}