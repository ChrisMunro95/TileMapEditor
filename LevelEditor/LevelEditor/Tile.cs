using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using System.IO;

namespace LevelEditor
{
    class Tile
    {
        private MainWindow window;

        private BitmapSource tileSet;
        private List<CroppedBitmap> tiles = new List<CroppedBitmap>();
        private int tileSize = 32;

        public Tile(MainWindow window, BitmapSource sourceTileSet)
        {
            //set the window so its not null
            this.window = window;
            this.tileSet = sourceTileSet;
        }

        public void createTileList()
        {
            //clear the list just in case
            tiles.Clear();

            //loop through the original tileSet img, adding each tile to the tile List
            for (int y = 0; y < tileSet.PixelHeight; y += tileSize)
            {
                for (int x = 0; x < tileSet.PixelWidth; x += tileSize)
                {
                    try
                    {
                        CroppedBitmap img = new CroppedBitmap(tileSet, new Int32Rect(x, y, tileSize, tileSize));
                        tiles.Add(img);
                    }
                    catch
                    {

                    }
                }
            }

            window.loadTiles(tiles);

        }

    }
}
