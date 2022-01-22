using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwViewer.Shape
{
    class JwwImageShape : ICadShape
    {
        byte[] mBytes = null;
        Image mImage = null;
        //画像の基点は左下
        public CadPoint P0 { get; set; } = new CadPoint();
        public float Width { get; set; } = 100.0f;
        public float Height { get; set; } = 100.0f;
        public float AngleDeg { get; set; } = 0.0f;
        public byte[] Bytes { 
            get => mBytes; 
            set {
                mBytes = value;
                mImage = null;
            } 
        }
        public JwwImageShape() { }

        public Image GetImage()
        {
            if (mImage == null && Bytes != null)
            {
                var ms = new MemoryStream(Bytes);
                mImage = Bitmap.FromStream(ms);
            }
            return mImage;
        }

        public void OnDraw(Graphics g, DrawContext d)
        {
            var p0 = d.DocToCanvas(P0);
            var w = d.DocToCanvas(Width);
            var h = d.DocToCanvas(Height);
            var image = GetImage();
            if (image == null) return;
            var saved = g.Save();
            g.TranslateTransform(p0.X, p0.Y);
            g.RotateTransform((float)d.DocToCanvasAngle(AngleDeg));
            //基準点が左下だから移動する。
            g.DrawImage(image, 0, -h, w, h);
            g.Restore(saved);
        }
        
    }
}
