using JwwViewer.Shape;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwViewer
{
    class JwwToShapeConverter
    {
        JwwHelper.JwwImage[] mImages;
        public JwwToShapeConverter(JwwHelper.JwwImage[] images)
        {
            mImages = images;
        }

        public ICadShape Convert(JwwHelper.JwwData jd)
        {
            if (jd is JwwHelper.JwwMoji m && m.m_string.StartsWith("^@BM"))
            {
                return CreateImageShape(m);
            }
            return jd switch
            {
                JwwHelper.JwwSen s => new LineShape(s),
                JwwHelper.JwwEnko s => new ArcShape(s),
                JwwHelper.JwwSolid s => new SolidShape(s),
                JwwHelper.JwwMoji s => new TextShape(s),
                JwwHelper.JwwTen s => new DotShape(s),
                JwwHelper.JwwBlock s => new BlockShape(s),
                _ => null
            };
        }


        private ImageShape CreateImageShape(JwwHelper.JwwMoji js)
        {
            var s0 = js.m_string.Substring(4);
            var s1 = s0.Split(',');
            var name = s1[0];
            name = name.Replace("\\", "/");
            if (name.Substring(0, 6) == "%temp%")
            {
                var imageName = name.Substring(6);
                var jwwImage = Array.Find<JwwHelper.JwwImage>(mImages, x =>
                {
                    var ext = x.ImageName.Substring(x.ImageName.Length - 3);
                    if (ext == ".gz")
                    {
                        return imageName == x.ImageName.Substring(0, x.ImageName.Length - 3);
                    }
                    else
                    {
                        return imageName == x.ImageName;
                    }
                });
                if (jwwImage != null)
                {
                    try
                    {
                        var ext = jwwImage.ImageName.Substring(jwwImage.ImageName.Length - 3);
                        if (ext == ".gz")
                        {
                            using var rs = new MemoryStream(jwwImage.Buffer);
                            using var gz = new GZipStream(rs, CompressionMode.Decompress);
                            using var tmp = new MemoryStream();
                            gz.CopyTo(tmp);
                            var s = new ImageShape(js);
                            if (double.TryParse(s1[1], out double w) &&
                                double.TryParse(s1[2], out double h) &&
                                double.TryParse(s1[6], out double angle))
                            {
                                s.Bytes = tmp.GetBuffer();
                                //角度は文字列のほうを使うみたい
                                s.AngleDeg = (float)angle;//jj.m_degKakudo;
                                s.P0 = new CadPoint(js.m_start_x, js.m_start_y);
                                s.Width = (float)w;
                                s.Height = (float)h;
                                return s;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine($"Image read error:{jwwImage.ImageName}");
                    }
                }
            }
            return null;
        }
    }
}
