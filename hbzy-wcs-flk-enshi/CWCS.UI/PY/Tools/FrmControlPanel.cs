using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace CWCS.UI
{
    public partial class FrmControlPanel : Form
    {
        public FrmControlPanel()
        {
            InitializeComponent();
        }

        private void FrmControlPanel_Load(object sender, EventArgs e)
        {
            checkSC01.Checked = false;
            checkSC02.Checked = false;
            checkSC03.Checked = false;
            checkSC04.Checked = false;
            checkSC05.Checked = false;
            checkSC06.Checked = false;
            button1.Click += SaveButton_Click;
            button2.Click += SaveButton_Click;
            button3.Click += SaveButton_Click;
            button4.Click += SaveButton_Click;
            string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            //string strFileName = AppDomain.CurrentDomain.BaseDirectory + @"WCSConfiguration.xml";
            XmlDocument doc = new XmlDocument();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(strFileName, settings);
          
            doc.Load(reader);
            XmlNodeList nodeList = doc.GetElementsByTagName("appSettings")[0].ChildNodes;
            foreach (XmlElement element in nodeList)
            {
                //if (element.Attributes["key"].Value == "MoistrueCheck"  && element.Attributes["value"].Value.Equals("1"))
                //{
                //    checkMoisture.Checked = true;
                //}
                if (element.Attributes["key"].Value.Contains("SC") && element.Attributes["value"].Value.Equals("1"))
                {
                    if (element.Attributes["key"].Value.Equals("SC01")) checkSC01.Checked = true;
                    if (element.Attributes["key"].Value.Equals("SC02")) checkSC02.Checked = true;
                    if (element.Attributes["key"].Value.Equals("SC03")) checkSC03.Checked = true;
                    if (element.Attributes["key"].Value.Equals("SC04")) checkSC04.Checked = true;
                    if (element.Attributes["key"].Value.Equals("SC05")) checkSC05.Checked = true;
                    if (element.Attributes["key"].Value.Equals("SC06")) checkSC06.Checked = true;
                }

            }
            reader.Close();
        }

       

        public void SaveButton_Click(object sender, EventArgs e)
        {
            string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            //string strFileName = AppDomain.CurrentDomain.BaseDirectory + @"WCSConfiguration.xml";
            FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read,FileShare.ReadWrite);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(fs, settings);
            XDocument doc = XDocument.Load(reader);
            fs.Close();

            var elements = doc.Root.Element("appSettings").Elements("add");
            string valueToWrite = string.Empty;

            var c = ((Button)sender).Parent.Controls;
            foreach (Control checkbox in c)
            {
                if (checkbox is CheckBox)
                {
                    //if (checkbox.Name.Equals("checkBoxWriteLotNumber3"))
                    //{
                    //    valueToWrite = checkBoxWriteLotNumber3.Checked ? "1" : "0";
                    //    var item = elements.Where(a => a.Attribute("key").Value.Equals("WriteLotNumberCheck3")).SingleOrDefault();
                    //    item.SetAttributeValue("value", valueToWrite);
                    //}
                }
                if (checkbox is TextBox)
                {
                    //if(checkbox.Name.Equals("textBoxLotNumber"))
                    //{
                    //    var item = elements.Where(a => a.Attribute("key").Value.Equals("LotNumber")).SingleOrDefault();
                    //    item.SetAttributeValue("value", textBoxLotNumber.Text.Trim());
                    //}
                }

                if (checkbox is ComboBox)
                {
                    //if (checkbox.Name.Equals("comboBoxIsFlake"))
                    //{
                    //    var item = elements.Where(a => a.Attribute("key").Value.Equals("IsFlake")).SingleOrDefault();
                    //    item.SetAttributeValue("value", comboBoxIsFlake.Text.Trim()== "是"?"1":"0");
                    //}
                }

            }
            try
            {
                doc.Save(strFileName);
                MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show("保存失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                reader.Close();
            }
                
            


        }

    }
}
