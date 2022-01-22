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

    }
}
