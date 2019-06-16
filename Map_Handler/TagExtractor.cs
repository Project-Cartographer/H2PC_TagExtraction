using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlamLib.Test;

namespace Map_Handler
{
    public partial class TagExtractor : Form
    {
        static string DestinationFolder = "";
        static bool isRecursive ;
        static bool isOverrideOn ;
        static bool isOutDBOn ;
        static int TotalTags;

        Dictionary<int, string> TagsList = new Dictionary<int, string>();
      
        bool isDecompileMap_ = false;
        public TagExtractor()
        {
            InitializeComponent();
        }

        public TagExtractor(Dictionary<int,string> Extraction_list,bool DecompileMap)
        {

            InitializeComponent();

            isDecompileMap_ = DecompileMap;
            TotalTags = MainBox.AllTagslist.Count;


            if (!DecompileMap)
            {
                TagsList = Extraction_list;

                Initialize_tags_que();

            }
            else
            {
                Text = "Decompile Map";
                extract_button.Text = "Decompile";
                recursive_radio_.Enabled = false;

                TagsList = Extraction_list;

                Initialize_tags_que();

            }
        }
        void Initialize_tags_que()
        {
            if (TagsList == null)
                return; //EROROORR

            
            for (int o = 0; o < TagsList.Count; o++)
                richTextBox1.AppendText("[" + TagsList.ElementAt(o).Key.ToString("X") + "] " + TagsList.ElementAt(o).Value + "\n");

            tag_count_stats.Text = "Total Tags : " + TotalTags;
        }
        private void button2_Click(object sender, EventArgs e)
        {                  
        
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = fbd.SelectedPath;                
                
            }
        
         }

        private void extract_button_Click(object sender, EventArgs e)
        {
            if (!isDecompileMap_)
            {
                isRecursive = recursive_radio_.Checked;
                isOverrideOn = override_tags_.Checked;
                isOutDBOn = output_db_.Checked;

                DestinationFolder = textBox1.Text;

                string mapName = DATA_READ.Read_File_from_file_location(MainBox.map_name);
                int TotalTags = TagsList.Count;
                curent_tag_status.Visible = true;


                if (DestinationFolder == "")
                {

                    curent_tag_status.Text = "Select a Destination Folder Please";
                    return;
                }


                curent_tag_status.Text = "Initializing Decompiler";

                MainBox.CloseMap();

                List<int> extract_list = TagsList.Keys.ToList<int>();
                MainBox.H2Test.Halo2_ExtractTagCache(extract_list, isRecursive, isOutDBOn, isOverrideOn, DestinationFolder, mapName);

                /*
                progressBar1.Value = 0;
                progressBar1.Maximum = TotalTags;
                int index = 0;
                foreach (int i in TagsList.Keys)
                {
                    tag_count_stats.Text = "[" + index + "/" + TotalTags +"]";
                    curent_tag_status.Text = "Extracting Objects : " + TagsList.Values.ElementAt(index);
                    MainBox.H2Test.Halo2_ExtractTagCache(i, isRecursive, isOutDBOn, isOverrideOn, DestinationFolder, mapName);
                    progressBar1.Value++; //update the progress bar
                    index++;
                }
                */

                if (MessageBox.Show("Extraction Done!", "Progress", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    try
                    {
                        MainBox.ReOpenMap();
                    }
                    catch (Exception ex)
                    {
                        TagsList.Clear();
                    }
                }
            }
            else
            {
                isRecursive = true;
                isOverrideOn = override_tags_.Checked;
                isOutDBOn = output_db_.Checked;

                DestinationFolder = textBox1.Text;

                string mapName = DATA_READ.Read_File_from_file_location(MainBox.map_name);
                
                curent_tag_status.Visible = true;
                tag_count_stats.Visible = true;

                if (DestinationFolder == "")
                {

                    curent_tag_status.Text = "Select a Destination Folder Please";
                    return;
                }


                curent_tag_status.Text = "Initializing Decompiler";
                MainBox.CloseMap();
                

                List<int> extract_list = MainBox.AllTagslist.Keys.ToList<int>();
                MainBox.H2Test.Halo2_ExtractTagCache(extract_list, isRecursive, isOutDBOn, isOverrideOn, DestinationFolder, mapName);

                /*
                progressBar1.Value = 0;
                progressBar1.Maximum = TotalTags;
                int index = 1;
                foreach (int i in MainBox.AllTagslist.Keys)
                {
                    tag_count_stats.Text = "[" + index++ + "/" + TotalTags + "]";
                    curent_tag_status.Text = "Extracting Objects : " + TagsList.Values.ElementAt(index);
                    MainBox.H2Test.Halo2_ExtractTagCache(i, isRecursive, isOutDBOn, isOverrideOn, DestinationFolder, mapName);
                    progressBar1.Value++; //update the progress bar
                    
                }
                */

                if (MessageBox.Show("Extraction Done!", "Progress", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    MainBox.ReOpenMap();
                }

            }
                            

        }



        
    }
}
