using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "CellSettings", menuName = "ScriptableObjects/Create CellSettings", order = 1)]
    public class CellSettings : ScriptableObject
    {
        public CellView Prefab;
        public List<Cells> ListCells;
        
        [Serializable]
        public class Cells
        {
            public Sprite Sprite;
            public Sprite[] IdleAnim;
            public Sprite[] BoomAnim;
        }
    }
}