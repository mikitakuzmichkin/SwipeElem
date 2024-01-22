using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NextButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void Start()
        {
            _button.onClick.AddListener(NextLevel);
        }

        private void NextLevel()
        {
            ProjectContext.GetInstance<TableView>().SetNextLevel();
        }
    }
}