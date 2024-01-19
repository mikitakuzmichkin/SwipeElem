using System;
using Extensions;
using UnityEngine;

namespace UI
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private float _up;
        [SerializeField] private float _down;
        [SerializeField] private float _left;
        [SerializeField] private float _right;

        public float GetAspectRatio =>  _Width / _Height;
        private float _Width => (_right + _left);
        private float _Height => (_up + _down);

        private void Start()
        {
            Debug.Log("local " + transform.localScale);
            Debug.Log("lossyScale " + transform.lossyScale);
        }

        public void SetSize(float width, float height)
        {
            var parent = transform.parent;
            transform.parent = null;
            transform.localScale = new Vector3(width / (_Width * transform.lossyScale.x),  
                height / (_Height * transform.localScale.y), 
                transform.localScale.z);
            transform.parent = parent;
        }

        public void SetPos(Vector3 pos)
        {
            pos.x -= (_up - _down) / 2f * transform.localScale.x;
            pos.y -= (_right - _left) / 2f * transform.localScale.y;
            transform.position = pos;
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
        
    }
}