using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DATA_STRUCTURES;


namespace Map_Handler
{
    /// <summary>
    /// a structure to facilitate passing of arguments
    /// </summary>
    struct global_tag_instance
    {
        public byte[] tag_data;//tag data containing both meta and raw data
        public string tag_type;
        public string tag_file_loc;
    };
    /// <summary>
    /// meta_cache_write class implementation
    /// Purpose:To stream_line the cache writing process
    /// </summary>
    class meta_cache_writer
    {
        /// <summary>
        /// internal to facilitate writer implementation
        /// </summary>
        struct cache_table_entities
        {
            public byte[] tag_table_data;
            public byte[] tag_data;
            public byte[] tag_raw;
            public byte[] rebase_data;
            public byte[] index_rebase_data;
            public string file_loc;
        };
        SortedList<int, cache_table_entities> tag_instances;

        List<injectRefs> compile_list;
        List<UnisonRefs> type_ref_list;

        const int header_size = 0x40;

        int magic = 'm' + 'o' << 16 + 'd' << 32 + 'u' << 48;
        int version_iteration = 0x1001;
        int tag_index = 0;
        int tag_raw_size = 0;
        int tag_data_size = 0;
        int rebase_table_size = 0;
        int index_rebase_table_size = 0;
        int file_loc_buffer_size = 0;
        string parent_map;

