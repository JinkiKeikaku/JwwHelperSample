﻿using JwwViewer.Shape;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JwwViewer
{
    public partial class Form1 : Form
    {
        List<ICadShape> mShapes = new();
        List<BlockEntity> mBlockEntities = new();

        DrawContext dc;// = new DrawContext();

        public Form1()
        {
            InitializeComponent();
            panel1.AutoScroll = true;
            this.panel1.MouseWheel += panel1_MouseWheel;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = new OpenFileDialog();
            d.Filter = "Jww Files|*.jww|All Files|*.*";
            if (d.ShowDialog() != DialogResult.OK) return;
            OpenFile(d.FileName);
        }
        void OpenFile(String path)
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
        void Completed(JwwHelper.JwwReader reader)
        {
            mShapes.Clear();
            ConvertBlockEntities(reader);
            var cv = new JwwToShapeConverter();
            foreach (var jd in reader.DataList)
            {
                var s = cv.Convert(jd);
                if(s != null) mShapes.Add(s);
            }
            dc = new DrawContext(reader.Header, mBlockEntities);
            CalcSize();
            panel1.Invalidate();
        }

        /// <summary>
        /// ブロック図形のデータ処理。
        /// </summary>
        private void ConvertBlockEntities(JwwHelper.JwwReader reader)
        {
            mBlockEntities.Clear();
            var cv = new JwwToShapeConverter();
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            if (dc == null) return;
            var saved = g.Save();
            g.TranslateTransform(panel1.AutoScrollPosition.X, panel1.AutoScrollPosition.Y);
            foreach (var s in mShapes)
            {
                s.OnDraw(g, dc);
            }
//            g.DrawLine(d.Pen, 0, 0, 100, 100);
            g.Restore(saved);
        }

        private void panel1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (dc == null) return;
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

            ///process mouse event
        }

        void CalcSize()
        {
            if (dc == null) return;
            var ps = dc.DocToCanvas(dc.PaperSize);
            panel1.AutoScrollMinSize = new Size((int)ps.Width, (int)ps.Height);
            panel1.AutoScrollPosition = new Point(
                Math.Max(0, (int)ps.Width / 2 - Width / 2),
                Math.Max(0, (int)ps.Height / 2 - Height / 2)
            );

        }

    }
}
