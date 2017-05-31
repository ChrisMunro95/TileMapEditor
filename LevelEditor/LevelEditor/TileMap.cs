using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LevelEditor
{
    class TileMap
    {
        private MainWindow window;

        int row, col, tileSize;
        int[,] tileData;

        private int width_, height_;

        public bool inUse;

        public TileMap(MainWindow window, int width, int height, int tileSize)
        {
            this.window = window;
            this.tileSize = tileSize;
            this.width_ = width;
            this.height_ = height;

            row = width / tileSize;
            col = height / tileSize;
            tileData = new int[row, col];

            //set all the intial values to -1 so there is no confliction with the tile idx later on.
            for (int y = 0; y < col; y++)
            {
                for (int x = 0; x < row; x++)
                {
                    tileData[x, y] = -1;
                }
            }

            inUse = false;
        }

        //set the tile within the array at the point where the mosue is clicked
        public void setTile(Point point, int p)
        {
            int x = (int)point.X / tileSize;
            int y = (int)point.Y / tileSize;
            try
            {
                if (x >= -1 && x < width_ && y >= -1 && y < height_ && tileData[x, y] != p)
                {
                    tileData[x, y] = p;

                    window.addTileToCanvas(x, y, tileSize, p);
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Sorry, Something went wrong :(");
                return;
            }
        }

        //get a certain tile from the array
        public int getTile(int x, int y)
        {
            return tileData[x, y];
        }

        public void deleteTile(Point point)
        {
            int x = (int)point.X / tileSize;
            int y = (int)point.Y / tileSize;

            try
            {
                if (x >= -1 && x < width_ && y >= -1 && y < height_ && tileData[x, y] != -1)
                {
                    tileData[x, y] = -1;

                    window.addTileToCanvas(x, y, tileSize, -1);
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Sorry, Something went wrong :(");
                return;
            }
        }

        public void clearTileMap()
        {
            for (int y = 0; y < col; y++)
            {
                for (int x = 0; x < row; x++)
                {
                    tileData[x, y] = -1;
                }
            }

            //redraw the map
            window.tileMap_Canvas.Children.Clear();
            window.drawGrid(width_, height_, tileSize);
        }

        public bool checkForInUse()
        {
            //loop through the tile data checking if any tile have been added to the map;
            for (int y = 0; y < col; y++)
            {
                for (int x = 0; x < row; x++)
                {
                    if (tileData[x, y] != -1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void save(string filename)
        {
            TextWriter writer = null;

            writer = File.CreateText(filename);

            //loop through the array saving each number split by a ','
            for (int y = 0; y < col; y++)
            {
                for (int x = 0; x < row; x++)
                {
                    writer.Write(getTile(x, y) + ", ");
                }
                writer.Write(writer.NewLine);
            }

            writer.Dispose();
        }

        public void load(string filename)
        {

            string[] lines = File.ReadAllLines(filename);

            int width = lines[0].Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Length;
            int height = lines.Length;

            tileData = new int[width, height];
            int x = 0;
            int y = 0;

            foreach (var line in lines)
            {
                try
                {
                    x = 0;
                    foreach (var num in line.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        //convert the number from a string to an int
                        tileData[x, y] = Convert.ToInt32(num);
                        x++;
                    }
                    y++;
                }
                catch
                {
                    System.Windows.MessageBox.Show("Invalid File, Please make sure you are loading a map file");
                    return;
                }
            }
            //add the loaded map to the canvas
            window.loadTilesToCanvas(tileData, width, height, tileSize);

        }

    }


}
