using EntityTools.Settings;
using System.Drawing;

namespace EntityTools.Quester.Mapper
{
    /// <summary>
    /// Набор кистей и карандашей для рисования
    /// </summary>
    public class MapperDrawingTools
    {
        public MapperDrawingTools()
        {
            var mapperFormSettings = EntityTools.Config.Mapper.MapperForm;

            _unidirectionalPathBrush = new SolidBrush(mapperFormSettings.UnidirectionalPathColor);
            _unidirectionalPathPen = new Pen(mapperFormSettings.UnidirectionalPathColor);
            _bidirectionalPathBrush = new SolidBrush(mapperFormSettings.BidirectionalPathColor);
            _bidirectionalPathPen = new Pen(mapperFormSettings.BidirectionalPathColor);

            _enemyBrush = new SolidBrush(mapperFormSettings.EnemyColor);
            _friendBrush = new SolidBrush(mapperFormSettings.FriendColor);
            _playerBrush = new SolidBrush(mapperFormSettings.PlayerColor);
            _otherNPCBrush = new SolidBrush(mapperFormSettings.OtherNPCColor);

            _nodeBrush = new SolidBrush(mapperFormSettings.NodeColor);                
            _skillnodeBrush = new SolidBrush(mapperFormSettings.SkillNodeColor);

            EntityTools.Config.Mapper.MapperForm.PropertyChanged += MapperForm_PropertyChanged;
        }

        private void MapperForm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is MapperFormSettings mapperFormSettings)
            {
                if(e.PropertyName == nameof(MapperFormSettings.UnidirectionalPathColor))
                {
                    _unidirectionalPathBrush = new SolidBrush(mapperFormSettings.UnidirectionalPathColor);
                    _unidirectionalPathPen = new Pen(mapperFormSettings.UnidirectionalPathColor);
                }
                else if (e.PropertyName == nameof(MapperFormSettings.BidirectionalPathColor))
                {
                    _bidirectionalPathBrush = new SolidBrush(mapperFormSettings.BidirectionalPathColor);
                    _bidirectionalPathPen = new Pen(mapperFormSettings.BidirectionalPathColor);
                }
                else if (e.PropertyName == nameof(MapperFormSettings.EnemyColor))
                    _enemyBrush = new SolidBrush(mapperFormSettings.EnemyColor);
                else if (e.PropertyName == nameof(MapperFormSettings.FriendColor))
                    _friendBrush = new SolidBrush(mapperFormSettings.FriendColor);
                else if (e.PropertyName == nameof(MapperFormSettings.PlayerColor))
                    _playerBrush = new SolidBrush(mapperFormSettings.PlayerColor);
                else if (e.PropertyName == nameof(MapperFormSettings.OtherNPCColor))
                    _otherNPCBrush = new SolidBrush(mapperFormSettings.OtherNPCColor);
                else if (e.PropertyName == nameof(MapperFormSettings.NodeColor))
                    _nodeBrush = new SolidBrush(mapperFormSettings.NodeColor);
                else if (e.PropertyName == nameof(MapperFormSettings.SkillNodeColor))
                    _skillnodeBrush = new SolidBrush(mapperFormSettings.SkillNodeColor);
            }
        }

        public Brush BidirectionalPathBrush => _bidirectionalPathBrush;
        private Brush _bidirectionalPathBrush;

        public Pen BidirectionalPathPen => _bidirectionalPathPen;
        private Pen _bidirectionalPathPen;

        public Brush UnidirectionalPathBrush => _unidirectionalPathBrush;
        private Brush _unidirectionalPathBrush;

        public Pen UnidirectionalPathPen => _unidirectionalPathPen;
        private Pen _unidirectionalPathPen;

        public Color BackgroundColor => EntityTools.Config.Mapper.MapperForm.BackgroundColor;

        public Brush EnemyBrush => _enemyBrush;
        private Brush _enemyBrush;

        public Brush FriendBrush => _friendBrush;
        private Brush _friendBrush;

        public Brush PlayerBrush => _playerBrush;
        private Brush _playerBrush;

        public Brush OtherNPCBrush => _otherNPCBrush;
        private Brush _otherNPCBrush;

        public Brush NodeBrush => _nodeBrush;
        private Brush _nodeBrush;

        public Brush SkillnodeBrush => _skillnodeBrush;
        private Brush _skillnodeBrush;
        

    }
}
