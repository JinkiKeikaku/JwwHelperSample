using System.Drawing;

namespace JwwViewer.Shape
{
    class LineShape : ICadShape
    {
        JwwHelper.JwwSen mData;
        public LineShape(JwwHelper.JwwSen data)
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
        public JwwHelper.JwwData CreateJwwData()
        {
            var s = new JwwHelper.JwwSen();
            s.m_nGLayer = mData.m_nGLayer;
            s.m_nLayer = mData.m_nLayer;
            s.m_nPenColor = mData.m_nPenColor;
            s.m_nPenStyle = mData.m_nPenStyle;
            s.m_nPenWidth = mData.m_nPenWidth;
            s.m_sFlg = mData.m_sFlg;
            s.m_start_x = mData.m_start_x;
            s.m_start_y = mData.m_start_y;
            s.m_end_x = mData.m_end_x;
            s.m_end_y = mData.m_end_y;
            return s;
        }
    }
}
