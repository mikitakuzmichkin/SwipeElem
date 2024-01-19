using System;
using SO;
using UnityEngine;

namespace DefaultNamespace
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private CellSettings _cellSettings;
        [SerializeField] private Levels _levels;

        private void Start()
        {
            ProjectContext.Bind(_cellSettings);
            ProjectContext.Bind<ILevels>(_levels);
        }
    }
}