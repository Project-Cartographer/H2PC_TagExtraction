using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace DATA_STRUCTURES
{
    /// <summary>
    /// A struct to contain some info on various SID in the map file
    /// </summary>
    struct StringID_info
    {
        public int  string_index_table_index;
        public int string_table_offset;
        public int StringID;
        public string STRING;
    }
    /// <summary>
    /// contains information that is required to update the StringIDs to match the newer map
    /// </summary>
    struct StringIDRef
    {
        public int old_SID;
        public int new_SID;
        public string STRING;
    }
    /// <summary>
    /// contains some info on the tag loaded from the map
    /// </summary>
    struct tag_info
    {
        public int datum_index;
        public string type;
        public string file_loc;
    }
    /// <summary>
    /// just a struct to contain values about the tagRefs in the meta data
    /// </summary>
    struct tagRef
    {
        public string type;
        public int datum_index;
    }
    /// <summary>
    /// just a struct to contain data about the meta that are being compiled
    /// </summary>
    struct injectRefs
    {
        public int old_datum;
        public int new_datum;
        public string file_name;
        public string type;
    };
    /// <summary>
    /// a struct containing data about some crap
    /// </summary>
    struct UnisonRefs
    {
        public string type;
        public int new_datum;
        public string file_name;
    };
    /// <summary>
    /// A class representing the structure halo2 plugins
    /// </summary>
    class plugins_field
    {
        string name;
        int offset;
        int entry_size;

        Dictionary<int, string> Tag_refs;//<offset,name>
        Dictionary<int, string> Data_refs;//<offset,name>
        Dictionary<int, string> stringID;//<offset,name>

        Dictionary<int, string> WCTag_refs;//<offset,name>(withClass tagRefs,somewhat different from the normal tags)

        List<plugins_field> reflexive;

        public plugins_field(string name,int off,int entry_size)
        {
            //initialse some stuff
            this.name = name;
            this.offset = off;
            this.entry_size = entry_size;

            //dynamic allocation
            Tag_refs = new Dictionary<int, string>();
            Data_refs = new Dictionary<int, string>();
            stringID = new Dictionary<int, string>();
            WCTag_refs = new Dictionary<int, string>();

            reflexive = new List<plugins_field>();
        }

        public void Add_tag_ref(int off, string name)
        {
            Tag_refs.Add(off, name);
        }
        public void Add_data_ref(int off, string name)
        {
            Data_refs.Add(off, name);
        }
        public void Add_field(plugins_field field)
        {
            reflexive.Add(field);
        }
        public void Add_stringid_ref(int off, string name)
        {
            stringID.Add(off, name);
        }
        public void Add_WCtag_ref(int off, string name)
        {
            WCTag_refs.Add(off, name);
        }

        public List<int> Get_tag_ref_list()
        {
            return Tag_refs.Keys.ToList<int>();
        }
        public List<int> Get_data_ref_list()
        {
            return Data_refs.Keys.ToList<int>();
        }
        public List<int> Get_stringID_ref_list()
        {
            return stringID.Keys.ToList<int>();
        }
        public List<int> Get_WCtag_ref_list()
        {
            return WCTag_refs.Keys.ToList<int>();
        }
        public List<plugins_field> Get_reflexive_list()
        {
            return reflexive;
        }

        public int Get_offset()
        {
            return offset;
        }
        public int Get_entry_size()
        {
            return entry_size;
        }

        /// <summary>
        /// Returns the structure of the concerned field in Tree node format
        /// </summary>
        /// <returns></returns>
        public TreeNode Get_field_structure()
        {
            TreeNode ret = new TreeNode();

            ret.Text = name+":"+offset.ToString("X")+":"+entry_size.ToString("X");
            
            List<int> Temp_keys;//some temp variable

            //let add the tag_refs
            Temp_keys = Tag_refs.Keys.ToList<int>();
            foreach(int key in Temp_keys)
            {
                ret.Nodes.Add(Tag_refs[key]+":"+key.ToString("X"));
            }

            //Lets add the data refs
            Temp_keys = Data_refs.Keys.ToList<int>();
            foreach (int key in Temp_keys)
            {
                ret.Nodes.Add(Data_refs[key] + ":" + key.ToString("X"));
            }
            //Lets add the stringid refs
            Temp_keys = stringID.Keys.ToList<int>();
            foreach (int key in Temp_keys)
            {
                ret.Nodes.Add(stringID[key] + ":" + key.ToString("X"));
            }
            //Lets add the WCTagRefs
            Temp_keys = WCTag_refs.Keys.ToList<int>();
            foreach (int key in Temp_keys)
            {
                ret.Nodes.Add(WCTag_refs[key] + ":" + key.ToString("X"));
            }
            //Let add the reflexive fields
            foreach (plugins_field temp in reflexive)
            {
                ret.Nodes.Add(temp.Get_field_structure());
            }

            return ret;
        }
    }
    /// <summary>
    /// a class containing the data that aren't contained in tags but are reffered by them
    /// i decided to use the memory address of the extended_meta as an identity
    /// </summary>
    class extended_meta
    {
        /// <summary>
        /// extended meta are contained outside the tags and could only be identified by their memory_addresses
        /// </summary>
        int mem_off;
        int size;
        int entry_size;

        byte[] data;
        StreamReader map_stream;
        plugins_field plugin;

        List<int> ref_reflexive;// a list containing the offset to the location where the reflexive fields are refered in the meta
        List<int> ref_tags;//a list containing the offsets to the locations where other tags (datum_indexes) are refered in the meta
        List<int> ref_data;//a list containing the offsets to the locations where data_refs are refered in the meta,data refs have similar behaviour as reflexive but it points to a data stuff in the meta
        List<int> ref_stringID;//a list containing the offsets to the locations where stringId is refered
        List<int> ref_WCtags;//a list containing the offsets of tagRefs with withClass Attribute,they are a bit different as they only contain the datum index of the refered tag

        //i decided to use the memory address of the extended_meta as an identity
        Dictionary<int, int> ref_extended;//<offset,mem_off>,a dictionary containing the offsets to the locations where the extended meta stuff is referred.      
        Dictionary<int, extended_meta> list_extended;//<mem_off,extended_meta obj>,a dictionary containing the extended meta by their memory address to prevent redundancy.

        /// <summary>
        /// extended meta is similar to the meta
        /// </summary>
        /// <param name="mem_address"></param>
        /// <param name="size">the total size of the extended meta containg all occurences</param>
        /// <param name="count"></param>
        /// <param name="plugin"></param>
        /// <param name="sr"></param>
        public extended_meta(int mem_address,int size,int count,plugins_field plugin,StreamReader sr)
        {
            this.mem_off = mem_address;
            this.size = size;
            this.plugin = plugin;
            this.map_stream = sr;
            this.entry_size = size / count;

            //some meta reading prologue
            int table_off = DATA_READ.ReadINT_LE(0x10, map_stream);
            int table_size = DATA_READ.ReadINT_LE(0x14, map_stream);
            int table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4, map_stream) + 0x20;
            int scnr_off = table_off + table_size;
            int scnr_memaddr = DATA_READ.ReadINT_LE(table_start + 0x8, map_stream);//scnr tag index is 0x0(mostly)

            //read the extended_meta from the map            
            data = new byte[this.size];

            if (!DATA_READ.Check_shared(sr))
            {
                //normal map
                map_stream.BaseStream.Position = scnr_off + (mem_off - scnr_memaddr);
            }
            else
            {
                //shared map
                map_stream.BaseStream.Position = scnr_off + (mem_off - 0x3c000);
                //0x3c000 is a hardcoded value in blam engine
            }

            map_stream.BaseStream.Read(data, 0, this.size);

            //lets initialise some lists and dictionaries
            ref_data = new List<int>();
            ref_tags = new List<int>();
            ref_stringID = new List<int>();
            ref_reflexive = new List<int>();
            ref_extended = new Dictionary<int, int>();
            list_extended = new Dictionary<int, extended_meta>();
            ref_WCtags = new List<int>();

            //we we have loop through the extended meta stuff
            //but first make sure is a reflexive field or not
            if (plugin != null)
            {
                for (int i = 0; i < count; i++)
                {
                    List_deps(i * entry_size, plugin);
                }
            }
        }

        /// <summary>
        /// a function to fill in all the Lists and Dictionaries
        /// </summary>
        /// <param name="off">the starting offset where the stuff is being read</param>
        /// <param name="fields">the concerned field(s) in that section</param>
        void List_deps(int off, plugins_field fields)
        {
            List<int> temp = fields.Get_tag_ref_list();

            //first we look for tag_refs and add them
            foreach (int i in temp)
            {
                int Toff = off + i;//it contains type
                //we add this off to the list if it doesnt contain the off already
                if (!ref_tags.Contains(Toff))
                {
                    ref_tags.Add(Toff);
                }
            }
            //then we look for data_refs and add them
            /*
            temp = fields.Get_data_ref_list();
            foreach (int i in temp)
            {
                int Toff = off + i;
                //we add this off to the list if it doesnt contain the off already
                if (!ref_data.Contains(Toff))
                {
                    ref_data.Add(Toff);
                }
            }
            */
            //***      added this logic 
            temp = fields.Get_data_ref_list();
            foreach (int i in temp)
            {
                int Toff = off + i;

                int length = DATA_READ.ReadINT_LE(Toff, data);
                int field_memaddr = DATA_READ.ReadINT_LE(Toff + 4, data);

                int field_off = field_memaddr - mem_off;//its the offset of the field from the starting of the meta data

                if (length != 0)
                {
                    //now we check whether its inside meta or a case of extended meta
                    if ((field_memaddr >= mem_off) && (field_off < size))
                    {
                        //inside meta
                        //we add this off to the list if it doesnt contain the off already
                        if (!ref_data.Contains(Toff))
                        {
                            ref_data.Add(Toff);
                        }

                    }
                    else
                    {
                        //Here i am using the same method used in the case extended reflexive fields.

                        //extended meta(IN SUCCESSFULL RUN ,EXTENDED META ONLY APPEARS ONLY WHEN WE READ FROM A MAP)

                        //but first we check whether we are reading meta from a map,or an exracted file,its rather easy
                        if (list_extended != null)
                        {
                            //we add this off to the list if it doesnt contain the off already
                            if (!ref_extended.ContainsKey(Toff))
                            {
                                ref_extended.Add(Toff, field_memaddr);
                                //now we create and add extended_meta to the list if it isnt already there
                                if (!list_extended.ContainsKey(field_memaddr))
                                {
                                    extended_meta temp_extend = new extended_meta(field_memaddr, length, 1, null, map_stream);
                                    list_extended.Add(field_memaddr, temp_extend);
                                }
                                //we dont need to look into them as extended meta does it for us
                            }
                        }                        
                    }

                }

            }
            //then we look for stringId refs and add them
            temp = fields.Get_stringID_ref_list();
            foreach (int i in temp)
            {
                int Toff = off + i;
                //we add this off to the list if it doesnt contain the off already
                if (!ref_stringID.Contains(Toff))
                {
                    ref_stringID.Add(Toff);
                }
            }
            //now we look into reflexive fields and extended meta and add them accordingly
            List<plugins_field> Ptemp = fields.Get_reflexive_list();
            foreach (plugins_field i_Pfield in Ptemp)
            {
                int Toff = off + i_Pfield.Get_offset();//field table off contains count

                int count = DATA_READ.ReadINT_LE(Toff, data);
                int field_memaddr = DATA_READ.ReadINT_LE(Toff + 4, data);
                int entry_size = i_Pfield.Get_entry_size();//entry_size

                int field_off = field_memaddr - mem_off;//its the offset of the field from the starting of the meta data

                if (count > 0)
                {
                    //now we check whether its inside meta or a case of extended meta
                    if ((field_memaddr >= mem_off) && (field_off < size))
                    {
                        //inside meta
                        //we add this off which contains reflexive table to the list if it doesnt contain the off already
                        if (!ref_reflexive.Contains(Toff))
                        {
                            ref_reflexive.Add(Toff);
                            //after adding it to the list we look into them,recursively
                            for (int j = 0; j < count; j++)
                            {
                                List_deps(field_off + j * entry_size, i_Pfield);
                            }
                        }

                    }
                    else
                    {
                        //extended meta
                        //we add this off to the list if it doesnt contain the off already
                        if (!ref_extended.ContainsKey(Toff))
                        {
                            ref_extended.Add(Toff, field_memaddr);
                            //now we create and add extended_meta to the list if it isnt already there
                            if (!list_extended.ContainsKey(field_memaddr))
                            {
                                extended_meta temp_extend = new extended_meta(field_memaddr, entry_size * count, count, i_Pfield, map_stream);
                                list_extended.Add(field_memaddr, temp_extend);
                            }
                            //we dont need to look into them as extended meta does it for us
                        }

                    }
                }
            }
            //now we go for withClass attribute tagRefs,they are a bit different as they only contain the datum index of the refered tag
            temp = fields.Get_WCtag_ref_list();
            foreach (int i in temp)
            {
                int Toff = off + i;
                //we add this off to the list if it doesnt contain the off already
                if (!ref_WCtags.Contains(Toff))
                {
                    ref_WCtags.Add(Toff);
                }
            }

        }
        /// <summary>
        /// use to rebase current meta data to a newer memory address(affects reflexive,extended and data ref fields only)
        /// </summary>
        /// <param name="new_base">the new memory address to which the meta has to rebased</param>
        public void Rebase_meta(int new_base)
        {
            //plugin_ref is not present only in the case of extended dataref
            if (plugin != null)
            {
                //first rebase reflexive fields
                foreach (int off in ref_reflexive)
                {
                    int old_mem_addr = DATA_READ.ReadINT_LE(off + 4, data);
                    int new_mem_addr = new_base + (old_mem_addr - mem_off);

                    DATA_READ.WriteINT_LE(new_mem_addr, off + 4, data);
                }
                //then we rebase all the dataref fields
                foreach (int off in ref_data)
                {
                    int old_mem_addr = DATA_READ.ReadINT_LE(off + 4, data);
                    int new_mem_addr = new_base + (old_mem_addr - mem_off);

                    DATA_READ.WriteINT_LE(new_mem_addr, off + 4, data);
                }
                //for extended meta stuff we are gonna first rebase the extended meta(s) first and set the offsets accordingly
                //well extende meta are gonna follow meta one by one
                int extended_new_base = new_base + size;

                List<int> key_mems = list_extended.Keys.ToList<int>();
                //Rebase extended meta
                foreach (int temp_key in key_mems)
                {
                    extended_meta temp_meta = list_extended[temp_key];
                    temp_meta.Rebase_meta(extended_new_base);

                    extended_new_base += temp_meta.Get_Total_size();
                }
                //now lets update the offsets with the newer values
                List<int> extend_off = ref_extended.Keys.ToList<int>();
                foreach (int temp_off in extend_off)
                {
                    int extend_mem_addr = ref_extended[temp_off];
                    extended_meta temp_ext = list_extended[extend_mem_addr];

                    int new_mem_addr = temp_ext.Get_mem_addr();

                    DATA_READ.WriteINT_LE(new_mem_addr, temp_off + 4, data);
                }
            }
            //update the base to which meta has been compiled
            mem_off = new_base;
                
        }
        /// <summary>
        /// returns the total size of the meta along with all extended meta sizes
        /// </summary>
        /// <returns></returns>
        public int Get_Total_size()
        {
            int Tsize = size;

            if (plugin != null)
            {
                //lets add the extended meta sizes
                List<int> key_mems = list_extended.Keys.ToList<int>();

                foreach (int temp_key in key_mems)
                {
                    extended_meta temp_meta = list_extended[temp_key];
                    Tsize += temp_meta.Get_Total_size();
                }
            }

            return Tsize;
        }
        /// <summary>
        /// gets the memory offset of the extende meta to which it is targeted
        /// </summary>
        /// <returns></returns>
        public int Get_mem_addr()
        {
            return mem_off;
        }
        /// <summary>
        /// Generates a full self dependent meta file(except tag refs)
        /// </summary>
        /// <returns></returns>
        public byte[] Generate_meta_file()
        {
            byte[] ret = new byte[this.Get_Total_size()];

            //we first copy the root meta data into it
            DATA_READ.ArrayCpy(ret, data, 0x0, size);

            if (plugin != null)
            {
                //now we go for extended meta
                List<int> extend_keys = list_extended.Keys.ToList<int>();
                //here we go
                foreach (int temp_key in extend_keys)
                {
                    extended_meta temp_meta = list_extended[temp_key];
                    int start_off = temp_meta.Get_mem_addr() - mem_off;
                    DATA_READ.ArrayCpy(ret, temp_meta.Generate_meta_file(), start_off, temp_meta.Get_Total_size());
                }
            }

            return ret;
        }
        /// <summary>
        /// used to null all the stringId in theconcerned meta and extended meta
        /// </summary>
        public void Null_StringID()
        {
            if (plugin != null)
            {
                //first null all my string ids
                foreach (int temp_off in ref_stringID)
                {
                    DATA_READ.WriteINT_LE(0x0, temp_off, data);
                }
                //we then proceed for extended meta
                List<int> keys = list_extended.Keys.ToList<int>();
                foreach (int temp_key in keys)
                {
                    extended_meta temp_meta = list_extended[temp_key];
                    temp_meta.Null_StringID();
                }
            }
        }
        /// <summary>
        /// return a list all tagRefs mentioned in the meta and the extended meta
        /// </summary>
        /// <returns></returns>
        public List<tagRef> Get_all_tag_refs()
        {
            List<tagRef> ret = new List<tagRef>();

            if (plugin != null)
            {
                //first i add all my the tagRefs in the concerned meta
                foreach (int temp_off in ref_tags)
                {
                    string type = DATA_READ.ReadTAG_TYPE(temp_off, data);
                    int temp_datum = DATA_READ.ReadINT_LE(temp_off + 4, data);

                    tagRef temp_tagref = new tagRef();
                    temp_tagref.type = type;
                    temp_tagref.datum_index = temp_datum;

                    ret.Add(temp_tagref);
                }
                //then we add the extended_meta dependencies
                List<int> key_list = list_extended.Keys.ToList<int>();
                foreach (int temp_key in key_list)
                {
                    extended_meta temp_meta = list_extended[temp_key];
                    ret.AddRange(temp_meta.Get_all_tag_refs());
                }
                //listing the WCtagRefs
                //we can only do this when we are reading from a map
                if (map_stream != null)
                {
                    //some meta reading prologue
                    int table_off = DATA_READ.ReadINT_LE(0x10, map_stream);
                    int table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4, map_stream) + 0x20;

                    foreach (int temp_off in ref_WCtags)
                    {
                        int temp_datum = DATA_READ.ReadINT_LE(temp_off, data);//we read it from meta data
                        string type = DATA_READ.ReadTAG_TYPE(table_start + (0xFFFF & temp_datum) * 0x10, map_stream);//we read this from map stream

                        tagRef temp_WCtagref = new tagRef();
                        temp_WCtagref.datum_index = temp_datum;
                        temp_WCtagref.type = type;

                        ret.Add(temp_WCtagref);
                    }
                }
            }

            return ret;
        }
    }
    /// <summary>
    /// a class containing the data of the concerned meta
    /// </summary>
    class meta
    {
        string type;
        int datum_index;       
        int mem_off;// the memory address to which the tag is designed to be loaded at
        int size;

        string path;

        byte[] data;
        StreamReader map_stream;
        plugins_field plugin;
       

        List<int> ref_reflexive;// a list containing the offset to the location where the reflexive fields are refered in the meta
        List<int> ref_tags;//a list containing the offsets to the locations where other tags (datum_indexes) are refered in the meta
        List<int> ref_data;//a list containing the offsets to the locations where data_refs are refered in the meta,data refs have similar behaviour as reflexive but it points to a data stuff in the meta
        List<int> ref_stringID;//a list containing the offsets to the locations where stringId is refered
        List<int> ref_WCtags;//a list containing the offsets of tagRefs with withClass Attribute,they are a bit different as they only contain the datum index of the refered tag

        //i decided to use the memory address of the extended_meta as an identity
        Dictionary<int, int> ref_extended;//<offset,mem_off>,a dictionary containing the offsets to the locations where the extended meta stuff is referred.      
        Dictionary<int, extended_meta> list_extended;//<mem_off,extended_meta obj>,a dictionary containing the extended meta by their memory address to prevent redundancy.

        /// <summary>
        /// use to read meta data from a map file
        /// </summary>
        /// <param name="datum_index">the datum index of the tag</param>
        /// <param name="sr">the path of the tag.eg: characters/elite/elite_mp</param>
        /// <param name="sr">the stream object</param>
        public meta(int datum_index,string path,StreamReader sr)
        {
            //initialise some stuff
            this.datum_index = datum_index;
            this.path = path;
            map_stream = sr;

            //some meta reading prologue
            int table_off =DATA_READ.ReadINT_LE(0x10,map_stream);
            int table_size = DATA_READ.ReadINT_LE(0x14,map_stream);
            int table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4,map_stream) + 0x20;
            int scnr_off = table_off + table_size;
            int scnr_memaddr = DATA_READ.ReadINT_LE(table_start + 0x8,map_stream);//scnr tag index is 0x0(mostly)

            //steps concerned with the specified meta
            type = DATA_READ.ReadTAG_TYPE(table_start + (0xFFFF & datum_index) * 0x10, map_stream);
            mem_off = DATA_READ.ReadINT_LE(table_start + ((0xFFFF & datum_index) * 0x10) + 0x8, map_stream);        

            size = DATA_READ.ReadINT_LE(table_start + ((0xFFFF & datum_index) * 0x10) + 0xC, map_stream);

            //read the meta from the map            
            data = new byte[size];

            if (!DATA_READ.Check_shared(sr))
            {
                //normal map
                map_stream.BaseStream.Position = scnr_off + (mem_off - scnr_memaddr);
            }
            else
            {
                //shared map
                map_stream.BaseStream.Position = scnr_off + (mem_off - 0x3c000);
                //0x3c000 is a hardcoded value in blam engine
            }

            map_stream.BaseStream.Read(data, 0, size);

            //read and store the plugin structure
            plugin = DATA_READ.Get_Tag_stucture_from_plugin(type);

            //lets initialise some lists and dictionaries
            ref_data = new List<int>();
            ref_tags = new List<int>();
            ref_reflexive = new List<int>();
            ref_stringID = new List<int>();
            ref_extended = new Dictionary<int, int>();
            list_extended = new Dictionary<int, extended_meta>();
            ref_WCtags = new List<int>();

            if (size == 0)
                return;

            //now lets search for all kinds of stuff
            List_deps(0x0, plugin);

        }
        /// <summary>
        /// used to read meta data from a meta file
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="path"></param>
        public meta(byte[] meta, string type, int size,string path)
        {
            data = meta;
            this.type = type;
            this.size = size;
            this.path = path;
            this.mem_off = 0x0;//i usually rebase meta to 0x0 when extracting

            //lets initialise some lists and dictionaries
            ref_data = new List<int>();
            ref_tags = new List<int>();
            ref_reflexive = new List<int>();
            ref_stringID = new List<int>();
            ref_WCtags = new List<int>();
            //while extracting the meta ,i fix all the extended meta stuff so we dont need it now

            //plugin
            plugin = DATA_READ.Get_Tag_stucture_from_plugin(type);

            if (size == 0)
                return;
            
            List_deps(0x0, plugin);
        }
        /// <summary>
        /// used to read meta data from a meta file along with ability to modify mem_off
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="path"></param>
        /// <param name="mem_off"></param>
        public meta(byte[] meta, string type, int size,string path,int mem_off)
        {
            data = meta;
            this.type = type;
            this.size = size;
            this.path = path;
            this.mem_off = mem_off;//this is for awkward cases when i rebase the meta to some other shit

            //lets initialise some lists and dictionaries
            ref_data = new List<int>();
            ref_tags = new List<int>();
            ref_reflexive = new List<int>();
            ref_stringID = new List<int>();
            ref_WCtags = new List<int>();
            //while extracting the meta ,i fix all the extended meta stuff so we dont need it now

            plugin = DATA_READ.Get_Tag_stucture_from_plugin(type);

            if (size == 0)
                return;

            List_deps(0x0, plugin);
        }
        /// <summary>
        /// a function to fill in all the Lists and Dictionaries
        /// </summary>
        /// <param name="off">the starting offset where the stuff is being read</param>
        /// <param name="fields">the concerned field(s) in that section</param>
        void List_deps(int off,plugins_field fields)
        {
            List<int> temp = fields.Get_tag_ref_list();
            
            //first we look for tag_refs and add them
            foreach(int i in temp)
            {
                int Toff = off + i;//it contains type
                //we add this off to the list if it doesnt contain the off already
                if (!ref_tags.Contains(Toff))
                {
                    ref_tags.Add(Toff);
                }
            }
            //then we look for data_refs and add them
            //but they have some issue,some dataref refers data outside of the tag
            //so we have to be a bit carefull
            temp = fields.Get_data_ref_list();
            foreach(int i in temp)
            {
                int Toff = off + i;

                int length = DATA_READ.ReadINT_LE(Toff, data);
                int field_memaddr = DATA_READ.ReadINT_LE(Toff + 4, data);

                int field_off = field_memaddr - mem_off;//its the offset of the field from the starting of the meta data

                if(length!=0)
                {
                    //now we check whether its inside meta or a case of extended meta
                    if ((field_memaddr >= mem_off) && (field_off < size))
                    {
                        //inside meta
                        //we add this off to the list if it doesnt contain the off already
                        if (!ref_data.Contains(Toff))
                        {
                            ref_data.Add(Toff);
                        }

                    }
                    else
                    {
                        //Here i am using the same method used in the case extended reflexive fields.

                        //extended meta(IN SUCCESSFULL RUN ,EXTENDED META ONLY APPEARS ONLY WHEN WE READ FROM A MAP)

                        //but first we check whether we are reading meta from a map,or an exracted file,its rather easy
                        if (list_extended != null)
                        {
                            //we add this off to the list if it doesnt contain the off already
                            if (!ref_extended.ContainsKey(Toff))
                            {
                                ref_extended.Add(Toff, field_memaddr);
                                //now we create and add extended_meta to the list if it isnt already there
                                if (!list_extended.ContainsKey(field_memaddr))
                                {
                                    extended_meta temp_extend = new extended_meta(field_memaddr, length, 1, null, map_stream);
                                    list_extended.Add(field_memaddr, temp_extend);
                                }
                                //we dont need to look into them as extended meta does it for us
                            }
                        }
                        else
                        {
                            //the program will only reach here when u try to use an extended meta on meta file.
                            //any meta which i extract from a map file have all issues of extended_meta fixed.
                            throw new Exception("Meta file " + path + "." + type + " is broken.\nEither debug the extraction proceedure or fix the meta file");
                        }
                    }

                }

            }
            //then we look for stringId refs and add them
            temp = fields.Get_stringID_ref_list();
            foreach (int i in temp)
            {
                int Toff = off + i;
                //we add this off to the list if it doesnt contain the off already
                if (!ref_stringID.Contains(Toff))
                {
                    ref_stringID.Add(Toff);
                }
            }
            //now we look into reflexive fields and extended meta and add them accordingly
            List<plugins_field> Ptemp = fields.Get_reflexive_list();
            foreach(plugins_field i_Pfield in Ptemp)
            {
                int Toff = off + i_Pfield.Get_offset();//field table off contains count

                int count = DATA_READ.ReadINT_LE(Toff, data);
                int field_memaddr = DATA_READ.ReadINT_LE(Toff + 4, data);
                int entry_size = i_Pfield.Get_entry_size();//entry_size

                int field_off = field_memaddr - mem_off;//its the offset of the field from the starting of the meta data

                if (count > 0)
                {
                    //now we check whether its inside meta or a case of extended meta
                    if ((field_memaddr >= mem_off) && (field_off < size))
                    {
                        //inside meta
                        //we add this off which contains reflexive table to the list if it doesnt contain the off already
                        if (!ref_reflexive.Contains(Toff))
                        {
                            ref_reflexive.Add(Toff);
                            //after adding it to the list we look into them,recursively
                            for (int j = 0; j < count; j++)
                            {
                                List_deps(field_off + j * entry_size, i_Pfield);
                            }
                        }

                    }
                    else
                    {
                        //extended meta(IN SUCCESSFULL RUN ,EXTENDED META ONLY APPEARS ONLY WHEN WE READ FROM A MAP)

                        //but first we check whether we are reading meta from a map,or an exracted file,its rather easy
                        if (list_extended != null)
                        {
                            //we add this off to the list if it doesnt contain the off already
                            if (!ref_extended.ContainsKey(Toff))
                            {
                                ref_extended.Add(Toff, field_memaddr);
                                //now we create and add extended_meta to the list if it isnt already there
                                if (!list_extended.ContainsKey(field_memaddr))
                                {
                                    extended_meta temp_extend = new extended_meta(field_memaddr, entry_size * count, count, i_Pfield, map_stream);
                                    list_extended.Add(field_memaddr, temp_extend);
                                }
                                //we dont need to look into them as extended meta does it for us
                            }
                        }
                        else
                        {
                            //the program will only reach here when u try to use an extended meta on meta file.
                            //any meta which i extract from a map file have all issues of extended_meta fixed.
                            throw new Exception("Meta file " + path + "." + type + " is broken.\nEither debug the extraction proceedure or fix the meta file");
                        }

                    }
                }
            }
            //now we go for withClass attribute tagRefs,they are a bit different as they only contain the datum index of the refered tag
            temp = fields.Get_WCtag_ref_list();
            foreach (int i in temp)
            {
                int Toff = off + i;
                //we add this off to the list if it doesnt contain the off already
                if (!ref_WCtags.Contains(Toff))
                {
                    ref_WCtags.Add(Toff);
                }
            }
        }
        /// <summary>
        /// use to rebase current meta data to a newer memory address(affects reflexive,extended and data ref fields only)
        /// </summary>
        /// <param name="new_base">the new memory address to which the meta has to rebased</param>
        public void Rebase_meta(int new_base)
        {

            //first rebase reflexive fields
            foreach (int off in ref_reflexive)
            {
                int old_mem_addr = DATA_READ.ReadINT_LE(off + 4, data);
                int new_mem_addr = new_base + (old_mem_addr - mem_off);

                DATA_READ.WriteINT_LE(new_mem_addr, off + 4, data);
            }
            //then we rebase all the dataref fields
            foreach(int off in ref_data)
            {
                int old_mem_addr = DATA_READ.ReadINT_LE(off + 4, data);
                int new_mem_addr = new_base + (old_mem_addr - mem_off);

                DATA_READ.WriteINT_LE(new_mem_addr, off + 4, data);
            }
            //we venture in extended meta if it is even concerned
            if (list_extended != null)
            {
                //for extended meta stuff we are gonna first rebase the extended meta(s) first and set the offsets accordingly
                //well extende meta are gonna follow meta one by one
                int extended_new_base = new_base + size;

                List<int> key_mems = list_extended.Keys.ToList<int>();
                //Rebase extended meta
                foreach (int temp_key in key_mems)
                {
                    extended_meta temp_meta = list_extended[temp_key];
                    temp_meta.Rebase_meta(extended_new_base);

                    extended_new_base += temp_meta.Get_Total_size();
                }
                //now lets update the offsets with the newer values
                List<int> extend_off = ref_extended.Keys.ToList<int>();
                foreach (int temp_off in extend_off)
                {
                    int extend_mem_addr = ref_extended[temp_off];
                    extended_meta temp_ext = list_extended[extend_mem_addr];

                    int new_mem_addr = temp_ext.Get_mem_addr();

                    DATA_READ.WriteINT_LE(new_mem_addr, temp_off + 4, data);
                }
            }

            //update the base to which meta has been compiled
            mem_off = new_base;
        }
        /// <summary>
        /// returns the total size of the meta along with all extended meta sizes
        /// </summary>
        /// <returns></returns>
        public int Get_Total_size()
        {
            int Tsize = size;

            if (list_extended != null)
            {
                //lets add the extended meta sizes
                List<int> key_mems = list_extended.Keys.ToList<int>();

                foreach (int temp_key in key_mems)
                {
                    extended_meta temp_meta = list_extended[temp_key];
                    Tsize += temp_meta.Get_Total_size();
                }
            }

            return Tsize;
        }
        /// <summary>
        /// Generates a full self dependent meta file(except tag refs)
        /// </summary>
        /// <returns></returns>
        public byte[] Generate_meta_file()
        {
            byte[] ret = new byte[this.Get_Total_size()];

            //we first copy the root meta data into it
            DATA_READ.ArrayCpy(ret, data, 0x0, size);

            if (list_extended != null)
            {
                //now we go for extended meta
                List<int> extend_keys = list_extended.Keys.ToList<int>();
                //here we go
                foreach (int temp_key in extend_keys)
                {
                    extended_meta temp_meta = list_extended[temp_key];
                    int start_off = temp_meta.Get_mem_addr() - mem_off;
                    DATA_READ.ArrayCpy(ret, temp_meta.Generate_meta_file(), start_off, temp_meta.Get_Total_size());
                }
            }

            return ret;
        }
        /// <summary>
        /// returns the path of the concerned meta
        /// </summary>
        /// <returns></returns>
        public string Get_Path()
        {
            return path;
        }
        /// <summary>
        /// return the type of tag
        /// </summary>
        /// <returns></returns>
        public string Get_Type()
        {
            return type;
        }
        /// <summary>
        /// used to null all the stringId in theconcerned meta and extended meta
        /// </summary>
        public void Null_StringID()
        {
            //first null all my string ids
            foreach (int temp_off in ref_stringID)
            {                
                DATA_READ.WriteINT_LE(0x0, temp_off, data);               
            }

            if (list_extended != null)
            {
                //we then proceed for extended meta
                List<int> keys = list_extended.Keys.ToList<int>();
                foreach (int temp_key in keys)
                {
                    extended_meta temp_meta = list_extended[temp_key];
                    temp_meta.Null_StringID();
                }
            }
        }
        /// <summary>
        /// return a list all tagRefs mentioned in the meta and the extended meta
        /// </summary>
        /// <returns></returns>
        public List<tagRef> Get_all_tag_refs()
        {
            List<tagRef> ret = new List<tagRef>();

            //first i add all my the tagRefs in the concerned meta
            foreach (int temp_off in ref_tags)
            {
                string type = DATA_READ.ReadTAG_TYPE(temp_off, data);
                int temp_datum = DATA_READ.ReadINT_LE(temp_off + 4, data);

                //i only list them if they are valid
                if (temp_datum != -1)
                {
                    tagRef temp_tagref = new tagRef();
                    temp_tagref.type = type;
                    temp_tagref.datum_index = temp_datum;

                    ret.Add(temp_tagref);
                }
            }
            //list_extended object is only created when we are reading from a map
            if (list_extended != null)
            {
                //then we add the extended_meta dependencies
                List<int> key_list = list_extended.Keys.ToList<int>();
                foreach (int temp_key in key_list)
                {
                    extended_meta temp_meta = list_extended[temp_key];
                    ret.AddRange(temp_meta.Get_all_tag_refs());
                }
            }
            //listing the WCtagRefs
            //we can only do this when we are reading from a map
            if(map_stream!=null)
            {
                //some meta reading prologue
                int table_off = DATA_READ.ReadINT_LE(0x10, map_stream);
                int table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4, map_stream) + 0x20;                

                foreach(int temp_off in ref_WCtags)
                {
                    int temp_datum = DATA_READ.ReadINT_LE(temp_off, data);//we read it from meta data
                    if (temp_datum != -1)
                    {
                        string type = DATA_READ.ReadTAG_TYPE(table_start + (0xFFFF & temp_datum) * 0x10, map_stream);//we read this from map stream

                        tagRef temp_WCtagref = new tagRef();
                        temp_WCtagref.datum_index = temp_datum;
                        temp_WCtagref.type = type;

                        ret.Add(temp_WCtagref);
                    }
                }
            }

            return ret;
        }
        /// <summary>
        /// a function that updates the datum indexes acoording to the list supplied
        /// </summary>
        /// <param name="tag_list"></param>
        /// <returns>return a log about different encounters</returns>
        public string Update_datum_indexes(List<injectRefs> tag_list, List<UnisonRefs> type_ref_list)
        {
            string log = "\nUPDATE DATUM : " + path;

            //we loop through each offset

            //Updating TagRefs
            foreach (int temp_off in ref_tags)
            {
                int temp_old_datum = DATA_READ.ReadINT_LE(temp_off + 4, data);
                //next we loop through each list
                bool sucess = false;

                foreach (injectRefs temp_ref in tag_list)
                {
                    int old_datum = temp_ref.old_datum;
                    int new_datum = temp_ref.new_datum;

                    if (old_datum == temp_old_datum)
                    {
                        //we found a match
                        if (!sucess)
                        {
                            DATA_READ.WriteINT_LE(new_datum, temp_off + 4, data);
                            log += "\nSuccesfully refered " + temp_old_datum.ToString("X") + " to " + new_datum.ToString("X");
                            sucess = true;
                        }
                        else log += "\nMultiple occurences of old datum " + old_datum + " has been found";
                    }
                }
                if (!sucess)
                {
                    //next we try to link the tags by using tag type
                    string type = DATA_READ.ReadTAG_TYPE(temp_off, data);

                    foreach(UnisonRefs temp_uni in type_ref_list)
                    {
                        if (type == temp_uni.type && temp_old_datum != -1)
                        {
                            sucess = true;
                            DATA_READ.WriteINT_LE(temp_uni.new_datum, temp_off + 4, data);
                            log += "\nUnison reference " + temp_uni.type + " " + temp_old_datum.ToString("X") + " to " + temp_uni.new_datum.ToString("X");
                        }
                    }
                    if (!sucess)
                        log += "\nCouldnot find reference to " + temp_old_datum.ToString("X");
                }
                
            }
            //Updating WCtagRefs
            foreach (int temp_off in ref_WCtags)
            {
                int temp_old_datum = DATA_READ.ReadINT_LE(temp_off, data);
                bool sucess = false;

                foreach (injectRefs temp_ref in tag_list)
                {
                    int old_datum = temp_ref.old_datum;
                    int new_datum = temp_ref.new_datum;

                    if (old_datum == temp_old_datum)
                    {
                        //we found a match
                        if (!sucess)
                        {
                            DATA_READ.WriteINT_LE(new_datum, temp_off, data);
                            log += "\nSuccesfully refered " + temp_old_datum.ToString("X") + " to " + new_datum.ToString("X");
                            sucess = true;
                        }
                        else log += "\nMultiple occurences of old datum " + old_datum + " has been found";
                    }
                }
                if (!sucess)
                {
                    //Unfortunately WCtagRefs dont contain the type of tag to which they refer

                    log += "\nCouldnot find reference to " + temp_old_datum.ToString("X");
                }
            }

            return log;

        }
        /// <summary>
        /// A function that updates the StringIDS of the tag to match with the newer map
        /// </summary>
        /// <param name="SID_list"></param>
        /// <returns>return info on various encounters</returns>
        public string Update_StringID(List<StringIDRef> SID_list)
        {
            string log = "\nUPDATE StringID : " + path;

            foreach(int temp_off in ref_stringID)
            {

                int temp_old_SID = DATA_READ.ReadINT_LE(temp_off, data);

                if (temp_old_SID != 0)
                {
                    foreach (StringIDRef temp_SID_ref in SID_list)
                    {
                        int old_SID = temp_SID_ref.old_SID;
                        int new_SID = temp_SID_ref.new_SID;

                        if (temp_old_SID == old_SID)
                        {
                            if (new_SID != -1)
                            {
                                DATA_READ.WriteINT_LE(new_SID, temp_off, data);
                                log += "\nSuccesfully refered " + temp_old_SID.ToString("X") + " to " + new_SID.ToString("X") + " : " + temp_SID_ref.STRING;
                                break;
                            }
                            else
                            {
                                log += "\nCouldnt refer " + temp_old_SID.ToString("X") + " : " + temp_SID_ref.STRING;
                                break;
                            }
                        }

                    }
                }


            }

            return log;
        }
    }
}
