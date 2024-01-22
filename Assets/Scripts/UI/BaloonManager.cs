using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class BaloonManager : MonoBehaviour
    {
        public List<Balloon> balloons = new List<Balloon>();

        void Start()
        {
            for (int i = 0; i < balloons.Count; i++)
            {
                balloons[i].gameObject.SetActive(false);
            }
        }

        void Update()
        {
            var count = Random.Range(1, balloons.Count);
            for (int i = 0; i < balloons.Count; i++)
            {
                if(balloons[i].gameObject.activeSelf)
                {
                    continue;
                }
                
                balloons[i].Spawn();
                count--;
                if (count < 0)
                {
                    break;
                }
            }
        }
    }
}