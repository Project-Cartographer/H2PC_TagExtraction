using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATA_STRUCTURES;
using System.Xml;
using System.Globalization;
using System.Windows.Forms;
using System.IO;

namespace Map_Handler
{
    class Resyncer
    {
        string resync_type;
        string directory;

        //list of meta which are gonna be resynced
        List<injectRefs> compile_list;
        List<string> scneario_list;
        List<UnisonRefs> type_ref_list;//they are used to universally reference a tag depending on the type of tagRef

        /// <summary>
        /// constructor to intialize stuff
        /// </summary>
        /// <param name="file"></param>
        /// <param name="type"></param>
        public Resyncer(string file,string type)
        {
            directory = DATA_READ.ReadDirectory_from_file_location(file);
            resync_type = type;

            XmlDocument xd = new XmlDocument();
            xd.Load(file);

            compile_list = new List<injectRefs>();
            scneario_list = new List<string>();
            type_ref_list = new List<UnisonRefs>();

            foreach (XmlNode Xn in xd.SelectNodes("config/tag"))
            {
                injectRefs temp = new injectRefs();

                temp.old_datum = int.Parse(Xn.SelectSingleNode("datum").InnerText, NumberStyles.HexNumber);
                temp.new_datum = -1;
                temp.file_name = Xn.SelectSingleNode("name").InnerText;
                temp.type = DATA_READ.ReadTAG_TYPE_form_name(temp.file_name);

                scneario_list.Add(Xn.SelectSingleNode("scenario").InnerText);

                //lets add the tag to the list
                compile_list.Add(temp);
            }
            
            sync();
        }
        /// <summary>
        /// actuall function to resync the desired type of tagRefs in accordance with the new map
        /// </summary>
        void sync()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            MessageBox.Show("OPEN PARENT MAP", "GitGud");
            ofd.Filter = "Halo 2 Vista Map (*.map)|*.map";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string parent_map = ofd.FileName;

                MessageBox.Show("OPEN TARGET MAP", "GitGud");
                ofd.Filter = "Halo 2 Vista Map (*.map)|*.map";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string target_loc = ofd.FileName;

                    string log = "LOG_BOX\n";

                    List<injectRefs> resync_list = Get_Resync_List(parent_map, target_loc);

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
                            log += obj.Update_datum_indexes(resync_list, type_ref_list);


