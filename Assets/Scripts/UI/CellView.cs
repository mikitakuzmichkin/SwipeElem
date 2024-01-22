using System;
using System.Collections;
using DefaultNamespace;
using DG.Tweening;
using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace UI
{
    public class CellView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float MIN_DELAY = 1f;
        private const float MAX_DELAY = 3f;
        private const float IDLE_DURATION = 1.5f;

        [SerializeField] private float _up;
        [SerializeField] private float _down;
        [SerializeField] private float _left;
        [SerializeField] private float _right;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Sprite[] _boomAnim;
        private int _boomIndex;
        private Sequence _sequence;
        private Sprite[] _idleAnim;
        private int _idleAnimIndex;
        private float _minDistance;

        private Vector3 _startPoint;

        public float GetAspectRatio => _Width / _Height;
        private float _Width => _right + _left;
        private float _Height => _up + _down;

        public int BoomIndex
        {
            get => _boomIndex;
            set
            {
                if (_sequence != null)
                {
                    _sequence.Kill();
                    _sequence = null;
                }

                _boomIndex = value;
                SetSprite(_boomAnim[value]);
            }
        }

        private int IdleIndex
        {
            get => _idleAnimIndex;
            set
            {
                _idleAnimIndex = value;
                SetSprite(_idleAnim[value]);
            }
        }

        public int BoomIndexMax => _boomAnim.Length - 1;
        public event Action<CellView, EMove> onMove;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            var corner = transform.position.GetCornersFromSides(_up * transform.lossyScale.y,
                _down * transform.lossyScale.y,
                _left * transform.lossyScale.x,
                _right * transform.lossyScale.x);
            GizmosWrapper.DrawGizmosRect(corner.leftUpCorner, corner.rightUpCorner, corner.leftDownCorner,
                corner.rightDownCorner);
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
                var swipeVector = endPos - _startPoint;
                // Нормализуем вектор свайпа, чтобы он имел длину 1
                swipeVector = swipeVector.normalized;
                // Вызываем метод, который определяет направление свайпа
                onMove?.Invoke(this, GetSwipeDirection(swipeVector));
            }
        }

        public void Init()
        {
            Loop();
        }

        private void Loop()
        {
            var delay = Random.Range(MIN_DELAY, MAX_DELAY);
            _sequence?.Kill();
            _sequence = DOTween.Sequence().Append(DOTween.To(() => IdleIndex, x => IdleIndex = x, _idleAnim.Length - 1, IDLE_DURATION))
                .SetEase(Ease.Linear)
                .AppendInterval(delay)
                .AppendCallback(() =>
                {
                    IdleIndex = 0;
                    Loop();
                })
                .SetLoops(-1, LoopType.Restart);
            Debug.Log("Anim Loop");
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

        public void SetAnim(Sprite[] idleAnim, Sprite[] boomAnim)
        {
            _idleAnim = idleAnim;
            _boomAnim = boomAnim;
            _idleAnimIndex = 0;
            _boomIndex = 0;
        }

        public void SetOrder(int order)
        {
            _spriteRenderer.sortingOrder = order;
        }

        public void Destroy()
        {
            _sequence?.Kill();
            Destroy(gameObject);
        }

        private EMove GetSwipeDirection(Vector3 swipeVector)
        {
            // Объявляем переменную для хранения минимального угла между вектором свайпа и одним из направлений
            var minAngle = Mathf.Infinity;
            // Объявляем переменную для хранения направления свайпа
            var swipeDirection = EMove.None;
            // Создаем массив из четырех направлений
            Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right};
            // Создаем массив из четырех строк, которые соответствуют направлениям
            EMove[] directionNames = {EMove.Up, EMove.Down, EMove.Left, EMove.Right};
            // Проходим по массиву направлений в цикле
            for (var i = 0; i < directions.Length; i++)
            {
                // Вычисляем угол между вектором свайпа и текущим направлением
                var angle = Vector3.Angle(swipeVector, directions[i]);
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