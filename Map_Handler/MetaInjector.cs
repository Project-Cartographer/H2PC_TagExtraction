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
using System.Globalization;

//i assume that it would be only used to inject stuff in sp maps as runtime injection has been achieved at the time of writing
//i assume that this wont be used to inject stuff in shared.map as this could break all existing maps

namespace Map_Handler
{
    public partial class MetaInjector : Form
    {
        string tags_directory;

        public MetaInjector()
        {
            InitializeComponent();
        }

        private void selectMeta_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "config files(*.xml)|*.xml";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                metaFileTextBox.Text = ofd.FileName;
                tags_directory = DATA_READ.ReadDirectory_from_file_location(ofd.FileName);
            }
        }

        private void selectTargetMap_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Halo 2 Vista Map (*.map)|*.map";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                targetMapTextBox.Text = ofd.FileName;
            }
        }

        private void saveMapAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.DefaultExt = "Halo 2 Vista Map (*.map)|*.map";
            svd.AddExtension = true;

            if (svd.ShowDialog() == DialogResult.OK)
            {
                saveMapTextBox.Text = svd.FileName;
            }
        }
        private void inject_Click(object sender, EventArgs e)
        {
            if (metaFileTextBox.Text.Length != 0 && targetMapTextBox.Text.Length != 0 && saveMapTextBox.Text.Length != 0)
            {
                StreamReader input_map_stream = new StreamReader(targetMapTextBox.Text);
                StreamWriter output_map_stream = new StreamWriter(saveMapTextBox.Text);
                StreamWriter inject_log_stream = new StreamWriter(DATA_READ.ReadDirectory_from_file_location(targetMapTextBox.Text) + "\\inject_logs.txt");

                //retrieve tag list
                List<tag_info> map_tag_list = new List<tag_info>();
                List<injectRefs> inject_tag_list = new List<injectRefs>();
                List<injectRefs> common_tag_list = new List<injectRefs>();
                List<UnisonRefs> type_ref_list = new List<UnisonRefs>();
                string log = "\nINJECTION LOG : ";

                load_tag_list_from_map(out map_tag_list, targetMapTextBox.Text);
                load_tag_list_from_config(metaFileTextBox.Text, out inject_tag_list, out common_tag_list, ref map_tag_list);

                log += "\nMap Name : " + targetMapTextBox.Text;
                log += "\nModule Name : " + metaFileTextBox.Text;
                log += "\nSaving Map As : " + saveMapTextBox.Text;

                
                if (common_tag_list.Count != 0)
                {
                    log += "\nSearching for Common Tags In Map ...........";
                    //found tags 
                        foreach (var i in common_tag_list)
                            log += "\nFound tag 0x" + i.old_datum.ToString("X") + " : 0x" + i.new_datum.ToString("X") + " : " + i.file_name;
                 }
                 else
                        log += "\nNo Common Tags found in Map........";
                

                //compile tags list
                List<injectRefs> compile_list = new List<injectRefs>();
                compile_list.AddRange(inject_tag_list);

                foreach (var i in common_tag_list)
                    compile_list.Remove(i);

                //<---------------------------- needs logic update------------------------------->sure
                int has_modified_file_tables = DATA_READ.ReadINT_LE(0x2F8, input_map_stream);
                int has_appended_tag_table = DATA_READ.ReadINT_LE(0x2FC, input_map_stream);

                //copy the whole data and write upto the tags or injected RAW
                int file_size = DATA_READ.ReadINT_LE(0x8, input_map_stream);
                byte[] input_map_file = new byte[file_size];
                int tags_offset = DATA_READ.ReadINT_LE(0x10, input_map_stream);  
                              
                input_map_stream.BaseStream.Position = 0x0;
                input_map_stream.BaseStream.Read(input_map_file, 0x0, file_size);

                if (has_modified_file_tables == 0)
                {
                    //non-injected map,copy upto the meta start
                    output_map_stream.BaseStream.Write(input_map_file, 0x0, tags_offset);
                }
                else
                {
                    //modded map copy upto the end of injected RAW block
                    output_map_stream.BaseStream.Write(input_map_file, 0x0, DATA_READ.ReadINT_LE(0x304, input_map_file) + DATA_READ.ReadINT_LE(0x308, input_map_file));
                }

                //actual stuff     
                int new_tag_count = DATA_READ.ReadINT_LE(0x2CC, input_map_stream) + compile_list.Count;
                byte[] new_tag_tables = new byte[0x10 * new_tag_count];
                Queue<byte[]> meta_list = new Queue<byte[]>();

                int old_parent_info_count;
                //read the old parent_info count
                if (has_appended_tag_table == 0)
                {
                     old_parent_info_count = DATA_READ.ReadINT_LE(tags_offset + 0x4, input_map_file);
                }
                else
                {
                    old_parent_info_count = DATA_READ.ReadINT_LE(0x300, input_map_file);
                }

                //copy old table
                int tag_tables_offset = tags_offset + 0xC * DATA_READ.ReadINT_LE(tags_offset + 0x4, input_map_stream) + 0x20;
                DATA_READ.ArrayCpy(new_tag_tables, input_map_file, 0x0, tag_tables_offset, 0x10 * DATA_READ.ReadINT_LE(0x2CC, input_map_stream));

                //copy sound gestalate table declaration
                byte[] ugh_tag_table_element = new byte[0x10];
                DATA_READ.ArrayCpy(ugh_tag_table_element, new_tag_tables, 0, 0x10 * (DATA_READ.ReadINT_LE(0x2CC, input_map_stream) - 1), 0x10);

                int tag_data_size;

                if (has_appended_tag_table == 0)
                    tag_data_size = DATA_READ.ReadINT_LE(0x1C, input_map_stream);
                else
                    tag_data_size = tag_tables_offset - tags_offset;

                int ext_RAW_base, ext_RAW_size;
                int ext_meta_base, ext_meta_size;

                if (has_appended_tag_table==0)
                {
                    ext_RAW_base = DATA_READ.ReadINT_LE(0x10, input_map_stream);
                    ext_RAW_size = 0x0;
                    ext_meta_base = DATA_READ.ReadINT_LE(0x20, input_map_stream) + tag_data_size;
                    ext_meta_size = 0x0;
                }
                else
                {
                    ext_RAW_base = DATA_READ.ReadINT_LE(0x304, input_map_stream);
                    ext_RAW_size = DATA_READ.ReadINT_LE(0x308, input_map_stream);
                    ext_meta_base = DATA_READ.ReadINT_LE(0x20, input_map_stream) + tag_data_size;
                    ext_meta_size = 0x0;
                }
                

                //write RAW data and maintain meta data list
                foreach (var temp_ref in compile_list)
                {
                    if (File.Exists(tags_directory + "\\" + temp_ref.file_name))
                    {
                        //lets open the file
                        FileStream fs = new FileStream(tags_directory + "\\" + temp_ref.file_name, FileMode.Append);
                        long tag_file_size = fs.Position;
                        fs.Close();

                        //tag file containing both meta and RAW data
                        byte[] t_meta_file = new byte[tag_file_size];
                        //Filestream imposed some probs
                        StreamReader sr = new StreamReader(tags_directory + "\\" + temp_ref.file_name);
                        sr.BaseStream.Read(t_meta_file, 0, (int)tag_file_size);
                        sr.Dispose();

                        //retrieve RAW_data
                        RAW_data raw_obj = new RAW_data(t_meta_file, temp_ref.type);
                        raw_obj.rebase_RAW_data(ext_RAW_base + ext_RAW_size);
                        byte[] t_RAW_data = raw_obj.get_RAW_data();
                        long tag_raw_file_size = raw_obj.get_total_RAW_size();

                        //lets copy the actual meta
                        long tag_meta_file_size = tag_file_size - tag_raw_file_size;
                        byte[] meta = new byte[tag_meta_file_size];
                        DATA_READ.ArrayCpy(meta, t_meta_file, 0, (int)(tag_meta_file_size));

                        //rebase the meta
                        meta obj = new meta(meta, temp_ref.type, (int)tag_meta_file_size, temp_ref.file_name);
                        log+=obj.Update_datum_indexes(inject_tag_list, type_ref_list);
                        obj.Rebase_meta(ext_meta_base + ext_meta_size);

                        //add to listing
                        meta_list.Enqueue(meta);

                        //write raw to file
                        output_map_stream.BaseStream.Write(t_RAW_data, 0x0, (int)tag_raw_file_size);

                        //tag_table_stuff
                        DATA_READ.WriteTAG_TYPE_LE(temp_ref.type, (temp_ref.new_datum & 0xFFFF) * 0x10, new_tag_tables);
                        DATA_READ.WriteINT_LE(temp_ref.new_datum, (temp_ref.new_datum & 0xFFFF) * 0x10 + 4, new_tag_tables);
                        DATA_READ.WriteINT_LE(ext_meta_base + ext_meta_size, (temp_ref.new_datum & 0xFFFF) * 0x10 + 8, new_tag_tables);
                        DATA_READ.WriteINT_LE((int)tag_meta_file_size, (temp_ref.new_datum & 0xFFFF) * 0x10 + 0xC, new_tag_tables);

                        //log += "\n Written tag " + temp_ref.file_name + " with new datum as " + temp_ref.new_datum.ToString("X");

                        //add the sizes
                        ext_meta_size += (int)tag_meta_file_size;
                        ext_RAW_size += (int)tag_raw_file_size;
                    }
                }
                //write to tag_names_buffer and maintain tag names index table
                int new_tag_names_buffer_offset = align_0x200((int)output_map_stream.BaseStream.Position);
                output_map_stream.BaseStream.Position = new_tag_names_buffer_offset;
                byte[] new_tag_names_index_table = new byte[0x4 * new_tag_count];

                //original tags
                byte[] lol = { 0 };
                foreach (var map_tag_ref in map_tag_list)
                {
                    DATA_READ.WriteINT_LE((int)output_map_stream.BaseStream.Position - new_tag_names_buffer_offset, (map_tag_ref.datum_index & 0xFFFF) * 0x4, new_tag_names_index_table);

                    output_map_stream.BaseStream.Write(Encoding.ASCII.GetBytes(map_tag_ref.file_loc), 0x0, map_tag_ref.file_loc.Length);
                    output_map_stream.BaseStream.Write(lol, 0, 1);
                }
                //new tags
                foreach (var new_tag_ref in compile_list)
                {
                    DATA_READ.WriteINT_LE((int)output_map_stream.BaseStream.Position - new_tag_names_buffer_offset, (new_tag_ref.new_datum & 0xFFFF) * 0x4, new_tag_names_index_table);
                    string tagname = new_tag_ref.file_name;
                    //remove tag_group at the end
                    tagname = tagname.Substring(0, tagname.LastIndexOf("."));

                    output_map_stream.BaseStream.Write(Encoding.ASCII.GetBytes(tagname), 0x0, tagname.Length);
                    output_map_stream.BaseStream.Write(lol, 0, 1);
                }
                //ugh! tag
                DATA_READ.WriteINT_LE((int)output_map_stream.BaseStream.Position - new_tag_names_buffer_offset, ((new_tag_count - 1) & 0xFFFF) * 0x4, new_tag_names_index_table);
                output_map_stream.BaseStream.Write(Encoding.ASCII.GetBytes("screw this game!"), 0x0, "screw this game!".Length);
                output_map_stream.BaseStream.Write(lol, 0, 1);

                int new_tag_names_buffer_size = (int)output_map_stream.BaseStream.Position - new_tag_names_buffer_offset;
                //write index table to disk
                int new_tag_names_index_table_offset = align_0x200((int)output_map_stream.BaseStream.Position);
                output_map_stream.BaseStream.Position = new_tag_names_index_table_offset;
                output_map_stream.BaseStream.Write(new_tag_names_index_table, 0x0, new_tag_names_index_table.Length);

                //writing the tag meta data
                //original meta
                byte[] tag_data = new byte[DATA_READ.ReadINT_LE(0x1C, input_map_stream)];
                DATA_READ.ArrayCpy(tag_data, input_map_file, 0, tags_offset, DATA_READ.ReadINT_LE(0x1C, input_map_stream));

                //update matg and ugh! datum indices
                int virtual_base = DATA_READ.ReadINT_LE(0x20, input_map_stream);
                _update_matg_and_ugh(new_tag_count - 1, tag_data, virtual_base);
                    //add to tag tables
                    DATA_READ.WriteINT_LE(new_tag_count - 1, 0x4, ugh_tag_table_element);
                    DATA_READ.ArrayCpy(new_tag_tables, ugh_tag_table_element, (new_tag_count - 1) * 0x10, 0x10);

                //write tag data
                int new_tag_offset = align_0x200((int)output_map_stream.BaseStream.Position);
                output_map_stream.BaseStream.Position = new_tag_offset;
                output_map_stream.BaseStream.Write(tag_data, 0x0, tag_data_size);

                //custom meta
                foreach (var t_meta in meta_list)
                    output_map_stream.BaseStream.Write(t_meta, 0x0, t_meta.Length);

                //tag_tables
                int new_tag_tables_multiplier = align(((int)output_map_stream.BaseStream.Position - new_tag_offset - 0x20), 0xC) / 0xC;//lol
                int new_tag_tables_offset = new_tag_offset + 0xC * new_tag_tables_multiplier + 0x20;
                int new_tag_tables_offset_from_table_declaration = 0xC * new_tag_tables_multiplier + 0x20;

                output_map_stream.BaseStream.Position = new_tag_tables_offset;
                output_map_stream.BaseStream.Write(new_tag_tables, 0x0, new_tag_tables.Length);

                int new_file_size = align_0x200((int)output_map_stream.BaseStream.Position);
                output_map_stream.BaseStream.Position = new_file_size - 1;
                output_map_stream.BaseStream.WriteByte(0);
                int new_tag_data_size = new_file_size - new_tag_offset;

                //modify the tag_table header
                byte[] temp = new byte[0x4];
                DATA_READ.WriteINT_LE(new_tag_tables_multiplier, 0x0, temp);
                output_map_stream.BaseStream.Position = new_tag_offset + 0x4;
                output_map_stream.BaseStream.Write(temp, 0x0, 4);
                DATA_READ.WriteINT_LE(new_tag_tables_offset_from_table_declaration, 0x0, temp);
                output_map_stream.BaseStream.Position = new_tag_offset + 0x8;
                output_map_stream.BaseStream.Write(temp, 0x0, 4);
                DATA_READ.WriteINT_LE(new_tag_count, 0x0, temp);
                output_map_stream.BaseStream.Position = new_tag_offset + 0x18;
                output_map_stream.BaseStream.Write(temp, 0x0, 4);

                //read the header
                byte[] new_map_header = new byte[0x800];                                
                DATA_READ.ArrayCpy(new_map_header, input_map_file, 0x0, 0x800);

                //modify the map header
                DATA_READ.WriteINT_LE(new_file_size, 0x8, new_map_header);//file_size
                DATA_READ.WriteINT_LE(new_tag_offset, 0x10, new_map_header);//tag_offset
                DATA_READ.WriteINT_LE(new_tag_data_size - DATA_READ.ReadINT_LE(0x14, new_map_header), 0x18, new_map_header);//data_size
                DATA_READ.WriteINT_LE(new_tag_data_size, 0x1C, new_map_header);//tag_size

                DATA_READ.WriteINT_LE(new_tag_count, 0x2CC, new_map_header);//TagNamesCount
                DATA_READ.WriteINT_LE(new_tag_names_buffer_offset, 0x2D0, new_map_header);//TagNamesBufferOffset
                DATA_READ.WriteINT_LE(new_tag_names_buffer_size, 0x2D4, new_map_header);//TagNamesBufferSize
                DATA_READ.WriteINT_LE(new_tag_names_index_table_offset, 0x2D8, new_map_header);//TagNamesIndicesOffset

                //write our custom stuff
                DATA_READ.WriteINT_LE(0x1, 0x2F8, new_map_header);
                DATA_READ.WriteINT_LE(0x1, 0x2FC, new_map_header);
                DATA_READ.WriteINT_LE(old_parent_info_count, 0x300, new_map_header);
                DATA_READ.WriteINT_LE(ext_RAW_base, 0x304, new_map_header);
                DATA_READ.WriteINT_LE(ext_RAW_size, 0x308, new_map_header);

                //write out header to disk
                output_map_stream.BaseStream.Position = 0;
                output_map_stream.BaseStream.Write(new_map_header, 0, 0x800);
                inject_log_stream.Write(log);

                inject_log_stream.Close();
                output_map_stream.Close();
                input_map_stream.Close();

                LogBox lb = new LogBox(log);
                lb.VerticalScroll.Visible = true;
                lb.Show();
                MessageBox.Show("PEEEP PEEP", "DONE");

            }
            else
                MessageBox.Show("Fill all the required fields", "Error");
        }
        //unmodified tag header still containing the old tag_count
        private void _update_matg_and_ugh(int ugh_new_datum_index, byte[] tag_data,int virtual_base)
        {
            int table_offset = DATA_READ.ReadINT_LE(0x4, tag_data) * 0xC + 0x20;

            //matg
            int matg_datum_index = DATA_READ.ReadINT_LE(0x10, tag_data);            
            int matg_mem_off = DATA_READ.ReadINT_LE(table_offset + (matg_datum_index & 0xFFFF) * 0x10 + 0x8, tag_data);
            int matg_off = matg_mem_off - virtual_base;
            int sound_off = DATA_READ.ReadINT_LE(matg_off + 0xC4, tag_data) - virtual_base;
            DATA_READ.WriteINT_LE(ugh_new_datum_index, sound_off + 0x20, tag_data);
            //ugh!
            int ugh_old_datum_index = DATA_READ.ReadINT_LE(0x18, tag_data) - 1;            
            int ugh_mem_off = DATA_READ.ReadINT_LE(table_offset + (ugh_old_datum_index & 0xFFFF) * 0x10 + 0x8, tag_data);            
            int ugh_off = ugh_mem_off - virtual_base;
            int extra_info_count = DATA_READ.ReadINT_LE(ugh_off + 0x50, tag_data);
            int extra_info_off = DATA_READ.ReadINT_LE(ugh_off + 0x54, tag_data) - virtual_base;

            for(int i=0;i<extra_info_count;i++)
            {
                int block_off = extra_info_off + i * 0x2C;
                DATA_READ.WriteINT_LE(ugh_new_datum_index, block_off + 0x20, tag_data);
            }

        }
        private void load_tag_list_from_config(string file_loc, out List<injectRefs> tag_list,out List<injectRefs> common_list, ref List<tag_info> map_tag_list)
        {
            tag_list = new List<injectRefs>();
            common_list = new List<injectRefs>();
            XmlDocument xd = new XmlDocument();
            xd.Load(file_loc);

            int new_datum_index = map_tag_list.Count;

            foreach (XmlNode Xn in xd.SelectNodes("config/tag"))
            {
                injectRefs temp = new injectRefs();

                temp.old_datum = int.Parse(Xn.SelectSingleNode("datum").InnerText, NumberStyles.HexNumber);
                temp.file_name = remove_redundancy(Xn.SelectSingleNode("name").InnerText);
                temp.type = DATA_READ.ReadTAG_TYPE_form_name(temp.file_name);

                //check for its existence in the map
                int t_result = -1;
                if (addExistingTagsCheckBox.Checked == false)
                {
                    if (map_tag_list.Count != 0)
                    {
                        foreach (var map_tag in map_tag_list)
                        {
                            if (map_tag.file_loc == temp.file_name)
                                t_result = map_tag.datum_index;
                        }
                    }
                }
                if (t_result == -1)
                {
                    temp.new_datum = new_datum_index++;

                    //lets add the tag to the list
                    tag_list.Add(temp);
                }
                else
                {
                    temp.new_datum = t_result;

                    //lets add the tag to the list
                    tag_list.Add(temp);
                    //and common list too
                    common_list.Add(temp);
                }        
            }
        }
        //neglect the ugh! tag so we get one less of tag count
        private void load_tag_list_from_map(out List<tag_info> tag_list,string map_loc)
        {
            StreamReader map_stream = new StreamReader(map_loc);

            tag_list = new List<tag_info>();

            int tags_offset = DATA_READ.ReadINT_LE(0x10, map_stream);
            int tag_table = tags_offset + 0xC * DATA_READ.ReadINT_LE(tags_offset + 0x4, map_stream) + 0x20;


            int tag_count = DATA_READ.ReadINT_LE(0x2CC, map_stream);
            int tag_names_buffer = DATA_READ.ReadINT_LE(0x2D0, map_stream);
            int tag_names_buffer_size = DATA_READ.ReadINT_LE(0x2D4, map_stream);
            int tag_names_index_table = DATA_READ.ReadINT_LE(0x2D8, map_stream);

            for (int i = 0; i < (tag_count - 1); i++)
            {
                //really have no interest in retrieving type
                int datum_index = DATA_READ.ReadINT_LE(tag_table + 0x10 * i + 0x4, map_stream);
                int mem_off = DATA_READ.ReadINT_LE(tag_table + 0x10 * i + 0x8, map_stream);

                int buffer_offset = DATA_READ.ReadINT_LE(tag_names_index_table + i * 0x4, map_stream);

                if ((buffer_offset - tag_names_buffer) < tag_names_buffer_size && (0xFFFF & datum_index) == i && mem_off != -1 && mem_off != 0)
                {
                    string file_name = DATA_READ.ReadSTRING(tag_names_buffer + buffer_offset, map_stream); //DATA_READ.ReadSTRINGPATH(tag_names_buffer + buffer_offset, map_stream);
                    tag_info temp = new tag_info();
                    temp.datum_index = datum_index;
                    temp.file_loc = file_name;

                    tag_list.Add(temp);
                }
            }
            map_stream.Close();
        }        
        private int align_0x200(int val)
        {
            return align(val, 0x200);
            //return (val | 0x1FF) + 1;
        }
        private int align(int val,int alignment)
        {
            int r = val % alignment;
            if (r == 0)
                return val;
            else return (val / alignment + 1) * alignment;
        }
        private string remove_redundancy(string arg0)
        {
            string t = "";
            char last = '\0';
            foreach(var i in arg0)
            {
                if (i == '\\')
                {
                    if (last != '\\')
                        t += i;
                }               
                else t += i;
                
                last = i;
            }
            return t;
        }
    }
}
