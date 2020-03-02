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
using DATA_STRUCTURES;
using System.Xml;

namespace Map_Handler
{
    public partial class MetaExtractor : Form
    {
        //StreamReader current_stream;//the map stream
        //Dictionary<int, string> current_tag_names;//the list of string ID

        List<tagRef> extraction_list;//a list containing the datum indexes waiting for extraction
        List<int> extracted_list;//a list containing the datum indexes already extracted
        List<tagRef> unextracted_list;//list of datums which arent contained in the current map but in the shared map

        StreamReader map_stream;
        StreamReader mp_shared_stream;
        StreamReader sp_shared_stream;
        StreamReader mainmenu_stream;
        //string[] non_extractable = {"stem"};//list of tags that arent to be extracted


        public MetaExtractor(int datum,string type,StreamReader map_stream, StreamReader mp_shared_stream, StreamReader sp_shared_stream, StreamReader mainmenu_stream)
        {
            InitializeComponent();

            //initialise all stuff         
            this.map_stream = map_stream;
            this.mp_shared_stream = mp_shared_stream;
            this.sp_shared_stream = sp_shared_stream;
            this.mainmenu_stream = mainmenu_stream;

            extraction_list = new List<tagRef>();
            extracted_list = new List<int>();
            unextracted_list = new List<tagRef>();            

            //lets add the currently passed datum to the list
            tagRef temp = new tagRef();
            temp.datum_index = datum;
            temp.type = type;

            extraction_list.Add(temp);
        }

