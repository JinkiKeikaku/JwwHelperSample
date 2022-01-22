using JwwHelper;
using JwwViewer.Shape;
using System.Collections.Generic;
using System.Drawing;

namespace JwwViewer
{
    class DrawContext
    {
        public Pen Pen = new(Color.Black);
        public SizeF PaperSize;
        public float Scale = 4.0f;
        JwwHeader mHeader;
        public List<BlockEntity> BlockEntities { get;}
        public PointF Origin { get; set; }

        public DrawContext(JwwHeader header, List<BlockEntity> blockEntities)
        {
            mHeader = header;
            BlockEntities = blockEntities;
            PaperSize = Helpers.GetPaperSize(header.m_nZumen);
            Origin = new PointF(PaperSize.Width / 2.0f, PaperSize.Height / 2.0f);
        }


        //public void SetHeader(JwwHeader header)
        //{
        //    mHeader = header;
        //    PaperSize = Helpers.GetPaperSize(header.m_nZumen);
        //}

        public SizeF DocToCanvas(SizeF size)
        {
            return new SizeF(size.Width * Scale, size.Height * Scale);
        }

        public float DocToCanvas(double radius)
        {
            return (float)radius * Scale;
        }

        public PointF DocToCanvas(double x, double y)
        {
            return new PointF((float)(x + Origin.X) * Scale, (float)(-y + Origin.Y) * Scale);
        }
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

        public void ApplyPenColor(int penColor)
        {
            Pen.Color = ConvertColor(penColor);
        }

        Dictionary<int, Color> mColorMap = null;
        public Color ConvertColor(int pen)
        {
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


    }
}
