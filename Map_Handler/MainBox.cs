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
*/


namespace Map_Handler
{

    public partial class MainBox : Form
    {
        #region EXTRACTION_RELATED_VARS                
        static StreamReader map_stream;//stream reader
        int table_off;//offset of the table
        int table_start;//start of the Actual Tables
        int table_size;//size of the table
        int file_table_offset;//file table offset from where the strings begin

        static bool map_loaded = false;//is the map loaded
        int scnr_memaddr;
        int scnr_off;
        public static string map_name="";//name of the map along woth destination

        List<int> datum_list = new List<int>();

        public static BlamLib.Test.Halo2 H2Test = new BlamLib.Test.Halo2(); //Blam Lib Tests project
        public static BlamLib.Blam.Halo2.Converter H2Convert = new BlamLib.Blam.Halo2.Converter();

     
        

        Dictionary<int, string> SID_list;//a list containing tag path stuff :P
        public static Dictionary<int, string> AllTagslist;
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
                AllTagslist = new Dictionary<int, string>();

                table_off = DATA_READ.ReadINT_LE(0x10, map_stream);
                table_size = DATA_READ.ReadINT_LE(0x14, map_stream);
                file_table_offset = DATA_READ.ReadINT_LE(0x2D0, map_stream);

                table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4, map_stream) + 0x20;

                scnr_off = table_off + table_size;
                scnr_memaddr = DATA_READ.ReadINT_LE(table_start + 0x8, map_stream);//scnr tag index is 0x0

                map_name =map_file.FileName;
                
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
                        AllTagslist.Add(datum_index, path);//Adding only Map Specific tags with Internal Reference only to list 
                                 


                    if (treeView1.Nodes.IndexOfKey(type) == -1)
                    {
                        treeView1.Nodes.Add(type, type);
                    }
                    int index = treeView1.Nodes.IndexOfKey(type);
                    //HEX Values contains ABCDEF
                    treeView1.Nodes[index].Nodes.Add(tag_table_REF.ToString(), path);

                    //add this stuff to the SID list
                    SID_list.Add(datum_index, path);

                    //ugh! is basically the last tag
                    if (type.CompareTo("ugh!") == 0)
                        break;

                    path_start += path.Length + 1;
                }


            }


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
        private void extractTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(map_loaded)
            {

                Dictionary<int, string> Extractlist = new Dictionary<int, string>();
                if (treeView1.SelectedNode!=null)
                {
                    //Extraction for a single tag
                    if (treeView1.SelectedNode.Name.CompareTo(treeView1.SelectedNode.Text) != 0)
                    {
                        
                        int tag_table_ref = Int32.Parse(treeView1.SelectedNode.Name);
                        int datum_index = DATA_READ.ReadINT_LE(tag_table_ref + 4, map_stream);                    
                        Extractlist.Add(datum_index, treeView1.SelectedNode.Text);
                        
                    }
                    else
                    {
                        //Extraction for a whole same bunch of tags
                        
                        foreach (TreeNode tn in treeView1.SelectedNode.Nodes)
                        {
                            int tag_table_ref = Int32.Parse(tn.Name);
                            int datum_index = DATA_READ.ReadINT_LE(tag_table_ref + 4,map_stream);
                            Extractlist.Add(datum_index, tn.Text);
                        }
                  
                    }
                    TagExtractor ob = new TagExtractor(Extractlist,false);
                    ob.Show();                    

                }
                else
                {
                     MessageBox.Show("Select a Tag First!", "CRASHED!!", MessageBoxButtons.OK);                  
                                          
                }

            }
            else
            {
                MessageBox.Show("No Map Loaded ,Reload it", "Error!!", MessageBoxButtons.OK);
            }




        }

        private void decompileMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map_loaded)
            {                            
                TagExtractor ob = new TagExtractor(AllTagslist,true);        
                ob.Show();
            }

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

            TagToolStripMenu.Visible = false;
            metaToolStripMenuItem.Visible = false;

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseMap();
            Application.Exit();
        }

        private void defaultMaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ToDO: Complete this UI
            WIP();
            /*

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;


            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                H2V_BaseMapsDirectory = fbd.SelectedPath;
                
            }*/
            
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

                File.WriteAllLines(Application.StartupPath + @"\TagsList.txt", x);
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

        private void sndtagFixesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "config files(*.xml)|*.xml";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                snd_fixes snd = new snd_fixes(ofd.FileName);
            }
        }

        private void dumpshadinfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map_loaded)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.ShowNewFolderButton = true;

                MessageBox.Show("Select tags directory");


                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string  tags_directory = fbd.SelectedPath;

                    StreamWriter log = new StreamWriter(tags_directory + '\\' + map_name.Substring(map_name.LastIndexOf('\\') + 1) + "_shad_logs.txt");

                    foreach (TreeNode element in treeView1.Nodes["shad"].Nodes)
                    {
                        int table_ref = Int32.Parse(element.Name);
                        int datum = DATA_READ.ReadINT_LE(table_ref + 4, map_stream);
                        int mem_off = DATA_READ.ReadINT_LE(table_ref + 8, map_stream);
                        int size = DATA_READ.ReadINT_LE(table_ref + 0xc, map_stream);

                        meta meta_obj = new meta(datum, SID_list[datum], map_stream);
                        meta_obj.Rebase_meta(0x0);

                        if (meta_obj.Get_Total_size()!=0)
                        {
                            byte[] meta_data = meta_obj.Generate_meta_file();

                            string text_path = tags_directory + '\\' + SID_list[datum] + ".txt";

                            //lets create our directory
                            System.IO.Directory.CreateDirectory(DATA_READ.ReadDirectory_from_file_location(text_path));

                            StreamWriter sw = new StreamWriter(text_path);

                            //supoosing each shad contains only one Post process block element
                            int PPB_off = DATA_READ.ReadINT_LE(0x24, meta_data);

                            int stem_datum = DATA_READ.ReadINT_LE(PPB_off, meta_data);
                            int bitmap_count = DATA_READ.ReadINT_LE(PPB_off + 0x4, meta_data);
                            int bitmapB_off = DATA_READ.ReadINT_LE(PPB_off + 0x8, meta_data);

                            //write the stemp path
                            string out_temp;
                            if (stem_datum != 0 && stem_datum != -1)
                            {
                                if (SID_list.TryGetValue(stem_datum,out out_temp))
                                    sw.WriteLine(SID_list[stem_datum]);
                                else sw.WriteLine("Couldnt find tag_path to index : 0x" + stem_datum.ToString("X"));
                            }
                            for (int i = 0; i < bitmap_count; i++)
                            {
                                int bitm_datum = DATA_READ.ReadINT_LE(bitmapB_off + i * 0xC, meta_data);
                                if (bitm_datum != 0 && bitm_datum != -1)
                                {
                                    if (SID_list.TryGetValue(bitm_datum, out out_temp))
                                        sw.WriteLine(SID_list[bitm_datum]);
                                    else sw.WriteLine("Couldnt find tag_path to index : 0x" + bitm_datum.ToString("X"));
                                }
                            }

                            log.WriteLine("Dumped log 0x" + datum.ToString("X") + " : \\" + SID_list[datum]+".txt");

                            sw.Close();
                        }
                        else
                        {
                            log.WriteLine("Couldnt read tag 0x" + datum.ToString("X") + " : " + mem_off.ToString("X") + " : " + size.ToString("X"));
                        }
                    }
                    log.Close();
                }
            }
            else MessageBox.Show("Load a map first");
        }
    }
}
