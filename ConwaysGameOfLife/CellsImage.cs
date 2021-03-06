﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ConwaysGameOfLife
{
    public class CellsImage: IDisposable
    {
        public CellsImage(bool[,] cells)
        {
            Cells = cells;
        }
        public CellsImage()
        {
            // wtf?
            Def();
        }

        public static int PixToCell
        {
            get { return Config.Conf.PixToCell; }
            set { Config.Conf.PixToCell = value; }
        }

        static public bool[,] GetCells(Bitmap img)
        {
            int w = img.Width / PixToCell;
            int h = img.Height / PixToCell;
            bool[,] cells = new bool[w, h];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    // TODO: конфиг для цвета
                    cells[j, i] = img.GetPixel((j * PixToCell) + 1, (i * PixToCell) + 1) == Config.Conf.AliveColor;
                }
            }

            return cells;
        }
        static public Bitmap GetImg(bool[,] cells)
        {
            int w = cells.GetLength(0);
            int h = cells.GetLength(1);  
          
            Size s = new Size(w,h);
            if (!s.Equals(Config.Conf.worldSize))
                throw new ArgumentException("Размеры массива несоответствуют размерам в файле конфигурации");

            Bitmap img = new Bitmap(w*PixToCell, h*PixToCell);
            using (Graphics g = Graphics.FromImage(img))
            {
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (cells[i, j])
                        {
                            g.FillRectangle(new SolidBrush(Config.Conf.AliveColor), i * PixToCell, j * PixToCell, PixToCell, PixToCell);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(Config.Conf.DeadColor), i * PixToCell, j * PixToCell, PixToCell, PixToCell);
                        }
                    }

                }
            }
            return img;
        }

        bool[,] cells;
        Bitmap img;

        public Bitmap Image
        {
            get { return img; }
            set
            {
                img = value;
                cells = GetCells(img);
            }
        }
        public bool[,] Cells
        {
            get { return cells; }
            set
            {
                img = GetImg(value);
                cells = value;
            }
        }

        public void Def()
        {
            Size s = Config.Conf.worldSize;
            cells = new bool[s.Width, s.Height];
            img = GetImg(cells);
        }

        public void Dispose()
        {
            img.Dispose();
        }
        public bool IsValid
        {
            get
            {
                Size s = new Size(cells.GetLength(0), cells.GetLength(1));
                return s.Equals(Config.Conf.worldSize);
                // Для проверки размеров текущего поля с размерами в конфигурационном файле
                // при изменении настроек из GUI
            }
        }

        // неявное приведение
        static public implicit operator bool[,](CellsImage c)
        {
            return c.cells;
        }
        static public implicit operator Bitmap(CellsImage c)
        {
            return c.img;
        }

        public bool this[int w, int h]
        {
            get { return cells[w, h]; }
            set 
            {
                // нерационально (новое изображение при каждом изменении)
                cells[w, h] = value;
                img = GetImg(cells);
            }
        }

    }
}
