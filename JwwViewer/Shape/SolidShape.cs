using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using static System.MathF;

namespace JwwViewer.Shape
{
    class SolidShape : ICadShape
    {
        JwwHelper.JwwSolid mData;
        public SolidShape(JwwHelper.JwwSolid data)
        {
            mData = data;
        }
        public JwwHelper.JwwData CreateJwwData()
        {
            var s = new JwwHelper.JwwSolid();
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
            s.m_Color = mData.m_Color;
            s.m_DPoint2_x = mData.m_DPoint2_x;
            s.m_DPoint2_y = mData.m_DPoint2_y;
            s.m_DPoint3_x = mData.m_DPoint3_x;
            s.m_DPoint3_y = mData.m_DPoint3_y;
            return s;

            return mData;
        }
        public void OnDraw(Graphics g, DrawContext d)
        {
            var col = mData.m_nPenColor == 10 ?
                Helpers.ColorRefToColor(mData.m_Color) :
                d.ConvertColor(mData.m_nPenColor);
            using var brush = new SolidBrush(col);
            if (mData.m_nPenStyle < 101)
            {
                var points = new List<PointF>();
                points.Add(d.DocToCanvas(mData.m_start_x, mData.m_start_y));
                points.Add(d.DocToCanvas(mData.m_end_x, mData.m_end_y));
                points.Add(d.DocToCanvas(mData.m_DPoint2_x, mData.m_DPoint2_y));
                if (mData.m_start_x != mData.m_DPoint3_x || mData.m_start_y != mData.m_DPoint3_y)
                {
                    points.Add(d.DocToCanvas(mData.m_DPoint3_x, mData.m_DPoint3_y));
                }
                g.FillPolygon(brush, points.ToArray());
                return;
            }
            var p0 = d.DocToCanvas(mData.m_start_x, mData.m_start_y);
            var radius = (float)d.DocToCanvas(mData.m_end_x);
            var inRadius = (float)d.DocToCanvas(mData.m_DPoint3_y);
            var flatness = (float)mData.m_end_y;
            var ry = (float)(radius * flatness);
            var angle = (float)Helpers.RadToDeg(d.DocToCanvasAngle(mData.m_DPoint2_x));
            var startRad = (float)d.DocToCanvasAngle(mData.m_DPoint2_y);
            var startDeg = (float)Helpers.RadToDeg(startRad);
            var sweepRad = (float)d.DocToCanvasAngle(mData.m_DPoint3_x);
            var sweepDeg = (float)Helpers.RadToDeg(sweepRad);
            var saved = g.Save();
            g.TranslateTransform(p0.X, p0.Y);
            g.RotateTransform(angle);
            //円ソリッド。
            if (mData.m_nPenStyle == 101)
            {
                switch (mData.m_DPoint3_y)
                {
                    case 100.0: //円
                        {
                            g.FillEllipse(brush, -radius, -ry, radius * 2, ry * 2);
                            break;
                        }
                    case 0.0: //Pie
                        {
                            g.FillPie(brush, -radius, -ry, radius * 2, ry * 2, startDeg, sweepDeg);

                            //var path = new GraphicsPath();
                            //path.AddArc(-radius, -ry, radius * 2, ry * 2, startDeg, sweepDeg);
                            //path.AddLine(0, 0, radius * Cos(startRad), ry * Sin(startRad + sweepRad));
                            //path.AddLine(0, 0, radius * Cos(startRad + sweepRad), ry * Sin(startRad + sweepRad));
                            //g.FillPath(brush, path);
                            break;
                        }
                    case 5.0://Arc
                        {
                            var path = new GraphicsPath();
                            path.AddArc(-radius, -ry, radius * 2, ry * 2, startDeg, sweepDeg);
                            path.AddLine(
                                radius * Cos(startRad), ry * Sin(startRad + sweepRad), 
                                radius * Cos(startRad + sweepRad), ry * Sin(startRad + sweepRad)
                            );
                            g.FillPath(brush, path);
                        }
                        break;
                    case -1.0://outer circle
                        {
                            var path = new GraphicsPath();
                            path.AddArc(-radius, -ry, radius * 2, ry * 2, startDeg, sweepDeg);

                            var dp1 = new PointF(0, 1);
                            var dp2 = new PointF(0, 1);
                            dp2 = dp2.Rotate(sweepRad);
                            var p1 = new PointF(radius, 0);
                            var p2 = new PointF(radius, 0);
                            p2 = p2.Rotate(sweepRad);
                            var (pp, flag) = Helpers.GetCrossPoint(p1, Helpers.Add(p1, dp1), p2, Helpers.Add(p2, dp2));

                            p1 = p1.Rotate(startRad);
                            p1.Y *= flatness;
                            p2 = p2.Rotate(startRad);
                            p2.Y *= flatness;
                            pp = pp.Rotate(startRad);
                            pp.Y *= flatness;
                            path.AddLine(p1, pp);
                            path.AddLine(pp, p2);
                            g.FillPath(brush, path);
                        }
                        break;
                    default:
                        break;
                }
            }
            //円環ソリッド。サンプルのため手抜きで105と106を同じとした。
            if (mData.m_nPenStyle == 105 || mData.m_nPenStyle == 106)
            {
                //Windowsはこれで表示されるみたいだが、本当は２つの円弧の向きを反対方向にしなければいけないはず。
                float inRy = inRadius * flatness;
                var p1 = new PointF(radius, 0);
                var p2 = new PointF(radius, 0);
                p2 = p2.Rotate(sweepRad);
                p1 = p1.Rotate(startRad);
                p2 = p2.Rotate(startRad);
                p1.Y *= flatness;
                p2.Y *= flatness;
                var p3 = new PointF(inRadius, 0);
                var p4 = new PointF(inRadius, 0);
                p4 = p4.Rotate(sweepRad);
                p3 = p3.Rotate(startRad);
                p4 = p4.Rotate(startRad);
                p3.Y *= flatness;
                p4.Y *= flatness;
                var path = new GraphicsPath();
                path.AddArc(-radius, -ry, radius * 2, ry * 2, startDeg, sweepDeg);
                path.AddLine(p1, p3);
                path.AddArc(-inRadius, -inRy, inRadius * 2, inRy * 2, startDeg, sweepDeg);
                path.AddLine(p4, p2);
                g.FillPath(brush, path);
            }
            g.Restore(saved);
            return;
        }
    }
}