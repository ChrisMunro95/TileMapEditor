using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace LevelEditor
{
    class Enemy
    {

        private string type_;
        private int health_;
        private double speed_;

        public Enemy(){}

        public Enemy(string type, int health, double speed)
        {
            this.type_ = type;
            this.health_ = health;
            this.speed_ = Math.Round(speed, 2);
        }

        public void loadEnemySettings(string filename, List<Enemy> EnemyList)
        {
            XDocument doc = new XDocument();

            try
            {
                doc = XDocument.Load(filename);
                EnemyList.Clear();
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
                    EnemyList.Add(new Enemy(node.Attribute("Type").Value.ToString(),
                                            Convert.ToInt32(node.Attribute("Health").Value),
                                            float.Parse(node.Attribute("Speed").Value)
                                            )
                                  );
                }
                catch
                {
                    System.Windows.MessageBox.Show("Invalid File, Please make sure you are loading an Enemy Settings File");
                    //reload defualt settings
                    loadEnemySettings(@"ConfigFiles\enemyConfig.xml", EnemyList);
                    return;
                }
            }
        }

        public void saveEnemySettings(List<Enemy> EnemyList, string filename)
        {
            XDocument saveDoc = new XDocument();

            XElement rootNode = new XElement("EnemyCreeps");

            XElement[] subNode = new XElement[EnemyList.Count];

            for (int i = 0; i < EnemyList.Count; i++)
            {
                //create a subnode
                subNode[i] = new XElement("Creep");
                //set the attribute values to the subnode
                subNode[i].SetAttributeValue("Type", EnemyList[i].getType());
                subNode[i].SetAttributeValue("Health", EnemyList[i].getHealth());
                subNode[i].SetAttributeValue("Speed", EnemyList[i].getSpeed());

                //add the subnode to the root node
                rootNode.Add(subNode[i]);
            }

            saveDoc.Add(rootNode);
            saveDoc.Save(filename);
            //@"..\..\ConfigFiles\custom_enemyConfig.xml"
        }

        #region Getters and Setter
        public string getType()
        {
            return type_;
        }

        public int getHealth()
        {
            return health_;
        }
        public void setHealth(int value)
        {
            health_ = value;
        }

        public double getSpeed()
        {
            return speed_;
        }
        public void setSpeed(double value)
        {
            //clamp the double value to 2 decimal places
            speed_ = Math.Round(value, 2);
        }

        #endregion
    }
}
