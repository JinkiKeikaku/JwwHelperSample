using System.Drawing;

namespace JwwViewer.Shape
{
    class JwwDotShape : ICadShape
    {
        JwwHelper.JwwTen mData;
        public JwwDotShape(JwwHelper.JwwTen data)
        {
            mData = data;
        }
        public void OnDraw(Graphics g, DrawContext d)
        {
            var p0 = d.DocToCanvas(mData.m_start_x, mData.m_start_y);
            var radius = d.DocToCanvas(1.0);
            var r = new RectangleF(-radius + p0.X, -radius + p0.Y, radius * 2, radius * 2);
            d.ApplyPenColor(mData.m_nPenColor);
            g.DrawEllipse(d.Pen, r);
        }
    }
}
