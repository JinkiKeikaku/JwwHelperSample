using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using static System.MathF;

namespace JwwViewer
{
    static class Helpers
    {
        /// <summary>
        /// 読み込むDLLを６４ビットと３２ビットで切り替えます。
        /// ProgramのMain()で呼んでください。
        /// </summary>
        public static void SelectDll()
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
            {
                if (e.Name.StartsWith("JwwHelper,", StringComparison.OrdinalIgnoreCase))
                {
                    string fileName = Path.Combine(assemblyDir,
                    string.Format("JwwHelper_{0}.dll", (IntPtr.Size == 4) ? "x86" : "x64"));
                    Debug.WriteLine($"LoadAssembly::{fileName}");
                    return Assembly.LoadFile(fileName);
                }
                return null;
            };
        }

        /// <summary>
        /// ヘッダーの図面サイズコード（？）から用紙サイズを取得
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static SizeF GetPaperSize(int code)
        {
            if (code < 15)
            {
                return mPaperSize[code];
            }
            return mPaperSize[3];    //わからなかったらひとまずA3
        }

        /// <summary>
        /// RadianからDegreeに変換。jwwの角度は文字以外はRadian。GDI+はDegreeなので変換が必要。
        /// </summary>
        public static double RadToDeg(double angleRad)
        {
            return 180.0 * angleRad / Math.PI;
        }

        /// <summary>
        /// GDIの色をGDI+の色に変換
        /// </summary>
        public static Color ColorRefToColor(int c)
        {
            return Color.FromArgb((byte)(c & 255), (byte)((c >> 8) & 255), (byte)((c >> 16) & 255));
        }

        /// <summary>
        /// 点の引き算
        /// </summary>
        public static PointF Sub (this PointF p1, PointF p2)
        {
            return new PointF(p1.X - p2.X, p1.Y - p2.Y);
        }

        /// <summary>
        /// 点の足し算
        /// </summary>
        public static PointF Add (this PointF p1, PointF p2)
        {
            return new PointF(p1.X + p2.X, p1.Y + p2.Y);
        }

        /// <summary>
        /// 点の回転。角度単位はRadian。
        /// </summary>
        public static PointF Rotate(this PointF p, float rad)
        {
            var c = Cos(rad);
            var s = Sin(rad);
            return new PointF(p.X * c - p.Y * s, p.X * s + p.Y * c);
        }

        /// <summary>
        /// 誤差を含めた比較。ABS([x]-[y])が誤差より小さければtrue。
        /// </summary>
        public static bool FloatEQ(float x, float y)
        {
            return Abs(x - y) < 0.00001f;
        }

        /// <summary>
        /// 直線[p11]-[p12]と[p21]-[p22]の交点を返す。交点がない場合はタプルの[flag]がfalse。
        /// </summary>
        public static (PointF p, bool flag) GetCrossPoint(PointF p11, PointF p12, PointF p21, PointF p22)
        {
            var dp1 = Sub(p12, p11);
            var dp2 = Sub(p22, p21);
            var dp3 = Sub(p11, p21);
            var a = dp1.X * dp2.Y - dp2.X * dp1.Y;
            if (FloatEQ(a, 0.0f)) return (new PointF(), false);
            var t = (dp2.X * dp3.Y - dp3.X * dp2.Y) / a;
            var cp = new PointF(dp1.X * t + p11.X, dp1.Y * t + p11.Y);
            return (cp, true);
        }

        /// <summary>
        /// 用紙サイズのテーブル
        /// </summary>
        static readonly SizeF[] mPaperSize = new SizeF[]{
            new SizeF(1189, 841), //A0
            new SizeF(841, 594),  //A1
            new SizeF(594, 420),  //A2
            new SizeF(420, 297),  //A3
            new SizeF(297, 210),  //A4
            new SizeF(210, 148),  //A5???使わない
            new SizeF(210, 148),  //A6???使わない
            new SizeF(148, 105),  //A7???使わない
            new SizeF(1682, 1189),  //8:2A
            new SizeF(2378, 1682),  //9:3A
            new SizeF(3364, 2378),  //10:4A
            new SizeF(4756, 3364),  //11:5A
            new SizeF(10000, 7071),  //12:10m
            new SizeF(50000, 35355),  //13:50m
            new SizeF(100000, 70711)  //14:100m
        };
    }
}