        private void Folder_select_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = fbd.SelectedPath;
            }
        }

        private void Extract_Click(object sender, EventArgs e)
        {
            StreamReader current_stream = map_stream;

            string log = "\nEXTRACTION LOG : ";//or log

            string rel_path = get_debug_tag_name(extraction_list[0].datum_index, current_stream) + "." + extraction_list[0].type;

            XmlTextWriter xw = new XmlTextWriter(textBox1.Text + "\\" + DATA_READ.Read_File_from_file_location(rel_path)+".xml", Encoding.UTF8);
            xw.Formatting = Formatting.Indented;
            xw.WriteStartElement("config");

            if (textBox1.Text.Length > 0)
            {
                for (int i = 0; i < extraction_list.Count; )
                {
                    tagRef temp_tagref = extraction_list[i];

                    int datum = temp_tagref.datum_index;
                    string type = temp_tagref.type;

                    if (datum!=-1 && datum!=0)
                    {
                        //few check for non_extractable types
                        bool isnon_extractable = false;

                   /*     foreach(string tempNE in non_extractable)
                        {
                            if(type.CompareTo(tempNE)==0)
                            {
                                isnon_extractable = true;
                                break;
                            }
                        }
                        */


                        if (!extracted_list.Contains(datum)&&!isnon_extractable)
                        {
                            if (File.Exists(Application.StartupPath + "\\plugins\\" + type + ".xml"))
                            {
                                if (!is_tag_shared(datum,current_stream))
                                {
                                    meta obj = new meta(datum, get_debug_tag_name(datum,current_stream), current_stream);
                                    obj.Rebase_meta(0x0);

                                    if (checkBox1.Checked)
                                    {
                                        obj.Null_StringID();
                                    }

                                    if (radioButton1.Checked == true)
                                    {
                                        List<tagRef> refs_temp = obj.Get_all_tag_refs();
                                        //add recursivity
                                        foreach (tagRef my_tempy in extraction_list)
                                        {
                                            refs_temp.Remove(my_tempy);
                                        }

                                        extraction_list.AddRange(refs_temp);
                                        //to remove redundancy                              

                                    }

                                    byte[] data = obj.Generate_meta_file();

                                    RAW_data raw_obj = new RAW_data(data, type, 0x0, current_stream, mp_shared_stream, sp_shared_stream, mainmenu_stream);
                                    raw_obj.rebase_RAW_data(obj.Get_Total_size());

                                    byte[] raw_data = raw_obj.get_RAW_data();

                                    string path = textBox1.Text + "\\" + obj.Get_Path() + "." + obj.Get_Type();
                                    string directory = DATA_READ.ReadDirectory_from_file_location(path);

                                    //lets create our directory
                                    System.IO.Directory.CreateDirectory(directory);
                                    //create our file
                                    StreamWriter sw = new StreamWriter(path);
                                    sw.BaseStream.Write(data, 0, obj.Get_Total_size());
                                    sw.BaseStream.Write(raw_data, 0, raw_obj.get_total_RAW_size());
                                    sw.Dispose();

                                    //write to configuration xml
                                    xw.WriteStartElement("tag");
                                    xw.WriteStartElement("name");
                                    xw.WriteString(obj.Get_Path() + "." + type);//writing in the inner most level ie,name
                                    xw.WriteEndElement();//name level
                                    xw.WriteStartElement("datum");
                                    xw.WriteString(datum.ToString("X"));//writing in the inner most level ie here,datum
                                    xw.WriteEndElement();//datum level
                                    xw.WriteStartElement("scenario");
                                    xw.WriteString(DATA_READ.ReadSTRINGPATH(0x1C8, current_stream));
                                    xw.WriteEndElement();//scenario level                        
                                    xw.WriteEndElement();//tag level

                                    //at least mention this in the logs
                                    log += "\nExtracted meta " + datum.ToString("X") + " to " + path;

                                    //add it to the extracted list
                                    extracted_list.Add(datum);
                                }
                                else
                                {
                                    unextracted_list.Add(temp_tagref);
                                    log += "\nShared map refered datum_index " + datum.ToString("X");
                                }
                            }
                            else
                            {
                                log += "\nCannot extract tag : " + datum.ToString("X");
                                log += "\nPlugin " + type + ".xml doesnt exist";                                
                                extracted_list.Add(datum);
                            }
                        }
                    }

                    i++;//update count

                    if (i == extraction_list.Count)
                    {
                        //reached the end of list for the current map

                        //now lets add the shared tag_refs   
                        extraction_list.AddRange(unextracted_list);
                        unextracted_list.Clear();
                        //update the map stream to shared stream
                        current_stream = mp_shared_stream;
                    }
                }
                //close the config field and close the xml handle
                xw.WriteEndElement();
                xw.Dispose();

                //logs 
                StreamWriter sw_1 = new StreamWriter(textBox1.Text + "\\extraction_logs.txt");
                sw_1.Write(log);
                sw_1.Close();

                //Log box
                LogBox lb = new LogBox(log);
                lb.Show();

                //wprk is now done so lets close this stupid box
                this.Close();               
            }
            else MessageBox.Show("At least Select the Directory", "Error");
        }
        private string get_debug_tag_name(int datum_index,StreamReader map_stream)
        {
            byte[] map_header=new byte[0x800];
            map_stream.BaseStream.Read(map_header, 0x0, 0x800);

            int buffer_offset = DATA_READ.ReadINT_LE(0x2D0, map_stream);   
            int buffer_size = DATA_READ.ReadINT_LE(0x2D4, map_stream); 
            int buffer_indices = DATA_READ.ReadINT_LE(0x2D8, map_stream);

            int string_off = DATA_READ.ReadINT_LE(buffer_indices + 4 * (0xFFFF & datum_index), map_stream);
            
            return DATA_READ.ReadSTRINGPATH(buffer_offset + string_off, map_stream);             
        }
        private bool is_tag_shared(int datum_index,StreamReader map_stream)
        {
            int table_off = DATA_READ.ReadINT_LE(0x10, map_stream);

            int table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4, map_stream) + 0x20;

            int tag_table_REF = table_start + 0x10 * (0xffff & datum_index);

            //lets check the mem addrs validity before adding it to the list
            int mem_addr = DATA_READ.ReadINT_LE(tag_table_REF + 8, map_stream);

            //shared referenced tags have memory offset 0
            return mem_addr==0;
        }

    }
}
