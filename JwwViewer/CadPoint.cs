using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwViewer
{
    class CadPoint
    {
        /// 座標X
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// 座標Y
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// コンストラクター
        /// </summary>
        public CadPoint() { }
        /// <summary>
        /// コンストラクター
        /// </summary>
        public CadPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public PointF ToPointF()
        {
            return new PointF((float)X, (float)Y);
        }
        public void Set(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public void Set(CadPoint p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }
        /// <summary>
        /// 座標を(0, 0)基準で回転。角度はradian。
        /// </summary>
        public void Rotate(double rad)
        {
            if (Helpers.FloatEQ((float)rad, 0.0f)) return;
            var c = Math.Cos(rad);
            var s = Math.Sin(rad);
            var xx = X * c - Y * s;
            var yy = X * s + Y * c;
            X = xx;
            Y = yy;
        }



    }
}
