using System.Drawing;

namespace JwwViewer.Shape
{
    class JwwLineShape : ICadShape
    {
        JwwHelper.JwwSen mData;
        public JwwLineShape(JwwHelper.JwwSen data)
        {
            mData = data;
        }
        public void OnDraw(Graphics g, DrawContext d)
        {
            var p1 = d.DocToCanvas(mData.m_start_x, mData.m_start_y);
            var p2 = d.DocToCanvas(mData.m_end_x, mData.m_end_y);
            d.ApplyPenColor(mData.m_nPenColor);
            g.DrawLine(d.Pen, p1, p2);
        }
    }
}
