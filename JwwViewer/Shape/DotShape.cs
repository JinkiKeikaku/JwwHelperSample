using System.Drawing;

namespace JwwViewer.Shape
{
    class DotShape : ICadShape
    {
        JwwHelper.JwwTen mData;
        public DotShape(JwwHelper.JwwTen data)
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
        public JwwHelper.JwwData CreateJwwData()
        {
            var s = new JwwHelper.JwwTen();
            s.m_nGLayer = mData.m_nGLayer;
            s.m_nLayer = mData.m_nLayer;
            s.m_nPenColor = mData.m_nPenColor;
            s.m_nPenStyle = mData.m_nPenStyle;
            s.m_nPenWidth = mData.m_nPenWidth;
            s.m_sFlg = mData.m_sFlg;
            s.m_start_x = mData.m_start_x;
            s.m_start_y = mData.m_start_y;
            s.m_bKariten = mData.m_bKariten;
            s.m_dBairitsu = mData.m_dBairitsu;
            s.m_radKaitenKaku = mData.m_radKaitenKaku;

            return mData;
        }
    }
}
