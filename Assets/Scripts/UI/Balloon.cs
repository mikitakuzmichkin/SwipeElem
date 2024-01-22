using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI
{
    public class Balloon : MonoBehaviour
    {
        public float minHeight = 0f;
        public float maxHeight = 5f;
        public float minSpeed = 1f;
        public float maxSpeed = 3f;
        [SerializeField] private float _width = 2;

        public void Spawn()
        {
            gameObject.SetActive(true);
            float height = Random.Range(minHeight, maxHeight);
            float speed = Random.Range(minSpeed, maxSpeed);
            transform.localPosition = new Vector3(0f, height, 0f);
            var pos = transform.position;
            transform.DOLocalPath(new Vector3[]
            {
                new Vector3(-_width / 2f, height * Mathf.Sin(Random.Range(0f, 360f)), 0f),
                new Vector3( 0, height * Mathf.Sin(Random.Range(0f, 360f)), 0f),
                new Vector3( _width, height * Mathf.Sin(Random.Range(0f, 360f)), 0f)
            }, speed, PathType.Linear).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.localPosition = Vector3.zero;
                gameObject.SetActive(false);
            });
        }
    }
}