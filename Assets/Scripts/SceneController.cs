using System;
using SO;
using UI;
using UnityEngine;

namespace DefaultNamespace
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private CellSettings _cellSettings;
        [SerializeField] private Levels _levels;
        [SerializeField] private TableView _tableView;

        private void Start()
        {
            ProjectContext.Bind(_cellSettings);
            ProjectContext.Bind<ILevels>(_levels);
            ProjectContext.Bind(_tableView);
        }
    }
}