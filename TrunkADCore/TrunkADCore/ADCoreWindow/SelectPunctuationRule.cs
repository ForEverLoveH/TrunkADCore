using HZH_Controls;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkADCore.ADCoreSystem.ADCoreSys;

namespace TrunkADCore.ADCoreWindow
{
    public partial class SelectPunctuationRule : Form
    {
        public SelectPunctuationRule()
        {
            InitializeComponent();
        }
        

        private void SelectPunctuationRule_Load(object sender, EventArgs e)
        {
            uiComboBox2.Items.Clear();
            for (int i = 0; i < 100; i += 10)
            {
                uiComboBox2.Items.Add((i) + "");
            }
            for (int i = 100; i <= 1000; i += 100)
            {

                uiComboBox2.Items.Add(i + "");
            }
            uiComboBox3.Items.Clear();
            for (int i = 5; i <= 100; i += 5)
            {
                uiComboBox3.Items.Add(i + "");
            }
            for (int i = 200; i <= 1000; i += 100)
            {
                uiComboBox3.Items.Add(i + "");
            }
        }
        public int colum = 0;
        public  int initDis = 0;
        public  int distance = 0;

        private void uiButton1_Click(object sender, EventArgs e)
        {
            uiLabel4.Text = "";
            if (uiComboBox1.Text.Trim() == "" ||uiComboBox2.Text.Trim() == "" || uiComboBox3.Text.Trim() == "")
            {
                uiLabel4.Text = "请检测参数选择";
            }
            else
            {
                int.TryParse(uiComboBox1.Text, out colum);
                int.TryParse(uiComboBox2.Text, out initDis);
                int.TryParse(uiComboBox3.Text, out distance);
                DialogResult= DialogResult.OK;
            }
              
            
        }

        
    }
    
}
