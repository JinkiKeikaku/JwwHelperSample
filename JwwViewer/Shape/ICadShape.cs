using System.Drawing;

namespace JwwViewer.Shape
{
    interface ICadShape
    {
        void OnDraw(Graphics g, DrawContext d);
        public JwwHelper.JwwData CreateJwwData();
    }
}
