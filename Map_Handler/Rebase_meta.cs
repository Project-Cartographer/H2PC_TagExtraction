using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using DATA_STRUCTURES;
using System.IO;


namespace Map_Handler
{
    public partial class Rebase_meta : Form
    {
        //list of meta which are gonna be compiled
        List<injectRefs> compile_list;
        string directory;
        List<string> tag_scenarios;
        List<UnisonRefs> type_ref_list;//they are used to universally reference a tag depending on the type of tagRef


        public Rebase_meta(string file)
        {
            InitializeComponent();

            XmlDocument xd = new XmlDocument();
            xd.Load(file);
            compile_list = new List<injectRefs>();
            tag_scenarios = new List<string>();
            type_ref_list = new List<UnisonRefs>();

            directory = DATA_READ.ReadDirectory_from_file_location(file);
            int new_index = 0x3BA4;//new datum_indexes starting from 0x3BA4

            foreach (XmlNode Xn in xd.SelectNodes("config/tag"))
            {
                injectRefs temp = new injectRefs();

                temp.old_datum = int.Parse(Xn.SelectSingleNode("datum").InnerText, NumberStyles.HexNumber);
                temp.new_datum = new_index++;
                temp.file_name = Xn.SelectSingleNode("name").InnerText;
                temp.type = DATA_READ.ReadTAG_TYPE_form_name(temp.file_name);

                tag_scenarios.Add(Xn.SelectSingleNode("scenario").InnerText);

                //lets add the tag to the list
                compile_list.Add(temp);
            }
            //now lets fill the unison List
            List<string> blacklisted_type = new List<string>();

            foreach(injectRefs inj_temp in compile_list)
            {
                if (!blacklisted_type.Contains(inj_temp.type))
                {
                    bool any_occurence = false;
                    for (int i = 0; i < type_ref_list.Count(); i++)
                    {
                        UnisonRefs uni_temp = type_ref_list[i];
                        if (uni_temp.type == inj_temp.type)
                        {
                            any_occurence = true;
                            blacklisted_type.Add(inj_temp.type);
                            type_ref_list.Remove(uni_temp);
                        }
                    }
                    if (!any_occurence)
                    {
                        UnisonRefs my_temp_ref = new UnisonRefs();
                        my_temp_ref.type = inj_temp.type;
                        my_temp_ref.new_datum = inj_temp.new_datum;
                        my_temp_ref.file_name = inj_temp.file_name;

                        type_ref_list.Add(my_temp_ref);
                    }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string log = "\nCOMPILATION : ";

            //the File where we are gonna compile our stuff
            StreamWriter sw = new StreamWriter(directory + "\\tags.meta");
            //the Tables data
            StreamWriter sw_t = new StreamWriter(directory + "\\tables.meta");
            //creating a table
            byte[] tables = new byte[0x10 * compile_list.Count];
            //index of custom metas
            int custom_table_index = 0;

            int new_base = Int32.Parse(textBox1.Text.Substring(2), NumberStyles.HexNumber);//the offset from the map_mem_base 0x1511020(lockout)

            foreach (injectRefs temp_ref in compile_list)
            {
                if (File.Exists(directory + "\\" + temp_ref.file_name))
                {
                    //lets open the file
                    FileStream fs = new FileStream(directory + "\\" + temp_ref.file_name, FileMode.Append);
                    long size = fs.Position;
                    fs.Close();

                    //lets load it into memory
                    byte[] meta = new byte[size];

                    //Filestream imposed some probs
                    StreamReader sr = new StreamReader(directory + "\\" + temp_ref.file_name);
                    //lets read the data
                    sr.BaseStream.Read(meta, 0, (int)size);
                    sr.Dispose();

                    meta obj = new meta(meta, temp_ref.type, (int)size, temp_ref.file_name);
                    log += obj.Update_datum_indexes(compile_list, type_ref_list);
                    obj.Rebase_meta(new_base);


                    //write it to the file
                    sw.BaseStream.Write(meta, 0, (int)size);

                    DATA_READ.WriteTAG_TYPE_LE(temp_ref.type, custom_table_index * 0x10, tables);
                    DATA_READ.WriteINT_LE(temp_ref.new_datum, custom_table_index * 0x10 + 4, tables);
                    DATA_READ.WriteINT_LE(new_base, custom_table_index * 0x10 + 8, tables);
                    DATA_READ.WriteINT_LE((int)size, custom_table_index * 0x10 + 0xC, tables);

                    log += "\n Written tag " + temp_ref.file_name + " with new datum as " + temp_ref.new_datum.ToString("X");

                    //increase the tag_offset
                    new_base += (int)size;

                }
                else log += "\nFile doesnt exists : " + temp_ref.file_name;
                custom_table_index++;
            }
            //atleast mention the universally acclaimed tag
            log += "\ntype referenced tags are :";
            foreach (UnisonRefs uni_temp in type_ref_list)
                log += "\nReffered " + uni_temp.type + " to " + uni_temp.new_datum.ToString("X") + " file : " + uni_temp.file_name;

            sw_t.BaseStream.Write(tables, 0, 0x10 * compile_list.Count());
            //add the scenarios to which the tags are linked
            foreach (string st in tag_scenarios)
            {
                byte[] lol = {0};
                sw.BaseStream.Write(Encoding.ASCII.GetBytes(st), 0, st.Length);
                sw.BaseStream.Write(lol, 0, 1);
            }

            sw.Dispose();
            sw_t.Dispose();

            //lets launch the log box
            LogBox lb = new LogBox(log);
            lb.Show();
            this.Close();
        }
    }
}
