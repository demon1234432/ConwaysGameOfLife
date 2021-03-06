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

        void setImage(Bitmap img)
        {
            pictureBox.Image = img;
            pictureBox.Refresh();
        }
        void lg_generationUpdate(object sender, Game.CellsEventArgs e)
        {            
            // замена картинки
            pictureBox.Invoke(new imgvoiddel(setImage), e.cells.Image);
        }
        private void GameForm_Load(object sender, EventArgs e)
        {


            setImage(lg.Cells.Image);

        }

        delegate void imgvoiddel(Bitmap img);
        delegate void voiddel();

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
            setImage(CellsImage.GetImg(lg.Cells));
            try
            {
                lg.SetRules(GameRules.Parse(Config.Conf.GameRules));
            }
            catch (Exception)
            {
                lg.SetRules(GameRules.Conways);
            }
        }


        private void randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool b = lg.TimerEnabled;
            lg.Stop();
            lg.Random();
            setImage(lg.Cells);
            if (b) lg.Start();
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
            lg.Cells.Dispose();
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new Exception("не работает");
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lg.Stop();
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LifeGameConverter.SaveToFile(lg.Cells.Cells, saveFileDialog.FileName);
            }
        }

        private void cleanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lg.Stop();
            lg.Cells.Def();
            setImage(lg.Cells.Image);
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            int w = e.Location.X / Config.Conf.PixToCell;
            int h = e.Location.Y / Config.Conf.PixToCell;
            lg.SetCell(w, h);
            if (!lg.TimerEnabled)
            {
                lg.upd();
                pictureBox.Invoke(new imgvoiddel(setImage), lg.Cells.Image);
            }
            
        }
    }
}
