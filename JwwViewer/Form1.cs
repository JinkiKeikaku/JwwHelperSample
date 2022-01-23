using JwwViewer.Properties;
using JwwViewer.Shape;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace JwwViewer
{
    public partial class Form1 : Form
    {
        List<ICadShape> mShapes = new();
        List<BlockEntity> mBlockEntities = new();
        string[,] LayerName = new string[16, 16];
        string[] LayerGroupName = new string[16];

        DrawContext dc;// = new DrawContext();

        public Form1()
        {
            InitializeComponent();
            panel1.AutoScroll = true;
            this.panel1.MouseWheel += panel1_MouseWheel;
        }


        /// <summary>
        /// 読み込み処理
        /// </summary>
        private void OpenFile(String path)
        {
            try
            {
                if (Path.GetExtension(path) == ".jww")
                {
                    //JwwReaderが読み込み用のクラス。Completedは読み込み完了時に実行される関数。
                    //"d:\\ccc\\"はファイルに同梱画像があった時に画像が保存されるフォルダ。
                    using var reader = new JwwHelper.JwwReader(Completed);
                    reader.Read(path);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error");
                dc = null;
                panel1.Invalidate();
            }
        }
        //dllでjwwファイル読み込み完了後に呼ばれます。これは確認用のコードです。
        private void Completed(JwwHelper.JwwReader reader)
        {
            mShapes.Clear();
            LayerNameToTextBox(reader.Header);
            ConvertBlockEntities(reader);
            var cv = new JwwToShapeConverter(reader.Images);
            //図形を変換して配列に入れる。表示するだけなら変換しなくてなんとかなるが、
            //普通は利用する時に何らかの変換がいるから。
            foreach (var jd in reader.DataList)
            {
                var s = cv.Convert(jd);
                if (s != null) mShapes.Add(s);
            }
            //dcは表示する時に使う情報保持オブジェクト。
            dc = new DrawContext(reader.Header, mBlockEntities);
            //スクロールバーなんかの設定。
            CalcSize();
            //panel1を無効化してpanel1のpaintが呼ばれる。
            panel1.Invalidate();
        }

        /// <summary>
        /// レイヤ名をテキストボックスに入れる。
        /// </summary>
        private void LayerNameToTextBox(JwwHelper.JwwHeader header)
        {
            string[] prefix = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            Array.Clear(LayerName, 0, 256);
            Array.Clear(LayerGroupName, 0, 16);
            var sb = new StringBuilder();
            for (var j = 0; j < 16; j++)
            {
                LayerGroupName[j] = header.m_aStrGLayName[j];
                sb.AppendLine($"{prefix[j]} : {LayerGroupName[j]}");
                for (var i = 0; i < 16; i++)
                {
                    LayerName[j, i] = header.m_aStrLayName[j][i];
                    sb.AppendLine($"- {prefix[i]} : {LayerName[j, i]}");
                }
            }
            textBox1.Text = sb.ToString();
        }

        /// <summary>
        /// 読み込み用のブロック図形実体の処理。
        /// </summary>
        private void ConvertBlockEntities(JwwHelper.JwwReader reader)
        {
            mBlockEntities.Clear();
            var cv = new JwwToShapeConverter(reader.Images);
            //Block図形本体の処理
            foreach (var jb in reader.DataListList)
            {
                var be = new BlockEntity(jb.m_nNumber);
                jb.EnumerateDataList(x =>
                {
                    var s = cv.Convert(x);
                    if (s != null) be.Shapes.Add(s);
                    return true;    //trueで処理継続
                });
                mBlockEntities.Add(be);
            }
        }

        /// <summary>
        /// 保存処理
        /// </summary>
        private void SaveFile(string path)
        {
            //JwwのHeaderは設定項目が多い。ひとつづつ設定すると大変なのでテンプレートファイルを使う。
            //このサンプルではテンプレートファイルはリソースとした。
            var tmp = Path.GetTempFileName();
            var buf = Resources.template;
            File.WriteAllBytes(tmp, buf);
            var w = new JwwHelper.JwwWriter();
            w.InitHeader(tmp);
            File.Delete(tmp);

            //こんな感じでHeaderの項目を設定する。内容は本家の説明を参照してください。
            w.Header.m_strMemo = $"Jww保存テスト\n{DateTime.Now.ToString()}";

            var cv = new ShapeToJwwConverter();

            //図形書き出し。
            foreach (var s in mShapes)
            {
                var jd = cv.Convert(s);
                if (jd != null) w.AddData(jd);
            }

            //ブロック図形実体の処理。ブロック図形実体はJwwDataList。
            foreach (var be in mBlockEntities)
            {
                var dataList = new JwwHelper.JwwDataList();
                dataList.m_nNumber = (short)be.ID;
                //本当はブロック図形名は後尾に"@@SfigorgFlag@@4"とかを付けるのかもしれない。
                dataList.m_strName = $"Block_{be.ID}";
                dataList.m_lGroup = 0;
                dataList.m_nGLayer = 0;
                dataList.m_nLayer = 0;
                dataList.m_nPenColor = 2;
                dataList.m_nPenWidth = 0;
                dataList.m_nPenStyle = 1;
                dataList.m_sFlg = 0;
                dataList.m_bReffered = cv.BlockIDSet.Contains(be.ID);
                foreach (var s in be.Shapes)
                {
                    var jd = cv.Convert(s);
                    if (jd != null) dataList.Add(jd);
                }
                w.AddDataList(dataList);
            }
            //同梱画像書き出し。
            foreach (var image in cv.Images)
            {
                w.AddImage(image);
            }
            w.Write(path);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = new OpenFileDialog();
            d.Filter = "Jww Files|*.jww|All Files|*.*";
            if (d.ShowDialog() != DialogResult.OK) return;
            OpenFile(d.FileName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = new SaveFileDialog();
            d.Filter = "Jww Files|*.jww|All Files|*.*";
            if (d.ShowDialog() != DialogResult.OK) return;
            SaveFile(d.FileName);
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            if (dc == null) return;
            var saved = g.Save();

            g.TranslateTransform(
                dc.Scale * dc.PaperSize.Width / 2 + panel1.AutoScrollPosition.X,
                dc.Scale * dc.PaperSize.Height / 2 + panel1.AutoScrollPosition.Y
            );
            g.ScaleTransform(dc.Scale, dc.Scale);
            foreach (var s in mShapes)
            {
                s.OnDraw(g, dc);
            }
            g.Restore(saved);
        }

        private void panel1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (dc == null) return;
            /// マウスホイールでは拡大縮小のみ行う。
            if (e.Delta < 0)
            {
                dc.Scale *= 2 / 3.0f;
            }
            else
            {
                dc.Scale *= 1.5f;
            }
            CalcSize();
            panel1.Invalidate();
        }

        /// <summary>
        /// スクロールの設定
        /// </summary>
        private void CalcSize()
        {
            if (dc == null) return;
            var ps = new Size((int)(dc.PaperSize.Width * dc.Scale), (int)(dc.PaperSize.Height * dc.Scale));
            panel1.AutoScrollMinSize = new Size((int)ps.Width, (int)ps.Height);
            panel1.AutoScrollPosition = new Point(
                Math.Max(0, (int)ps.Width / 2 - Width / 2),
                Math.Max(0, (int)ps.Height / 2 - Height / 2)
            );
        }

        private void panel1_ClientSizeChanged(object sender, EventArgs e)
        {
            CalcSize();
        }
    }
}
