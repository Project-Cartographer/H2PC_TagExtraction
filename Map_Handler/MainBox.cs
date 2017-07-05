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

/*
Created and developed by Himanshu01
Also uses BlamLib by Kornner Studios for tag extraction 
*/


namespace Map_Handler
{

    public partial class MainBox : Form
    {
        List<int> datum_list = new List<int>();

        #region EXTRACTION_RELATED_VARS
        
        StreamReader map_stream;//stream reader
        int table_off;//offset of the table
        int table_start;//start of the Actual Tables
        int table_size;//size of the table
        int file_table_offset;//file table offset from where the strings begin
        string[] supported_type = { "char", "scnr", "sky " };//list of supported tag types
        MetaExtractor meta_extract;//Meta extraction dailogue BOX
        bool map_loaded = false;//is the map loaded
        string log = "LOG_START";//Log box text
        List<int> extracted_datums;//list of tags extracted
        int scnr_memaddr;
        int scnr_off;
        XmlTextWriter xw;//xml writer to write the config file
        string map_name;//name of the map
        Halo2 obj = new Halo2();

        int index = 0;//progress bar stuff and tag extraction

        #endregion

        #region INJECTION_RELATED_VARS

        struct tag_refs
        {
           public int old_datum;
           public int new_datum;
           public string file_name;
           public string type;
        };
        List<tag_refs> inject_list;

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

                table_off = ReadINT_LE(0x10);
                table_size = ReadINT_LE(0x14);
                file_table_offset = ReadINT_LE(0x2D0);

                table_start = table_off + 0xC * ReadINT_LE(table_off + 4) + 0x20;

                scnr_off = table_off + table_size;
                scnr_memaddr = ReadINT_LE(table_start + 0x8);//scnr tag index is 0x0

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

                string type = ReadTAG_TYPE(tag_table_REF);
                int datum_index = ReadINT_LE(tag_table_REF+4);

                //lets check the mem addrs validity before adding it to the list
                int mem_addr = ReadINT_LE(tag_table_REF + (datum_index & 0xffff) * 0x10 + 8);

                if (mem_addr!=0x0)
                    datum_list.Add(datum_index);//lets add this to the list
                        

                string path = ReadSTRING(file_table_offset+path_start);                


                    if (treeView1.Nodes.IndexOfKey(type) == -1)
                    {
                        treeView1.Nodes.Add(type, type);
                    }
                    int index = treeView1.Nodes.IndexOfKey(type);
                    //HEX Values contains ABCDEF
                    treeView1.Nodes[index].Nodes.Add(tag_table_REF.ToString(),path);
                
                //ugh! is basically the last tag
                if (type.CompareTo("ugh!") == 0)
                    break;

