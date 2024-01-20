using UI;

namespace DefaultNamespace
{
    public class GridController
    {
        private TableView _view;
        private GridModel _model;
        public GridController(TableView view, GridModel model)
        {
            _view = view;
            _model = model;
            _view.onCellMoved += model.ChangePlace;
            _model.onCellChangePlace += _view.CellPlaceChangedAnim;
        }
    }
}