                            //write it to the file
                            StreamWriter sw = new StreamWriter(directory + "\\" + temp_ref.file_name);
                            sw.BaseStream.Seek(0, SeekOrigin.Begin);
                            sw.BaseStream.Write(meta, 0, (int)size);
                            sw.Dispose();

                        }
                        else log += "\nFile doesnt exists : " + temp_ref.file_name;

                    }
                    StreamWriter sw_1 = new StreamWriter(directory + "\\Tagref_resync_log.txt");
                    sw_1.Write(log);

                    LogBox lol = new LogBox(log);
                    lol.Show();

                }
            }
        }
        /// <summary>
        /// Generates a list resynced tags
        /// </summary>
        /// <param name="parent_map_loc"></param>
        /// <param name="new_map_loc"></param>
        /// <returns></returns>
        List<injectRefs> Get_Resync_List(string parent_map_loc,string new_map_loc)
        {
            List<injectRefs> ret = new List<injectRefs>();

            List<tag_info> Parent_map_tags = Get_Tag_list(parent_map_loc);
            List<tag_info> New_map_tags = Get_Tag_list(new_map_loc);


            List<tag_info> reduced_A = new List<tag_info>();
            List<tag_info> reduced_B = new List<tag_info>();

            foreach (tag_info tempA in Parent_map_tags)
                if (tempA.type.CompareTo(resync_type) == 0)
                    reduced_A.Add(tempA);

            foreach (tag_info tempB in New_map_tags)
                if (tempB.type.CompareTo(resync_type) == 0)
                    reduced_B.Add(tempB);

            for (int i = 0; i < reduced_A.Count; i++)
            {
                for (int j = 0; j < reduced_B.Count; j++)
                {
                    if (reduced_A[i].file_loc.CompareTo(reduced_B[j].file_loc) == 0)
                    {
                        injectRefs temp = new injectRefs();

                        temp.old_datum = reduced_A[i].datum_index;
                        temp.new_datum = reduced_B[j].datum_index;
                        temp.type = reduced_A[i].type;

                        ret.Add(temp);

                        reduced_A.RemoveAt(i--);
                        reduced_B.RemoveAt(j);

                        break;
                    }
                }
            }
                        
            return ret;
        }
        /// <summary>
        /// Gets the list of tags contained in the map
        /// </summary>
        /// <param name="map_loc"></param>
        /// <returns></returns>
        List<tag_info> Get_Tag_list(string map_loc)
        {
                List<tag_info> ret = new List<tag_info>();


                StreamReader map_stream = new StreamReader(map_loc);

                int table_off = DATA_READ.ReadINT_LE(0x10, map_stream);
                int table_size = DATA_READ.ReadINT_LE(0x14, map_stream);
                int file_table_offset = DATA_READ.ReadINT_LE(0x2D0, map_stream);
                int table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4, map_stream) + 0x20;


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
                        tag_info temp = new tag_info();

                        temp.datum_index = datum_index;
                        temp.type = type;
                        temp.file_loc = path;

                        ret.Add(temp);

                        //ugh! is basically the last tag
                        if (type.CompareTo("ugh!") == 0)
                            break;

                        path_start += path.Length + 1;
                    }
                }
                map_stream.Close();
                        
            return ret;
        }

    }
    class Resync_SID
    {
        string directory;

        //list of meta which are gonna be resynced
        List<injectRefs> compile_list;
        List<string> scneario_list;

        public Resync_SID(string file)
        {
            directory = DATA_READ.ReadDirectory_from_file_location(file);

            XmlDocument xd = new XmlDocument();
            xd.Load(file);

            compile_list = new List<injectRefs>();
            scneario_list = new List<string>();

            foreach (XmlNode Xn in xd.SelectNodes("config/tag"))
            {
                injectRefs temp = new injectRefs();

                temp.old_datum = int.Parse(Xn.SelectSingleNode("datum").InnerText, NumberStyles.HexNumber);
                temp.new_datum = -1;
                temp.file_name = Xn.SelectSingleNode("name").InnerText;
                temp.type = DATA_READ.ReadTAG_TYPE_form_name(temp.file_name);

                scneario_list.Add(Xn.SelectSingleNode("scenario").InnerText);

                //lets add the tag to the list
                compile_list.Add(temp);
            }
            sync();
        }
        /// <summary>
        /// Actual syncing process takes place over here
        /// </summary>
        void sync()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            MessageBox.Show("OPEN PARENT MAP", "GitGud");
            ofd.Filter = "Halo 2 Vista Map (*.map)|*.map";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string parent_map = ofd.FileName;

                MessageBox.Show("OPEN TARGET MAP", "GitGud");
                ofd.Filter = "Halo 2 Vista Map (*.map)|*.map";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string target_loc = ofd.FileName;

                    string log = "LOG_BOX\n";

                    List<StringIDRef> resync_list = Generate_SID_refs(parent_map, target_loc);

                    foreach (injectRefs temp_ref in compile_list)
                    {                  
                        ///
                        ///Doing string comaparision stuff takes a incredible amount of time
                        ///    

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
                            log += obj.Update_StringID(resync_list);


                            //write it to the file
                            StreamWriter sw = new StreamWriter(directory + "\\" + temp_ref.file_name);
                            sw.BaseStream.Seek(0, SeekOrigin.Begin);
                            sw.BaseStream.Write(meta, 0, (int)size);
                            sw.Dispose();

                        }
                        else log += "\nFile doesnt exists : " + temp_ref.file_name;

                    }
                    StreamWriter sw_1 = new StreamWriter(directory + "\\StringID_resync_logs.txt");
                    sw_1.Write(log);

                    LogBox lol = new LogBox(log);
                    lol.Show();

                }
            }
        }
        /// <summary>
        /// Generates the reference data required to remap StringIDs
        /// </summary>
        /// <param name="parent_map"></param>
        /// <param name="target_map"></param>
        /// <returns></returns>
        List<StringIDRef> Generate_SID_refs(string parent_map_loc,string target_map_loc)
        {
            List<StringID_info> parent_map = Get_SID_list(parent_map_loc);
            List<StringID_info> target_map = Get_SID_list(target_map_loc);

            List<StringIDRef> ret = new List<StringIDRef>();

            //adding the stuff that is getting linked
            for (int i = 0; i < parent_map.Count; i++)
            {
                for (int j = 0; j < target_map.Count; j++)
                {
                    if (parent_map[i].STRING.CompareTo(target_map[j].STRING) == 0)
                    {
                        StringIDRef ret_temp = new StringIDRef();

                        ret_temp.old_SID = parent_map[i].StringID;
                        ret_temp.new_SID = target_map[j].StringID;
                        ret_temp.STRING = parent_map[i].STRING;

                        ret.Add(ret_temp);

                        parent_map.RemoveAt(i--);
                        target_map.RemoveAt(j);

                        break;
                    }
                }
            }
            //Now adding the remaining stuff,which didnt got linked
            foreach(StringID_info unlinked_junk in parent_map)
            {
                StringIDRef ret_temp = new StringIDRef();

                ret_temp.old_SID = unlinked_junk.StringID;
                ret_temp.new_SID = -1;
                ret_temp.STRING = unlinked_junk.STRING;

                ret.Add(ret_temp);            
            }

            return ret;
        }


        /// <summary>
        /// Generates a list of StringIndexes contained in the map
        /// </summary>
        /// <returns>List of StringID_info</returns>
        List<StringID_info> Get_SID_list(string map_loc)
        {
            List<StringID_info> ret = new List<StringID_info>();

            StreamReader map_stream = new StreamReader(map_loc);

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

                    ret.Add(SIDI);
                }
            }

            map_stream.Close();

            return ret;
        }
    }
}
