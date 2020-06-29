using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Map_Handler
{
    /// <summary>
    /// just a structure to facilitate writer implementation
    /// </summary>
    struct global_tag_instance
    {
        public byte[] tag_table_data;
        public byte[] tag_data;
        public byte[] tag_raw;
        public byte[] rebase_data;
        public byte[] index_rebase_data;
        public string file_loc;
    };
    /// <summary>
    /// meta_cache_write class implementation
    /// Purpose:To stream_line the cache writing process
    /// Caution:->Tag_RAW has to be correctly rebased before being added to the list
    ///            as it requires meta structure implementation
    ///         ->Tag rebase table and index rebase table has to correctly offset(wrt to each tag) before being added to the list
    /// This class servers no other purpose other than to write the meta data onto the disk in a compounded form
    /// Rebasing and other stuff has to be carried out prior before utilizing this writer class
    /// </summary>
    class meta_cache_writer
    {
        SortedList<int, global_tag_instance> tag_instances;

        const int header_size = 0x3C;

        int version_iteration = 0x1000;
        int tag_raw_size = 0;
        int tag_data_size = 0;
        int rebase_data_size = 0;
        int index_rebase_data_size = 0;
        int file_loc_size = 0;
        string parent_map;

        public meta_cache_writer(string parent_map)
        {
            tag_instances = new SortedList<int, global_tag_instance>();
            this.parent_map = parent_map;
        }
        public int get_header_size()
        {
            return header_size;
        }
        public void add_tag_instance(global_tag_instance arg0)
        {
            tag_instances.Add(BitConverter.ToInt32(arg0.tag_table_data, 0x4), arg0);
            tag_raw_size += arg0.tag_raw.Length;
            tag_data_size += arg0.tag_data.Length;
            rebase_data_size += arg0.rebase_data.Length;
            index_rebase_data_size += arg0.index_rebase_data.Length;
            file_loc_size += arg0.file_loc.Length + 1;///null terminating string
        }
        public void _write(string file_loc)
        {
            StreamWriter output_stream = new StreamWriter(file_loc);

            int BLOCK_START_OFFSET = header_size;
            
            //write header

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
            output_stream.BaseStream.Write(BitConverter.GetBytes(rebase_data_size), 0x0, 0x4);
            ///write datum_index_rebase_table offset
            BLOCK_START_OFFSET += rebase_data_size;
            output_stream.BaseStream.Write(BitConverter.GetBytes(BLOCK_START_OFFSET), 0x0, 0x4);
            ///write datum_index_rebase_table_size
            output_stream.BaseStream.Write(BitConverter.GetBytes(index_rebase_data_size), 0x0, 0x4);
            ///write tag_names offset
            BLOCK_START_OFFSET += index_rebase_data_size;
            output_stream.BaseStream.Write(BitConverter.GetBytes(BLOCK_START_OFFSET), 0x0, 0x4);
            ///write tag_names size
            output_stream.BaseStream.Write(BitConverter.GetBytes(file_loc_size), 0x0, 0x4);
            ///write parent_map offset
            BLOCK_START_OFFSET += file_loc_size;
            output_stream.BaseStream.Write(BitConverter.GetBytes(BLOCK_START_OFFSET), 0x0, 0x4);
            ///write parent_map size
            output_stream.BaseStream.Write(BitConverter.GetBytes(parent_map.Length + 1), 0x0, 0x4);

            //write actual data
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

            output_stream.Dispose();
        }        
    }
}
