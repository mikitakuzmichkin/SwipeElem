using System;
using DefaultNamespace;
using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CellView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float _up;
        [SerializeField] private float _down;
        [SerializeField] private float _left;
        [SerializeField] private float _right;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider2D;
        
        //temp
        [SerializeField] private EMove _move;
        //
        private Vector3 _startPoint;
        private float _minDistance;
        
        public float GetAspectRatio =>  _Width / _Height;
        public event Action<CellView, EMove> onMove;
        private float _Width => (_right + _left);
        private float _Height => (_up + _down);

        public int BoomIndex
        {
            get => _boomIndex;
            set
            { 
                _boomIndex = value;
                SetSprite(_boomAnim[value]);
            }
        }

        public int BoomIndexMax => _boomAnim.Length;

        private Sprite[] _boomAnim;
        private int _boomIndex;

        private void Update()
        {
            if (_move != EMove.None)
            {
                onMove?.Invoke(this, _move);
                _move = EMove.None;
            }
        }

        public void SetSize(float width, float height)
        {
            var parent = transform.parent;
            transform.parent = null;
            transform.localScale = new Vector3(width / (_Width * transform.lossyScale.x),  
                height / (_Height * transform.localScale.y), 
                transform.localScale.z);
            transform.parent = parent;
            _minDistance = Mathf.Min(width, height) / 2f;
        }

        public void SetPos(Vector3 pos)
        {
            pos.x -= (_up - _down) / 2f * transform.localScale.x;
            pos.y -= (_right - _left) / 2f * transform.localScale.y;
            transform.position = pos;
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
        
        public void SetAnim(Sprite[] boomAnim)
        {
            _boomAnim = boomAnim;
        }

        public void SetOrder(int order)
        {
            _spriteRenderer.sortingOrder = order;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            var corner = transform.position.GetCornersFromSides(_up * transform.lossyScale.y,
                _down * transform.lossyScale.y,
                _left * transform.lossyScale.x,
                _right * transform.lossyScale.x);
            GizmosWrapper.DrawGizmosRect(corner.leftUpCorner, corner.rightUpCorner, corner.leftDownCorner, corner.rightDownCorner);
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnPointerDown");
            _startPoint = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Vector3 endPos = eventData.position;
            if (Vector3.Distance(_startPoint, endPos) > _minDistance)
            {
                // Вычисляем вектор свайпа как разность между конечной и начальной точкой
                Vector3 swipeVector = endPos - _startPoint;
                // Нормализуем вектор свайпа, чтобы он имел длину 1
                swipeVector = swipeVector.normalized;
                // Вызываем метод, который определяет направление свайпа
                onMove?.Invoke(this, GetSwipeDirection(swipeVector));
            }
        }
        
        private EMove GetSwipeDirection(Vector3 swipeVector)
        {
            // Объявляем переменную для хранения минимального угла между вектором свайпа и одним из направлений
            float minAngle = Mathf.Infinity;
            // Объявляем переменную для хранения направления свайпа
            EMove swipeDirection = EMove.None;
            // Создаем массив из четырех направлений
            Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right};
            // Создаем массив из четырех строк, которые соответствуют направлениям
            EMove[] directionNames = {EMove.Up, EMove.Down, EMove.Left, EMove.Right};
            // Проходим по массиву направлений в цикле
            for (int i = 0; i < directions.Length; i++)
            {
                // Вычисляем угол между вектором свайпа и текущим направлением
                float angle = Vector3.Angle(swipeVector, directions[i]);
                // Если угол меньше минимального угла, то обновляем минимальный угол и направление свайпа
                if (angle < minAngle)
                {
                    minAngle = angle;
                    swipeDirection = directionNames[i];
                }
            }
            // Выводим направление свайпа в консоль
            Debug.Log("Направление свайпа: " + swipeDirection);
            return swipeDirection;
        }
    }
}