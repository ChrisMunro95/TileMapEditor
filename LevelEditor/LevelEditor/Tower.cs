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
    class Tower
    {
        private string name_;
        private float damage_;
        private float range_;
        private double fireRate_;
        private int cost_;

        public Tower()
        {

        }

        public Tower(string name, float damage, float range, double fireRate, int cost)
        {
            this.name_ = name;
            this.damage_ = damage;
            this.range_ = range;
            this.fireRate_ = fireRate;
            this.cost_ = cost;
        }

        //load tower settings in a list of towers
        public void loadTowersSettings(string filename, List<Tower> TowersList)
        {      
            XDocument doc = new XDocument();

            try
            {
                doc = XDocument.Load(filename);
                TowersList.Clear();
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
                    TowersList.Add(new Tower(node.Attribute("Name").Value.ToString(),
                                             Convert.ToInt32(node.Attribute("Damage").Value),
                                             Convert.ToInt32(node.Attribute("Range").Value),
                                             Convert.ToDouble(node.Attribute("FireRate").Value),
                                             Convert.ToInt32(node.Attribute("Cost").Value)
                                             )
                                  );
                }
                catch
                {
                    System.Windows.MessageBox.Show("Invalid File, Please make sure you are loading a Tower Settings File");
                    //reload defualt settings
                    loadTowersSettings(@"ConfigFiles\towersConfig.xml", TowersList);
                    return;
                }
            }
        }

        //save tower setting into a custom xml file
        public void saveTowerSettings(List<Tower> TowersList, string filename)
        {
            XDocument saveDoc = new XDocument();

            XElement rootNode = new XElement("Towers");

            XElement[] subNode = new XElement[TowersList.Count];

            for (int i = 0; i < TowersList.Count; i++)
            {
                //create a subnode
                subNode[i] = new XElement("Tower");
                //set the attribute values to the subnode
                subNode[i].SetAttributeValue("Name", TowersList[i].getName());
                subNode[i].SetAttributeValue("Damage", TowersList[i].getDamage());
                subNode[i].SetAttributeValue("Range", TowersList[i].getRange());
                subNode[i].SetAttributeValue("FireRate", TowersList[i].getFireRate());
                subNode[i].SetAttributeValue("Cost", TowersList[i].getCost());

                //add the subnode to the root node
                rootNode.Add(subNode[i]);
            }

            saveDoc.Add(rootNode);
            saveDoc.Save(filename);

        }

#region Getters and setters
        public string getName()
        {
            return name_;
        }
        public void setName(string value)
        {
            name_ = value;
        }

        public float getDamage()
        {
            return damage_;
        }
        public void setDamage(float value)
        {
            damage_ = value;
        }
        public float getRange()
        {
            return range_;
        }
        public void setRange(float value)
        {
            range_ = value;
        }
        public double getFireRate()
        {
            return fireRate_;
        }
        public void setFireRate(double value)
        {
            fireRate_ = value;
        }
        public int getCost()
        {
            return cost_;
        }
        public void setCost(int value)
        {
            cost_ = value;
        }
#endregion

    }
}
