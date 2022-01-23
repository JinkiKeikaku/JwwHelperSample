using System.Drawing;

namespace JwwViewer.Shape
{
    class ArcShape : ICadShape
    {
        JwwHelper.JwwEnko mData;
        public ArcShape(JwwHelper.JwwEnko data)
        {
            mData = data;
        }
        public void OnDraw(Graphics g, DrawContext d)
        {
            var p0 = d.DocToCanvas(mData.m_start_x, mData.m_start_y);
            var saved = g.Save();
            g.TranslateTransform(p0.X, p0.Y);
            var angleRad = d.DocToCanvasAngle(mData.m_radKatamukiKaku);
            g.RotateTransform((float)Helpers.RadToDeg(angleRad));
            var radius = d.DocToCanvas(mData.m_dHankei);
            var ry = (float)(radius * mData.m_dHenpeiRitsu);
            var r = new RectangleF(-radius, -ry, radius * 2, ry * 2);
            var startRad = d.DocToCanvasAngle(mData.m_radKaishiKaku);
            var sweepRad = d.DocToCanvasAngle(mData.m_radEnkoKaku);
            var sa = (float)Helpers.RadToDeg(startRad);
            var sw = (float)Helpers.RadToDeg(sweepRad);
            d.ApplyPenColor(mData.m_nPenColor);
            if (mData.m_bZenEnFlg == 0)
            {
                g.DrawArc(d.Pen, r, sa, sw);
            }
            else
            {
                g.DrawEllipse(d.Pen, r);
            }
            g.Restore(saved);
        }
        public JwwHelper.JwwData CreateJwwData()
        {
            var s = new JwwHelper.JwwEnko();
            s.m_nGLayer = mData.m_nGLayer;
            s.m_nLayer = mData.m_nLayer;
            s.m_nPenColor = mData.m_nPenColor;
            s.m_nPenStyle = mData.m_nPenStyle;
            s.m_nPenWidth = mData.m_nPenWidth;
            s.m_sFlg = mData.m_sFlg;
            s.m_start_x = mData.m_start_x;
            s.m_start_y = mData.m_start_y;
            s.m_bZenEnFlg = mData.m_bZenEnFlg;
            s.m_dHankei = mData.m_dHankei;
            s.m_dHenpeiRitsu = mData.m_dHenpeiRitsu;
            s.m_radEnkoKaku = mData.m_radEnkoKaku;
            s.m_radKaishiKaku = mData.m_radKaishiKaku;
            s.m_radKatamukiKaku = mData.m_radKatamukiKaku;
            return s;
        }
    }
}
