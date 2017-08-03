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
using BlamLib.Test;
using DATA_STRUCTURES;

/*
Created and developed by Himanshu01
Also uses BlamLib by Kornner Studios for tag extraction 
*/


namespace Map_Handler
{

    public partial class MainBox : Form
    {
        #region EXTRACTION_RELATED_VARS                
        StreamReader map_stream;//stream reader
        int table_off;//offset of the table
        int table_start;//start of the Actual Tables
        int table_size;//size of the table
        int file_table_offset;//file table offset from where the strings begin

        bool map_loaded = false;//is the map loaded
        int scnr_memaddr;
        int scnr_off;
        string map_name;//name of the map along woth destination

        List<int> datum_list = new List<int>();
        Halo2 obj = new Halo2();//Blam Lib onject
        int index = 0;//progress bar stuff and tag extraction

        Dictionary<int, string> SID_list;//a list containing string index stuff

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

                table_off = DATA_READ.ReadINT_LE(0x10, map_stream);
                table_size = DATA_READ.ReadINT_LE(0x14, map_stream);
                file_table_offset = DATA_READ.ReadINT_LE(0x2D0, map_stream);

                table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4, map_stream) + 0x20;

                scnr_off = table_off + table_size;
                scnr_memaddr = DATA_READ.ReadINT_LE(table_start + 0x8, map_stream);//scnr tag index is 0x0

                map_name =map_file.FileName;

                initialize_treeview();
                map_loaded = true;
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
                        datum_list.Add(datum_index);//lets add this to the list               


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
        private void extractTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(map_loaded)
            {                

                if (treeView1.SelectedNode!=null)
                {
                    //Extraction for a single tag
                    if (treeView1.SelectedNode.Name.CompareTo(treeView1.SelectedNode.Text) != 0)
                    {
                        
                        int tag_table_ref = Int32.Parse(treeView1.SelectedNode.Name);
                        int datum_index = DATA_READ.ReadINT_LE(tag_table_ref + 4, map_stream);

                        map_stream.Close();                       
                        obj.Halo2_ExtractTagCache(datum_index, DATA_READ.Read_File_from_file_location(map_name));
                        map_stream = new StreamReader(map_name);
                        
                    }
                    else
                    {
                        //Extraction for a whole same bunch of tags
                        List<int> DatumsList = new List<int>();
                        foreach (TreeNode tn in treeView1.SelectedNode.Nodes)
                        {
                            int tag_table_ref = Int32.Parse(tn.Name);
                            int datum_index = DATA_READ.ReadINT_LE(tag_table_ref + 4,map_stream);
                            DatumsList.Add(datum_index);
                        }

                        map_stream.Close();

                        int index = 1;

                        foreach (int i in DatumsList)
                        {
                            obj.Halo2_ExtractTagCache(i, DATA_READ.Read_File_from_file_location(map_name));

                            progressBar1.Value = (index++)*100 / DatumsList.Count;//update the progress bar
                        }                        

                        map_stream = new StreamReader(map_name);                       


                    }                
                }

            }
            progressBar1.Value = 0;//reset the progress bar

        }

        private void decompileMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map_loaded)
            {

                map_stream.Close();//has to close BlamLib issues
                map_loaded = false;//sorry you cannot do any map stuff now

                timer2.Enabled=true;
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //i used timer instead of a loop because of the fancy progress bar(IT looks COOL)

            if (index < datum_list.Count)
            {
              
                    obj.Halo2_ExtractTagCache(datum_list[index], DATA_READ.Read_File_from_file_location(map_name));
                    progressBar1.Value = (index + 1) * 100 / datum_list.Count;//update the progress bar             

                    //we have to increment the index
                    index++;
                  
                
            }
            else
            {
                progressBar1.Value = 0;//reset the progres bar
                map_stream = new StreamReader(map_name);//lets load the map
                map_loaded = true;//well now u are now free
                index = 0;//reset the index
                timer2.Enabled = false;
                
            }

        }
        
        #endregion

    }
}
