using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Map_Handler
{
    public partial class Resyncer_Dialog_Box : Form
    {
        public Resyncer_Dialog_Box()
        {
            InitializeComponent();

            comboBox1.Items.Add("stem");
            comboBox1.Items.Add("bitm");
            comboBox1.Items.Add("snd!");
            comboBox1.Items.Add("effe");

            comboBox1.Text = "stem";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            The purpose of this following Resync function is to reconfigure the stem tagRefs to match with the tags of the new map.             
            */

            //File dailogue to select the config file .shad.xml
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "config files(*.xml)|*.xml";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Resyncer Rs = new Resyncer(ofd.FileName, comboBox1.Text);
            }
            this.Close();
        }
    }
}
