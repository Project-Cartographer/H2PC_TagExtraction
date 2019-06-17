using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Globalization;
using BlamLib;
using DATA_STRUCTURES;

/*
Created and developed by Himanshu01 in H2PC Project Cartographer Team
Also uses BlamLib by Kornner Studios for tag extraction 
<3 Hamp
*/


namespace Map_Handler
{

    public partial class MainBox : Form
    {
        #region EXTRACTION_RELATED_VARS 
       

        // For tag extraction
        Dictionary<int, string> AddList = new Dictionary<int, string>();
        Dictionary<int, string> ExtractList = new Dictionary<int, string>();
        
        static string DestinationFolder = "";
        static bool isRecursive;
        static bool isOverrideOn;
        static bool isOutDBOn;
        
        

        //




        
        static StreamReader map_stream;//stream reader
        int table_off;//offset of the table
        int table_start;//start of the Actual Tables
        int table_size;//size of the table
        int file_table_offset;//file table offset from where the strings begin
        
        

        static bool map_loaded = false;//is the map loaded
        int scnr_memaddr;
        int scnr_off;
        public static string map_name="";//name of the map along woth destination
        public static string map_path= "";//path of the mapfile above, where we look for shared/ui etc.
        List<int> datum_list = new List<int>();
        string settings_path = Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), "H2_PCHandlerSettings.txt");
        public static BlamLib.Test.Halo2 H2Test = new BlamLib.Test.Halo2(); //Blam Lib Tests project
        public static BlamLib.Blam.Halo2.Converter H2Convert = new BlamLib.Blam.Halo2.Converter();

     
        

        Dictionary<int, string> SID_list;//a list containing string index stuff
        public static Dictionary<int, string> AllTagList;
        public static string H2V_BaseMapsDirectory;

        #endregion


        public MainBox()
        {
            InitializeComponent();
                
        }

        #region NON_BLAM_LIB_EXTRACTION

        

        private void openMapToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //Map opening stuff
            OpenFileDialog map_file = new OpenFileDialog();
            map_file.Filter = "Halo 2 Vista Map (*.map)|*.map";

            if (map_file.ShowDialog() == DialogResult.OK)
            {
                map_stream = new StreamReader(map_file.FileName);
                SID_list = new Dictionary<int, string>();//initialise our SIDs
                AllTagList = new Dictionary<int, string>();

                table_off = DATA_READ.ReadINT_LE(0x10, map_stream);
                table_size = DATA_READ.ReadINT_LE(0x14, map_stream);
                file_table_offset = DATA_READ.ReadINT_LE(0x2D0, map_stream);

                table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4, map_stream) + 0x20;

                scnr_off = table_off + table_size;
                scnr_memaddr = DATA_READ.ReadINT_LE(table_start + 0x8, map_stream);//scnr tag index is 0x0

                map_name =map_file.FileName;
                map_path = Path.GetDirectoryName(map_name);
                textBox1.Text = "Map Loaded -  " + map_name;
                textBox4.Text = "0 Tags Selected";
                initialize_treeview();
                map_loaded = true;
                TagToolStripMenu.Visible = true;
                metaToolStripMenuItem.Visible = true;
            }           


        }

        /// <summary>
        /// Initialises tree view upon opening a map file
        /// </summary>
        void initialize_treeview()
        {
            treeView1.Nodes.Clear();

            int tag_count = 0;
            int path_start = 0;

            for (int i = 0; ; i++)
            {
                int tag_table_REF = table_start + 0x10 * i;

                if (tag_table_REF > table_size + table_start)
                    break;
                               
                string type = DATA_READ.ReadTAG_TYPE(tag_table_REF, map_stream);
                int datum_index = DATA_READ.ReadINT_LE(tag_table_REF + 4, map_stream);
                string path = DATA_READ.ReadSTRING(file_table_offset + path_start, map_stream);

                if (datum_index != -1)
                {
                    //lets check the mem addrs validity before adding it to the list
                    int mem_addr = DATA_READ.ReadINT_LE(tag_table_REF + (datum_index & 0xffff) * 0x10 + 8, map_stream);

                    if (mem_addr != 0x0)
                        AllTagList.Add(datum_index, path);//Adding only Map Specific tags with Internal Reference only to list 
                                 


                    if (treeView1.Nodes.IndexOfKey(type) == -1)
                    {
                        treeView1.Nodes.Add(type, "- "+type);
                    }
                    int index = treeView1.Nodes.IndexOfKey(type);
                    
                    //HEX Values contains ABCDEF
                    treeView1.Nodes[index].Nodes.Add(tag_table_REF.ToString(), "- "+path);

                    //add this stuff to the SID list
                    SID_list.Add(datum_index, path);

                    //ugh! is basically the last tag
                    if (type.CompareTo("ugh!") == 0)
                        break;

                    path_start += path.Length + 1;
                }

                tag_count = i;
            }
            treeView1.Sort();
            textBox2.Text = tag_count.ToString() + " Total Tags";

        }

        private void extractMetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map_loaded)
            {
                if (treeView1.SelectedNode != null)
                {
                    int tag_table_ref = Int32.Parse(treeView1.SelectedNode.Name);
                    string type = DATA_READ.ReadTAG_TYPE(tag_table_ref, map_stream);
                    int datum_index = DATA_READ.ReadINT_LE(tag_table_ref + 4, map_stream);

                    //Meta Extractor
                    MetaExtractor meta_extract;             
                    meta_extract = new MetaExtractor(datum_index, type, SID_list, map_stream);
                    meta_extract.Show();

                }
                else MessageBox.Show("Select a TAG", "Hint");
            }
            else
            {
                MessageBox.Show("Select a map First", "Hint");
            }
        }

        //function display the tag structure
        private void getTagStructureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map_loaded)
            {
                if (treeView1.SelectedNode != null)
                {
                    string type = DATA_READ.ReadTAG_TYPE(Int32.Parse(treeView1.SelectedNode.Name),map_stream);

                    plugins_field temp = DATA_READ.Get_Tag_stucture_from_plugin(type);
                    if (temp != null)
                    {
                        TreeNode tn = temp.Get_field_structure();
                                            
                        tn.Text = type;

                        treeView1.Nodes.Clear();
                        treeView1.Nodes.Add(tn);
                    }
                    else MessageBox.Show("The plugin of type "+type+" doesn't exist", "ERROR");

                    map_loaded = false;

                }
                else MessageBox.Show("Select a TAG", "Hint");
            }
            else
            {
                MessageBox.Show("Select a map First", "Hint");
            }
        }     

        private void CompileMetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //File dailogue to select the config file
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "config files(*.xml)|*.xml";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Rebase_meta RM = new Rebase_meta(ofd.FileName);
                RM.Show();
            }           
            
        }

        #endregion

        #region BLAM_LIB_EXTRACTION

        //Tag extraction stuff
        public static void CloseMap()
        {
            if (map_loaded)
            {
                map_stream.Close();
                map_loaded = false;
            }

        }
        public static void ReOpenMap()
        {
            if (!map_loaded)
            {
                map_stream = new StreamReader(map_name);
                map_loaded = true;
            }

        }


        public void UnCheckAll(TreeNodeCollection nodes)
        {

            foreach (System.Windows.Forms.TreeNode tagitem in nodes)
            {
                tagitem.Checked = false;
                if (tagitem.Nodes.Count != 0)
                    UnCheckAll(tagitem.Nodes);
            }
        } 



        public void CheckedTags(TreeNodeCollection nodes)
        {
            
            foreach (System.Windows.Forms.TreeNode tagitem in nodes)
            {
              
                if (tagitem.Checked)
                {
                    if (tagitem.Level != 0)
                    {
                        int tag_table_ref = Int32.Parse(tagitem.Name);
                        int datum_index = DATA_READ.ReadINT_LE(tag_table_ref + 4, map_stream);
                        if (!AddList.ContainsKey(datum_index))
                            AddList.Add(datum_index, tagitem.Text);
                        
                    }
                    else
                        CheckedTags(tagitem.Nodes);
                }
                if (tagitem.Nodes.Count != 0)
                    CheckedTags(tagitem.Nodes);
            }
            textBox4.Text = AddList.Count.ToString() + " Tags Selected";
        } 
        


        private void extractTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(map_loaded)
            {




                AddList.Clear();
                CheckedTags(treeView1.Nodes);

                AddTags();
            }
            else
            {
                MessageBox.Show("No Map Loaded ,Reload it", "Error!!", MessageBoxButtons.OK);
            }
        }


        private void AddTags()
        {





            progressBar1.Value = 0;


              if (AddList == null)
                  return; //EROROORR


              for (int o = 0; o < AddList.Count; o++)
              {
                  if (!ExtractList.ContainsKey(AddList.ElementAt(o).Key))
                  {
                      ExtractList.Add(AddList.ElementAt(o).Key, AddList.ElementAt(o).Value);
                      richTextBox1.AppendText("[" + AddList.ElementAt(o).Key.ToString("X") + "] " + AddList.ElementAt(o).Value + "\n");
                  }
              }
              extract_button.Enabled = true;
              clear_button.Enabled = true;
              AddList.Clear();
              UnCheckAll(treeView1.Nodes);
              label4.Text = ExtractList.Count.ToString() + " Tags Added";
              textBox4.Text = "0 Tags Selected";




        }

        private void decompileMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map_loaded)
            {
                AddList = AllTagList;
                    AddTags();
            }
            extract_button.Enabled=true;
            clear_button.Enabled = true;

        }


        #endregion
        

 
        private void closeMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseMap();
            treeView1.Nodes.Clear();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            richTextBox1.Text = "";
            TagToolStripMenu.Visible = false;
            metaToolStripMenuItem.Visible = false;
            progressBar1.Value = 0;

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseMap();
            Application.Exit();
        }

        private void defaultMaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setMapsFolder();
        }

        private void setMapsFolder()
        {
            

            
            
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;


            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                H2V_BaseMapsDirectory = fbd.SelectedPath;


                MessageBox.Show(H2V_BaseMapsDirectory);
                System.IO.File.WriteAllText(settings_path , H2V_BaseMapsDirectory);
            }
            
            if (!map_loaded)
            {
                textBox1.Text="Maps Folder - " + H2V_BaseMapsDirectory;
            }

        }

        void DumpTagList()
        {  if (treeView1.SelectedNode.Nodes.Count != 0)
            {
                string[] x = new string[treeView1.SelectedNode.Nodes.Count];
                int i = 0;
                foreach (TreeNode tn in treeView1.SelectedNode.Nodes)
                {
                    int tag_table_ref = Int32.Parse(tn.Name);
                    int datum_index = DATA_READ.ReadINT_LE(tag_table_ref + 4, map_stream);

                    string Name = System.IO.Path.GetFileNameWithoutExtension(tn.Text);

                    x[i++] = Name + "," + "0x" + datum_index.ToString("X"); ;

                }

                File.WriteAllLines(Application.StartupPath + @"\AddList.txt", x);
            }
            else
            {
                MessageBox.Show("Select Tag Nodes First", "Error", MessageBoxButtons.OK);
            }
            
        }

        void WIP()
        {
            MessageBox.Show("Work In Progress!!", "WIP", MessageBoxButtons.OK);
        }

        private void hCEGBXModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ToDO: Complete this with UI
           // WIP();         
           
        }

        private void hCEToH2VSoundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ToDO: Complete this with UI
            WIP();
        }

        private void hCECollisionModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ToDO: Complete this with UI
            WIP();
        }

        private void ExtractImportlStripMenuItem_Click(object sender, EventArgs e)
        {
            String BrowseDirectory = "";
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;


            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BrowseDirectory = fbd.SelectedPath;
                H2Test.Halo2TestImportInfoExtraction(BrowseDirectory,"");
                MessageBox.Show("Done ", "Progress", MessageBoxButtons.OK);
            }
        }

        private void dumpSelectedTagsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DumpTagList();
            MessageBox.Show("Done ", "Progress", MessageBoxButtons.OK);
        }

        private void tests1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            //H2Test.Halo2TestCacheOutputPc();
        }

        private void resyncshadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Resyncer_Dialog_Box RDB = new Resyncer_Dialog_Box();
            RDB.Show();
        }

        private void resyncStringIDsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "config files(*.xml)|*.xml";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Resync_SID RSID = new Resync_SID(ofd.FileName);
            }

        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.openMapToolStripMenuItem_Click(sender, e);
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.closeMapToolStripMenuItem_Click(sender, e);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.extractTagToolStripMenuItem_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.decompileMapToolStripMenuItem_Click(sender, e);
        }





      //Code from VB/C# example documents. "TreeView.AfterCheck Event"



        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            { 
                node.Checked = !nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
           if (e.Action != TreeViewAction.Unknown)
            {
                
                if (e.Node.Nodes.Count > 0)
                {
                  
                    this.CheckAllChildNodes(e.Node, !e.Node.Checked);
                }
                AddList.Clear();
                CheckedTags(treeView1.Nodes);
            }
           
        }

        private void extract_button_Click(object sender, EventArgs e)
        {
            isRecursive = recursive_radio_.Checked;
            isOverrideOn = override_tags_.Checked;
            isOutDBOn = output_db_.Checked;
            

            DestinationFolder = textBox3.Text;

            
            string mapName = DATA_READ.Read_File_from_file_location(MainBox.map_name);
            int TotalTags = AddList.Count;
            current_tag_status.Visible = true;


            if (DestinationFolder == "")
            {

                current_tag_status.Text = "Select a Destination Folder Please";
                return;
            }

            



            current_tag_status.Text = "Initializing Decompiler";


            MainBox.CloseMap();
            progressBar1.Value = 0;
            progressBar1.Maximum = ExtractList.Count;
            int index = 0;
            foreach (int i in ExtractList.Keys)
            {
                
                current_tag_status.Text = "Extracting Objects : " + ExtractList.Values.ElementAt(index);
                MainBox.H2Test.Halo2_ExtractTagCache(i, isRecursive, isOutDBOn, isOverrideOn, DestinationFolder, H2V_BaseMapsDirectory + "\\", mapName);
                progressBar1.Value++; //update the progress bar
                index++;
            }

             current_tag_status.Text = "Extraction Complete!";
            if (MessageBox.Show("Extraction Done!", "Progress", MessageBoxButtons.OK) == DialogResult.OK)
            {
                MainBox.ReOpenMap();
            }
            clear_button.Enabled=false;
            extract_button.Enabled=false;
            ExtractList.Clear();
            richTextBox1.Text = "";
        }

        private void clear_button_Click(object sender, EventArgs e)
        {
            clear_button.Enabled = false;
            extract_button.Enabled = false;
            ExtractList.Clear();
            richTextBox1.Text = "";
            label4.Text = "";
        }

        private void MainBox_Load(object sender, EventArgs e)
        {
            ForceMaps();

        }

        private void ForceMaps()
        {
            if (File.Exists(settings_path))
            {
                H2V_BaseMapsDirectory = File.ReadAllText(settings_path);
                textBox1.Text = "Maps Folder - " + H2V_BaseMapsDirectory;
            }
            else
            {
                MessageBox.Show("Please select your Maps Folder");
                setMapsFolder();
                ForceMaps();
            }

           

        }

        private void button6_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox3.Text = fbd.SelectedPath;

            }
        }

      

        
       

    

        






      
        

       
    }
}