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
    public partial class MetaExtractor : Form
    {
            
        string destination;
        bool extract=false;


        public MetaExtractor()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                destination = fbd.SelectedPath;
                textBox1.Text = destination;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox1.Text.Length > 0)
                extract = true;
            else MessageBox.Show("At least Select the Directory", "Error");
        }

        #region GET_FUNCS


        public bool GET_RECURSIVE()
        {
            return radioButton1.Checked;
        }
        public bool GET_EXTRACT()
        {
            return extract;
        }
        public string GET_DIRECTORY()
        {
            return destination;
        }
        #endregion
    }
}
