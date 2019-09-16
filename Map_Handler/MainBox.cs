﻿using System;
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

        static StreamReader map_stream;//stream reader
        int table_off;//offset of the table
        int table_start;//start of the Actual Tables
        int table_size;//size of the table
        int file_table_offset;//file table offset from where the strings begin

        static bool map_loaded = false;//is the map loaded
        int scnr_memaddr;
        int scnr_off;
        public static string map_name = "";//name of the map along woth destination
        public static string map_path = "";//path of the mapfile above, where we look for shared/ui etc.
        List<int> datum_list = new List<int>();
        string settings_path = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.ApplicationData), "H2_PCHandlerSettings.txt");
        public static BlamLib.Test.Halo2 H2Test = new BlamLib.Test.Halo2(); //Blam Lib Tests project
        public static BlamLib.Blam.Halo2.Converter H2Convert = new BlamLib.Blam.Halo2.Converter();

        Dictionary<int, string> SID_list;//a list containing tag path stuff :P
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

                map_name = map_file.FileName;
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
                        treeView1.Nodes.Add(type, "- " + type);
                    }
                    int index = treeView1.Nodes.IndexOfKey(type);

                    //HEX Values contains ABCDEF
                    treeView1.Nodes[index].Nodes.Add(tag_table_REF.ToString(), "- " + path);

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
                    string type = DATA_READ.ReadTAG_TYPE(Int32.Parse(treeView1.SelectedNode.Name), map_stream);

                    plugins_field temp = DATA_READ.Get_Tag_stucture_from_plugin(type);
                    if (temp != null)
                    {
                        TreeNode tn = temp.Get_field_structure();

                        tn.Text = type;

                        treeView1.Nodes.Clear();
                        treeView1.Nodes.Add(tn);
                    }
                    else MessageBox.Show("The plugin of type " + type + " doesn't exist", "ERROR");

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
            if (map_loaded)
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
            extract_button.Enabled = true;
            clear_button.Enabled = true;
        }

        private void sbspltmpHaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            H2Test.sbsptoltmp_cluster_block_copy(@"C:\Program Files (x86)\Microsoft Games\Halo 2 Map Editor\tags\scenarios\multi\example\example_example_lightmap.scenario_structure_lightmap", @"C:\Program Files (x86)\Microsoft Games\Halo 2 Map Editor\tags\scenarios\multi\example\example.scenario_structure_bsp");
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
                System.IO.File.WriteAllText(settings_path, H2V_BaseMapsDirectory);
            }

            if (!map_loaded)
            {
                textBox1.Text = "Maps Folder - " + H2V_BaseMapsDirectory;
            }

        }

        void DumpTagList()
        {
            if (treeView1.SelectedNode.Nodes.Count != 0)
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
                H2Test.Halo2TestImportInfoExtraction(BrowseDirectory, "");
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

            List<int> extract_list = ExtractList.Keys.ToList<int>();
            MainBox.H2Test.Halo2_ExtractTagCache(extract_list, isRecursive, isOutDBOn, isOverrideOn, DestinationFolder, map_path + "\\", mapName);
            /*
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
            */

            current_tag_status.Text = "Extraction Complete!";
            if (MessageBox.Show("Extraction Done!", "Progress", MessageBoxButtons.OK) == DialogResult.OK)
            {
                MainBox.ReOpenMap();
            }
            clear_button.Enabled = false;
            extract_button.Enabled = false;
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

        private void sndtagFixesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "config files(*.xml)|*.xml";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                snd_fixes snd = new snd_fixes(ofd.FileName);
            }
        }

        private void EmulateShaderDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();

            MessageBox.Show("Please select the Shader Log.");

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "plugins", "shaderstemplates", "blank.shader");
                string shaderpath = Path.GetDirectoryName(fbd.FileName);
                string tagfolder = Path.Combine(shaderpath, "TAGS");
                string workingdirectory = Environment.CurrentDirectory;
                Directory.CreateDirectory(tagfolder);

                foreach (string line in File.ReadLines(fbd.FileName))
                {
                    var sectionShaderParameter = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### SHADER PARAMETERS ###")     // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### SHADER PARAMETERS END ###") // Take lines until the end
                        .ToList();                                            // Convert the result to List<string>

                    var sectionBitmap = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### BITMAPS ###")           // Skip up to the header
                        .Skip(1)                                          // Skip the header
                        .TakeWhile(s => s != "### BITMAPS END ###")       // Take lines until the end
                        .ToList();                                        // Convert the result to List<string>

                    var sectionLightmap = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### LIGHTMAP ###")     // Skip up to the header
                        .Skip(1)                                            // Skip the header
                        .TakeWhile(s => s != "### LIGHTMAP END ###") // Take lines until the end
                        .ToList();

                    var sectionPixelConstant = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### PIXEL CONSTANTS ###")     // Skip up to the header
                        .Skip(1)                                            // Skip the header
                        .TakeWhile(s => s != "### PIXEL CONSTANTS END ###") // Take lines until the end
                        .ToList();                                          // Convert the result to List<string>

                    var sectionVertexConstant = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### VERTEX CONSTANTS ###")     // Skip up to the header
                        .Skip(1)                                            // Skip the header
                        .TakeWhile(s => s != "### VERTEX CONSTANTS END ###") // Take lines until the end
                        .ToList();

                    List<string> SupportedShaders = new List<string>();

                    SupportedShaders.Add("active_camo_opaque.shader_template.txt");
                    SupportedShaders.Add("active_camo_transparent.shader_template.txt");
                    SupportedShaders.Add("add_illum_detail.shader_template.txt");
                    SupportedShaders.Add("ammo_meter.shader_template.txt");
                    SupportedShaders.Add("bloom.shader_template.txt");
                    SupportedShaders.Add("bumped_environment_additive.shader_template.txt");
                    SupportedShaders.Add("bumped_environment_blended.shader_template.txt");
                    SupportedShaders.Add("bumped_environment_darkened.shader_template.txt");
                    SupportedShaders.Add("bumped_environment_mask_colored.shader_template.txt");
                    SupportedShaders.Add("bumped_environment_masked.shader_template.txt");
                    SupportedShaders.Add("cortana.shader_template.txt");
                    SupportedShaders.Add("cortana_holographic_active_camo.shader_template.txt");
                    SupportedShaders.Add("illum.shader_template.txt");
                    SupportedShaders.Add("illum_bloom.shader_template.txt");
                    SupportedShaders.Add("illum_clamped.shader_template.txt");
                    SupportedShaders.Add("illum_detail.shader_template.txt");
                    SupportedShaders.Add("one_add_illum.shader_template.txt");
                    SupportedShaders.Add("one_add_illum_detail.shader_template.txt");
                    SupportedShaders.Add("one_alpha_env_illum.shader_template.txt");
                    SupportedShaders.Add("one_alpha_env_illum_specular_mask.shader_template.txt");
                    SupportedShaders.Add("tex_bump.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_dbl_spec.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_3_channel.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_3_channel_combined.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_3_channel_occlusion_combined.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_combined.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_detail_honor_guard.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_four_change_color_no_lod.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_two_change_color.shader_template.txt");
                    SupportedShaders.Add("tex_bump_illum.shader_template.txt");
                    SupportedShaders.Add("tex_bump_illum_alpha_test.shader_template.txt");
                    SupportedShaders.Add("tex_bump_illum_bloom.shader_template.txt");
                    SupportedShaders.Add("tex_bump_meter_illum.shader_template.txt");
                    SupportedShaders.Add("tex_bump_plasma_one_channel_illum.shader_template.txt");
                    SupportedShaders.Add("two_add_env_illum.shader_template.txt");
                    SupportedShaders.Add("two_alpha_env_illum.shader_template.txt");

                    string ShaderTemplate = Path.GetFileName(sectionShaderParameter[0]) + ".shader_template.txt";

                    string PixelTemplate = Path.GetFileName(sectionShaderParameter[0]) + ".shader_template_pixel.txt";

                    string[] BitmapLabels = File.ReadAllLines(Path.Combine(workingdirectory, "plugins", "shaderstemplates", ShaderTemplate));

                    string[] PixelLabels = File.ReadAllLines(Path.Combine(workingdirectory, "plugins", "pixeltemplates", PixelTemplate));

                    int ShaderParameterCount = BitmapLabels.GetLength(0);

                    int PixelParameterCount = PixelLabels.GetLength(0);

                    string tagname = Path.Combine(tagfolder, line.Substring(0, (line.Length - 4)) + ".shader");
                    Directory.CreateDirectory(Directory.GetParent(tagname).ToString());

                    using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    using (var ds = new FileStream(tagname, FileMode.Create, FileAccess.ReadWrite))
                    using (var ms = new MemoryStream())
                    using (var bw = new BinaryWriter(ms))
                    using (var br = new BinaryReader(ms))

                    {
                        fs.CopyTo(ms);
                        ms.Position = 0;

                        bw.BaseStream.Seek(88, SeekOrigin.Begin);
                        bw.Write(Convert.ToInt32(sectionShaderParameter[0].Length));     //Write shader path length
                        bw.BaseStream.Seek(99, SeekOrigin.Begin);
                        bw.Write(Convert.ToInt32(sectionShaderParameter[1].Length));     //Write material_name length
                        bw.BaseStream.Seek(114, SeekOrigin.Begin);
                        bw.Write(Convert.ToByte(sectionShaderParameter[2]));             //Write Flags 1 byte = water 2 byte = sort first 4 byte = no active camo
                        bw.BaseStream.Seek(116, SeekOrigin.Begin);
                        bw.Write(Convert.ToInt32(ShaderParameterCount + PixelParameterCount));                       //Write parameter count
                        bw.BaseStream.Seek(172, SeekOrigin.Begin);
                        bw.Write(Convert.ToByte(sectionShaderParameter[9]));             //Write shader_lod_bias
                        bw.BaseStream.Seek(174, SeekOrigin.Begin);
                        bw.Write(Convert.ToByte(sectionShaderParameter[5]));             //Write dynamic_light_specular_type value
                        bw.BaseStream.Seek(176, SeekOrigin.Begin);
                        bw.Write(Convert.ToByte(sectionShaderParameter[6]));             //Write lightmap_type value
                        bw.BaseStream.Seek(180, SeekOrigin.Begin);
                        bw.Write(Convert.ToSingle(sectionShaderParameter[7]));           //Write lightmap_specular_brightness value
                        bw.Write(Convert.ToSingle(sectionShaderParameter[8]));           //Write lightmap_ambient_bias value
                        bw.BaseStream.Seek(200, SeekOrigin.Begin);
                        bw.Write(Convert.ToSingle(sectionShaderParameter[3]));           //Write depth_bias_offset value
                        bw.Write(Convert.ToSingle(sectionShaderParameter[4]));           //Write depth_bias_slope_scale value

                        bw.BaseStream.Seek(208, SeekOrigin.Begin);

                        bw.Write(Encoding.UTF8.GetBytes(sectionShaderParameter[0]));     //Write shader path
                        bw.Write(Convert.ToByte(0));
                        bw.Write(Encoding.UTF8.GetBytes(sectionShaderParameter[1]));     //Write material_name
                        bw.Write(Encoding.UTF8.GetBytes("dfbt"));
                        bw.Write(Convert.ToInt32(0));
                        bw.Write(Convert.ToInt32(ShaderParameterCount + PixelParameterCount));                       //Write parameter count
                        bw.Write(Convert.ToInt32(52));

                        for (int i = 0; i < ShaderParameterCount; i++)
                        {
                            bw.Write(Convert.ToInt16(0));

                            if (sectionBitmap[i] == " ")
                            {
                                sectionBitmap[i] = "";
                            }

                            byte[] flip = new byte[2];
                            flip = BitConverter.GetBytes(Convert.ToInt16(BitmapLabels[i].Length));
                            Array.Reverse(flip);

                            bw.Write(BitConverter.ToInt16(flip, 0));
                            bw.Write(Convert.ToInt32(0));
                            bw.Write(Encoding.UTF8.GetBytes("mtib"));
                            bw.Write(Convert.ToInt32(0));
                            bw.Write((Convert.ToInt32(sectionBitmap[i].Length)));
                            bw.Write(Convert.ToInt32(-1));

                            bw.Write(Convert.ToInt32(0));
                            bw.Write(Convert.ToInt32(0));
                            bw.Write(Convert.ToInt32(0));
                            bw.Write(Convert.ToInt32(0));
                            bw.Write(Convert.ToInt32(0));
                            // 5 time 5 time 5 time 5 time 5 time WCW Champion

                            bw.Write(Convert.ToInt32(-1));

                            bw.Write(Convert.ToInt32(10088844));
                            //Look I don't design the tags I just write them to the file. I need to write 10088844 and I don't care why.
                        }

                        for (int i = 0; i < PixelParameterCount; i++)
                        {
                            bw.Write(Convert.ToInt16(0));

                            if (PixelLabels[i] == " ")
                            {
                                PixelLabels[i] = "";
                            }

                            byte[] flip = new byte[2];
                            flip = BitConverter.GetBytes(Convert.ToInt16(PixelLabels[i].Length));
                            Array.Reverse(flip);

                            bw.Write(BitConverter.ToInt16(flip, 0));
                            bw.Write(Convert.ToInt32(0));
                            bw.Write(Encoding.UTF8.GetBytes("mtib"));
                            bw.Write(Convert.ToInt32(0));
                            bw.Write((Convert.ToInt32(0)));
                            bw.Write(Convert.ToInt32(-1));

                            #region Shader Specific Fixes

                            double b = 0.00392156862;

                            #region Active Camo Opaque
                            if (ShaderTemplate == "active_camo_opaque.shader_template.txt")
                            {
                                if (PixelLabels[i] == "refraction_bump_amount")     //vertex constant index 2 x value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "refraction_geometry_amount") //vertex constant index 3 y value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Active Camo Transparent
                            if (ShaderTemplate == "active_camo_transparent.shader_template.txt")
                            {
                                if (PixelLabels[i] == "refraction_bump_amount")     //vertex constant index 2 x value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "refraction_geometry_amount") //vertex constant index 3 y value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Add Illum Detail
                            if (ShaderTemplate == "add_illum_detail.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")        //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_color") //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Ammo Meter
                            if (ShaderTemplate == "ammo_meter.shader_template.txt")
                            {
                                if (PixelLabels[i] == "meter_gradient_min") //pixel constants index 1 value - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToSingle(0));          //A
                                    bw.Write(Convert.ToSingle(0));          //R
                                    bw.Write(Convert.ToSingle(0));          //G
                                    bw.Write(Convert.ToSingle(0));          //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "meter_gradient_max") //pixel constants index 2 value - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToSingle(0));          //A
                                    bw.Write(Convert.ToSingle(0));          //R
                                    bw.Write(Convert.ToSingle(0));          //G
                                    bw.Write(Convert.ToSingle(0));          //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "meter_empty_color")  //pixel constants index 3 value - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToSingle(0));          //A
                                    bw.Write(Convert.ToSingle(0));          //R
                                    bw.Write(Convert.ToSingle(0));          //G
                                    bw.Write(Convert.ToSingle(0));          //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "meter_amount")       //pixel constants index 0 value - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToSingle(0));          //A
                                    bw.Write(Convert.ToSingle(0));          //R
                                    bw.Write(Convert.ToSingle(0));          //G
                                    bw.Write(Convert.ToSingle(0));          //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bloom
                            if (ShaderTemplate == "bloom.shader_template.txt")
                            {
                                if (PixelLabels[i] == "lightmap_emissive_color") //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "bloom")                   //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bumped Environment Additive
                            if (ShaderTemplate == "bumped_environment_additive.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")          //vertex constant index 0 x,y,z rgb floats - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color") //vertex constant index 1 x,y,z rgb floats - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")          //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness") //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")          //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")          //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bumped Environment Blended
                            if (ShaderTemplate == "bumped_environment_blended.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")             //vertex constant index 0 x,y,z rgb floats - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")    //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")             //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")    //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")             //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")             //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_translucent_color") //runtime properites tag block lightmap transparent colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_translucent_alpha") //runtime properties tag block lightmap transparent alpha - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bumped Environment Darkened
                            if (ShaderTemplate == "bumped_environment_darkened.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //vertex constant index 0 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0));  //G
                                    bw.Write(Convert.ToSingle(0));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0));  //G
                                    bw.Write(Convert.ToSingle(0));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bumped Environment Mask Colored
                            if (ShaderTemplate == "bumped_environment_mask_colored.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //vertex constant index 0 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0));  //G
                                    bw.Write(Convert.ToSingle(0));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0));  //G
                                    bw.Write(Convert.ToSingle(0));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "mask_color0")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "mask_color1")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "mask_color2")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bumped Environment Masked
                            if (ShaderTemplate == "bumped_environment_masked.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //vertex constant index 0 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0));  //G
                                    bw.Write(Convert.ToSingle(0));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0));  //G
                                    bw.Write(Convert.ToSingle(0));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Cortana
                            if (ShaderTemplate == "cortana.shader_template.txt")
                            {
                                if (PixelLabels[i] == "flat_environment_color")                                         //vertex constant index 0 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0));  //G
                                    bw.Write(Convert.ToSingle(0));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")                                //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0));  //G
                                    bw.Write(Convert.ToSingle(0));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")                                         //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));  //A
                                    bw.Write(Convert.ToSingle(0));  //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "color_wide")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "color_medium")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "color_sharp")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "perpendicular_brightness")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "fade_bias")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "bloom")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_tint_color")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Cortana Holographic Active Camo
                            if (ShaderTemplate == "cortana_holographic_active_camo.shader_template.txt")
                            {
                                if (PixelLabels[i] == "refraction_bump_amount")                                        //unused? moved from the shad file to some other file on package? Only used on lighting and thrown away on package? - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                                             //A
                                    bw.Write(Convert.ToSingle(0));                                             //R
                                    bw.Write(Convert.ToSingle(0));                                             //G
                                    bw.Write(Convert.ToSingle(0));                                             //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "refraction_geometry_amount")                                        //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "refraction_x_offset")                               //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Illum
                            if (ShaderTemplate == "illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region illum Bloom
                            if (ShaderTemplate == "illum_bloom.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "bloom")                                        //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region illum Clamped
                            if (ShaderTemplate == "illum_clamped.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region illum Detail
                            if (ShaderTemplate == "illum_detail.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region One Add Illum
                            if (ShaderTemplate == "one_add_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")                                   //vertex constant index 2 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")                          //vertex constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")                                   //vertex constant index 4 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[16]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[17]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[18]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[19]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")                          //vertex constant index 5 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[20]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[21]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[22]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[23]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emmisive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emmisive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region One Add Illum Detail
                            if (ShaderTemplate == "one_add_illum_detail.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")                                   //vertex constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")                          //vertex constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")                                   //vertex constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")                          //vertex constant index 7 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region One Alpha Env Illum
                            if (ShaderTemplate == "one_alpha_env_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "environment_color")                             //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")                            //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")                          //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")                                          //vertex constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")                                 //vertex constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")                                          //vertex constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")                                 //vertex constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region One Alpha Env Illum Specular Mask
                            if (ShaderTemplate == "one_alpha_env_illum_specular_mask.shader_template.txt")
                            {
                                if (PixelLabels[i] == "environment_color")                             //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")                            //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")                          //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")                                          //vertex constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")                                 //vertex constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")                                          //vertex constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")                                 //vertex constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump
                            if (ShaderTemplate == "tex_bump.shader_template.txt")
                            {
                                if (PixelLabels[i] == "ambient_factor")                                        //unused? moved from the shad file to some other file on package? Only used on lighting and thrown away on package? - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(0));                                             //A
                                    bw.Write(Convert.ToSingle(0));                                             //R
                                    bw.Write(Convert.ToSingle(0));                                             //G
                                    bw.Write(Convert.ToSingle(0));                                             //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                        //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                               //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env
                            if (ShaderTemplate == "tex_bump_env.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Dbl Spec
                            if (ShaderTemplate == "tex_bump_env_dbl_spec.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum
                            if (ShaderTemplate == "tex_bump_env_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //Not written to packaged shad tag? - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));                 //A
                                    bw.Write(Convert.ToInt32(0));                 //R
                                    bw.Write(Convert.ToInt32(0));                 //G
                                    bw.Write(Convert.ToInt32(0));                 //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //Not written to packaged shad tag? - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));                //A
                                    bw.Write(Convert.ToInt32(0));                //R
                                    bw.Write(Convert.ToInt32(0));                //G
                                    bw.Write(Convert.ToInt32(0));                //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum 3 Channel
                            if (ShaderTemplate == "tex_bump_env_illum_3_channel.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 10 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[40]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[41]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[42]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[43]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 11 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[44]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[45]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[46]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[47]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 12 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[48]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[49]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[50]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[51]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 13 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[52]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[53]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[54]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[55]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color") //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_brightness") //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color") //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_brightness") //pixel constant index 9 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum 3 Channel Combined
                            if (ShaderTemplate == "tex_bump_env_illum_3_channel_combined.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color") //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_brightness") //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color") //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_brightness") //pixel constant index 9 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum 3 Channel Occlusion Combined
                            if (ShaderTemplate == "tex_bump_env_illum_3_channel_occlusion_combined.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 13 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[52]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[53]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[54]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[55]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 14 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[56]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[57]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[58]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[59]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 15 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[60]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[61]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[62]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[63]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 16 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[64]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[65]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[66]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[67]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color") //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_brightness") //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color") //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_brightness") //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "occlusion_a_color") //pixel constant index 10 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "occlusion_b_color") //pixel constant index 11 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "occlusion_c_color") //pixel constant index 12 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum Combined
                            if (ShaderTemplate == "tex_bump_env_illum_combined.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum Detail Honor Guard
                            if (ShaderTemplate == "tex_bump_env_illum_detail_honor_guard.shader_template.txt")
                            {
                                if (PixelLabels[i] == "detail_map_value_scale")                                 //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "burn_scale")                                             //pixel constant index 0 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap half life - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[4]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_tint_color")                                         //vertex constant index 6 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[24])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[25])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[26])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[27])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //vertex constant index 7 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[28])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[29])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[30])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[31])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //vertex constant index 8 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[32])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[33])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[34]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[35]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //vertex constant index 9 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[36]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[37]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[38]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[39]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 2 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum Four Change Color No Lod
                            if (ShaderTemplate == "tex_bump_env_illum_four_change_color_no_lod.shader_template.txt")
                            {
                                if (PixelLabels[i] == "ambient_factor")   //Not written to shad file - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_tint_color")                                       //vertex constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[16])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[17])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[18])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[19])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                              //vertex constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[20])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[21])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[22])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[23])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                      //vertex constant index 6 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[24]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[25]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[26]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[27]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                             //vertex constant index 7 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[28]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[29]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[30]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[31]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum Two Change Color
                            if (ShaderTemplate == "tex_bump_env_illum_two_change_color.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                       //vertex constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                              //vertex constant index 6 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[24]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[25]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[26]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[27]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                      //vertex constant index 7 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[28]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[29]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[30]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[31]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                             //vertex constant index 8 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[32]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[33]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[34]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[35]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Illum
                            if (ShaderTemplate == "tex_bump_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Illum Alpha Test
                            if (ShaderTemplate == "tex_bump_illum_alpha_test.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 2 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Illum Bloom
                            if (ShaderTemplate == "tex_bump_illum_bloom.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "bloom") //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Meter Illum
                            if (ShaderTemplate == "tex_bump_meter_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "meter_on_color")   //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "meter_off_color")  //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "meter_value")      //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Plasma One Channel Illum
                            if (ShaderTemplate == "tex_bump_plasma_one_channel_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "channel_a_color")  //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color")  //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color")  //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "time")             //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                               //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Two Add Env Illum
                            if (ShaderTemplate == "two_add_env_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "environment_color")  //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")  //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")  //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")  //vertex constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")             //vertex constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")  //vertex constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")             //vertex constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                               //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Two Alpha Env Illum
                            if (ShaderTemplate == "two_alpha_env_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "environment_color")  //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")  //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")  //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")  //vertex constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")             //vertex constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")  //vertex constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")             //vertex constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_translucent_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[6]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[7]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[8]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_translucent_alpha")                               //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[5]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                               //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            if (!SupportedShaders.Contains(ShaderTemplate))
                            {
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                            }

                            #endregion
                            // 5 time 5 time 5 time 5 time 5 time WCW Champion

                            bw.Write(Convert.ToInt32(-1));

                            bw.Write(Convert.ToInt32(10088844));
                            //Look I don't design the tags I just write them to the file. I need to write 10088844 and I don't care why.
                        }
                        for (int i = 0; i < ShaderParameterCount; i++)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(BitmapLabels[i]));
                            bw.Write(Encoding.UTF8.GetBytes(sectionBitmap[i]));
                            if (sectionBitmap[i].Length > 0)
                            {
                                bw.Write(Convert.ToByte(0));
                            }
                            /*if (BitmapLabels[i] == "detail_map")
                            {
                                bw.Write(Encoding.UTF8.GetBytes("dfbt"));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                            }*/
                        }
                        for (int i = 0; i < PixelParameterCount; i++)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(PixelLabels[i]));
                        }
                        ms.Position = 0;
                        ds.Position = 0;
                        ms.CopyTo(ds);

                        ms.Close();
                        fs.Close();
                        ds.Close();
                    }
                }
                MessageBox.Show("Finished Creating Tags");
            }
        }

        private void CreatePluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Elmer Source Code
            // <3 Hamp

            string tagdirectory = "";

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;

            MessageBox.Show("Please select a directory containing .shader_template files. These will be converted to plugins for the shader emulator.");

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tagdirectory = fbd.SelectedPath;

                var stfiles = Directory.GetFiles(tagdirectory, $"*.shader_template", SearchOption.AllDirectories);
                var stfiles2 = Directory.GetFiles(tagdirectory, $"*.shader_template", SearchOption.AllDirectories);
                string outpath = Path.Combine(Environment.CurrentDirectory, "plugins", "shaderstemplates");
                string outpath2 = Path.Combine(Environment.CurrentDirectory, "plugins", "pixeltemplates");
                Directory.CreateDirectory(outpath);
                Directory.CreateDirectory(outpath2);

                foreach (var stfile2 in stfiles2)
                {
                    int magic = new int();

                    int propcount = new int();
                    int catcount = new int();

                    List<string> pixelID = new List<string>();

                    using (var fs = new FileStream(stfile2, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    using (var ms = new MemoryStream())
                    using (var bw = new BinaryWriter(ms))
                    using (var br = new BinaryReader(ms))
                    {
                        fs.CopyTo(ms);
                        ms.Position = 0;

                        br.BaseStream.Seek(80, SeekOrigin.Begin);
                        magic = magic + br.ReadInt16();

                        br.BaseStream.Seek(108, SeekOrigin.Begin);
                        propcount = br.ReadInt16();

                        br.BaseStream.Seek(120, SeekOrigin.Begin);
                        catcount = br.ReadInt16();

                        int[] catskipcount = new int[catcount];   // we use this as a collection of both description text length
                                                                  // as well as any other random data of indeterminate length 
                                                                  // along the way that we need to skip reading over. Sloppy! <3 Hamp
                        int[] parcount = new int[catcount];

                        br.BaseStream.Seek(236 + magic, SeekOrigin.Begin);

                        if (propcount > 0)
                        {
                            br.BaseStream.Seek(16, SeekOrigin.Current);
                            magic = magic + 16;
                        }

                        for (int i = 0; i < propcount; i++)
                        {
                            br.BaseStream.Seek(7, SeekOrigin.Current);
                            magic = magic + br.ReadByte();
                        }

                        magic = magic + (propcount * 8);

                        br.BaseStream.Seek(236 + magic, SeekOrigin.Begin);
                        //MessageBox.Show(br.BaseStream.Position.ToString() + "test");

                        if (catcount > 0)
                        {
                            br.BaseStream.Seek(16, SeekOrigin.Current);
                            magic = magic + 16;
                        }

                        for (int i = 0; i < catcount; i++)
                        {
                            br.BaseStream.Seek(3, SeekOrigin.Current);
                            catskipcount[i] = catskipcount[i] + br.ReadByte();
                            parcount[i] = br.ReadByte();

                            if (parcount[i] == 0)
                            {
                                parcount[i] = 1;
                            }
                            br.BaseStream.Seek(11, SeekOrigin.Current);
                        }

                        magic = magic + (catcount * 16);

                        br.BaseStream.Seek(236 + magic, SeekOrigin.Begin);

                        for (int i = 0; i < catcount; i++)
                        {
                            br.BaseStream.Seek(catskipcount[i], SeekOrigin.Current);
                            br.BaseStream.Seek(16, SeekOrigin.Current);

                            int[] descriptionlength = new int[parcount[i]];
                            int[] namelength = new int[parcount[i]];
                            int[] tagpath = new int[parcount[i]];
                            int[] ID = new int[parcount[i]];
                            for (int n = 0; n < parcount[i]; n++)
                            {
                                namelength[n] = 0;
                                br.BaseStream.Seek(2, SeekOrigin.Current);

                                byte[] flip = br.ReadBytes(2);
                                Array.Reverse(flip);
                                namelength[n] = BitConverter.ToInt16(flip, 0);

                                descriptionlength[n] = br.ReadInt16();

                                br.BaseStream.Seek(18, SeekOrigin.Current);
                                ID[n] = br.ReadInt16();

                                br.BaseStream.Seek(10, SeekOrigin.Current);
                                tagpath[n] = br.ReadInt16();

                                br.BaseStream.Seek(34, SeekOrigin.Current);
                            }

                            for (int n = 0; n < parcount[i]; n++)
                            {

                                if (ID[n] >= 1)
                                {
                                    //MessageBox.Show(br.BaseStream.Position.ToString() + "--nl" + namelength[n]);
                                    pixelID.Add(new string(br.ReadChars(namelength[n])));
                                    //MessageBox.Show("Wrote-" + bitmapID[bitmapID.Count - 1]);
                                }

                                else
                                {
                                    br.BaseStream.Seek(namelength[n], SeekOrigin.Current);
                                    //MessageBox.Show("Skipped-"+namelength[n].ToString());
                                }
                                //MessageBox.Show(br.BaseStream.Position.ToString() + "--ps" + descriptionlength[n]);
                                br.BaseStream.Seek(descriptionlength[n], SeekOrigin.Current);

                                if (tagpath[n] > 0)
                                {
                                    // MessageBox.Show(br.BaseStream.Position.ToString() + "--tp" + tagpath[n] + 1);
                                    br.BaseStream.Seek(tagpath[n] + 1, SeekOrigin.Current);
                                }
                            }
                        }

                        string output2 = Path.Combine(outpath2, Path.GetFileName(stfile2) + "_pixel.txt");

                        System.IO.File.WriteAllText(output2, "");

                        foreach (var pixel in pixelID)
                        {
                            //  MessageBox.Show(bitm);
                            System.IO.File.AppendAllText(output2, pixel + Environment.NewLine);
                        }
                    }
                }
                foreach (var stfile in stfiles)
                {
                    int magic = new int();

                    int propcount = new int();
                    int catcount = new int();

                    List<string> bitmapID = new List<string>();

                    using (var fs = new FileStream(stfile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    using (var ms = new MemoryStream())
                    using (var bw = new BinaryWriter(ms))
                    using (var br = new BinaryReader(ms))
                    {
                        fs.CopyTo(ms);
                        ms.Position = 0;

                        br.BaseStream.Seek(80, SeekOrigin.Begin);
                        magic = magic + br.ReadInt16();

                        br.BaseStream.Seek(108, SeekOrigin.Begin);
                        propcount = br.ReadInt16();

                        br.BaseStream.Seek(120, SeekOrigin.Begin);
                        catcount = br.ReadInt16();

                        int[] catskipcount = new int[catcount];   // we use this as a collection of both description text length
                                                                  // as well as any other random data of indeterminate length 
                                                                  // along the way that we need to skip reading over. Sloppy! <3 Hamp
                        int[] parcount = new int[catcount];

                        br.BaseStream.Seek(236 + magic, SeekOrigin.Begin);

                        if (propcount > 0)
                        {
                            br.BaseStream.Seek(16, SeekOrigin.Current);
                            magic = magic + 16;
                        }

                        for (int i = 0; i < propcount; i++)
                        {
                            br.BaseStream.Seek(7, SeekOrigin.Current);
                            magic = magic + br.ReadByte();
                        }

                        magic = magic + (propcount * 8);

                        br.BaseStream.Seek(236 + magic, SeekOrigin.Begin);
                        //MessageBox.Show(br.BaseStream.Position.ToString() + "test");

                        if (catcount > 0)
                        {
                            br.BaseStream.Seek(16, SeekOrigin.Current);
                            magic = magic + 16;
                        }

                        for (int i = 0; i < catcount; i++)
                        {
                            br.BaseStream.Seek(3, SeekOrigin.Current);
                            catskipcount[i] = catskipcount[i] + br.ReadByte();
                            parcount[i] = br.ReadByte();

                            if (parcount[i] == 0)
                            {
                                parcount[i] = 1;
                            }
                            br.BaseStream.Seek(11, SeekOrigin.Current);
                        }

                        magic = magic + (catcount * 16);

                        br.BaseStream.Seek(236 + magic, SeekOrigin.Begin);

                        for (int i = 0; i < catcount; i++)
                        {
                            br.BaseStream.Seek(catskipcount[i], SeekOrigin.Current);
                            br.BaseStream.Seek(16, SeekOrigin.Current);

                            int[] descriptionlength = new int[parcount[i]];
                            int[] namelength = new int[parcount[i]];
                            int[] tagpath = new int[parcount[i]];
                            int[] ID = new int[parcount[i]];
                            for (int n = 0; n < parcount[i]; n++)
                            {
                                namelength[n] = 0;
                                br.BaseStream.Seek(2, SeekOrigin.Current);

                                byte[] flip = br.ReadBytes(2);
                                Array.Reverse(flip);
                                namelength[n] = BitConverter.ToInt16(flip, 0);

                                descriptionlength[n] = br.ReadInt16();

                                br.BaseStream.Seek(18, SeekOrigin.Current);
                                ID[n] = br.ReadInt16();

                                br.BaseStream.Seek(10, SeekOrigin.Current);
                                tagpath[n] = br.ReadInt16();

                                br.BaseStream.Seek(34, SeekOrigin.Current);
                            }

                            for (int n = 0; n < parcount[i]; n++)
                            {

                                if (ID[n] == 0)
                                {
                                    //MessageBox.Show(br.BaseStream.Position.ToString() + "--nl" + namelength[n]);
                                    bitmapID.Add(new string(br.ReadChars(namelength[n])));
                                    //MessageBox.Show("Wrote-" + bitmapID[bitmapID.Count - 1]);
                                }

                                else
                                {
                                    br.BaseStream.Seek(namelength[n], SeekOrigin.Current);
                                    //MessageBox.Show("Skipped-"+namelength[n].ToString());
                                }
                                //MessageBox.Show(br.BaseStream.Position.ToString() + "--ps" + descriptionlength[n]);
                                br.BaseStream.Seek(descriptionlength[n], SeekOrigin.Current);

                                if (tagpath[n] > 0)
                                {
                                    // MessageBox.Show(br.BaseStream.Position.ToString() + "--tp" + tagpath[n] + 1);
                                    br.BaseStream.Seek(tagpath[n] + 1, SeekOrigin.Current);
                                }
                            }
                        }
                        string output = Path.Combine(outpath, Path.GetFileName(stfile) + ".txt");

                        System.IO.File.WriteAllText(output, "");

                        foreach (var bitm in bitmapID)
                        {
                            //  MessageBox.Show(bitm);
                            System.IO.File.AppendAllText(output, bitm + Environment.NewLine);
                        }
                    }
                }

                string blankshader = Path.Combine(outpath, "blank.shader");

                using (var fs = new FileStream(blankshader, FileMode.Create, FileAccess.ReadWrite))
                using (var ms = new MemoryStream())
                using (var bw = new BinaryWriter(ms))
                using (var br = new BinaryReader(ms))

                {
                    fs.CopyTo(ms);
                    ms.Position = 0;

                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Encoding.UTF8.GetBytes("dahs"));

                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Convert.ToInt32(64));

                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Convert.ToInt32(-16777215));

                    bw.Write(Encoding.UTF8.GetBytes("!MLBdfbt"));

                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Convert.ToInt32(1));

                    bw.Write(Convert.ToInt32(128));

                    bw.Write(Encoding.UTF8.GetBytes("mets"));

                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Convert.ToInt32(-1));

                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Convert.ToInt32(-1));

                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Convert.ToInt32(-1));

                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Convert.ToInt32(-1));

                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Convert.ToInt32(-1));

                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Encoding.UTF8.GetBytes("tils"));

                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Convert.ToInt32(-1));

                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));

                    bw.Write(Convert.ToInt32(-1));

                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));
                    bw.Write(Convert.ToInt32(0));

                    ms.Position = 0;
                    ms.CopyTo(fs);
                }
                MessageBox.Show("Finished Writing Plugins");
            }
        }

        private void DumpShadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                if (map_loaded)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    fbd.ShowNewFolderButton = true;

                    MessageBox.Show("Select a directory to export the shader dump. Preferably an empty folder.");

                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string tags_directory = fbd.SelectedPath;

                        StreamWriter log = new StreamWriter(tags_directory + '\\' + map_name.Substring(map_name.LastIndexOf('\\') + 1) + ".shader_log");

                        //StringId list to dump maerial name
                        List<StringID_info> StringID_list = new List<StringID_info>();

                        int string_table_count = DATA_READ.ReadINT_LE(0x170, map_stream);
                        int string_index_table_offset = DATA_READ.ReadINT_LE(0x178, map_stream);
                        int string_table_offset = DATA_READ.ReadINT_LE(0x17C, map_stream);

                        for (int index = 0; index < string_table_count; index++)
                        {
                            int table_off = DATA_READ.ReadINT_LE(string_index_table_offset + index * 0x4, map_stream) & 0xFFFF;
                            string STRING = DATA_READ.ReadSTRING(string_table_offset + table_off, map_stream);

                            if (STRING.Length > 0)
                            {
                                int SID = DATA_READ.Generate_SID(index, 0x0, STRING);//set is 0x0 cuz i couldnt figure out any other value

                                StringID_info SIDI = new StringID_info();
                                SIDI.string_index_table_index = string_index_table_offset + index * 0x4;
                                SIDI.string_table_offset = table_off;
                                SIDI.StringID = SID;
                                SIDI.STRING = STRING;

                                StringID_list.Add(SIDI);
                            }
                        }

                        foreach (TreeNode element in treeView1.Nodes["shad"].Nodes)
                        {
                            int table_ref = Int32.Parse(element.Name);
                            int datum = DATA_READ.ReadINT_LE(table_ref + 4, map_stream);
                            int mem_off = DATA_READ.ReadINT_LE(table_ref + 8, map_stream);
                            int size = DATA_READ.ReadINT_LE(table_ref + 0xc, map_stream);

                            meta meta_obj = new meta(datum, SID_list[datum], map_stream);
                            meta_obj.Rebase_meta(0x0);

                            if (meta_obj.Get_Total_size() != 0)
                            {
                                byte[] meta_data = meta_obj.Generate_meta_file();

                                string text_path = tags_directory + '\\' + SID_list[datum] + ".txt";

                                //lets create our directory
                                System.IO.Directory.CreateDirectory(DATA_READ.ReadDirectory_from_file_location(text_path));

                                StreamWriter sw = new StreamWriter(text_path);

                                //supoosing each shad contains only one Post process block element
                                int PPB_off = DATA_READ.ReadINT_LE(0x24, meta_data);
                                int RTP_off = DATA_READ.ReadINT_LE(0x10, meta_data);

                                int stem_datum = DATA_READ.ReadINT_LE(PPB_off, meta_data);
                                int bitmap_count = DATA_READ.ReadINT_LE(PPB_off + 0x4, meta_data);
                                int bitmapB_off = DATA_READ.ReadINT_LE(PPB_off + 0x8, meta_data);
                                int pixel_const_count = DATA_READ.ReadINT_LE(PPB_off + 0xC, meta_data);
                                int pixel_const_off = DATA_READ.ReadINT_LE(PPB_off + 0x10, meta_data);
                                int vertex_const_count = DATA_READ.ReadINT_LE(PPB_off + 0x14, meta_data);
                                int vertex_const_off = DATA_READ.ReadINT_LE(PPB_off + 0x18, meta_data);

                                sw.WriteLine("### SHADER PARAMETERS ###");
                                //write the stemp path
                                string out_temp;
                                if (stem_datum != 0 && stem_datum != -1)
                                {
                                    if (SID_list.TryGetValue(stem_datum, out out_temp))
                                        sw.WriteLine(SID_list[stem_datum]);
                                    else sw.WriteLine("---");
                                }
                                //write the material name
                                int mat_StringId = DATA_READ.ReadINT_LE(0x8, meta_data);
                                for (int i = 0; i < StringID_list.Count; i++)
                                {
                                    if (StringID_list[i].StringID == mat_StringId)
                                    {
                                        sw.WriteLine(StringID_list[i].STRING);
                                        break;
                                    }
                                    else if (i == (StringID_list.Count - 1))
                                        sw.WriteLine("");
                                }
                                //write the flags                            
                                sw.WriteLine(BitConverter.ToInt16(meta_data, 0x16));
                                //write depth bias offset
                                sw.WriteLine(BitConverter.ToSingle(meta_data, 0x54));
                                //write depth bias slope scale
                                sw.WriteLine(BitConverter.ToSingle(meta_data, 0x58));
                                //write dynamic specular type
                                sw.WriteLine(BitConverter.ToInt16(meta_data, 0x3E));
                                //write Lightmap type
                                sw.WriteLine(BitConverter.ToInt16(meta_data, 0x40));
                                //write lightmap specular brightness
                                sw.WriteLine(BitConverter.ToSingle(meta_data, 0x44));
                                //write Lightmap Ambient Bias
                                sw.WriteLine(BitConverter.ToSingle(meta_data, 0x48));
                                //write Shader LOD Bias
                                sw.WriteLine(BitConverter.ToInt16(meta_data, 0x3C));
                                sw.WriteLine("### SHADER PARAMETERS END ###");
                                sw.WriteLine("");
                                sw.WriteLine("### BITMAPS ###");
                                //dump the bitmap names  
                                for (int i = 0; i < bitmap_count; i++)
                                {
                                    int bitm_datum = DATA_READ.ReadINT_LE(bitmapB_off + i * 0xC, meta_data);

                                    if (bitm_datum != 0 && bitm_datum != -1)
                                    {
                                        if (SID_list.TryGetValue(bitm_datum, out out_temp))
                                            if (SID_list[bitm_datum] == "")
                                            {
                                                sw.WriteLine(" ");
                                            }
                                            else
                                            {
                                                sw.WriteLine(SID_list[bitm_datum]);
                                            }
                                        else sw.WriteLine("---");
                                    }
                                    else
                                    {
                                        sw.WriteLine(" ");
                                    }
                                }
                                sw.WriteLine("### BITMAPS END ###");
                                sw.WriteLine("");
                                sw.WriteLine("### LIGHTMAP ###");
                                sw.WriteLine(BitConverter.ToSingle(meta_data, RTP_off + 0x1C)); //A - Write lightmap emmisive power
                                sw.WriteLine(BitConverter.ToSingle(meta_data, RTP_off + 0x10)); //R - Write lightmap emmisive color
                                sw.WriteLine(BitConverter.ToSingle(meta_data, RTP_off + 0x14)); //G - Write lightmap emmisive color
                                sw.WriteLine(BitConverter.ToSingle(meta_data, RTP_off + 0x18)); //B - Write lightmap emmisive color
                                sw.WriteLine(BitConverter.ToSingle(meta_data, RTP_off + 0x24)); // Write lightmap half life
                                sw.WriteLine(BitConverter.ToSingle(meta_data, RTP_off + 0x48)); //A - Write lightmap transparent alpha
                                sw.WriteLine(BitConverter.ToSingle(meta_data, RTP_off + 0x3C)); //R - Write lightmap transparent color
                                sw.WriteLine(BitConverter.ToSingle(meta_data, RTP_off + 0x40)); //G - Write lightmap transparent color
                                sw.WriteLine(BitConverter.ToSingle(meta_data, RTP_off + 0x44)); //B - Write lightmap transparent color

                                sw.WriteLine("### LIGHTMAP END ###");
                                sw.WriteLine("");
                                sw.WriteLine("### PIXEL CONSTANTS ###");
                                //write pixel constants with each A,R,G,B in seperate lines
                                for (int i = 0; i < pixel_const_count; i++)
                                {
                                    uint colour = BitConverter.ToUInt32(meta_data, pixel_const_off + i * 0x4);
                                    sw.WriteLine(colour >> 24);
                                    sw.WriteLine((colour >> 16) & 0x000000ff);
                                    sw.WriteLine((colour >> 8) & 0x000000ff);
                                    sw.WriteLine(colour & 0x000000ff);
                                }
                                sw.WriteLine("### PIXEL CONSTANTS END ###");
                                sw.WriteLine("");
                                sw.WriteLine("### VERTEX CONSTANTS ###");
                                for (int i = 0; i < vertex_const_count; i++)
                                {
                                    sw.WriteLine(BitConverter.ToSingle(meta_data, (vertex_const_off + (i * 0x10)) + 0xC)); // A - Write vertex constant w
                                    sw.WriteLine(BitConverter.ToSingle(meta_data, (vertex_const_off + (i * 0x10)) + 0x0)); // R - Write vertex constant l
                                    sw.WriteLine(BitConverter.ToSingle(meta_data, (vertex_const_off + (i * 0x10)) + 0x4)); // G - Write vertex constant j
                                    sw.WriteLine(BitConverter.ToSingle(meta_data, (vertex_const_off + (i * 0x10)) + 0x8)); // B - Write vertex constant k
                                }
                                sw.WriteLine("### VERTEX CONSTANTS END ###");

                                log.WriteLine(SID_list[datum] + ".txt");

                                sw.Close();
                            }
                            else
                            {
                                log.WriteLine("---");
                            }
                        }
                        log.Close();
                        MessageBox.Show("Extraction Complete");
                    }
                }
                else MessageBox.Show("Load a map first");
            }
        }

        private void dumpTagListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map_loaded)
            {
                SaveFileDialog svd = new SaveFileDialog();

                svd.FileName = map_name.Substring(map_name.LastIndexOf('\\') + 1) + "_tag_list.txt";
                svd.Filter = "Text File|*.txt";

                if (svd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    StreamWriter log = new StreamWriter(svd.FileName);

                    var key_list = SID_list.Keys.ToList();

                    for (int i = 0; i < key_list.Count; i++)
                        log.WriteLine("0x" + key_list[i].ToString("X") + ',' + SID_list[key_list[i]]);

                    log.Close();
                }
            }
        }

        private void dumpStringIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map_loaded)
            {
                SaveFileDialog svd = new SaveFileDialog();

                svd.FileName = map_name.Substring(map_name.LastIndexOf('\\') + 1) + "_StringID_list.txt";
                svd.Filter = "Text File|*.txt";

                if (svd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    StreamWriter log = new StreamWriter(svd.FileName);

                    List<StringID_info> StringID_list = new List<StringID_info>();

                    int string_table_count = DATA_READ.ReadINT_LE(0x170, map_stream);
                    int string_index_table_offset = DATA_READ.ReadINT_LE(0x178, map_stream);
                    int string_table_offset = DATA_READ.ReadINT_LE(0x17C, map_stream);

                    for (int index = 0; index < string_table_count; index++)
                    {
                        int table_off = DATA_READ.ReadINT_LE(string_index_table_offset + index * 0x4, map_stream) & 0xFFFF;
                        string STRING = DATA_READ.ReadSTRING(string_table_offset + table_off, map_stream);

                        if (STRING.Length > 0)
                        {
                            int SID = DATA_READ.Generate_SID(index, 0x0, STRING);//set is 0x0 cuz i couldnt figure out any other value

                            StringID_info SIDI = new StringID_info();
                            SIDI.string_index_table_index = string_index_table_offset + index * 0x4;
                            SIDI.string_table_offset = table_off;
                            SIDI.StringID = SID;
                            SIDI.STRING = STRING;

                            StringID_list.Add(SIDI);
                        }
                    }

                    for (int i = 0; i < StringID_list.Count; i++)
                        log.WriteLine("0x" + StringID_list[i].StringID.ToString("X") + ',' + StringID_list[i].STRING);

                    log.Close();

                }
            }
        }
    }
}