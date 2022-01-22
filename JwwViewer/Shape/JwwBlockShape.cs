using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwViewer.Shape
{
    class JwwBlockShape : ICadShape
    {
        JwwHelper.JwwBlock mData;
        public JwwBlockShape(JwwHelper.JwwBlock data)
        {
            mData = data;
        }
        public void OnDraw(Graphics g, DrawContext d)
        {
            var p0 = d.DocToCanvas(mData.m_DPKijunTen_x, mData.m_DPKijunTen_y);
            var be = d.BlockEntities.Find(x => x.ID == mData.m_nNumber);
            if(be != null)
            {
                //ここでは単純にgraphicsの座標変換を利用した。
                //このサンプルでは線幅と線種を無視しているためこの方法が使えると言うことに注意。
                //実用するには図形に変形関数を実装したほうがよい。
                var saved = g.Save();
                var tmp = d.Origin;
                d.Origin = new PointF(0,0);
                g.TranslateTransform(p0.X, p0.Y);
                g.ScaleTransform((float)mData.m_dBairitsuX, (float)mData.m_dBairitsuY);
                g.RotateTransform((float)Helpers.RadToDeg(mData.m_radKaitenKaku));
                foreach(var s in be.Shapes)
                {
                    s.OnDraw(g, d);
                }
                d.Origin = tmp;
                g.Restore(saved);
            }
        }
    }
}