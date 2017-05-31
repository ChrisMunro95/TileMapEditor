using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for NewDialogBox.xaml
    /// </summary>
    public partial class NewDialogBox : Window
    {
        private int width_;
        private int height_;
        string filename = "";

        private int tileSize_;

        public NewDialogBox()
        {
            InitializeComponent();
        }

        public int getWidthValue()
        {
            string txt = WidthInput.Text;
            width_ = int.Parse(txt);
            return width_;
        }
        public int getHeightValue()
        {
            height_ = int.Parse(HeightInput.Text);
            return height_;
        }

        public int getTileSizeValue()
        {
            tileSize_ = int.Parse(TileSize_Input.Text);
            return tileSize_;
        }

        public string getTileset()
        {
            return filename;
        }
        private void OkBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BrowseBTN_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = dlg.ShowDialog();
            //dlg.FileName = "";
            dlg.DefaultExt = "png"; //defualt extension
            dlg.Filter = "(*.png)|*.png"; //filters by extention

            if (result == true)
            {
                //open the document
                filename = dlg.FileName;
            }

        }
    }
}
