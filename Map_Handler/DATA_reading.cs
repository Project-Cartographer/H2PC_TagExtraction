using System;
using System.IO;
using System.Windows.Forms;
using DATA_STRUCTURES;
using System.Xml;
using System.Globalization;
using System.Runtime.InteropServices;

class DATA_READ
{
    /// <summary>
    /// Returns the little endian integer at the specified position
    /// </summary>
    /// <param name="position">the Offset from the file start</param>
    /// <returns></returns>
    public static int ReadINT_LE(int position, StreamReader map_stream)
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
    /// read little endian int from the meta data
    /// </summary>
    /// <param name="position"></param>
    /// <param name="meta"></param>
    /// <returns></returns>
    public static int ReadINT_LE(int position, byte[] meta)
    {
        int S = 0;

        for (int i = 0; i < 4; i++)
        {
            S += meta[position + 3 - i] * (int)Math.Pow(0x100, 3 - i);
        }

        return S;
    }
    /// <summary>
    /// Returns the string starting at the specified position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string ReadSTRING(int position, StreamReader map_stream)
    {
        map_stream.DiscardBufferedData();
        map_stream.BaseStream.Position = position;

        string text = "";

        while (true)
        {
            char c = (char)map_stream.Read();

            if (c == '\0')
                break;

            text += c;
        }

        text.Trim();
        return text;
    }
    /// <summary>
    /// return the the string at the specified position in form of path
    /// </summary>
    /// <param name="position"></param>
    /// <param name="map_stream"></param>
    /// <returns></returns>
    public static string ReadSTRINGPATH(int position, StreamReader map_stream)
    {
        map_stream.DiscardBufferedData();
        map_stream.BaseStream.Position = position;

        string text = "";

        while (true)
        {
            char c = (char)map_stream.Read();

            if(c=='\\')
                text+='\\';

            if (c == '\0')
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
    public static string ReadTAG_TYPE(int position, StreamReader map_stream)
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
    /// reads a 4 byte little endian tag type from the meta data,starting from a specified position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="meta"></param>
    /// <returns></returns>
    public static string ReadTAG_TYPE(int position, byte[] meta)
    {
        char[] type = new char[4];

        for (int i = 0; i < 4; i++)
            type[3 - i] = (char)meta[position + i];

        string C = new string(type);

        C = C.TrimEnd();

        return C;
    }
    /// <summary>
    /// Return the type of the tag from its name
    /// </summary>
    /// <param name="name">eg: elite.char</param>
    /// <returns></returns>
    public static string ReadTAG_TYPE_form_name(string name)
    {
        int i;
        for (i = 0; i < name.Length; i++)
        {
            if (name[i] == '.')
                break;
        }
        return name.Substring(i + 1);
    }
    /// <summary>
    /// Gets directory from file location
    /// </summary>
    /// <param name="file_loc">location of the file</param>
    /// <returns></returns>
    public static string ReadDirectory_from_file_location(string file_loc)
    {
        return file_loc.Substring(0, file_loc.LastIndexOf("\\"));
    }
    /// <summary>
    /// Return the file from the file location
    /// </summary>
    /// <param name="file_loc"></param>
    /// <returns></returns>
    public static string Read_File_from_file_location(string file_loc)
    {
        return file_loc.Substring(file_loc.LastIndexOf("\\") + 1);
    }
    /// <summary>
    /// Returns the file name without type
    /// </summary>
    /// <param name="file_loc"></param>
    /// <returns></returns>
    public static string Read_File_name_from_location(string file_loc)
    {
        int length = file_loc.LastIndexOf(".") - file_loc.LastIndexOf("\\") - 1;
        return file_loc.Substring(file_loc.LastIndexOf("\\") + 1, length);
    }
    /// <summary>
    /// Gets the text from the tree of the specified tag_ref_addr as the key
    /// </summary>
    /// <param name="tnc"></param>
    /// <param name="tag_ref_addr"></param>
    /// <returns></returns>
    public static string Get_PathfromTree(TreeNodeCollection tnc, int tag_ref_addr)
    {
        string ret = null;

        foreach (TreeNode tn in tnc)
        {
            //it has child nodes,and we only care for child nodes
            if (tn.Nodes.Count > 0)
                ret = Get_PathfromTree(tn.Nodes, tag_ref_addr);

            else if (Int32.Parse(tn.Name) == tag_ref_addr)
                ret = tn.Text;

            if (ret != null)
                return ret;

        }

        return null;
    }
   /// <summary>
   /// reads the specified plugin to generate the structure of the plugin
   /// </summary>
   /// <param name="type">the type of the tag</param>
    public static plugins_field Get_Tag_stucture_from_plugin(string type)
    {
        
        //Plugin Search Stuff
        string plugin_loc = Application.StartupPath + "\\plugins\\" + type + ".xml";

        if (File.Exists(plugin_loc))
        {
            FileStream fs = new FileStream(plugin_loc, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlTextReader xr = new XmlTextReader(fs);

            //well i am gonna run some checks for plugin structure
            //the number of element and end_element should be same in the plugin file
            int element = 0;
            int end_element = 0;
            while(xr.Read())
            {
                if ((xr.NodeType == XmlNodeType.Element))
                {
                    switch (xr.Name.ToLower())
                    {
                        case "reflexive":
                            element++;
                            break;

                    }

                }
                if (xr.NodeType == XmlNodeType.EndElement )
                {
                    switch (xr.Name.ToLower())
                    {
                        case "reflexive":
                            end_element++;
                            break;
                    }
                }
            }
            //has to dispose of old streams due to unaccounted problems
            xr.Dispose();
            fs.Dispose();

            if (element==end_element)
            {
                //recreating newer streams
                fs = new FileStream(plugin_loc, FileMode.Open, FileAccess.Read, FileShare.Read);
                xr = new XmlTextReader(fs);

                return Get_nodes(type,0x0,0x0,xr, false);//well we do have a base size for the tables inside the meta,but i simply dont care
            }
            else throw new Exception("Plugin structure invalid " + type + ".xml");

        }
        else MessageBox.Show("Error", "couldnt find the plugin of type " + type);

        return null;
    }
    /// <summary>
    /// Function to read a concerned field from the xml file and return its sub fields
    /// </summary>
    /// <param name="name">the name of the field</param>
    /// <param name="off">the offset of the field</param>
    /// <param name="entry_size">the entry size of the field</param>
    /// <param name="xr">the xmlTextReader object</param>
    /// <param name="child">a flag to denote whether the field being read is a child field or not</param>
    /// <returns></returns>
    public static plugins_field Get_nodes(string name,int off,int entry_size,XmlTextReader xr,bool child)
    {
        plugins_field ret = new plugins_field(name, off, entry_size);

        bool work_done = false;//at boolean to show whether reading in the concerned node is done

        while (xr.Read() && !work_done)
        {
            if ((xr.NodeType == XmlNodeType.Element))
            {
                switch (xr.Name.ToLower())
                {
                    case "tagref":
                        if (xr.AttributeCount == 3)
                        {
                            ret.Add_tag_ref(Int32.Parse(xr.GetAttribute("offset").Substring(2), NumberStyles.HexNumber), xr.GetAttribute("name"));
                        }
                        else if(xr.AttributeCount==4)
                        {
                            ret.Add_WCtag_ref(Int32.Parse(xr.GetAttribute("offset").Substring(2), NumberStyles.HexNumber), xr.GetAttribute("name"));
                        }
                        break;
                    case "dataref":
                        ret.Add_data_ref(Int32.Parse(xr.GetAttribute("offset").Substring(2), NumberStyles.HexNumber), xr.GetAttribute("name"));
                        break;
                    case "stringid":
                        ret.Add_stringid_ref(Int32.Parse(xr.GetAttribute("offset").Substring(2), NumberStyles.HexNumber), xr.GetAttribute("name"));
                        break;
                    case "reflexive":
                        string Tname = xr.GetAttribute("name");
                        int Toff = Int32.Parse(xr.GetAttribute("offset").Substring(2), NumberStyles.HexNumber);
                        int Tentry_size = Int32.Parse(xr.GetAttribute("entrySize").Substring(2), NumberStyles.HexNumber); ;

                        ret.Add_field(Get_nodes(Tname, Toff, Tentry_size, xr, true));
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

        return ret;
    }
    /// <summary>
    /// a functon to write value int the meta at the concerned location in little endian format
    /// </summary>
    /// <param name="value"></param>
    /// <param name="offset"></param>
    /// <param name="stream"></param>
    public static void WriteINT_LE(int value, int offset, byte[] meta)
    {
        int S = value;
        for (int i = 0; i < 4; i++)
        {
            int r = S % 0x100;
            meta[offset + i] = (byte)r;
            S = S>>8;
        }
    }
    /// <summary>
    /// a function to write 4 bytes char in little endian format at the specified postition of the array
    /// </summary>
    /// <param name="value"></param>
    /// <param name="offset"></param>
    /// <param name="meta"></param>
    public static void WriteTAG_TYPE_LE(string type, int offset, byte[] meta)
    {
        for (int i = 0; i < type.Length; i++)
        {
            meta[offset + 3 - i] = (byte)type[i];
        }
    }
    /// <summary>
    /// a function to copy array into another array
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="src"></param>
    /// <param name="dest_start">the starting postion from where the data is to copied to</param>
    /// <param name="length">the length of the source array</param>
    public static void ArrayCpy(byte[] dest,byte[] src,int dest_start,int length)
    {
        for(int i=0;i<length;i++)
        {
            dest[dest_start + i] = src[i];
        }
    }
    /// <summary>
    /// a function to copy array into another array
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="src"></param>
    /// <param name="dest_start">the starting postion from where the data is to copied to</param>
    /// <param name="length">the length of the source array</param>
    public static void ArrayCpy(byte[] dest, byte[] src, int dest_start, int src_start, int length)
    {
        for (int i = 0; i < length; i++)
        {
            dest[dest_start + i] = src[src_start + i];
        }
    }
    /// <summary>
    /// a function which checks whether the currently opened map is a shared map or not
    /// </summary>
    /// <returns></returns>
    public static bool Check_shared(StreamReader map_stream)
    {
        string map_name = ReadSTRING(0x1A4, map_stream);
        return map_name.Contains("shared");
    }
    /// <summary>
    /// Returns map name from scenario name
    /// </summary>
    /// <param name="scenario">scenario name</param>
    /// <returns>map name</returns>
    public static string Get_map_from_scenario(string scenario)
    {
        string map_name;
        map_name=scenario.Substring(scenario.LastIndexOf('\\') + 1)+".map";
        return map_name;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="table_index"></param>
    /// <param name="STRING"></param>
    /// <returns></returns>
    public static int Generate_SID(int table_index,int set,string STRING)
    {
        int l = (STRING.Length & 0xFF) << 24;
        int s = (set & 0xFF) << 16;
        int t = table_index & 0xFFFF;

        return (l | s | table_index);
    }
    /// <summary>
    /// Function to create struct from a byte array
    /// some marshal stuff that i found online(C++ is much better)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static T BytesToStructure<T>(byte[] bytes)
    {
        int size = Marshal.SizeOf(typeof(T));
        if (bytes.Length < size)
            throw new Exception("Invalid parameter");

        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.Copy(bytes, 0, ptr, size);
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
    /// <summary>
    /// another function
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static byte[] StructureToByteArray(object obj)
    {
        int len = Marshal.SizeOf(obj);

        byte[] arr = new byte[len];

        IntPtr ptr = Marshal.AllocHGlobal(len);

        Marshal.StructureToPtr(obj, ptr, true);

        Marshal.Copy(ptr, arr, 0, len);

        Marshal.FreeHGlobal(ptr);

        return arr;
    }
}