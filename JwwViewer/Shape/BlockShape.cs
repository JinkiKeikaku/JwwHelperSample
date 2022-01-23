using System.Drawing;

namespace JwwViewer.Shape
{
    class BlockShape : ICadShape
    {
        JwwHelper.JwwBlock mData;
        public BlockShape(JwwHelper.JwwBlock data)
        {
            mData = data;
        }

        public int ID => mData.m_nNumber;

        public void OnDraw(Graphics g, DrawContext d)
        {
            var p0 = d.DocToCanvas(mData.m_DPKijunTen_x, mData.m_DPKijunTen_y);
            var be = d.BlockEntities.Find(x => x.ID == ID);
            if(be != null)
            {
                //ここでは単純にGraphicsの座標変換を利用した。
                //このサンプルでは線幅と線種を無視しているためこの方法が使えると言うことに注意。
                //実用するには図形に変形関数を実装したほうがよい。
                var saved = g.Save();
                g.TranslateTransform(p0.X, p0.Y);
                g.ScaleTransform((float)mData.m_dBairitsuX, (float)mData.m_dBairitsuY);
                g.RotateTransform((float)Helpers.RadToDeg(mData.m_radKaitenKaku));
                foreach(var s in be.Shapes)
                {
                    s.OnDraw(g, d);
                }
                g.Restore(saved);
            }
        }
        public JwwHelper.JwwData CreateJwwData()
        {
            var s = new JwwHelper.JwwBlock();
            s.m_nGLayer = mData.m_nGLayer;
            s.m_nLayer = mData.m_nLayer;
            s.m_nPenColor = mData.m_nPenColor;
            s.m_nPenStyle = mData.m_nPenStyle;
            s.m_nPenWidth = mData.m_nPenWidth;
            s.m_sFlg = mData.m_sFlg;
            s.m_nNumber = mData.m_nNumber;
            s.m_DPKijunTen_x = mData.m_DPKijunTen_x;
            s.m_DPKijunTen_y = mData.m_DPKijunTen_y;
            s.m_dBairitsuX = mData.m_dBairitsuX;
            s.m_dBairitsuY = mData.m_dBairitsuY;
            s.m_radKaitenKaku = mData.m_radKaitenKaku;
            return mData;
        }

    }
}