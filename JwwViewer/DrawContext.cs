using JwwHelper;
using JwwViewer.Shape;
using System.Collections.Generic;
using System.Drawing;

namespace JwwViewer
{
    /// <summary>
    /// 描画用の情報保持クラス
    /// </summary>
    class DrawContext
    {
        public Pen Pen = new(Color.Black, 0.0f);
        public float Scale = 4.0f;
        public SizeF PaperSize = new Size(100, 100);
        public List<BlockEntity> BlockEntities { get;}



        public DrawContext(JwwHeader header, List<BlockEntity> blockEntities)
        {
            mHeader = header;
            BlockEntities = blockEntities;
            PaperSize = header == null ? new SizeF(297, 210) : Helpers.GetPaperSize(header.m_nZumen);
        }

        /// <summary>
        /// DocumentとGDI+の半径などの変換。
        /// </summary>
        public float DocToCanvas(double radius)
        {
            return (float)radius;
        }

        /// <summary>
        /// DocumentとGDI+の座標変換。Jwwは上が正なのでｙ座標のみ符号を変える。
        /// </summary>
        public PointF DocToCanvas(double x, double y)
        {
            return new PointF((float)x, (float)-y);
        }

        /// <summary>
        /// DocumentとGDI+の座標変換。Jwwは上が正なのでｙ座標のみ符号を変える。
        /// </summary>
        public PointF DocToCanvas(CadPoint p)
        {
            return DocToCanvas(p.X, p.Y);
        }

        /// <summary>
        /// DocumentとGDI+の角度の変換。Jwwの角度は左回り。GDI+は右回り。
        /// 符号を変えるだけだが座標変換に合わせて間違えないようにあえてこれを使う。
        /// </summary>
        public float DocToCanvasAngle(double angle)
        {
            return -(float)angle;
        }

        /// <summary>
        /// Jwwのpen番号から描画用のPenオブジェクトに色を割り当てる。
        /// </summary>
        public void ApplyPenColor(int penColor)
        {
            Pen.Color = ConvertColor(penColor);
        }

        /// <summary>
        /// Jwwのpen番号からの色変換。
        /// </summary>
        public Color ConvertColor(int pen)
        {
            if (mHeader == null) return Color.Black;
            if (mColorMap == null)
            {
                mColorMap = new Dictionary<int, Color>();
                for (var i = 0; i < 10; i++)
                {
                    var c = (int)mHeader.m_aPenColor[i];
                    var col = Helpers.ColorRefToColor(c);
                    if (col == Color.FromArgb(255, 255, 255)) col = Color.Black;
                    mColorMap[i] = col;
                }
                for (var i = 0; i <= 256; i++)
                {
                    var c = (int)mHeader.m_aPenColor_SXF[i];
                    var col = Helpers.ColorRefToColor(c);
                    if (col == Color.FromArgb(255, 255, 255))
                    {
                        col = Color.Black;
                    }
                    mColorMap[i + 100] = col;
                }
            }
            return mColorMap.GetValueOrDefault(pen, Color.Black);
        }

        private JwwHeader mHeader;
        private Dictionary<int, Color> mColorMap = null;

    }
}
