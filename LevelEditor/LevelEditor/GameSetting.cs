using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LevelEditor
{
    class GameSetting
    {
        private string name;
        private string difficulty;
        private string gameSpeed;
        private int waveLimit;
        private int startGold;
        private MainWindow window;

        public GameSetting() {}

        public GameSetting(string name, string difficulty, string gamespeed, int wavelimit, int startgold)
        {
            this.name = name;
            this.difficulty = difficulty;
            this.gameSpeed = gamespeed;
            this.waveLimit = wavelimit;
            this.startGold = startgold;
        }

        public void loadGameSettings(string filename, List<GameSetting> levelsList)
        {
  
           XDocument doc = new XDocument();
           
            try
            {
                doc = XDocument.Load(filename);
                levelsList.Clear();
            }
            catch (FileNotFoundException e)
            {
                System.Windows.MessageBox.Show("File failed to Load");
            }

            var rootNode = doc.Root;

            foreach (var node in rootNode.Elements())
            {
                try
                {
                    levelsList.Add(new GameSetting(node.Attribute("Name").Value.ToString(),
                                               node.Attribute("Difficulty").Value.ToString(),
                                               node.Attribute("GameSpeed").Value.ToString(),
                                               Convert.ToInt32(node.Attribute("WaveLimit").Value),
                                               Convert.ToInt32(node.Attribute("StartingGold").Value)
                                               )
                              );
                }
                catch
                {
                    System.Windows.MessageBox.Show("Invalid File, Please make sure you are loading a Game Settings File");
                    //reload defualt settings
                    loadGameSettings(@"ConfigFiles\gameConfig.xml", levelsList);
                    return;
                }
            }
            
        }

        public void saveGameSettings(string filename, List<GameSetting> levelList)
        {

            #region CreatXML
            XDocument saveDoc = new XDocument();

            XElement rootNode = new XElement("GameSettings");

            XElement[] subnode = new XElement[levelList.Count];

            for (int i = 0; i < levelList.Count; i++)
            {   
                //create the subnode
                subnode[i] = new XElement("Level");
                //set the attribute values to the subnode
                subnode[i].SetAttributeValue("Name", levelList[i].getName());
                subnode[i].SetAttributeValue("Difficulty", levelList[i].getDifficulty());
                subnode[i].SetAttributeValue("GameSpeed", levelList[i].getGameSpeed());
                subnode[i].SetAttributeValue("WaveLimit", levelList[i].getWaveLimit());
                subnode[i].SetAttributeValue("StartingGold", levelList[i].getStartGold());

                rootNode.Add(subnode[i]);
            }

            saveDoc.Add(rootNode);

            saveDoc.Save(filename);
            #endregion
        }

        #region Getters and Setters
        public void setWindow(MainWindow window)
        {
            this.window = window; 
        }

        public void setName(string value)
        {
            name = value;
        }
        //get Difficulty
        public string getName()
        {
            return name;
        }

        //set Difficulty
        public void setDifficulty(string value)
        {
            difficulty = value;
        }
        //get Difficulty
        public string getDifficulty()
        {
            return difficulty;
        }
        //set gameSpeed
        public void setGameSpeed(string value)
        {
            gameSpeed = value;
        }
        //get gameSpeed
        public string getGameSpeed()
        {
            return gameSpeed;
        }
        //set WaveLimit
        public void setWaveLimit(int value)
        {
            waveLimit = value;
        }
        //get waveLimit
        public int getWaveLimit()
        {
            return waveLimit;
        }
        //set start gold amount
        public void setStartGold(int value)
        {
            startGold = value;
        }
        //get waveLimit
        public int getStartGold()
        {
            return startGold;
        }
        #endregion
    }
}
