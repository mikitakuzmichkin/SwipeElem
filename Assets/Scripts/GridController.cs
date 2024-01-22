using Models;
using UI;

namespace DefaultNamespace
{
    public class GridController
    {
        private TableView _view;
        private GridModel _model;
        public GridController(TableView view)
        {
            _view = view;
        }

        public void Init(GridModel model)
        {
            if (_model != null)
            {
                _view.onCellMoved -= _model.ChangePlace;
                _model.onCellChangePlace -= _view.CellPlaceChangedAnim;
                _model.onCellFall -= _view.CellFallAnim;
                _model.onCellBoom -= _view.CellBoomAnim;
                _model.onWin -= _view.SetNextLevelAnim;
            }
            _model = model;
            _view.onCellMoved += _model.ChangePlace;
            _model.onCellChangePlace += _view.CellPlaceChangedAnim;
            _model.onCellFall += _view.CellFallAnim;
            _model.onCellBoom += _view.CellBoomAnim;
            _model.onWin += _view.SetNextLevelAnim;
        }
    }
}