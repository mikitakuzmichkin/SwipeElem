using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "LevelList", menuName = "ScriptableObjects/Create Level list", order = 0)]
    public class Levels : ScriptableObject, ILevels
    {
        [SerializeField] private List<Level> _list;

        public int[,] this[int index] => _list[index].GetMap();

        public int Count => _list.Count;
    }
}