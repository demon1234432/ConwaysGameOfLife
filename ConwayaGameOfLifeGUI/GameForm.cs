﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using ConwaysGameOfLife;
using System.IO;

namespace ConwayaGameOfLifeGUI
{
    public partial class GameForm : Form
    {
        Game lg = new Game();
        public GameForm()
        {
            InitializeComponent();
            resize();

            lg.generationUpdate += lg_generationUpdate;
        }

        void resize()
        {
            Size s = Config.Conf.worldSize;
            pictureBox.Width = s.Width * Config.Conf.PixToCell;
            pictureBox.Height = s.Height * Config.Conf.PixToCell;
            panel.Width = s.Width * Config.Conf.PixToCell;
            panel.Height = s.Height * Config.Conf.PixToCell;
        }

        /// <summary>
        /// НЕ ИСПОЛЬЗОВАТЬ! метод для вызова Invoke
        /// </summary>
        /// <param name="img"></param>
        void setImage(Bitmap img)
        {
            pictureBox.Image = img;
            pictureBox.Refresh();
        }
        void SetImage(Bitmap img)
        {
            pictureBox.Invoke(new imgvoiddel(setImage), img);
        }

        void lg_generationUpdate(object sender, Game.CellsEventArgs e)
        {
            // Clone() нужен?
            Bitmap img = (Bitmap)e.cells.Image;

            SetImage(img);
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            SetImage(lg.Cells.Image);
        }

        delegate void imgvoiddel(Bitmap img);

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lg.Stop();
            ConfigForm cf = new ConfigForm();
            cf.OnApplySetting += cf_OnApplySetting;

            cf.ShowDialog();
        }
        void cf_OnApplySetting(object sender, EventArgs e)
        {
            // проверка размера (обнуление Cells при несоответствии)
            if (!lg.Cells.IsValid)
            {
                lg.Update(new CellsImage());
            }

            // обновление свойств элементов управления
            resize();

            // замена изображения
            SetImage(CellsImage.GetImg(lg.Cells));
        }


        private void randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lg.Random();
            SetImage(lg.Cells.Image);
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lg.Start();
        }
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lg.Stop();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lg.Stop(); // подстраховка на случай исключений
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException("В разработке");
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException("В разработке");
        }

        private void cleanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lg.Stop();
            lg.Update(new CellsImage());
            SetImage(lg.Cells.Image);
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            int w = e.Location.X / Config.Conf.PixToCell;
            int h = e.Location.Y / Config.Conf.PixToCell;
            lg.SetCell(w, h);
            SetImage(lg.Cells.Image);
        }
    }
}
