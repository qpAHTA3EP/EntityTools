using System.Windows.Forms;

namespace EntityTools.Quester.Mapper.Tools
{
    public class MapperMouseEventArgs
    {
        public MapperMouseEventArgs(MouseButtons button, int click, double x, double y)
        {
            X = x;
            Y = y;
            Click = click;
            Button = button;
        }
        public double X { get; }
        public double Y { get; }
        public int Click { get; }
        public MouseButtons Button { get; }
    }
}