        public meta_cache_writer(string parent_map, List<injectRefs> compile_list, List<UnisonRefs> type_ref_list)
        {
            tag_instances = new SortedList<int, cache_table_entities>();
            this.parent_map = parent_map;
            this.compile_list = compile_list;
            this.type_ref_list = type_ref_list;
        }
        public void add_tag_instance(global_tag_instance arg0)
        {
            cache_table_entities t_tag_instance;

            RAW_data raw_obj = new RAW_data(arg0.tag_data, arg0.tag_type);
            meta meta_obj = new meta(arg0.tag_data, arg0.tag_type, arg0.tag_data.Length - raw_obj.get_total_RAW_size(), arg0.tag_file_loc);

            raw_obj.rebase_RAW_data(tag_raw_size + header_size);
            meta_obj.Rebase_meta(tag_data_size);
            meta_obj.Update_datum_indexes(compile_list, type_ref_list);

            var tag_rebase_table = meta_obj.Generate_rebase_table(tag_data_size).ToArray<int>();
            var tag_index_table = meta_obj.Generate_index_rebase_table(tag_data_size).ToArray<int>();

            byte[] tag_table_element = new byte[0x10];

            byte[] tag_type = Encoding.ASCII.GetBytes(arg0.tag_type);
            Array.Reverse(tag_type);
            tag_type.CopyTo(tag_table_element, 0x0);
            BitConverter.GetBytes(tag_index).CopyTo(tag_table_element, 0x4);
            BitConverter.GetBytes(tag_data_size).CopyTo(tag_table_element, 0x8);
            BitConverter.GetBytes(meta_obj.Get_Total_size()).CopyTo(tag_table_element, 0xC);

            t_tag_instance.tag_table_data = tag_table_element;
            t_tag_instance.tag_raw = raw_obj.get_RAW_data();
            t_tag_instance.tag_data = meta_obj.Generate_meta_data();
            t_tag_instance.rebase_data = new byte[tag_rebase_table.Length * 0x4];
            Buffer.BlockCopy(tag_rebase_table, 0, t_tag_instance.rebase_data, 0, tag_rebase_table.Length * 0x4);
            t_tag_instance.index_rebase_data = new byte[tag_index_table.Length * 0x4];
            Buffer.BlockCopy(tag_index_table, 0, t_tag_instance.index_rebase_data, 0, tag_index_table.Length * 0x4);
            t_tag_instance.file_loc = arg0.tag_file_loc;


            tag_instances.Add(tag_index, t_tag_instance);

            tag_index += 1;
            tag_raw_size += t_tag_instance.tag_raw.Length;
            tag_data_size += t_tag_instance.tag_data.Length;
            rebase_table_size += t_tag_instance.rebase_data.Length;
            index_rebase_table_size += t_tag_instance.index_rebase_data.Length;
            file_loc_buffer_size += t_tag_instance.file_loc.Length + 1;///null terminating string
        }
        public void write(string file_loc)
        {
            StreamWriter output_stream = new StreamWriter(file_loc);

            //write header
            _write_header(ref output_stream);
            //write actual data
            _write_body(ref output_stream);

            output_stream.Dispose();
        }       
        private void _write_header(ref StreamWriter output_stream)
        {
            int BLOCK_START_OFFSET = header_size;
            ///write magic
            output_stream.BaseStream.Write(BitConverter.GetBytes(magic), 0x0, 0x4);
            ///write version
            output_stream.BaseStream.Write(BitConverter.GetBytes(version_iteration), 0x0, 0x4);
            ///write RAW_table_offset
            output_stream.BaseStream.Write(BitConverter.GetBytes(BLOCK_START_OFFSET), 0x0, 0x4);
            ///write RAW_table_size
            output_stream.BaseStream.Write(BitConverter.GetBytes(tag_raw_size), 0x0, 0x4);
            ///write tag_table_offset
            BLOCK_START_OFFSET += tag_raw_size;
            output_stream.BaseStream.Write(BitConverter.GetBytes(BLOCK_START_OFFSET), 0x0, 0x4);
            ///write tag_table size
            output_stream.BaseStream.Write(BitConverter.GetBytes(tag_instances.Count * 0x10), 0x0, 0x4);
            ///write tag_data offset
            BLOCK_START_OFFSET += tag_instances.Count * 0x10;
            output_stream.BaseStream.Write(BitConverter.GetBytes(BLOCK_START_OFFSET), 0x0, 0x4);
            ///write tag_data_size
            output_stream.BaseStream.Write(BitConverter.GetBytes(tag_data_size), 0x0, 0x4);
            ///write rebase_table_offset
            BLOCK_START_OFFSET += tag_data_size;
            output_stream.BaseStream.Write(BitConverter.GetBytes(BLOCK_START_OFFSET), 0x0, 0x4);
            ///write rebase_table_size
            output_stream.BaseStream.Write(BitConverter.GetBytes(rebase_table_size), 0x0, 0x4);
            ///write datum_index_rebase_table offset
            BLOCK_START_OFFSET += rebase_table_size;
            output_stream.BaseStream.Write(BitConverter.GetBytes(BLOCK_START_OFFSET), 0x0, 0x4);
            ///write datum_index_rebase_table_size
            output_stream.BaseStream.Write(BitConverter.GetBytes(index_rebase_table_size), 0x0, 0x4);
            ///write tag_names offset
            BLOCK_START_OFFSET += index_rebase_table_size;
            output_stream.BaseStream.Write(BitConverter.GetBytes(BLOCK_START_OFFSET), 0x0, 0x4);
            ///write tag_names size
            output_stream.BaseStream.Write(BitConverter.GetBytes(file_loc_buffer_size), 0x0, 0x4);
            ///write parent_map offset
            BLOCK_START_OFFSET += file_loc_buffer_size;
            output_stream.BaseStream.Write(BitConverter.GetBytes(BLOCK_START_OFFSET), 0x0, 0x4);
            ///write parent_map size
            output_stream.BaseStream.Write(BitConverter.GetBytes(parent_map.Length + 1), 0x0, 0x4);
            BLOCK_START_OFFSET += parent_map.Length + 1;
        }
        private void _write_body(ref StreamWriter output_stream)
        {
            var tag_keys = tag_instances.Keys.ToList();
            ///write tag_raw
            foreach (var i in tag_keys)
                output_stream.BaseStream.Write(tag_instances[i].tag_raw, 0x0, tag_instances[i].tag_raw.Length);
            ///write tag_table
            foreach (var i in tag_keys)
                output_stream.BaseStream.Write(tag_instances[i].tag_table_data, 0x0, 0x10);            
            ///write tag_data
            foreach (var i in tag_keys)
                output_stream.BaseStream.Write(tag_instances[i].tag_data, 0x0, tag_instances[i].tag_data.Length);
            ///write rebase table
            foreach (var i in tag_keys)
                output_stream.BaseStream.Write(tag_instances[i].rebase_data, 0x0, tag_instances[i].rebase_data.Length);
            ///write index rebase table
            foreach (var i in tag_keys)
                output_stream.BaseStream.Write(tag_instances[i].index_rebase_data, 0x0, tag_instances[i].index_rebase_data.Length);
            ///write tag names
            foreach (var i in tag_keys)
                output_stream.BaseStream.Write(Encoding.ASCII.GetBytes(tag_instances[i].file_loc + '\0'), 0x0, tag_instances[i].file_loc.Length + 1);
            ///write parent map name
            output_stream.BaseStream.Write(Encoding.ASCII.GetBytes(parent_map + '\0'), 0x0, parent_map.Length + 1);
        }
    }
}
