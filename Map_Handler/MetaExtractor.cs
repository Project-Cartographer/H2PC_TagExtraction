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
        StreamReader map_stream;//the map stream
        Dictionary<int, string> SID_list;//the list of string ID
        List<tagRef> extraction_list;//a list containing the datum indexes waiting for extraction
        List<int> extracted_list;//a list containing the datum indexes already extracted

        string[] non_extractable = {"stem"};//list of tags that arent to be extracted


        public MetaExtractor(int datum,string type,Dictionary<int,string> SID_list,StreamReader sr)
        {
            InitializeComponent();

            //initialise all stuff         
            map_stream = sr;
            this.SID_list = SID_list;
            extraction_list = new List<tagRef>();
            extracted_list = new List<int>();

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
            string log = "\nEXTRACTION LOG : ";//or log

            string rel_path = SID_list[extraction_list[0].datum_index] + "." + extraction_list[0].type;

            XmlTextWriter xw = new XmlTextWriter(textBox1.Text + "\\" + DATA_READ.Read_File_from_file_location(rel_path)+".xml", Encoding.UTF8);
            xw.Formatting = Formatting.Indented;
            xw.WriteStartElement("config");

            if (textBox1.Text.Length > 0)
            {
                for (int i = 0; i < extraction_list.Count; i++)
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
                                if (SID_list.ContainsKey(datum))
                                {
                                    meta obj = new meta(datum, SID_list[datum], map_stream);
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

                                    string path = textBox1.Text + "\\" + obj.Get_Path() + "." + obj.Get_Type();
                                    string directory = DATA_READ.ReadDirectory_from_file_location(path);

                                    //lets create our directory
                                    System.IO.Directory.CreateDirectory(directory);
                                    //create our file
                                    StreamWriter sw = new StreamWriter(path);
                                    sw.BaseStream.Write(data, 0, obj.Get_Total_size());
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
                                    xw.WriteString(DATA_READ.ReadSTRINGPATH(0x1C8, map_stream));
                                    xw.WriteEndElement();//scenario level                        
                                    xw.WriteEndElement();//tag level

                                    //at least mention this in the logs
                                    log += "\nExtracted meta " + datum.ToString("X") + " to " + path;

                                    //add it to the extracted list
                                    extracted_list.Add(datum);
                                }
                                else log += "\nCouldnot find stringID to datum_index " + datum.ToString("X");
                            }
                            else
                            {
                                log += "\nPlugin " + type + ".xml doesnt exist";
                                extracted_list.Add(datum);
                            }
                        }
                    }
                }
                //close the config field and close the xml handle
                xw.WriteEndElement();
                xw.Dispose();

                //Log box
                LogBox lb = new LogBox(log);
                lb.Show();

                //wprk is now done so lets close this stupid box
                this.Close();               
            }
            else MessageBox.Show("At least Select the Directory", "Error");
        }


    }
}
