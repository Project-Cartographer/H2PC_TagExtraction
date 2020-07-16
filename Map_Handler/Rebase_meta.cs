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
using Map_Handler.Blam.Tags.sound;


namespace Map_Handler
{
    
    public partial class Rebase_meta : Form
    {
        //list of meta which are gonna be compiled
        List<injectRefs> compile_list;
        List<string> tag_scenarios;
        List<UnisonRefs> type_ref_list;//they are used to universally reference a tag depending on the type of tagRef
        string directory;
        string xml_file_loc;

        public Rebase_meta(string file)
        {
            InitializeComponent();

            XmlDocument xd = new XmlDocument();
            xd.Load(file);
            compile_list = new List<injectRefs>();
            tag_scenarios = new List<string>();
            type_ref_list = new List<UnisonRefs>();

            directory = DATA_READ.ReadDirectory_from_file_location(file);
            xml_file_loc = file;

            int new_index = 0x0;//new datum_indexes starting from 0x0,you better do

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
            _generate_cache();
            this.Close();
        }
        private void _generate_cache()
        {
            string log = "\nCOMPILATION : ";
            string cache_internal_name = DATA_READ.Read_File_name_from_location(xml_file_loc);

            meta_cache_writer my_cache_writer = new meta_cache_writer(cache_internal_name, compile_list, type_ref_list);

            foreach (injectRefs temp_ref in compile_list)
            {
                if (File.Exists(directory + "\\" + temp_ref.file_name))
                {
                    string debug_tag_name = temp_ref.file_name.Substring(0, temp_ref.file_name.IndexOf('.'));
                    string tag_loc = directory + "\\" + temp_ref.file_name;
                    if (temp_ref.type != "snd!")
                    {
                        //lets open the file
                        FileStream fs = new FileStream(tag_loc, FileMode.Append);
                        long total_file_size = fs.Position;
                        fs.Close();
                        //tag file containing both meta and RAW data
                        byte[] tag_file = new byte[total_file_size];
                        //Filestream imposed some probs
                        StreamReader sr = new StreamReader(tag_loc);
                        sr.BaseStream.Read(tag_file, 0, (int)total_file_size);
                        sr.Dispose();

                        global_tag_instance t_instance;
                        t_instance.tag_data = tag_file;
                        t_instance.tag_type = temp_ref.type;
                        t_instance.debug_tag_name = debug_tag_name;

                        //add to cache write list
                        log += my_cache_writer.add_tag_instance(t_instance);
                    }
                    else
                    {
                        string gestalt_loc = DATA_READ.ReadDirectory_from_file_location(tag_loc) + "\\" + DATA_READ.Read_File_name_from_location(tag_loc) + ".ugh!";

                        StreamReader snd_tag_stream = new StreamReader(tag_loc);
                        StreamReader gestalt_tag_stream = new StreamReader(gestalt_loc);

                        StreamReader[] sr_array = new StreamReader[0x1];
                        sr_array[0] = snd_tag_stream;
                        snd_tag snd_tag = new snd_tag(0x0, 0x0, ref sr_array);
                        sr_array[0] = gestalt_tag_stream;
                        sound_gestalt_tag gestalt_tag = new sound_gestalt_tag(0x0, 0x0, ref sr_array);

                        log += my_cache_writer.add_tag_instance(snd_tag, gestalt_tag, debug_tag_name);
                    }
                }
                else log += "\nFile doesnt exists : " + temp_ref.file_name;
            }
            my_cache_writer.write(directory);

            //mention the universally acclaimed tag
            log += "\ntype referenced tags are :";
            foreach (UnisonRefs uni_temp in type_ref_list)
                log += "\nReffered " + uni_temp.type + " to " + uni_temp.new_datum.ToString("X") + " file : " + uni_temp.file_name;

            //writing log 
            var sw = new StreamWriter(directory + "\\compile_log.txt");
            sw.Write(log);

            //lets launch the log box
            LogBox lb = new LogBox(log);
            lb.Show();
        }
    }
}
