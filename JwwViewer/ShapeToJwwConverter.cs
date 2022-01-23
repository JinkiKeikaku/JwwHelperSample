using JwwViewer.Shape;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwViewer
{
    class ShapeToJwwConverter
    {
        /// <summary>
        /// 同梱画像を保存するバッファ
        /// </summary>
        public List<JwwHelper.JwwImage> Images = new();
        public HashSet<int> BlockIDSet { get; } = new HashSet<int>();
        public ShapeToJwwConverter()
        {
        }


        public JwwHelper.JwwData Convert(ICadShape shape)
        {
            return shape switch
            {
                LineShape s => s.CreateJwwData(),
                ArcShape s => s.CreateJwwData(),
                DotShape s => s.CreateJwwData(),
                SolidShape s => s.CreateJwwData(),
                TextShape s => s.CreateJwwData(),
                ImageShape s => CreateImageData(s),
                BlockShape s => CreateBlockData(s),
                _ => null
            };
        }

        private JwwHelper.JwwBlock CreateBlockData(BlockShape s)
        {
            //ブロック図形は書き戻すだけとした。
            BlockIDSet.Add(s.ID);
            return (JwwHelper.JwwBlock)s.CreateJwwData();
        }

        private JwwHelper.JwwMoji CreateImageData(ImageShape shape)
        {
            var d = (JwwHelper.JwwMoji)shape.CreateJwwData();
            var s = new JwwHelper.JwwMoji();
            s.m_nGLayer = d.m_nGLayer;
            s.m_nLayer = d.m_nLayer;
            s.m_nPenColor = d.m_nPenColor;
            s.m_nPenStyle = d.m_nPenStyle;
            s.m_nPenWidth = d.m_nPenWidth;
            s.m_sFlg = d.m_sFlg;
            s.m_start_x = d.m_start_x;
            s.m_start_y = d.m_start_y;
            s.m_degKakudo = d.m_degKakudo;
            s.m_dKankaku = d.m_dKankaku;
            s.m_dSizeX = d.m_dSizeY;
            s.m_end_x = d.m_end_x;
            s.m_end_y = d.m_end_y;
            s.m_nMojiShu = d.m_nMojiShu;
            s.m_strFontName = d.m_strFontName;
            var (name, gzName, buffer) = CreateJwwImageInfo(shape);
            var image = JwwHelper.JwwImage.Create(gzName, shape.Bytes);
            //いったんImagesと言うリストに保存するが、直接JwwHelper.JwwWriterオブジェクトにAddImage()
            //しても構わない。このサンプルではいったん貯めてからまとめて書き出すこととする。
            Images.Add(image);
            s.m_string = name;
            return s;
        }

        //jw_cadではCDataMojiの文字に画像名が入る。フォーマットは、
        //"^@BM%temp%V__Picture_Sample_1.bmp,100,75,0,0,1,0"
        //100,75,0,0,1,0 => width,height, ?, ?, ?, amgle
        //まだよくわからないところがある。
        //圧縮にはgzipを使う。
        private (string name, string gzName, byte[] buffer) CreateJwwImageInfo(ImageShape s)
        {
            var bmName = $"C__Picture__{mImageNamePrefix + mImageNameIndex}.bmp";
            var name = $"^@BM%temp%{bmName},{s.Width},{s.Height},0,0,1,{s.AngleDeg}";
            var gzName = bmName + ".gz";
            mImageNameIndex++;
            using var rs = new MemoryStream(s.Bytes);
            var image = Image.FromStream(rs);
            if (image != null)
            {
                using var ws = new MemoryStream();
                image.Save(ws, System.Drawing.Imaging.ImageFormat.Bmp);
                var buffer = ws.GetBuffer();
                using var dst = new MemoryStream();
                using var gz = new GZipStream(dst, CompressionLevel.Optimal);
                gz.Write(buffer, 0, buffer.Length);
                gz.Close();
                return (name, gzName, dst.GetBuffer());
            }
            return (name, gzName, null);
        }
        private int mImageNameIndex = 0;
        private string mImageNamePrefix = "Sample_";


    }
}
