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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameSetting gameSettings = new GameSetting();
        List<GameSetting> levelsList = new List<GameSetting>();

        Tower tower = new Tower();
        List<Tower> towers = new List<Tower>();

        Enemy enemy = new Enemy();
        List<Enemy> enemyList = new List<Enemy>();

        //TILE MAP
        const int cols = 9;
        const int rows = 19;
        int tileSize = 32;
        public int mapWidth = 1176;
        public int mapHeight = 816;

        private TileMap tileMap;
        private Tile myTileSet;

        string tilesetFileName = "";

        ObservableCollection<Image> panelImages = new ObservableCollection<Image>();

        private double zoomMax = 1.5;
        private double zoomMin = 0.2;
        private double zoomSpeed = 0.001;
        private double zoom = 1;

        public MainWindow()
        {
            InitializeComponent();
            //disable window resizing
            //this.MaxWidth = 974;
            this.MinWidth = 974;
            //this.MaxHeight = 850;
            this.MinHeight = 850;

            #region GameSettings

            gameSettings.setWindow(this);
            //gameSettings = new GameSetting("", "", "", 0, 0);

            gameSettings.loadGameSettings(@"ConfigFiles\gameConfig.xml", levelsList);

            //show the different level in a list box
            for (int i = 0; i < levelsList.Count; i++)
            {
                lb_Levels.Items.Add(levelsList[i].getName());
            }
            //add the combo box difficulty setting options
            cb_Difficulty.Items.Add("Easy");
            cb_Difficulty.Items.Add("Medium");
            cb_Difficulty.Items.Add("Hard");
            //sets the combo box to display the defualt value on load
            if (cb_Difficulty.Items.CurrentItem == null)
            {
                cb_Difficulty.SelectedItem = levelsList[0].getDifficulty();
            }

            //add the combo box gamespeed setting options
            cb_gameSpeed.Items.Add("Slow");
            cb_gameSpeed.Items.Add("Fast");
            cb_gameSpeed.Items.Add("Fastest");
            if (cb_gameSpeed.Items.CurrentItem == null)
            {
                cb_gameSpeed.SelectedItem = levelsList[0].getGameSpeed();
            }

            //set the label of Wave Limit to the defualt setting
            lbl_WaveLimit.Content = levelsList[0].getWaveLimit();

            //set the label of starting gold to the defualt setting
            lbl_setGold.Content = levelsList[0].getStartGold();
            #endregion

            #region Towers
            //Tower Settings
            tower.loadTowersSettings(@"ConfigFiles\towersConfig.xml", towers);

            //show the different towers in a list box
            for (int i = 0; i < towers.Count; i++)
            {
                lb_Towers.Items.Add(towers[i].getName());
            }

            #endregion

            #region Enemys

            enemy.loadEnemySettings(@"ConfigFiles\enemyConfig.xml", enemyList);

            //add the enemys to the list box
            for (int i = 0; i < enemyList.Count; i++)
            {
                lb_Enemys.Items.Add(enemyList[i].getType());
            }

            #endregion

            #region Tiles
            //load the tile set
            #endregion
        }


        private void setTileMap()
        {
            try
            {
                NewDialogBox newWindow = new NewDialogBox();
                newWindow.ShowDialog();

                tilesetFileName = newWindow.getTileset();
                tileSize = newWindow.getTileSizeValue();

                myTileSet = new Tile(this, new BitmapImage(new Uri(@tilesetFileName, UriKind.Relative)));
                myTileSet.createTileList();

                mapWidth = newWindow.getWidthValue();
                mapHeight = newWindow.getHeightValue();

                //draw the tile grid on the canvas
                drawGrid(mapWidth - tileSize, mapHeight - tileSize, tileSize);
                //tileMap.checkForInUse();
                tileMap_Canvas.Width = mapWidth;
                tileMap_Canvas.Height = mapHeight;

                tileMap = new TileMap(this, mapWidth, mapHeight, tileSize);
            }
            catch
            {
                //return null;
            }
        }


        #region Toolbar
        //TODO add pop up boxes for load and save locations
        private void btn_Write_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "customMap"; //defualt filename
            dlg.DefaultExt = ".txt"; //defualt file extension
            //dlg.Filter = "*.txt";

            //show the dialog box
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                //save the map
                filename = dlg.FileName;
                tileMap.save(filename);

            }

        }

        private void btn_Read_Click(object sender, RoutedEventArgs e)
        {
            if (tileMap != null)
            {
                //before loading check to see if the canvas is already in use
                if (tileMap.checkForInUse() == false)
                {
                    string filename = "";

                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.FileName = "";
                    dlg.DefaultExt = "txt"; //defualt extension
                    dlg.Filter = "(*.txt)|*.txt"; //filters by extention

                    //show the dialog box
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        //open the document
                        filename = dlg.FileName;

                        if (!String.Equals(filename, String.Empty))
                        {
                            tileMap.load(filename);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please Save and Close the Current File");
                }
            }
            else
            {
                MessageBox.Show("There is no Tile map Canvas");
            }
        }


        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            //clear tile map data;
            tileMap.clearTileMap();
        }

        private void btn_New_Click(object sender, RoutedEventArgs e)
        {
            //clear tile map data;
            //tileMap.clearTileMap();

            //Open Dialog box with Settings
            setTileMap();
        }
        #endregion

        #region Sliders/Combo Boxes
        private void cb_Difficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < levelsList.Count; i++)
            {
                if (lb_Levels.SelectedItem == levelsList[i].getName())
                {
                    levelsList[i].setDifficulty(cb_Difficulty.SelectedItem.ToString());
                }
            }
        }

        private void cb_gameSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < levelsList.Count; i++)
            {
                if (lb_Levels.SelectedItem == levelsList[i].getName())
                {
                    levelsList[i].setGameSpeed(cb_gameSpeed.SelectedItem.ToString());
                }
            }
        }

        private void slder_WaveLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbl_WaveLimit.Content = Convert.ToInt32(slder_WaveLimit.Value);
            //set the value
            for (int i = 0; i < levelsList.Count; i++)
            {
                if (lb_Levels.SelectedItem == levelsList[i].getName())
                {
                    levelsList[i].setWaveLimit(Convert.ToInt32(slder_WaveLimit.Value));
                }
            }
        }

        private void slder_setGold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbl_setGold.Content = Convert.ToInt32(slder_setGold.Value);
            //set the value
            for (int i = 0; i < levelsList.Count; i++)
            {
                if (lb_Levels.SelectedItem == levelsList[i].getName())
                {
                    levelsList[i].setStartGold(Convert.ToInt32(slder_setGold.Value));
                }
            }
        }

        private void slder_Dmg_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ts_lbl_setDmg.Content = Convert.ToInt32(slder_Dmg.Value);
            //set the value
            for (int i = 0; i < towers.Count; i++)
            {
                if (lb_Towers.SelectedItem == towers[i].getName())
                {
                    towers[i].setDamage(Convert.ToInt32(slder_Dmg.Value));
                }
            }

        }

        private void slder_Range_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ts_lbl_setRange.Content = Convert.ToInt32(slder_Range.Value);
            //set the value
            for (int i = 0; i < towers.Count; i++)
            {
                if (lb_Towers.SelectedItem == towers[i].getName())
                {
                    towers[i].setRange(Convert.ToInt32(slder_Range.Value));
                }
            }
        }

        private void slder_fireRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ts_lbl_setFireRate.Content = Convert.ToDouble(slder_fireRate.Value);
            //set the value
            for (int i = 0; i < towers.Count; i++)
            {
                if (lb_Towers.SelectedItem == towers[i].getName())
                {
                    //clamp the double value to 2 decimal places
                    towers[i].setFireRate(Math.Round(Convert.ToDouble(slder_fireRate.Value), 2));
                }
            }
        }

        private void slder_Cost_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ts_lbl_cost.Content = Convert.ToInt32(slder_Cost.Value);

            for (int i = 0; i < towers.Count; i++)
            {
                if (lb_Towers.SelectedItem == towers[i].getName())
                {
                    //clamp the double value to 2 decimal places
                    towers[i].setCost(Convert.ToInt32(slder_Cost.Value));
                }
            }
        }

        private void slder_Health_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbl_setHealth.Content = Convert.ToInt32(slder_Health.Value);
            //set the value
            for (int i = 0; i < enemyList.Count; i++)
            {
                if ((string)lb_Enemys.SelectedItem == enemyList[i].getType())
                {
                    enemyList[i].setHealth(Convert.ToInt32(slder_Health.Value));
                }
            }
        }

        private void slder_Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lbl_setSpeed.Content = Convert.ToDouble(slder_Speed.Value);
            //set the value
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (lb_Enemys.SelectedItem == enemyList[i].getType())
                {
                    //clamp the double value to 2 decimal places
                    enemyList[i].setSpeed(Math.Round(Convert.ToDouble(slder_Speed.Value), 2));
                }
            }
        }
        #endregion

        #region Load/Save BTNs Gamesettings/Towers/Enemys
        private void gameSettingSave_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "customGameSettings.xml"; //defualt filename
            dlg.DefaultExt = ".xml"; //defualt file extension
            //dlg.Filter = "|*.xml";

            //show the dialog box
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                //save the map
                filename = dlg.FileName;
                gameSettings.saveGameSettings(filename, levelsList);

            }

        }

        private void gameSettingLoad_Click(object sender, RoutedEventArgs e)
        {
            //WORKAROUND for bug where once new settings were loaded the setting properties(combobox, labels) 
            //wouldnt update to the new loaded values
            //clear the items from the list box and re-add them after the load is complete
            lb_Levels.Items.Clear();

            string filename = "";

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "";
            dlg.DefaultExt = "xml"; //defualt extension
            dlg.Filter = "(*.xml)|*.xml"; //filters by extention

            //show the dialog box
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                //open the document
                filename = dlg.FileName;

                if (!String.Equals(filename, String.Empty))
                {
                    gameSettings.loadGameSettings(filename, levelsList);
                }
            }
            //WORKAROUND for bug where once new settings were loaded the setting properties(combobox, labels) 
            //wouldnt update to the new loaded values
            for (int i = 0; i < levelsList.Count; i++)
            {
                lb_Levels.Items.Add(levelsList[i].getName());
            }
        }
        private void towerSave_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "customTowerSettings.xml"; //defualt filename
            dlg.DefaultExt = ".xml"; //defualt file extension
            //dlg.Filter = "|*.xml";

            //show the dialog box
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                //save the map
                filename = dlg.FileName;
                tower.saveTowerSettings(towers, filename);

            }
            
        }
        private void towerLoad_Click(object sender, RoutedEventArgs e)
        {
            //WORKAROUND for bug where once new settings were loaded the setting properties(combobox, labels) 
            //wouldnt update to the new loaded values
            //clear the items from the list box and re-add them after the load is complete
            lb_Towers.Items.Clear();

            string filename = "";

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "";
            dlg.DefaultExt = "xml"; //defualt extension
            dlg.Filter = "(*.xml)|*.xml"; //filters by extention

            //show the dialog box
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                //open the document
                filename = dlg.FileName;

                if (!String.Equals(filename, String.Empty))
                {
                    tower.loadTowersSettings(filename, towers);
                }
            }
            //WORKAROUND for bug where once new settings were loaded the setting properties(combobox, labels) 
            //wouldnt update to the new loaded values
            for (int i = 0; i < towers.Count; i++)
            {
                lb_Towers.Items.Add(towers[i].getName());
            }
        }

        private void enemySave_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "customEnemySettings.xml"; //defualt filename
            dlg.DefaultExt = ".xml"; //defualt file extension
            //dlg.Filter = "|*.xml";

            //show the dialog box
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                //save the map
                filename = dlg.FileName;
                enemy.saveEnemySettings(enemyList, filename);

            }
           
        }

        private void enemyLoad_Click(object sender, RoutedEventArgs e)
        {
            //WORKAROUND for bug where once new settings were loaded the setting properties(combobox, labels) 
            //wouldnt update to the new loaded values
            //clear the items from the list box and re-add them after the load is complete
            lb_Enemys.Items.Clear();

            string filename = "";

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "";
            dlg.DefaultExt = "xml"; //defualt extension
            dlg.Filter = "(*.xml)|*.xml"; //filters by extention

            //show the dialog box
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                //open the document
                filename = dlg.FileName;

                if (!String.Equals(filename, String.Empty))
                {
                    enemy.loadEnemySettings(filename, enemyList);
                }
            }
            //WORKAROUND for bug where once new settings were loaded the setting properties(combobox, labels) 
            //wouldnt update to the new loaded values
            for (int i = 0; i < enemyList.Count; i++)
            {
                lb_Enemys.Items.Add(enemyList[i].getType());
            }
        }

        private void lb_Towers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < towers.Count; i++)
                if (lb_Towers.SelectedItem == towers[i].getName())
                {
                    //display the current values of the tower setting
                    ts_NameTXT.Content = towers[i].getName();
                    ts_lbl_setDmg.Content = towers[i].getDamage();
                    ts_lbl_setRange.Content = towers[i].getRange();
                    ts_lbl_setFireRate.Content = towers[i].getFireRate();
                    ts_lbl_cost.Content = towers[i].getCost().ToString();

                }
        }

        private void lb_Enemy_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (lb_Enemys.SelectedItem == enemyList[i].getType())
                {
                    lbl_setHealth.Content = enemyList[i].getHealth();
                    lbl_setSpeed.Content = enemyList[i].getSpeed();
                }
            }
        }

        private void lb_Levels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < levelsList.Count; i++)
            {
                if (lb_Levels.SelectedItem == levelsList[i].getName())
                {
                    cb_Difficulty.SelectedItem = levelsList[i].getDifficulty();
                    cb_gameSpeed.SelectedItem = levelsList[i].getGameSpeed();
                    lbl_WaveLimit.Content = levelsList[i].getWaveLimit();
                    lbl_setGold.Content = levelsList[i].getStartGold();
                }
            }
        }

        #endregion

        #region tilesets

        //load tile set and add each tile into a listbox
        public void loadTiles(List<CroppedBitmap> tileSets)
        {
            //look through each tile in the list adding each to the list view individually
            tileSet_listView.Items.Clear();
            foreach (var image in tileSets)
            {
                tileSet_listView.Items.Add(new Image() { Source = image, Height = tileSize });
            }

        }

        #endregion

        #region TileMap

        //draw a grid on the canvas
        public void drawGrid(int width, int height, int tileSize)
        {
            tileMap_Canvas.Children.Clear();
            tileMap_Canvas.Children.Add(new Rectangle() { Width = width, Height = height, Stroke = Brushes.LightGray, Fill = Brushes.Azure });
            for (int y = 0; y < height; y += tileSize)
            {
                for (int x = 0; x < width; x += tileSize)
                {
                    tileMap_Canvas.Children.Add(new Rectangle()
                    {
                        Width = tileSize,
                        Height = tileSize,
                        Margin = new Thickness(x, y, 0, 0),
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1.5
                    });
                }
            }
        }

        //add in the all the required tiles to the canvas that are loaded form the file
        public void loadTilesToCanvas(int[,] map, int width, int height, int tileSize)
        {

            //Reset the canvas to the new values so the scrollview is correct
            tileMap_Canvas.Width = width * tileSize;
            tileMap_Canvas.Height = height * tileSize;

            //re-draw the grid
            drawGrid(width * tileSize, height * tileSize, tileSize);

            for (int col = 0; col < height; col++)
            {
                for (int row = 0; row < width; row++)
                {
                    int idx = map[row, col];
                    if (idx != -1 && idx < tileSet_listView.Items.Count)
                    {
                        Image img = (Image)tileSet_listView.Items[idx];
                        Image toDraw = new Image();
                        toDraw.Source = img.Source;
                        toDraw.Width = tileSize;
                        toDraw.Height = tileSize;
                        toDraw.Margin = new Thickness(row * tileSize, col * tileSize, 0, 0);
                        tileMap_Canvas.Children.Add(toDraw);
                    }
                }
            }
        }

        //add a tile to the canvas in the position the player clicks
        public void addTileToCanvas(int x, int y, int tileSize, int index)
        {
            if (index != -1)
            {
                Image i = (Image)tileSet_listView.Items[index];
                Image toDraw = new Image();
                toDraw.Source = i.Source;
                toDraw.Width = tileSize;
                toDraw.Height = tileSize;
                toDraw.Margin = new Thickness(x * tileSize, y * tileSize, 0, 0);
                tileMap_Canvas.Children.Add(toDraw);
            }
            else
            {
                tileMap_Canvas.Children.Add(new Rectangle()
                {
                    Width = tileSize,
                    Height = tileSize,
                    Margin = new Thickness(x * tileSize, y * tileSize, 0, 0),
                    Stroke = Brushes.Gray,
                    Fill = Brushes.LightGray,
                    StrokeThickness = 1
                }
                                            );
            }
        }

        #endregion

        #region Mouse Input

        //set the tile to canvas at the position the mouse is clicked.
        private void tileMap_Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                tileMap.setTile(e.GetPosition(tileMap_Canvas), tileSet_listView.SelectedIndex);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                tileMap.deleteTile(e.GetPosition(tileMap_Canvas));
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += zoomSpeed * e.Delta;

            if (zoom < zoomMin)
            {
                zoom = zoomMin;
            }
            if (zoom > zoomMax)
            {
                zoom = zoomMax;
            }

            Point mousePos = e.GetPosition(tileMap_Canvas);

            if (zoom > 1)
            {
                ScaleTransform scale = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y);
                tileMap_Canvas.LayoutTransform = scale;
            }
            else
            {
                ScaleTransform scale = new ScaleTransform(zoom, zoom);
                tileMap_Canvas.LayoutTransform = scale;
            }
            e.Handled = true;
        }
        #endregion
       

    }
}
