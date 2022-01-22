using System.Drawing;

namespace JwwViewer.Shape
{
    class JwwArcShape : ICadShape
    {
        JwwHelper.JwwEnko mData;
        public JwwArcShape(JwwHelper.JwwEnko data)
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
    }
}
