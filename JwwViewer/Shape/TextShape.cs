using System.Drawing;

namespace JwwViewer.Shape
{
    class TextShape : ICadShape
    {
        JwwHelper.JwwMoji mData;
        public TextShape(JwwHelper.JwwMoji data)
        {
            mData = data;
        }
        public void OnDraw(Graphics g, DrawContext d)
        {
            //プリンタ情報（？）が文字で入っている（表示されない）。startとendが等しい時はそれらしいので無視
            if (mData.m_start_x == mData.m_end_x && mData.m_start_y == mData.m_end_y) return;
            //zサンプルのため文字は単純にDrawString()で書いています。
            //位置、サイズ、文字幅を合わせるためには努力が必要です。私も正解は知りません。
            //その他、文字間隔、縦書き、文字種、特殊文字なども考慮していません。
            var fontHeight = (float)d.DocToCanvas(mData.m_dSizeY);
            var p0 = d.DocToCanvas(mData.m_start_x, mData.m_start_y);
            var col = d.ConvertColor(mData.m_nPenColor);
            using var brush = new SolidBrush(col);
            using var font = new Font(mData.m_strFontName, fontHeight, GraphicsUnit.Pixel);
            var saved = g.Save();
            g.TranslateTransform(p0.X, p0.Y);
            g.RotateTransform((float)d.DocToCanvasAngle(mData.m_degKakudo));
            d.ApplyPenColor(mData.m_nPenColor);
            //文字は基点が左下なので移動しています。
            g.DrawString(mData.m_string, font, brush, new PointF(0, -fontHeight));
            g.Restore(saved);
        }
        public JwwHelper.JwwData CreateJwwData()
        {
            var s = new JwwHelper.JwwMoji();
            s.m_nGLayer = mData.m_nGLayer;
            s.m_nLayer = mData.m_nLayer;
            s.m_nPenColor = mData.m_nPenColor;
            s.m_nPenStyle = mData.m_nPenStyle;
            s.m_nPenWidth = mData.m_nPenWidth;
            s.m_sFlg = mData.m_sFlg;
            s.m_start_x = mData.m_start_x;
            s.m_start_y = mData.m_start_y;
            s.m_degKakudo = mData.m_degKakudo;
            s.m_dKankaku = mData.m_dKankaku;
            s.m_dSizeX = mData.m_dSizeY;
            s.m_end_x = mData.m_end_x;
            s.m_end_y = mData.m_end_y;
            s.m_nMojiShu = mData.m_nMojiShu;
            s.m_strFontName = mData.m_strFontName;
            s.m_string = mData.m_string;
            return s;
        }
    }
}