                path_start += path.Length+1;
            }


        }

        private void extractMetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map_loaded)
            {
                if (treeView1.SelectedNode != null)
                {
                    //Meta Data Extraction Dailogue BOX
                    meta_extract = new MetaExtractor();

                    timer1.Enabled = true;//it has to be done prior to show dialog box

                    meta_extract.ShowDialog();
                }
                else MessageBox.Show("Select a TAG", "Hint");
            }
            else
            {
                MessageBox.Show("Select a map First", "Hint");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CHECK_meta_dialogue_STATE();
        }

        /// <summary>
        /// To keep an eye in the Meta Dialog Box actions
        /// </summary>
        void CHECK_meta_dialogue_STATE()
        {

            if (meta_extract != null)
            {

                if (meta_extract.GET_EXTRACT())
                {
                    bool recursive = meta_extract.GET_RECURSIVE();
                    string directory = meta_extract.GET_DIRECTORY();
                    meta_extract.Close();
                    timer1.Enabled = false;


                    //resetting the log
                    log = "LOG START";

                    //reset the extracted datums
                    extracted_datums = new List<int>();


                    //extract the selected tag
                    //Parent nodes have same name and text,refer to initialise treeview function
                    if (treeView1.SelectedNode.Text.CompareTo(treeView1.SelectedNode.Name) != 0)
                    {                                             
                       
                        //the name is actually the key of a tree node
                        int tag_ref_table = int.Parse(treeView1.SelectedNode.Name);
                        //name depicts the following
                        int datum_index = ReadINT_LE(tag_ref_table + 0x4);                        

                        //creating an XmlTextWriter for our config file
                        xw = new XmlTextWriter(directory + "\\" + datum_index.ToString("X") + ".xml", Encoding.UTF8);
                        xw.Formatting = Formatting.Indented;//for a better xml text    
                        xw.WriteStartElement("config");


                        //extract the meta now                              
                        Extract_Meta(datum_index, directory, recursive);
                        
                    }
                    else
                    {
                        //extract the tag under the selected tag type node

                        //creating an XmlTextWriter for our config file
                        xw = new XmlTextWriter(directory + "\\" + treeView1.SelectedNode.Name + ".xml", Encoding.UTF8);
                        xw.Formatting = Formatting.Indented;//for a better xml text    
                        xw.WriteStartElement("config");

                        //loop through all the child nodes

                        foreach(TreeNode child in treeView1.SelectedNode.Nodes)
                        {
                            //the name is actually the key of a tree node
                            int tag_ref_table = int.Parse(child.Name);
                            //name depicts the following
                            int datum_index = ReadINT_LE(tag_ref_table + 0x4);

                            //extract the meta now                              
                            Extract_Meta(datum_index, directory, recursive);

                        }


                    }

                    //Display LOG BOX
                    LogBox log_form = new LogBox(log);
                    log_form.Show();

                    //close xml file link 
                    xw.WriteEndElement();
                    xw.Close();

                }
            }

        }

        //function display the tag structure
        private void getTagStructureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (map_loaded)
            {
                if (treeView1.SelectedNode != null)
                {
                    string type =ReadTAG_TYPE(Int32.Parse(treeView1.SelectedNode.Name));

                    TreeNode tn = Get_Tag_structure_Display_purpose(type);

                    if (tn != null)
                    {
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

        private void injectMetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Refresh the log text
            //i know it is a useless log
            log = "LOG START";

            //File dailogue
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "config files(*.xml)|*.xml";

            if (ofd.ShowDialog() == DialogResult.OK)
            {              
                XmlDocument xd = new XmlDocument();
                xd.Load(ofd.FileName);

                int new_index=0x3BA4;//new datum_indexes starting from 0x3BA4
                int absolute_off = 0x0;//the offset from the map_mem_base

                //list stuff
                inject_list = new List<tag_refs>();
                //lets do all the listing
                foreach (XmlNode Xn in xd.SelectNodes("config/tag"))
                {
                    tag_refs temp = new tag_refs();
                    
                    temp.old_datum = int.Parse(Xn.SelectSingleNode("datum").InnerText,NumberStyles.HexNumber);
                    temp.new_datum = new_index++;
                    temp.file_name = Xn.SelectSingleNode("name").InnerText;
                    temp.type = ReadTAG_TYPE_form_name(temp.file_name);

                    //lets add the tag to the list
                    inject_list.Add(temp);
                }
                //the File where we are gonna compile our stuff
                StreamWriter sw = new StreamWriter(ReadDirectory_from_file_location(ofd.FileName) + "\\tags.meta");
                //the Tables data
                StreamWriter sw_t = new StreamWriter(ReadDirectory_from_file_location(ofd.FileName) + "\\tables.meta");
                //creating a table
                byte[] tables = new byte[0x10*inject_list.Count];
                //index of custom metas
                int custom_table_index=0;

                //now the listing thing is done lets do the actual thing
                //now lets compile(manage) all the concerned meta
                foreach(tag_refs temp_ref in inject_list)
                {
                    if (File.Exists(ReadDirectory_from_file_location(ofd.FileName)+"\\"+temp_ref.file_name))
                    {
                        //lets open the file
                        FileStream fs = new FileStream(ReadDirectory_from_file_location(ofd.FileName) + "\\" + temp_ref.file_name,FileMode.Append);
                        long size = fs.Position;
                        fs.Close();

                        //lets load it into memory
                        byte[] meta = new byte[size];

                        //Filestream imposed some probs
                        StreamReader sr = new StreamReader(ReadDirectory_from_file_location(ofd.FileName) + "\\" + temp_ref.file_name);
                        //lets read the data
                        sr.BaseStream.Read(meta, 0, (int)size);

                        //tag structure
                        TreeNode tag_structure = Get_Tag_structure(temp_ref.type);
                        //the modification part
                        Reconfig_meta(meta, 0x0, absolute_off, tag_structure.Nodes);

                        //write it to the file
                        sw.BaseStream.Write(meta, 0, (int)size);

                        //write the tables
                        int S = temp_ref.new_datum;
                        int M = absolute_off;
                        int r;                       
                        for(int i=0;i<4;i++)
                        {
                            //write the type
                            if (i < temp_ref.type.Length)
                            {
                                tables[custom_table_index * 0x10 + 3-i] = (byte)temp_ref.type[i];
                            }
                            //write the datum index
                            r = S % 0x100;
                            tables[custom_table_index * 0x10 + 4 + i] = (byte)r;
                            //write the memory address
                            r = M % 0x100;
                            tables[custom_table_index * 0x10 + 8 + i] = (byte)r;

                            S /= 0x100;
                            M /= 0x100;
                                                    
                        }

                        log += "\n Written tag " + temp_ref.file_name + " with new datum as " + temp_ref.new_datum.ToString("X");
                        //increase the tag_offset
                        absolute_off += (int)size;
                        
                    }
                    else log += "\nFile doesnt exists : " + temp_ref.file_name;
                    custom_table_index++;
                }
                //close the stream handle
                sw.Close();
                //lets write the tables
                sw_t.BaseStream.Write(tables, 0, 0x10 * inject_list.Count);
                //close the handle
                sw_t.Close();
                //Time to display the log
                LogBox LB = new LogBox(log);
                LB.Show();

            }
        }
        
        void Reconfig_meta(byte[] meta, int off, int abs_mem_addr, TreeNodeCollection Tnc)
        {
            foreach (TreeNode temp in Tnc)
            {
                if (temp.Text[0] == 'T')
                {
                   
                        int tag_ref_off = Int32.Parse(temp.Name.Substring(2), NumberStyles.HexNumber);//the offset to the tag_ref
                        int ref_datum = 0;//the old reffered datum

                        //Read the little endian datum_index
                        for (int i = 0; i < 4; i++)
                        {
                            ref_datum += (int)Math.Pow(0x100, 3 - i) * meta[off + tag_ref_off + 4 + 3 - i];
                        }

                    //now lets find the old mentioned tag in the tag list which we are compiling
                    //and update it with the new one
                    bool succesffully_reffered = false;//if i have successfully refered the tag(single tag)

                    foreach(tag_refs temp_tag_ref in inject_list)
                    {
                        if(temp_tag_ref.old_datum==ref_datum)
                        {
                            if (!succesffully_reffered)
                            {
                                //you cannot refer multiple tags with same old datums
                                //thats a problem in your tags and should be fixed in the configuration xml

                                succesffully_reffered = true;

                                int S = temp_tag_ref.new_datum;
                                for (int i = 0; i < 4; i++)
                                {
                                    int r = S % 0x100;
                                                                        
                                    meta[off + tag_ref_off + 3 - i] = (byte)temp_tag_ref.type[i];
                                    meta[off + tag_ref_off + 4 + i] = (byte)(r);

                                    S /= 0x100;
                                }
                            }
                            else log += "\n Multiple meta has same OLD Datum : " + ref_datum.ToString("X")+" \nPlease fix your config files ";
                        }
                    }
                    if (!succesffully_reffered)
                    {
                        //if i cant find any reference in the config files
                        log += "\nCouldnt find reference to the tag : " + ref_datum.ToString("X");
                    }


                }
                else if (temp.Text[0] == 'R')
                {
                    int field_table_off = Int32.Parse(temp.Name.Substring(2), NumberStyles.HexNumber);

                    int count = 0;
                    int rel_off = 0;
                    int abs_off;//absolute offset of the concerned field from the starting of the mem_base
                    int entry_size = Int32.Parse(temp.Text.Substring(3), NumberStyles.HexNumber);//its kind of T0x32,T0x28,R0x32,R0x28

                    //lets read the little endian stuff
                    for (int i = 0; i < 4; i++)
                    {
                        count += (int)Math.Pow(0x100, i) * meta[off + field_table_off + i];
                        rel_off += (int)Math.Pow(0x100, i) * meta[off + field_table_off + 4 + i];
                    }
                    //we do any manipulations if the field contains any elements
                    if (count > 0)
                    {
                        abs_off = rel_off +abs_mem_addr;

                        //now lets update the mem_addr with absolute offse

                        //writing in the meta data in little endian format
                        int S = abs_off;
                        for (int j = 0; j < 4; j++)
                        {
                            int r = S % 0x100;
                            meta[off + field_table_off + 4 + j] = (byte)(S);
                            S /= 0x100;
                        }

                        //if the given field has child nodes then we are gonna call it again for them too
                        if (temp.Nodes.Count != 0)
                        {
                            for (int k = 0; k < count; k++)
                            {
                                int new_off = rel_off + entry_size * k;
                                Reconfig_meta(meta, new_off, abs_mem_addr, temp.Nodes);
                            }

                        }
                    }

                }
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
                        int datum_index = ReadINT_LE(tag_table_ref + 4);

                        map_stream.Close();                       
                        obj.Halo2_ExtractTagCache(datum_index,Read_File_from_file_location(map_name));
                        map_stream = new StreamReader(map_name);
                        
                    }
                    else
                    {
                        //Extraction for a whole same bunch of tags
                        List<int> DatumsList = new List<int>();
                        foreach (TreeNode tn in treeView1.SelectedNode.Nodes)
                        {
                            int tag_table_ref = Int32.Parse(tn.Name);
                            int datum_index = ReadINT_LE(tag_table_ref + 4);
                            DatumsList.Add(datum_index);
                        }

                        map_stream.Close();

                        int index = 1;

                        foreach (int i in DatumsList)
                        {
                            obj.Halo2_ExtractTagCache(i, Read_File_from_file_location(map_name));

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
              
                    obj.Halo2_ExtractTagCache(datum_list[index], Read_File_from_file_location(map_name));
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

        #region DATA_EXTRACTION
        /*
        Contains methods to read data from the map or tag file
        */

        /// <summary>
        /// Returns the little endian integer at the specified position
        /// </summary>
        /// <param name="position">the Offset from the file start</param>
        /// <returns></returns>
        int ReadINT_LE(int position)
        {
            map_stream.DiscardBufferedData();

            map_stream.BaseStream.Position = position;
            int S = 0;
            byte[] temp = new byte[4];

            map_stream.BaseStream.Read(temp, 0, 4);

            for (int i = 0; i < 4; i++)
            {
                S += temp[3 - i] * (int)Math.Pow(0x100, 3 - i);
            }

            return S;
        }
        /// <summary>
        /// Returns the string starting at the specified position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        string ReadSTRING(int position)
        {
            map_stream.DiscardBufferedData();
            map_stream.BaseStream.Position = position;

            string text ="";
            
            while(true)
            {
                char c = (char)map_stream.Read();              

                if (c=='\0')
                    break;

                text += c;
            }

            text.Trim();
            return text;
        }
        /// <summary>
        /// Return the little endian char[4] at the specified position
        /// </summary>
        /// <param name="position">the Ofset from the file start</param>
        /// <returns></returns>
        string ReadTAG_TYPE(int position)
        {
            map_stream.DiscardBufferedData();

            map_stream.BaseStream.Position = position;
            char[] type = new char[4];

            for (int i = 0; i < 4; i++)
                type[3 - i] = (char)map_stream.Read();

            string C = new string(type);

            C = C.TrimEnd();

            return C;
        }
        /// <summary>
        /// Return the type of the tag from its name
        /// </summary>
        /// <param name="name">eg: elite.char</param>
        /// <returns></returns>
        string ReadTAG_TYPE_form_name(string name)
        {
            int i;
            for (i=0;i<name.Length;i++)
            {
                if (name[i] == '.')
                    break;
            }
            return name.Substring(i+1);
        }
        /// <summary>
        /// Gets directory from file location
        /// </summary>
        /// <param name="file_loc">location of the file</param>
        /// <returns></returns>
        string ReadDirectory_from_file_location(string file_loc)
        {
            return file_loc.Substring(0, file_loc.LastIndexOf("\\"));
        }
        /// <summary>
        /// Return the file from the file location
        /// </summary>
        /// <param name="file_loc"></param>
        /// <returns></returns>
        string Read_File_from_file_location(string file_loc)
        {
            return file_loc.Substring(file_loc.LastIndexOf("\\")+1);
        }
        
        /// <summary>
        /// Gets the text from the tree of the specified tag_ref_addr as the key
        /// </summary>
        /// <param name="tnc"></param>
        /// <param name="tag_ref_addr"></param>
        /// <returns></returns>
        string Get_PathfromTree(TreeNodeCollection tnc,int tag_ref_addr)
        {
            string ret = null;

            foreach(TreeNode tn in tnc)
            {
                //it has child nodes,and we only care for child nodes
                if (tn.Nodes.Count > 0)
                   ret= Get_PathfromTree(tn.Nodes, tag_ref_addr);

                else if (Int32.Parse(tn.Name) == tag_ref_addr)
                    ret= tn.Text;

                if (ret != null)
                    return ret;
        
            }

            return null;
        }


        #endregion

        #region META_EXTRACT

       /// <summary>
       /// Function to extract Meta
       /// </summary>
       /// <param name="datum_index">datum index of the tag to be extracted</param>
       /// <param name="directory">the location where to be extracted</param>
       /// <param name="recursive">recursive flag</param>
        void Extract_Meta(int datum_index, string directory, bool recursive)
        {
            //table stuff
            int tag_table_addr = table_start + (datum_index & 0xFFFF) * 0x10;
            string type = ReadTAG_TYPE(tag_table_addr);
            int verify_datum = ReadINT_LE(tag_table_addr + 0x4);//just to verify the integrity of the datum index
            int mem_off = ReadINT_LE(tag_table_addr + 0x8);//memory offset or address(This is something which requires in depth knowledge)
            int size = ReadINT_LE(tag_table_addr + 0xC);

            //check whether it has been already extracted in this run to prevent a loophole
            bool already_extracted = false;

            foreach (int temp in extracted_datums)
                if (temp == datum_index)
                    already_extracted = true;

            //First add the datum to the common list
            extracted_datums.Add(datum_index);

            string file_name = Get_PathfromTree(treeView1.Nodes,tag_table_addr);//as i haven't explored the Strings

            if(file_name==null)
            {
                log += "\n Couldnot find the path of tag " + datum_index.ToString("X");
                file_name = datum_index.ToString("X");
            }


            //if not already extracted,then extract it
            if (!already_extracted)
            {                
                if (verify_datum == datum_index)
                {
                    if (mem_off != 0x0)
                    {
                        TreeNode tag_structure = Get_Tag_structure(type);

                        if (tag_structure != null)
                        {
                            map_stream.DiscardBufferedData();

                            byte[] tag_meta = new byte[size];//meta data
                            map_stream.BaseStream.Position = scnr_off + (mem_off - scnr_memaddr);
                            map_stream.BaseStream.Read(tag_meta, 0, size);

                            //here i change the memory_offsets into relative_offset and also check for refferenced tags
                            Manipulate_Meta(tag_meta, 0x0, mem_off, tag_structure.Nodes, recursive, directory);

                            //time to write the data into the disk
                            string file = directory + "\\" + file_name + "." + type;

                            Directory.CreateDirectory(ReadDirectory_from_file_location(file));



                            //write the file
                            StreamWriter sw = new StreamWriter(file);
                            sw.BaseStream.Write(tag_meta, 0, size);
                            sw.Dispose();


                            //write to config.xml
                            xw.WriteStartElement("tag");
                            xw.WriteStartElement("name");
                            xw.WriteString(file_name + "." + type);//writing in the inner most level ie,name
                            xw.WriteEndElement();//name level
                            xw.WriteStartElement("datum");
                            xw.WriteString(datum_index.ToString("X"));//writing in the inner most level ie here,datum
                            xw.WriteEndElement();//datum level
                            xw.WriteEndElement();//tag level


                            //write to log
                            log += "\nExtracted meta : " + file_name + "." + type;
                        }
                    }
                    else log += "\nShared map tag : " + file_name+"."+type;

                }
                else log += "\nInvalid Datum index : " + file_name;
            }
        }
        /// <summary>
        /// USED DURING EXTRACTION Manipulate tag_meta accordingly to change the offsets of reflexive fields  
        /// </summary>
        /// <param name="meta">the meta data</param>
        /// <param name="off">offset from where offset(those metioned in the plugins) to be calculated or added</param>
        /// <param name="mem_addr">this is address which should be added to memorybase allocated in memory</param>
        /// <param name="Tnc">the collection of nodes(fields) in the specified node or child nodes</param>
        /// <param name="recursive">a flag to decide whether to extract the tag recursively</param>
        /// <param name="directory">the directory where the tags are to be extracted</param>
        void Manipulate_Meta(byte[] meta,int off,int mem_addr,TreeNodeCollection Tnc,bool recursive,string directory)
        {          

            foreach(TreeNode temp in Tnc)
            {
                if (temp.Text[0] == 'T')
                {
                    //we extract the refered tags if recursive extraction is true
                    if (recursive)
                    {
                        int tag_ref_off = Int32.Parse(temp.Name.Substring(2),NumberStyles.HexNumber);
                        char[] ch = new char[4];
                        int ref_datum = 0;

                        //Read the little endian tag type string and little endian datum_index
                        for (int i = 0; i < 4; i++)
                        {
                            ch[i] = (char)meta[off + tag_ref_off + 3 - i];
                            ref_datum += (int)Math.Pow(0x100,3-i) * meta[off + tag_ref_off + 4 + 3-i];
                        }
                        string type = new string(ch);
                        type.Trim();

                        //lets extract the refered the tag also
                        Extract_Meta(ref_datum, directory, recursive);                        
                    }
                    
                }
                else if(temp.Text[0]=='R')
                {
                    int field_table_off = Int32.Parse(temp.Name.Substring(2),NumberStyles.HexNumber);

                    int count=0;
                    int field_memaddr=0;
                    int rel_off;//relative offset of the concerned field from the start of the meta
                    int entry_size = Int32.Parse(temp.Text.Substring(3),NumberStyles.HexNumber);//its kind of T0x32,T0x28,R0x32,R0x28

                    //lets read the little endian stuff
                    for(int i=0;i <4;i++)
                    {
                        count += (int)Math.Pow(0x100, i) * meta[off + field_table_off + i];
                        field_memaddr += (int)Math.Pow(0x100, i) * meta[off + field_table_off +4+ i];
                    }
                    //we do any manipulations if the field contains any elements
                    if (count > 0)
                    {
                        rel_off = field_memaddr - mem_addr;

                        //now lets update the mem_addr with relative offset
                        //relative offset is gonna be of great help during meta injection

                        //writing in the meta data in little endian format
                        int S = rel_off;
                        for (int j = 0; j < 4; j++)
                        {
                            int r = S % 0x100;
                            meta[off + field_table_off + 4 + j] = (byte)(r);
                            S /= 0x100;
                        }

                        //if the given field has child nodes then we are gonna call it again for them too
                        if (temp.Nodes.Count != 0)
                        {
                            for (int k = 0; k < count; k++)
                            {
                                int new_off = rel_off + entry_size * k;
                                Manipulate_Meta(meta, new_off, mem_addr, temp.Nodes, recursive, directory);
                            }

                        }
                    }                                
                    
                }
            }
        }
        #endregion

        #region COMPARISION
        /*
        Contains functions to compare DATA VALUES
        */

        /// <summary>
        /// returns true if the specified tag is supported for Extraction and Injection Purpose
        /// </summary>
        /// <param name="type">the tag type to be checked</param>
        /// <returns></returns>
        bool check_TAG_SUPPORT(string type)
        {
            foreach (string temp in supported_type)
            {
                if (temp.CompareTo(type) == 0)
                    return true;
            }
            return false;
        }


        #endregion

        #region TAG_STRUCTURE
        /// <summary>
        /// Function to get the tag structure in the form of nodes
        /// </summary>
        /// <param name="type">the type of the tag</param>
        /// <returns></returns>
        TreeNode Get_Tag_structure(string type)
        {
            //Plugin Search Stuff
            string plugin_loc = Application.StartupPath + "\\plugins\\" + type + ".xml";

            if (File.Exists(plugin_loc))
            {
                FileStream fs = new FileStream(plugin_loc, FileMode.Open, FileAccess.Read, FileShare.Read);

                XmlTextReader xr = new XmlTextReader(fs);
                return Get_Node(xr, false);

            }
            else log += "\nCouldnt find the plugin of type : " + type+".xml";

            return null;
        }  
        /// <summary>
        /// Function to get nodes from the Xml plugin
        /// </summary>
        /// <param name="xr">XmlTextReader object</param>
        /// <param name="child">a flag to show whether the concerned node is a child node</param>
        /// <returns></returns>
        TreeNode Get_Node(XmlReader xr, bool child)
        {
            bool work_done = false;//at boolean to show whether reading in the concerned node is done
            TreeNode my_node = new TreeNode();

            while (xr.Read() && !work_done)
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    switch (xr.Name.ToLower())
                    {
                        case "tagref":
                            my_node.Nodes.Add(xr.GetAttribute("offset"), "T");//Tdenotes tagRef
                            break;
                        case "reflexive":
                            string off = xr.GetAttribute("offset");
                            string name = xr.GetAttribute("name");
                            string entry_size = xr.GetAttribute("entrySize");

                            TreeNode temp = Get_Node(xr, true);//lets find the child nodes
                            temp.Name = off;//its actually the key
                            temp.Text = "R"+entry_size;//R denotes reflexive field and entry_size is also required in this case

                            my_node.Nodes.Add(temp);
                            break;

                    }

                }
                //this is for the child nodes
                if (xr.NodeType == XmlNodeType.EndElement && (child))
                {
                    switch (xr.Name.ToLower())
                    {
                        case "reflexive":
                            work_done = true;
                            break;
                    }
                }
            }

            return my_node;
        }
        #region DISPLAY

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        TreeNode Get_Tag_structure_Display_purpose(string type)
        {
            //Plugin Search Stuff
            string plugin_loc = Application.StartupPath + "\\plugins\\" + type + ".xml";

            if (File.Exists(plugin_loc))
            {
                FileStream fs = new FileStream(plugin_loc, FileMode.Open, FileAccess.Read, FileShare.Read);

                XmlTextReader xr = new XmlTextReader(fs);
                return Get_Node_Diplay_Purpose(xr, false);

            }
            else log += "\nCouldnt find the plugin of type : " + type + ".xml";

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xr"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        TreeNode Get_Node_Diplay_Purpose(XmlReader xr, bool child)
        {
            bool work_done = false;//at boolean to show whether reading in the concerned node is done
            TreeNode my_node = new TreeNode();

            while (xr.Read() && !work_done)
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    switch (xr.Name.ToLower())
                    {
                        case "tagref":
                            my_node.Nodes.Add(xr.GetAttribute("name") + ":" + xr.GetAttribute("offset") + ":" + xr.GetAttribute("entrySize"));
                            break;
                        case "reflexive":

                            string off = xr.GetAttribute("offset");
                            string name = xr.GetAttribute("name");
                            string entry_size = xr.GetAttribute("entrySize");

                            TreeNode temp = Get_Node_Diplay_Purpose(xr, true);//lets find the child nodes
                            temp.Name = off;//its actually the key
                            temp.Text = name +":"+ off +":"+entry_size;//R denotes reflexive field and entry_size is also required in this case

                            my_node.Nodes.Add(temp);
                            break;

                    }

                }
                //this is for the child nodes
                if (xr.NodeType == XmlNodeType.EndElement && (child))
                {
                    switch (xr.Name.ToLower())
                    {
                        case "reflexive":
                            work_done = true;
                            break;
                    }
                }
            }

            return my_node;
        }





        #endregion

        #endregion

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
