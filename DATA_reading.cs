using System;
using System.IO;

class DATA_READ
{

    /// <summary>
    /// Returns the little endian integer at the specified position
    /// </summary>
    /// <param name="position">the Offset from the file start</param>
    /// <returns></returns>
    static int ReadINT_LE(int position, StreamReader map_stream)
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
    /// Returns the string starting at the specified position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    static string ReadSTRING(int position, StreamReader map_stream)
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
    /// Return the little endian char[4] at the specified position
    /// </summary>
    /// <param name="position">the Ofset from the file start</param>
    /// <returns></returns>
    static string ReadTAG_TYPE(int position, StreamReader map_stream)
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
    /// Return the type of the tag from its name
    /// </summary>
    /// <param name="name">eg: elite.char</param>
    /// <returns></returns>
    static string ReadTAG_TYPE_form_name(string name)
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
    static string ReadDirectory_from_file_location(string file_loc)
    {
        return file_loc.Substring(0, file_loc.LastIndexOf("\\"));
    }
    /// <summary>
    /// Return the file from the file location
    /// </summary>
    /// <param name="file_loc"></param>
    /// <returns></returns>
    static string Read_File_from_file_location(string file_loc)
    {
        return file_loc.Substring(file_loc.LastIndexOf("\\") + 1);
    }

    /// <summary>
    /// Gets the text from the tree of the specified tag_ref_addr as the key
    /// </summary>
    /// <param name="tnc"></param>
    /// <param name="tag_ref_addr"></param>
    /// <returns></returns>
    static string Get_PathfromTree(TreeNodeCollection tnc, int tag_ref_addr)
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



}