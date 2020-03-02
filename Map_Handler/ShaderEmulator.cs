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
using System.Xml;
using System.Globalization;
using BlamLib;
using DATA_STRUCTURES;

namespace Map_Handler
{
    public partial class ShaderEmulator : Form
    {
        void mitb_block(BinaryWriter bw, string SectiomItem,byte reference_type, float alpha_value, float red_value, float green_value, float blue_value, int transform_count)
        {
            bw.Write(Convert.ToInt16(0));
            byte[] flip = new byte[2];
            flip = BitConverter.GetBytes(Convert.ToInt16(SectiomItem.Length));
            Array.Reverse(flip);
            bw.Write(BitConverter.ToInt16(flip, 0));    //Length of parameter name
            bw.Write(Convert.ToByte(reference_type));   //Value type? 0 = tag reference, 1 = float value, 2 = color value
            bw.Write(Convert.ToByte(0));
            bw.Write(Convert.ToInt16(0));
            bw.Write(Encoding.UTF8.GetBytes("mtib"));
            bw.Write(Convert.ToInt32(0));
            bw.Write((Convert.ToInt32(SectiomItem.Length))); //Length of bitmap tag path
            bw.Write(Convert.ToInt32(-1));
            bw.Write(Convert.ToSingle(alpha_value));    //A
            bw.Write(Convert.ToSingle(red_value));      //R
            bw.Write(Convert.ToSingle(green_value));    //G
            bw.Write(Convert.ToSingle(blue_value));     //B
            bw.Write(Convert.ToInt32(transform_count)); //Animation Properties Count
            bw.Write(Convert.ToInt32(-1));
            bw.Write(Convert.ToInt32(10088844));
        }
        void anim_block_part1(BinaryWriter bw, int anim_block_count)
        {
            bw.Write(Encoding.UTF8.GetBytes("dfbt"));
            bw.Write(Convert.ToInt32(0));
            bw.Write(Convert.ToInt32(anim_block_count)); //Anim block count?
            bw.Write(Convert.ToInt32(28));
        }

        void anim_block_part2(BinaryWriter bw, int anim_type)
        {
            bw.Write(Convert.ToInt32(anim_type)); //Anim type
            bw.Write(Convert.ToInt32(0));         //Input name related?
            bw.Write(Convert.ToInt32(0));         //Range name related?
            bw.Write(Convert.ToInt32(0));         //Time period
            bw.Write(Convert.ToInt32(28));
            bw.Write(Convert.ToInt32(273294104));
            bw.Write(Convert.ToInt32(9449588));   //Strings for Input/Range written after between this block and the next
        }

        void anim_block_part3_bitmap(BinaryWriter bw, int anim_type, byte function_type, byte range_flag, float anim_value_x, float anim_value_y)
        {
            bw.Write(Encoding.UTF8.GetBytes("PPAM"));
            bw.Write(Convert.ToInt32(1));
            bw.Write(Convert.ToInt32(1));
            bw.Write(Convert.ToInt32(12));
            bw.Write(Encoding.UTF8.GetBytes("dfbt"));
            bw.Write(Convert.ToInt32(0));
            bw.Write(Convert.ToInt32(anim_type));     //anim type
            bw.Write(Convert.ToInt32(1));
            bw.Write(Convert.ToByte(function_type));  //Function Type
            bw.Write(Convert.ToByte(range_flag));     //Range Flag 0 = unused / 20 = no range / 21 = range
            bw.Write(Convert.ToInt16(0));
            bw.Write(Convert.ToSingle(anim_value_x)); // Function Value X
            bw.Write(Convert.ToSingle(anim_value_y)); // Function Value Y
            bw.Write(Convert.ToInt32(0));
            bw.Write(Convert.ToInt32(0));
            bw.Write(Convert.ToInt32(1065353216));
            bw.Write(Convert.ToInt32(1065353216));
        }

        void anim_block_part3_color(BinaryWriter bw, int anim_type, byte function_type, byte range_flag, float anim_color_a, float anim_color_r, float anim_color_g, float anim_color_b, float anim_color_a_range, float anim_color_r_range, float anim_color_g_range, float anim_color_b_range)
        {
            bw.Write(Encoding.UTF8.GetBytes("PPAM"));
            bw.Write(Convert.ToInt32(1));
            bw.Write(Convert.ToInt32(1));
            bw.Write(Convert.ToInt32(12));
            bw.Write(Encoding.UTF8.GetBytes("dfbt"));
            bw.Write(Convert.ToInt32(0));
            bw.Write(Convert.ToInt32(anim_type));        //anim type
            bw.Write(Convert.ToInt32(1));
            bw.Write(Convert.ToByte(function_type));     //Function Type
            bw.Write(Convert.ToByte(range_flag));        //Range Flag 20 = no range / 21 = range
            bw.Write(Convert.ToInt16(0));
            bw.Write(Convert.ToByte(anim_color_b));      //B
            bw.Write(Convert.ToByte(anim_color_g));      //G
            bw.Write(Convert.ToByte(anim_color_r));      //R
            bw.Write(Convert.ToByte(anim_color_a));      //A
            bw.Write(Convert.ToInt32(0));
            bw.Write(Convert.ToInt32(0));
            bw.Write(Convert.ToByte(anim_color_b_range)); //B Range
            bw.Write(Convert.ToByte(anim_color_g_range)); //G Range
            bw.Write(Convert.ToByte(anim_color_r_range)); //R Range
            bw.Write(Convert.ToByte(anim_color_a_range)); //A Range
            bw.Write(Convert.ToInt32(1065353216));
            bw.Write(Convert.ToInt32(1065353216));
        }
        public void EmulateShaderDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();

            MessageBox.Show("Please select the Shader Log.");

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = Path.Combine(Environment.CurrentDirectory, "plugins", "shaderstemplates", "blank.shader");
                string shaderpath = Path.GetDirectoryName(fbd.FileName);
                string tagfolder = Path.Combine(shaderpath, "TAGS");
                string workingdirectory = Environment.CurrentDirectory;
                Directory.CreateDirectory(tagfolder);

                foreach (string line in File.ReadLines(fbd.FileName))
                {
                    var sectionShaderParameter = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### SHADER PARAMETERS ###")     // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### SHADER PARAMETERS END ###") // Take lines until the end
                        .ToList();                                            // Convert the result to List<string>

                    string ShaderTemplate = Path.GetFileName(sectionShaderParameter[0]) + ".shader_template.txt";

                    string PixelTemplate = Path.GetFileName(sectionShaderParameter[0]) + ".shader_template_pixel.txt";

                    var sectionBitmap = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### BITMAPS ###")               // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### BITMAPS END ###")           // Take lines until the end
                        .ToList();                                            // Convert the result to List<string>

                    var sectionBitmapIndex = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### BITMAPS INDEX ###")         // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### BITMAPS INDEX END ###")     // Take lines until the end
                        .ToList();                                            // Convert the result to List<string>

                    var sectionBitmapLabels = File.ReadAllLines(Path.Combine(workingdirectory, "plugins", "shaderstemplates", ShaderTemplate))
                        .ToList();

                    var sectionPixelLabels = File.ReadAllLines(Path.Combine(workingdirectory, "plugins", "pixeltemplates", PixelTemplate))
                        .ToList();

                    var sectionLightmap = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### LIGHTMAP ###")              // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### LIGHTMAP END ###")          // Take lines until the end
                        .ToList();                                            // Convert the result to List<string>

                    var sectionPixelConstant = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### PIXEL CONSTANTS ###")       // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### PIXEL CONSTANTS END ###")   // Take lines until the end
                        .ToList();                                            // Convert the result to List<string>

                    var sectionVertexConstant = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### VERTEX CONSTANTS ###")      // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### VERTEX CONSTANTS END ###")  // Take lines until the end
                        .ToList();                                            // Convert the result to List<string>

                    List<string> SectiomItems = new List<string>();
                    SectiomItems.AddRange(sectionBitmapLabels);
                    SectiomItems.AddRange(sectionPixelLabels);

                    List<string> SectiomItemLength = new List<string>();
                    SectiomItemLength.AddRange(sectionBitmap);
                    for (int i = 0; i < sectionPixelLabels.Count; i++)
                    {
                        SectiomItemLength.Add("");
                    }

                    List<string> SupportedShaders = new List<string>();

                    SupportedShaders.Add("active_camo_opaque.shader_template.txt");
                    SupportedShaders.Add("active_camo_transparent.shader_template.txt");
                    SupportedShaders.Add("add_illum_detail.shader_template.txt");
                    SupportedShaders.Add("ammo_meter.shader_template.txt");
                    SupportedShaders.Add("bloom.shader_template.txt");
                    SupportedShaders.Add("bumped_environment_additive.shader_template.txt");
                    SupportedShaders.Add("bumped_environment_blended.shader_template.txt");
                    SupportedShaders.Add("bumped_environment_darkened.shader_template.txt");
                    SupportedShaders.Add("bumped_environment_mask_colored.shader_template.txt");
                    SupportedShaders.Add("bumped_environment_masked.shader_template.txt");
                    SupportedShaders.Add("cortana.shader_template.txt");
                    //SupportedShaders.Add("cortana_holographic_active_camo.shader_template.txt");
                    SupportedShaders.Add("illum.shader_template.txt");
                    SupportedShaders.Add("illum_3_channel_opaque.shader_template.txt");
                    SupportedShaders.Add("illum_bloom.shader_template.txt");
                    SupportedShaders.Add("illum_bloom_opaque.shader_template.txt");
                    SupportedShaders.Add("illum_clamped.shader_template.txt");
                    SupportedShaders.Add("illum_detail.shader_template.txt");
                    SupportedShaders.Add("illum_opaque.shader_template.txt");
                    SupportedShaders.Add("one_add_illum.shader_template.txt");
                    SupportedShaders.Add("one_add_illum_detail.shader_template.txt");
                    SupportedShaders.Add("one_alpha_env_illum.shader_template.txt");
                    SupportedShaders.Add("one_alpha_env_illum_specular_mask.shader_template.txt");
                    SupportedShaders.Add("tex_bump.shader_template.txt");
                    SupportedShaders.Add("tex_bump_dprs_env_illum.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env.shader_template.txt");
                    //SupportedShaders.Add("tex_bump_env_dbl_spec.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_3_channel.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_3_channel_combined.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_3_channel_occlusion_combined.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_combined.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_detail_honor_guard.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_four_change_color_no_lod.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_two_change_color.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_illum_two_change_color_combined.shader_template.txt");
                    SupportedShaders.Add("tex_bump_env_two_change_color_multiply_map_self_illum.shader_template.txt");
                    SupportedShaders.Add("tex_bump_illum.shader_template.txt");
                    SupportedShaders.Add("tex_bump_illum_3_channel.shader_template.txt");
                    SupportedShaders.Add("tex_bump_illum_alpha_test.shader_template.txt");
                    SupportedShaders.Add("tex_bump_illum_bloom.shader_template.txt");
                    SupportedShaders.Add("tex_bump_meter_illum.shader_template.txt");
                    SupportedShaders.Add("tex_bump_plasma_one_channel_illum.shader_template.txt");
                    SupportedShaders.Add("two_add_env_illum.shader_template.txt");
                    SupportedShaders.Add("two_add_env_illum_3_channel.shader_template.txt");
                    SupportedShaders.Add("two_add_env_illum_active_camo.shader_template.txt");
                    SupportedShaders.Add("two_alpha_env_illum.shader_template.txt");

                    string[] BitmapLabels = File.ReadAllLines(Path.Combine(workingdirectory, "plugins", "shaderstemplates", ShaderTemplate));

                    string[] PixelLabels = File.ReadAllLines(Path.Combine(workingdirectory, "plugins", "pixeltemplates", PixelTemplate));

                    int ShaderParameterCount = BitmapLabels.GetLength(0);

                    int PixelParameterCount = PixelLabels.GetLength(0);

                    int ItemParameterCount = ShaderParameterCount + PixelParameterCount;

                    string tagname = Path.Combine(tagfolder, line.Substring(0, (line.Length - 4)) + ".shader");
                    Directory.CreateDirectory(Directory.GetParent(tagname).ToString());

                    using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    using (var ds = new FileStream(tagname, FileMode.Create, FileAccess.ReadWrite))
                    using (var ms = new MemoryStream())
                    using (var bw = new BinaryWriter(ms))
                    using (var br = new BinaryReader(ms))

                    {
                        fs.CopyTo(ms);
                        ms.Position = 0;

                        bw.BaseStream.Seek(88, SeekOrigin.Begin);
                        bw.Write(Convert.ToInt32(sectionShaderParameter[0].Length));           //Write shader path length
                        bw.BaseStream.Seek(99, SeekOrigin.Begin);
                        bw.Write(Convert.ToInt32(sectionShaderParameter[1].Length));           //Write material_name length
                        bw.BaseStream.Seek(114, SeekOrigin.Begin);
                        bw.Write(Convert.ToByte(sectionShaderParameter[2]));                   //Write Flags 1 byte = water 2 byte = sort first 4 byte = no active camo
                        bw.BaseStream.Seek(116, SeekOrigin.Begin);
                        if (!SupportedShaders.Contains(ShaderTemplate))
                        {
                            bw.Write(Convert.ToInt32(ShaderParameterCount)); //Write parameter count
                        }
                        else
                        {
                            bw.Write(Convert.ToInt32(ShaderParameterCount + PixelParameterCount)); //Write parameter count
                        }
                        bw.BaseStream.Seek(172, SeekOrigin.Begin);
                        bw.Write(Convert.ToByte(sectionShaderParameter[9]));                   //Write shader_lod_bias
                        bw.BaseStream.Seek(174, SeekOrigin.Begin);
                        bw.Write(Convert.ToByte(sectionShaderParameter[5]));                   //Write dynamic_light_specular_type value
                        bw.BaseStream.Seek(176, SeekOrigin.Begin);
                        bw.Write(Convert.ToByte(sectionShaderParameter[6]));                   //Write lightmap_type value
                        bw.BaseStream.Seek(180, SeekOrigin.Begin);
                        bw.Write(Convert.ToSingle(sectionShaderParameter[7]));                 //Write lightmap_specular_brightness value
                        bw.Write(Convert.ToSingle(sectionShaderParameter[8]));                 //Write lightmap_ambient_bias value
                        bw.BaseStream.Seek(200, SeekOrigin.Begin);
                        bw.Write(Convert.ToSingle(sectionShaderParameter[3]));                 //Write depth_bias_offset value
                        bw.Write(Convert.ToSingle(sectionShaderParameter[4]));                 //Write depth_bias_slope_scale value

                        bw.BaseStream.Seek(208, SeekOrigin.Begin);

                        bw.Write(Encoding.UTF8.GetBytes(sectionShaderParameter[0]));           //Write shader path
                        bw.Write(Convert.ToByte(0));
                        bw.Write(Encoding.UTF8.GetBytes(sectionShaderParameter[1]));           //Write material_name
                        bw.Write(Encoding.UTF8.GetBytes("dfbt"));
                        bw.Write(Convert.ToInt32(0));
                        if (!SupportedShaders.Contains(ShaderTemplate))
                        {
                            bw.Write(Convert.ToInt32(ShaderParameterCount)); //Write parameter count
                        }
                        else
                        {
                            bw.Write(Convert.ToInt32(ShaderParameterCount + PixelParameterCount)); //Write parameter count
                        }
                        bw.Write(Convert.ToInt32(52));

                        if (!SupportedShaders.Contains(ShaderTemplate))
                        {
                            for (int i = 0; i < ShaderParameterCount; i++)
                            {
                                if (sectionBitmap[i] == " ")
                                {
                                    sectionBitmap[i] = "";
                                }

                                bw.Write(Convert.ToInt16(0));

                                byte[] flip = new byte[2];
                                flip = BitConverter.GetBytes(Convert.ToInt16(BitmapLabels[i].Length));
                                Array.Reverse(flip);

                                bw.Write(BitConverter.ToInt16(flip, 0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write((Convert.ToInt32(sectionBitmap[i].Length)));
                                bw.Write(Convert.ToInt32(-1));

                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                // 5 time 5 time 5 time 5 time 5 time WCW Champion

                                bw.Write(Convert.ToInt32(-1));

                                bw.Write(Convert.ToInt32(10088844));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ItemParameterCount; i++)
                            {
                                float b = 0.00392156862f;

                                if (SectiomItems[i] == " ")
                                {
                                    SectiomItems[i] = "";
                                }

                                if (i >= ShaderParameterCount || SectiomItemLength[i] == " ")
                                {
                                    SectiomItemLength[i] = "";
                                }

                               

                                #region Active Camo Opaque
                                if (ShaderTemplate == "active_camo_opaque.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                   //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "base_map")                   //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "refraction_bump_amount")     //vertex constant index 2 x value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[9]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "refraction_geometry_amount") //vertex constant index 3 y value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[14]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Active Camo Transparent
                                if (ShaderTemplate == "active_camo_transparent.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                   //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "base_map")                   //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "refraction_bump_amount")     //vertex constant index 2 x value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[9]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "refraction_geometry_amount") //vertex constant index 3 y value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[14]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Add Illum Detail
                                if (ShaderTemplate == "add_illum_detail.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 5);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 5);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_map")   //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")        //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_color") //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Ammo Meter
                                if (ShaderTemplate == "ammo_meter.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "meter_gradient_min") //pixel constants index 1 value - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "meter_gradient_max") //pixel constants index 2 value - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "meter_empty_color")  //pixel constants index 3 value - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "meter_amount")       //pixel constants index 0 value - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                }
                                #endregion

                                #region Bloom
                                if (ShaderTemplate == "bloom.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "lightmap_emissive_map")   //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bloom_map")               //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_color") //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bloom")                   //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                }
                                #endregion

                                #region Bumped Environment Additive
                                if (ShaderTemplate == "bumped_environment_additive.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")         //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")          //vertex constant index 0 x,y,z rgb floats - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color") //vertex constant index 1 x,y,z rgb floats - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")          //vertex constant index 2 w value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness") //vertex constant index 3 w value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")          //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")          //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Bumped Environment Blended
                                if (ShaderTemplate == "bumped_environment_blended.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")            //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bump_map")                   //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                   //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "lightmap_translucent_map")   //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")             //vertex constant index 0 x,y,z rgb floats - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")    //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")             //vertex constant index 2 w value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")    //vertex constant index 3 w value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")             //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")             //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_translucent_color") //runtime properites tag block lightmap transparent colors value - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[6]), green_value: Convert.ToSingle(sectionLightmap[7]), blue_value: Convert.ToSingle(sectionLightmap[8]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_translucent_alpha") //runtime properties tag block lightmap transparent alpha - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[5]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Bumped Environment Darkened
                                if (ShaderTemplate == "bumped_environment_darkened.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")         //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")          //vertex constant index 0 x,y,z rgb floats - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color") //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")          //vertex constant index 2 w value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness") //vertex constant index 3 w value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")          //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")          //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Bumped Environment Mask Colored
                                if (ShaderTemplate == "bumped_environment_mask_colored.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")        //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bump_map")               //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")               //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")          //vertex constant index 0 x,y,z rgb floats - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color") //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")          //vertex constant index 2 w value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness") //vertex constant index 3 w value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")          //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")          //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "mask_color0")             //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "mask_color1")             //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "mask_color2")             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                }
                                #endregion

                                #region Bumped Environment Masked
                                if (ShaderTemplate == "bumped_environment_masked.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")         //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")          //vertex constant index 0 x,y,z rgb floats - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color") //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")          //vertex constant index 2 w value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness") //vertex constant index 3 w value - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")          //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")          //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Cortana
                                if (ShaderTemplate == "cortana.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "flat_environment_map")     //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_map")             //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_map")          //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "noise_map_a")              //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 5);
                                    }
                                    if (SectiomItems[i] == "noise_map_b")              //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 5);
                                    }
                                    if (SectiomItems[i] == "alpha_map")                //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "bloom_map")                //Bitmap Index 6
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 7
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 8
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "flat_environment_color")   //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_color")        //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_opacity")      //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "tint_color")               //vertex constant index 0 rgb floats - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_tint_color")      //vertex constant index 1 rgb floats - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "brightness")               //vertex constant index 2 alpha float - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_brightness")      //vertex constant index 3 alpha float - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "color_wide")               //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "color_medium")             //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "color_sharp")              //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "perpendicular_brightness") //vertex constant index 21 alpha float - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[84]), red_value: Convert.ToSingle(sectionVertexConstant[85]), green_value: Convert.ToSingle(sectionVertexConstant[86]), blue_value: Convert.ToSingle(sectionVertexConstant[87]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "fade_bias")                //pixel constant index 7 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[28]) * b, red_value: Convert.ToSingle(sectionPixelConstant[29]) * b, green_value: Convert.ToSingle(sectionPixelConstant[30]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[31]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bloom")                    //pixel constant index 4 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[16]) * b, red_value: Convert.ToSingle(sectionPixelConstant[17]) * b, green_value: Convert.ToSingle(sectionPixelConstant[18]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[19]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //vertex constant index 23 rgb float - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[92]), red_value: Convert.ToSingle(sectionVertexConstant[93]), green_value: Convert.ToSingle(sectionVertexConstant[94]), blue_value: Convert.ToSingle(sectionVertexConstant[95]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //vertex constant index 24 rgb float - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[96]), red_value: Convert.ToSingle(sectionVertexConstant[97]), green_value: Convert.ToSingle(sectionVertexConstant[98]), blue_value: Convert.ToSingle(sectionVertexConstant[99]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //vertex constant index 25 rgb float - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[100]), red_value: Convert.ToSingle(sectionVertexConstant[101]), green_value: Convert.ToSingle(sectionVertexConstant[102]), blue_value: Convert.ToSingle(sectionVertexConstant[103]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //vertex constant index 26 rgb float - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[104]), red_value: Convert.ToSingle(sectionVertexConstant[105]), green_value: Convert.ToSingle(sectionVertexConstant[106]), blue_value: Convert.ToSingle(sectionVertexConstant[107]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //pixel constant index 9 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[36]) * b, red_value: Convert.ToSingle(sectionPixelConstant[37]) * b, green_value: Convert.ToSingle(sectionPixelConstant[38]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[39]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //pixel constant index 10 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[40]) * b, red_value: Convert.ToSingle(sectionPixelConstant[41]) * b, green_value: Convert.ToSingle(sectionPixelConstant[42]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[43]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Cortana Holographic Active Camo
                                /*if (ShaderTemplate == "cortana_holographic_active_camo.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "refraction_bump_amount")     //unused? moved from the shad file to some other file on package? Only used on lighting and thrown away on package? - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "refraction_geometry_amount") //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "refraction_x_offset")        //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }*/
                                #endregion

                                #region Illum
                                if (ShaderTemplate == "illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_map")   //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")        //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_color") //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Illum 3 Channel Opaque
                                if (ShaderTemplate == "illum_3_channel_opaque.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")       //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")      //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness") //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")      //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness") //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")      //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")       //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")       //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region illum Bloom
                                if (ShaderTemplate == "illum_bloom.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "add_map")                 //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "mult_map")                //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_map")   //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bloom_map")               //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")        //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_color") //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bloom")                   //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                }
                                #endregion

                                #region illum Bloom Opaque
                                if (ShaderTemplate == "illum_bloom_opaque.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")   //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "add_map")          //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "mult_map")         //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "bloom_map")        //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "self_illum_color") //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")   //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")   //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bloom")            //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                }
                                #endregion

                                #region illum Clamped
                                if (ShaderTemplate == "illum_clamped.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_map")   //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")        //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_color") //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region illum Detail
                                if (ShaderTemplate == "illum_detail.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 5);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 5);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_map")   //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")        //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_color") //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_emissive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region illum Opaque
                                if (ShaderTemplate == "illum_opaque.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")   //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "self_illum_color") //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")   //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")   //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region One Add Illum
                                if (ShaderTemplate == "one_add_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")        //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "tint_color")              //vertex constant index 2 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_tint_color")     //vertex constant index 3 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "brightness")              //vertex constant index 4 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[16]), red_value: Convert.ToSingle(sectionVertexConstant[17]), green_value: Convert.ToSingle(sectionVertexConstant[18]), blue_value: Convert.ToSingle(sectionVertexConstant[19]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_brightness")     //vertex constant index 5 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[20]), red_value: Convert.ToSingle(sectionVertexConstant[21]), green_value: Convert.ToSingle(sectionVertexConstant[22]), blue_value: Convert.ToSingle(sectionVertexConstant[23]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_emmisive_color") //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_emmisive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region One Add Illum Detail
                                if (ShaderTemplate == "one_add_illum_detail.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")      //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "detail_map")          //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")    //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "tint_color")          //vertex constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "glancing_tint_color") //vertex constant index 5 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "brightness")          //vertex constant index 6 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "glancing_brightness") //vertex constant index 7 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                }
                                #endregion

                                #region One Alpha Env Illum
                                if (ShaderTemplate == "one_alpha_env_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")     //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_map")        //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_map")     //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")      //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "environment_color")   //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_color")   //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_opacity") //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")    //pixel constant index 3 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "tint_color")          //vertex constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_tint_color") //vertex constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "brightness")          //vertex constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_brightness") //vertex constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region One Alpha Env Illum Specular Mask
                                if (ShaderTemplate == "one_alpha_env_illum_specular_mask.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")     //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_map")     //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")      //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "environment_color")   //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_color")   //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_opacity") //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")    //pixel constant index 3 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "tint_color")          //vertex constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_tint_color") //vertex constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "brightness")          //vertex constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_brightness") //vertex constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump
                                if (ShaderTemplate == "tex_bump.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "lightmap_alphatest_map")  //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 5);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "ambient_factor")          //unused? moved from the shad file to some other file on package? Only used on lighting and thrown away on package? - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")          //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color") //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Dprs Env Illum
                                if (ShaderTemplate == "tex_bump_dprs_env_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                 //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")               //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")           //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //vertex constant index 8 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[32]), red_value: Convert.ToSingle(sectionVertexConstant[33]), green_value: Convert.ToSingle(sectionVertexConstant[34]), blue_value: Convert.ToSingle(sectionVertexConstant[35]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //vertex constant index 9 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[36]), red_value: Convert.ToSingle(sectionVertexConstant[37]), green_value: Convert.ToSingle(sectionVertexConstant[38]), blue_value: Convert.ToSingle(sectionVertexConstant[39]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //vertex constant index 10 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[40]), red_value: Convert.ToSingle(sectionVertexConstant[41]), green_value: Convert.ToSingle(sectionVertexConstant[42]), blue_value: Convert.ToSingle(sectionVertexConstant[43]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //vertex constant index 11 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[44]), red_value: Convert.ToSingle(sectionVertexConstant[45]), green_value: Convert.ToSingle(sectionVertexConstant[46]), blue_value: Convert.ToSingle(sectionVertexConstant[47]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //pixel constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[16]) * b, red_value: Convert.ToSingle(sectionPixelConstant[17]) * b, green_value: Convert.ToSingle(sectionPixelConstant[18]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[19]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")         //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")           //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")           //pixel constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")           //vertex constant index 12 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[48]), red_value: Convert.ToSingle(sectionVertexConstant[49]), green_value: Convert.ToSingle(sectionVertexConstant[50]), blue_value: Convert.ToSingle(sectionVertexConstant[51]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")  //vertex constant index 13 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[52]), red_value: Convert.ToSingle(sectionVertexConstant[53]), green_value: Convert.ToSingle(sectionVertexConstant[54]), blue_value: Convert.ToSingle(sectionVertexConstant[55]), transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env
                                if (ShaderTemplate == "tex_bump_env.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "active_camo_bump_map")    //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "environment_map")         //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")          //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color") //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")          //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness") //pixel constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")          //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[16]) * b, red_value: Convert.ToSingle(sectionPixelConstant[17]) * b, green_value: Convert.ToSingle(sectionPixelConstant[18]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[19]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color") //pixel constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[20]) * b, red_value: Convert.ToSingle(sectionPixelConstant[21]) * b, green_value: Convert.ToSingle(sectionPixelConstant[22]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[23]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Dbl Spec
                                /*if (ShaderTemplate == "tex_bump_env_dbl_spec.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")                                         //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")                                //pixel constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }*/
                                #endregion

                                #region Tex Bump Env Illum
                                if (ShaderTemplate == "tex_bump_env_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "active_camo_bump_map")     //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "base_map")                 //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")               //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")           //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //pixel constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //Not written to packaged shad tag? - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //Not written to packaged shad tag? - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")         //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")           //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")           //pixel constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")           //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[16]) * b, red_value: Convert.ToSingle(sectionPixelConstant[17]) * b, green_value: Convert.ToSingle(sectionPixelConstant[18]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[19]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")  //pixel constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[20]) * b, red_value: Convert.ToSingle(sectionPixelConstant[21]) * b, green_value: Convert.ToSingle(sectionPixelConstant[22]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[23]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum 3 Channel
                                if (ShaderTemplate == "tex_bump_env_illum_3_channel.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                 //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")               //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")           //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "lightmap_emmisive_map")    //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //pixel constant index 10 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[40]) * b, red_value: Convert.ToSingle(sectionPixelConstant[41]) * b, green_value: Convert.ToSingle(sectionPixelConstant[42]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[43]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //pixel constant index 11 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[44]) * b, red_value: Convert.ToSingle(sectionPixelConstant[45]) * b, green_value: Convert.ToSingle(sectionPixelConstant[46]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[47]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //pixel constant index 12 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[48]) * b, red_value: Convert.ToSingle(sectionPixelConstant[49]) * b, green_value: Convert.ToSingle(sectionPixelConstant[50]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[51]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //pixel constant index 13 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[52]) * b, red_value: Convert.ToSingle(sectionPixelConstant[53]) * b, green_value: Convert.ToSingle(sectionPixelConstant[54]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[55]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //pixel constant index 3 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")          //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness")     //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")          //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness")     //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")          //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness")     //pixel constant index 9 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")           //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")           //pixel constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")           //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")  //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum 3 Channel Combined
                                if (ShaderTemplate == "tex_bump_env_illum_3_channel_combined.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                 //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")               //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")           //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //pixel constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //Not written to packaged shad tag? - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //Not written to packaged shad tag? - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")          //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness")     //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")          //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness")     //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")          //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness")     //pixel constant index 9 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")           //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")           //pixel constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")           //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[16]) * b, red_value: Convert.ToSingle(sectionPixelConstant[17]) * b, green_value: Convert.ToSingle(sectionPixelConstant[18]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[19]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")  //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[20]) * b, red_value: Convert.ToSingle(sectionPixelConstant[21]) * b, green_value: Convert.ToSingle(sectionPixelConstant[22]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[23]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum 3 Channel Occlusion Combined
                                if (ShaderTemplate == "tex_bump_env_illum_3_channel_occlusion_combined.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                 //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "detail_map")               //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")           //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "occlusion_map")            //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //pixel constant index 13 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[52]) * b, red_value: Convert.ToSingle(sectionPixelConstant[53]) * b, green_value: Convert.ToSingle(sectionPixelConstant[54]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[55]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //pixel constant index 14 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[56]) * b, red_value: Convert.ToSingle(sectionPixelConstant[57]) * b, green_value: Convert.ToSingle(sectionPixelConstant[58]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[59]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //pixel constant index 15 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[60]) * b, red_value: Convert.ToSingle(sectionPixelConstant[61]) * b, green_value: Convert.ToSingle(sectionPixelConstant[62]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[63]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //pixel constant index 16 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[64]) * b, red_value: Convert.ToSingle(sectionPixelConstant[65]) * b, green_value: Convert.ToSingle(sectionPixelConstant[66]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[67]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //pixel constant index 3 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")          //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness")     //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")          //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness")     //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")          //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness")     //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "occlusion_a_color")        //pixel constant index 10 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "occlusion_b_color")        //pixel constant index 11 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "occlusion_c_color")        //pixel constant index 12 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")           //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")           //pixel constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")           //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")  //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum Combined
                                if (ShaderTemplate == "tex_bump_env_illum_combined.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "active_camo_bump_map")     //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "base_map")                 //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")               //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")           //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //pixel constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //Not written to packaged shad tag? - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //Not written to packaged shad tag? - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")         //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")           //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")           //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")           //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[16]) * b, red_value: Convert.ToSingle(sectionPixelConstant[17]) * b, green_value: Convert.ToSingle(sectionPixelConstant[18]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[19]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")  //pixel constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[20]) * b, red_value: Convert.ToSingle(sectionPixelConstant[21]) * b, green_value: Convert.ToSingle(sectionPixelConstant[22]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[23]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum Detail Honor Guard
                                if (ShaderTemplate == "tex_bump_env_illum_detail_honor_guard.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                 //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")               //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")           //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_detail")        //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map_value_scale")   //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "burn_scale")               //pixel constant index 0 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")         //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_power")           //runtime properties tag block lightmap half life - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[4]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")           //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //vertex constant index 6 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[24]), red_value: Convert.ToSingle(sectionVertexConstant[25]), green_value: Convert.ToSingle(sectionVertexConstant[26]), blue_value: Convert.ToSingle(sectionVertexConstant[27]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //vertex constant index 7 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[28]), red_value: Convert.ToSingle(sectionVertexConstant[29]), green_value: Convert.ToSingle(sectionVertexConstant[30]), blue_value: Convert.ToSingle(sectionVertexConstant[31]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //vertex constant index 8 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[32]), red_value: Convert.ToSingle(sectionVertexConstant[33]), green_value: Convert.ToSingle(sectionVertexConstant[34]), blue_value: Convert.ToSingle(sectionVertexConstant[35]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //vertex constant index 9 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[36]), red_value: Convert.ToSingle(sectionVertexConstant[37]), green_value: Convert.ToSingle(sectionVertexConstant[38]), blue_value: Convert.ToSingle(sectionVertexConstant[39]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //pixel constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //pixel constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[16]) * b, red_value: Convert.ToSingle(sectionPixelConstant[17]) * b, green_value: Convert.ToSingle(sectionPixelConstant[18]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[19]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")           //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")  //pixel constant index 2 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum Four Change Color No Lod
                                if (ShaderTemplate == "tex_bump_env_illum_four_change_color_no_lod.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "active_camo_bump_map")     //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "base_map")                 //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")               //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 3);
                                    }
                                    if (SectiomItems[i] == "change_color_map")         //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")           //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 6
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "ambient_factor")           //Not written to shad file - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")         //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //vertex constant index 4 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[16]), red_value: Convert.ToSingle(sectionVertexConstant[17]), green_value: Convert.ToSingle(sectionVertexConstant[18]), blue_value: Convert.ToSingle(sectionVertexConstant[19]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //vertex constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[20]), red_value: Convert.ToSingle(sectionVertexConstant[21]), green_value: Convert.ToSingle(sectionVertexConstant[22]), blue_value: Convert.ToSingle(sectionVertexConstant[23]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //vertex constant index 6 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[24]), red_value: Convert.ToSingle(sectionVertexConstant[25]), green_value: Convert.ToSingle(sectionVertexConstant[26]), blue_value: Convert.ToSingle(sectionVertexConstant[27]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //vertex constant index 7 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[28]), red_value: Convert.ToSingle(sectionVertexConstant[29]), green_value: Convert.ToSingle(sectionVertexConstant[30]), blue_value: Convert.ToSingle(sectionVertexConstant[31]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //pixel constant index 3 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")           //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")  //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum Two Change Color
                                if (ShaderTemplate == "tex_bump_env_illum_two_change_color.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "active_camo_bump_map")     //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "base_map")                 //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")               //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 3);
                                    }
                                    if (SectiomItems[i] == "change_color_map")         //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")           //Bitmap Index 6
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //vertex constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[20]) * b, red_value: Convert.ToSingle(sectionPixelConstant[21]) * b, green_value: Convert.ToSingle(sectionPixelConstant[22]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[23]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //vertex constant index 6 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[24]) * b, red_value: Convert.ToSingle(sectionPixelConstant[25]) * b, green_value: Convert.ToSingle(sectionPixelConstant[26]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[27]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //vertex constant index 7 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[28]) * b, red_value: Convert.ToSingle(sectionPixelConstant[29]) * b, green_value: Convert.ToSingle(sectionPixelConstant[30]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[31]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //vertex constant index 8 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[32]) * b, red_value: Convert.ToSingle(sectionPixelConstant[33]) * b, green_value: Convert.ToSingle(sectionPixelConstant[34]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[35]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //pixel constant index 3 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[12]) * b, red_value: Convert.ToSingle(sectionPixelConstant[13]) * b, green_value: Convert.ToSingle(sectionPixelConstant[14]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[15]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")         //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")           //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")           //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")           //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")  //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum Two Change Color Combined
                                if (ShaderTemplate == "tex_bump_env_illum_two_change_color_combined.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                 //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                 //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")               //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "change_color_map")         //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "environment_map")          //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")           //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")           //vertex constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[20]), red_value: Convert.ToSingle(sectionVertexConstant[21]), green_value: Convert.ToSingle(sectionVertexConstant[22]), blue_value: Convert.ToSingle(sectionVertexConstant[23]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color")  //vertex constant index 6 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[24]), red_value: Convert.ToSingle(sectionVertexConstant[25]), green_value: Convert.ToSingle(sectionVertexConstant[26]), blue_value: Convert.ToSingle(sectionVertexConstant[27]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")           //vertex constant index 7 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[28]), red_value: Convert.ToSingle(sectionVertexConstant[29]), green_value: Convert.ToSingle(sectionVertexConstant[30]), blue_value: Convert.ToSingle(sectionVertexConstant[31]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness")  //vertex constant index 8 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[32]), red_value: Convert.ToSingle(sectionVertexConstant[33]), green_value: Convert.ToSingle(sectionVertexConstant[34]), blue_value: Convert.ToSingle(sectionVertexConstant[35]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_factor") //Not written to shad file - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_midrange_tint_color")  //Not written to shad file - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")         //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_color")           //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")           //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")           //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color")  //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Two Change Color Multiply Map Self Illum
                                if (ShaderTemplate == "tex_bump_env_two_change_color_multiply_map_self_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "change_color_map")        //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "multiply_map")            //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 3);
                                    }
                                    if (SectiomItems[i] == "environment_map")         //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 6
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "env_tint_color")          //vertex constant index 5 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[20]) * b, red_value: Convert.ToSingle(sectionPixelConstant[21]) * b, green_value: Convert.ToSingle(sectionPixelConstant[22]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[23]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_tint_color") //vertex constant index 6 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[24]) * b, red_value: Convert.ToSingle(sectionPixelConstant[25]) * b, green_value: Convert.ToSingle(sectionPixelConstant[26]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[27]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_brightness")          //vertex constant index 7 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[28]) * b, red_value: Convert.ToSingle(sectionPixelConstant[29]) * b, green_value: Convert.ToSingle(sectionPixelConstant[30]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[31]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "env_glancing_brightness") //vertex constant index 8 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[32]) * b, red_value: Convert.ToSingle(sectionPixelConstant[33]) * b, green_value: Convert.ToSingle(sectionPixelConstant[34]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[35]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")          //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color") //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")        //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_power")          //Not written to packaged shad file - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")          //Not written to packaged shad file  - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Illum
                                if (ShaderTemplate == "tex_bump_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 5);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")        //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_power")          //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")          //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")          //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color") //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Illum 3 Channel
                                if (ShaderTemplate == "tex_bump_illum_3_channel.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")         //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")         //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")         //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness")    //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness")    //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness")    //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_power")          //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")          //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")          //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color") //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Illum Alpha Test
                                if (ShaderTemplate == "tex_bump_illum_alpha_test.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "alpha_test_map")          //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "lightmap_alphatest_map")  //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 5
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")        //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "specular_color")          //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color") //pixel constant index 2 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Illum Bloom
                                if (ShaderTemplate == "tex_bump_illum_bloom.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")          //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "bloom_map")               //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")        //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_power")          //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")          //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "bloom")                   //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "specular_color")          //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color") //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Meter Illum
                                if (ShaderTemplate == "tex_bump_meter_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "meter_map")               //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "meter_on_color")          //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "meter_off_color")         //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "meter_value")             //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "specular_color")          //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color") //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Tex Bump Plasma One Channel Illum
                                if (ShaderTemplate == "tex_bump_plasma_one_channel_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")                //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "base_map")                //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "detail_map")              //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 2);
                                    }
                                    if (SectiomItems[i] == "multichannel_map")        //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 4);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")         //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")         //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")         //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "time")                    //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "emissive_power")          //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")          //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_color")          //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_glancing_color") //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Two Add Env Illum
                                if (ShaderTemplate == "two_add_env_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")     //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_map")        //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_map")     //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")      //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "environment_color")   //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_color")   //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_opacity") //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")    //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "tint_color")          //vertex constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_tint_color") //vertex constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "brightness")          //vertex constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_brightness") //vertex constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")      //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Two Add Env Illum 3 Channel
                                if (ShaderTemplate == "two_add_env_illum_3_channel.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")      //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_map")         //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_map")      //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")       //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "environment_color")    //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_color")    //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_opacity")  //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")      //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")      //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")      //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "tint_color")           //vertex constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_tint_color")  //vertex constant index 2 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "brightness")           //vertex constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_brightness")  //vertex constant index 4 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[16]), red_value: Convert.ToSingle(sectionVertexConstant[17]), green_value: Convert.ToSingle(sectionVertexConstant[18]), blue_value: Convert.ToSingle(sectionVertexConstant[19]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")       //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")       //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Two Add Env Illum Active Camo
                                if (ShaderTemplate == "two_add_env_illum_active_camo.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")     //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_map")        //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_map")     //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")      //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "environment_color")   //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_color")   //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_opacity") //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")    //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "tint_color")          //vertex constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_tint_color") //vertex constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "brightness")          //vertex constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_brightness") //vertex constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")      //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion

                                #region Two Alpha Env Illum
                                if (ShaderTemplate == "two_alpha_env_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "environment_map")            //Bitmap Index 0
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "specular_map")               //Bitmap Index 1
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_map")            //Bitmap Index 2
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")             //Bitmap Index 3
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_translucent_map")   //Bitmap Index 4
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 0, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "environment_color")          //pixel constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[0]) * b, red_value: Convert.ToSingle(sectionPixelConstant[1]) * b, green_value: Convert.ToSingle(sectionPixelConstant[2]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[3]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_color")          //pixel constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionPixelConstant[4]) * b, red_value: Convert.ToSingle(sectionPixelConstant[5]) * b, green_value: Convert.ToSingle(sectionPixelConstant[6]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[7]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_opacity")        //pixel constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionPixelConstant[8]) * b, red_value: Convert.ToSingle(sectionPixelConstant[9]) * b, green_value: Convert.ToSingle(sectionPixelConstant[10]) * b, blue_value: Convert.ToSingle(sectionPixelConstant[11]) * b, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")           //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: 0, green_value: 0, blue_value: 0, transform_count: 1);
                                    }
                                    if (SectiomItems[i] == "tint_color")                 //vertex constant index 0 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[0]), red_value: Convert.ToSingle(sectionVertexConstant[1]), green_value: Convert.ToSingle(sectionVertexConstant[2]), blue_value: Convert.ToSingle(sectionVertexConstant[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_tint_color")        //vertex constant index 1 - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: Convert.ToSingle(sectionVertexConstant[4]), red_value: Convert.ToSingle(sectionVertexConstant[5]), green_value: Convert.ToSingle(sectionVertexConstant[6]), blue_value: Convert.ToSingle(sectionVertexConstant[7]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "brightness")                 //vertex constant index 2 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[8]), red_value: Convert.ToSingle(sectionVertexConstant[9]), green_value: Convert.ToSingle(sectionVertexConstant[10]), blue_value: Convert.ToSingle(sectionVertexConstant[11]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "glancing_brightness")        //vertex constant index 3 - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionVertexConstant[12]), red_value: Convert.ToSingle(sectionVertexConstant[13]), green_value: Convert.ToSingle(sectionVertexConstant[14]), blue_value: Convert.ToSingle(sectionVertexConstant[15]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_translucent_color") //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[6]), green_value: Convert.ToSingle(sectionLightmap[7]), blue_value: Convert.ToSingle(sectionLightmap[8]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "lightmap_translucent_alpha") //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[5]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_color")             //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 2, alpha_value: 0, red_value: Convert.ToSingle(sectionLightmap[1]), green_value: Convert.ToSingle(sectionLightmap[2]), blue_value: Convert.ToSingle(sectionLightmap[3]), transform_count: 0);
                                    }
                                    if (SectiomItems[i] == "emissive_power")             //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                    {
                                        mitb_block(bw,SectiomItems[i],reference_type: 1, alpha_value: Convert.ToSingle(sectionLightmap[0]), red_value: 0, green_value: 0, blue_value: 0, transform_count: 0);
                                    }
                                }
                                #endregion
                            }
                        }
                        if (!SupportedShaders.Contains(ShaderTemplate))
                        {
                            for (int i = 0; i < ShaderParameterCount; i++)
                            {
                                bw.Write(Encoding.UTF8.GetBytes(BitmapLabels[i]));
                                bw.Write(Encoding.UTF8.GetBytes(sectionBitmap[i]));
                                if (sectionBitmap[i].Length > 0)
                                {
                                    bw.Write(Convert.ToByte(0));
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ItemParameterCount; i++)
                            {
                                float b = 0.00392156862f;

                                if (SectiomItems[i] == " ")
                                {
                                    SectiomItems[i] = "";
                                }

                                if (i >= ShaderParameterCount || SectiomItemLength[i] == " ")
                                {
                                    SectiomItemLength[i] = "";
                                }

                                bw.Write(Encoding.UTF8.GetBytes(SectiomItems[i]));
                                bw.Write(Encoding.UTF8.GetBytes(SectiomItemLength[i]));

                                if (SectiomItemLength[i].Length > 0)
                                {
                                    bw.Write(Convert.ToByte(0));
                                }                               

                                #region Add Illum Detail
                                if (ShaderTemplate == "add_illum_detail.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 5);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part2(bw,anim_type: 13);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[3]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionBitmapIndex[0]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 5);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part2(bw,anim_type: 13);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[13]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[14]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[15]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[12]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionBitmapIndex[1]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[0]), anim_color_r: Convert.ToByte(sectionPixelConstant[1]), anim_color_g: Convert.ToByte(sectionPixelConstant[2]), anim_color_b: Convert.ToByte(sectionPixelConstant[3]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Ammo Meter
                                if (ShaderTemplate == "ammo_meter.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "meter_gradient_min")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[4]), anim_color_r: Convert.ToByte(sectionPixelConstant[5]), anim_color_g: Convert.ToByte(sectionPixelConstant[6]), anim_color_b: Convert.ToByte(sectionPixelConstant[7]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "meter_gradient_max")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[8]), anim_color_r: Convert.ToByte(sectionPixelConstant[9]), anim_color_g: Convert.ToByte(sectionPixelConstant[10]), anim_color_b: Convert.ToByte(sectionPixelConstant[11]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "meter_empty_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[12]), anim_color_r: Convert.ToByte(sectionPixelConstant[13]), anim_color_g: Convert.ToByte(sectionPixelConstant[14]), anim_color_b: Convert.ToByte(sectionPixelConstant[15]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "meter_amount")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[0]) * b, anim_value_y: 255);
                                    }
                                }
                                #endregion

                                #region Bloom
                                if (ShaderTemplate == "bloom.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bloom_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[3]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "bloom")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[0]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Bumped Environment Additive
                                if (ShaderTemplate == "bumped_environment_additive.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[17]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[18]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "base_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[21]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[22]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[23]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[20]), anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Bumped Environment Blended
                                if (ShaderTemplate == "bumped_environment_blended.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[17]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[18]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "base_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[21]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[22]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[23]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[20]), anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Bumped Environment Darkened
                                if (ShaderTemplate == "bumped_environment_darkened.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[17]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[18]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "base_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[21]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[22]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[23]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[20]), anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Bumped Environment Mask Colored
                                if (ShaderTemplate == "bumped_environment_mask_colored.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[17]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[18]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "base_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[21]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[22]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[23]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[20]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "mask_color0")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[4]), anim_color_r: Convert.ToByte(sectionPixelConstant[5]), anim_color_g: Convert.ToByte(sectionPixelConstant[6]), anim_color_b: Convert.ToByte(sectionPixelConstant[7]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "mask_color1")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[8]), anim_color_r: Convert.ToByte(sectionPixelConstant[9]), anim_color_g: Convert.ToByte(sectionPixelConstant[10]), anim_color_b: Convert.ToByte(sectionPixelConstant[11]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "mask_color2")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[0]), anim_color_r: Convert.ToByte(sectionPixelConstant[1]), anim_color_g: Convert.ToByte(sectionPixelConstant[2]), anim_color_b: Convert.ToByte(sectionPixelConstant[3]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Bumped Environment Masked
                                if (ShaderTemplate == "bumped_environment_masked.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[17]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[18]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "base_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[21]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[22]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[23]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[20]), anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Cortana
                                if (ShaderTemplate == "cortana.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "specular_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[17]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[22]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "alpha_blend_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[33]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[38]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[32]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[36]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "noise_map_a")
                                    {
                                        anim_block_part1(bw,anim_block_count: 5);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part2(bw,anim_type: 7);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[61]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[66]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[60]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[64]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: 0, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "noise_map_b")
                                    {
                                        anim_block_part1(bw,anim_block_count: 5);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part2(bw,anim_type: 7);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[69]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[74]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[68]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[72]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: 0, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "color_wide")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[24]), anim_color_r: Convert.ToByte(sectionPixelConstant[25]), anim_color_g: Convert.ToByte(sectionPixelConstant[26]), anim_color_b: Convert.ToByte(sectionPixelConstant[27]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "color_medium")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[20]), anim_color_r: Convert.ToByte(sectionPixelConstant[21]), anim_color_g: Convert.ToByte(sectionPixelConstant[22]), anim_color_b: Convert.ToByte(sectionPixelConstant[23]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "color_sharp")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[32]), anim_color_r: Convert.ToByte(sectionPixelConstant[33]), anim_color_g: Convert.ToByte(sectionPixelConstant[34]), anim_color_b: Convert.ToByte(sectionPixelConstant[35]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "alpha_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[77]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[82]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[76]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[80]), anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Illum
                                if (ShaderTemplate == "illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[3]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[0]), anim_color_r: Convert.ToByte(sectionPixelConstant[1]), anim_color_g: Convert.ToByte(sectionPixelConstant[2]), anim_color_b: Convert.ToByte(sectionPixelConstant[3]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Illum 3 Channel Opaque
                                if (ShaderTemplate == "illum_3_channel_opaque.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[3]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[8]), anim_color_r: Convert.ToByte(sectionPixelConstant[9]), anim_color_g: Convert.ToByte(sectionPixelConstant[10]), anim_color_b: Convert.ToByte(sectionPixelConstant[11]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[1]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[12]), anim_color_r: Convert.ToByte(sectionPixelConstant[13]), anim_color_g: Convert.ToByte(sectionPixelConstant[14]), anim_color_b: Convert.ToByte(sectionPixelConstant[15]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[6]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[16]), anim_color_r: Convert.ToByte(sectionPixelConstant[17]), anim_color_g: Convert.ToByte(sectionPixelConstant[18]), anim_color_b: Convert.ToByte(sectionPixelConstant[19]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[20]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region illum Bloom
                                if (ShaderTemplate == "illum_bloom.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[3]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "add_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[13]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[14]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[15]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[12]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "mult_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[17]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[18]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[19]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[16]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "bloom_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[21]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[22]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[23]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[20]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[0]), anim_color_r: Convert.ToByte(sectionPixelConstant[1]), anim_color_g: Convert.ToByte(sectionPixelConstant[2]), anim_color_b: Convert.ToByte(sectionPixelConstant[3]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "bloom")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[4]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region illum Bloom Opaque
                                if (ShaderTemplate == "illum_bloom_opaque.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[3]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "add_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[13]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[14]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[15]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[12]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "mult_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[17]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[18]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[19]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[16]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "bloom_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[21]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[22]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[23]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[20]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[0]), anim_color_r: Convert.ToByte(sectionPixelConstant[1]), anim_color_g: Convert.ToByte(sectionPixelConstant[2]), anim_color_b: Convert.ToByte(sectionPixelConstant[3]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "bloom")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[4]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region illum Clamped
                                if (ShaderTemplate == "illum_clamped.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[3]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[0]), anim_color_r: Convert.ToByte(sectionPixelConstant[1]), anim_color_g: Convert.ToByte(sectionPixelConstant[2]), anim_color_b: Convert.ToByte(sectionPixelConstant[3]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region illum Detail
                                if (ShaderTemplate == "illum_detail.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 5);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part2(bw,anim_type: 13);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[3]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionBitmapIndex[0]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 5);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part2(bw,anim_type: 13);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[13]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[14]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[15]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[12]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionBitmapIndex[1]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[0]), anim_color_r: Convert.ToByte(sectionPixelConstant[1]), anim_color_g: Convert.ToByte(sectionPixelConstant[2]), anim_color_b: Convert.ToByte(sectionPixelConstant[3]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region illum Opaque
                                if (ShaderTemplate == "illum_opaque.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[3]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[0]), anim_color_r: Convert.ToByte(sectionPixelConstant[1]), anim_color_g: Convert.ToByte(sectionPixelConstant[2]), anim_color_b: Convert.ToByte(sectionPixelConstant[3]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region One Add Illum
                                if (ShaderTemplate == "one_add_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[6]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[4]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[0]), anim_color_r: Convert.ToByte(sectionPixelConstant[1]), anim_color_g: Convert.ToByte(sectionPixelConstant[2]), anim_color_b: Convert.ToByte(sectionPixelConstant[3]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region One Add Illum Detail
                                if (ShaderTemplate == "one_add_illum_detail.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[6]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[0]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[4]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[14]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[8]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[12]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[0]), anim_color_r: Convert.ToByte(sectionPixelConstant[1]), anim_color_g: Convert.ToByte(sectionPixelConstant[2]), anim_color_b: Convert.ToByte(sectionPixelConstant[3]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "tint_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: (Convert.ToSingle(sectionVertexConstant[16]) * 255), anim_color_r: (Convert.ToSingle(sectionVertexConstant[17]) * 255), anim_color_g: (Convert.ToSingle(sectionVertexConstant[18]) * 255), anim_color_b: (Convert.ToSingle(sectionVertexConstant[19]) * 255), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "glancing_tint_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: (Convert.ToSingle(sectionVertexConstant[20]) * 255), anim_color_r: (Convert.ToSingle(sectionVertexConstant[21]) * 255), anim_color_g: (Convert.ToSingle(sectionVertexConstant[22]) * 255), anim_color_b: (Convert.ToSingle(sectionVertexConstant[23]) * 255), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[24]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "glancing_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[28]), anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region One Alpha Env Illum
                                if (ShaderTemplate == "one_alpha_env_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[25]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[30]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[24]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[28]), anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region One Alpha Env Illum Specular Mask
                                if (ShaderTemplate == "one_alpha_env_illum_specular_mask.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[17]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[22]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[16]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[20]), anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Tex Bump
                                if (ShaderTemplate == "tex_bump.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "base_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 5);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part2(bw,anim_type: 13);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[5]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[6]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[7]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[4]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionBitmapIndex[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Tex Bump Dprs Env Illum
                                if (ShaderTemplate == "tex_bump_dprs_env_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[8]), anim_color_r: Convert.ToByte(sectionPixelConstant[9]), anim_color_g: Convert.ToByte(sectionPixelConstant[10]), anim_color_b: Convert.ToByte(sectionPixelConstant[11]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env
                                if (ShaderTemplate == "tex_bump_env.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "base_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[5]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[6]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[7]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[4]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum
                                if (ShaderTemplate == "tex_bump_env_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[24]), anim_color_r: Convert.ToByte(sectionPixelConstant[25]), anim_color_g: Convert.ToByte(sectionPixelConstant[26]), anim_color_b: Convert.ToByte(sectionPixelConstant[27]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum 3 Channel
                                if (ShaderTemplate == "tex_bump_env_illum_3_channel.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[45]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[46]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[47]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[44]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[24]), anim_color_r: Convert.ToByte(sectionPixelConstant[25]), anim_color_g: Convert.ToByte(sectionPixelConstant[26]), anim_color_b: Convert.ToByte(sectionPixelConstant[27]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[17]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[28]), anim_color_r: Convert.ToByte(sectionPixelConstant[29]), anim_color_g: Convert.ToByte(sectionPixelConstant[30]), anim_color_b: Convert.ToByte(sectionPixelConstant[31]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[22]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[32]), anim_color_r: Convert.ToByte(sectionPixelConstant[33]), anim_color_g: Convert.ToByte(sectionPixelConstant[34]), anim_color_b: Convert.ToByte(sectionPixelConstant[35]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[36]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum 3 Channel Combined
                                if (ShaderTemplate == "tex_bump_env_illum_3_channel_combined.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[21]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[22]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[23]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[20]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[32]), anim_color_r: Convert.ToByte(sectionPixelConstant[33]), anim_color_g: Convert.ToByte(sectionPixelConstant[34]), anim_color_b: Convert.ToByte(sectionPixelConstant[35]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[25]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[36]), anim_color_r: Convert.ToByte(sectionPixelConstant[37]), anim_color_g: Convert.ToByte(sectionPixelConstant[38]), anim_color_b: Convert.ToByte(sectionPixelConstant[39]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[30]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[40]), anim_color_r: Convert.ToByte(sectionPixelConstant[41]), anim_color_g: Convert.ToByte(sectionPixelConstant[42]), anim_color_b: Convert.ToByte(sectionPixelConstant[43]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[44]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum 3 Channel Occlusion Combined
                                if (ShaderTemplate == "tex_bump_env_illum_3_channel_occlusion_combined.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "base_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[5]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[6]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[7]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[4]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[45]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[46]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[47]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[44]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "occlusion_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[49]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[50]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[51]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[48]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[24]), anim_color_r: Convert.ToByte(sectionPixelConstant[25]), anim_color_g: Convert.ToByte(sectionPixelConstant[26]), anim_color_b: Convert.ToByte(sectionPixelConstant[27]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[17]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[32]), anim_color_r: Convert.ToByte(sectionPixelConstant[33]), anim_color_g: Convert.ToByte(sectionPixelConstant[34]), anim_color_b: Convert.ToByte(sectionPixelConstant[35]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[22]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[36]), anim_color_r: Convert.ToByte(sectionPixelConstant[37]), anim_color_g: Convert.ToByte(sectionPixelConstant[38]), anim_color_b: Convert.ToByte(sectionPixelConstant[39]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[28]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "occlusion_a_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[40]), anim_color_r: Convert.ToByte(sectionPixelConstant[41]), anim_color_g: Convert.ToByte(sectionPixelConstant[42]), anim_color_b: Convert.ToByte(sectionPixelConstant[43]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "occlusion_b_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[44]), anim_color_r: Convert.ToByte(sectionPixelConstant[45]), anim_color_g: Convert.ToByte(sectionPixelConstant[46]), anim_color_b: Convert.ToByte(sectionPixelConstant[47]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "occlusion_c_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[48]), anim_color_r: Convert.ToByte(sectionPixelConstant[49]), anim_color_g: Convert.ToByte(sectionPixelConstant[50]), anim_color_b: Convert.ToByte(sectionPixelConstant[51]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum Combined
                                if (ShaderTemplate == "tex_bump_env_illum_combined.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[24]), anim_color_r: Convert.ToByte(sectionPixelConstant[25]), anim_color_g: Convert.ToByte(sectionPixelConstant[26]), anim_color_b: Convert.ToByte(sectionPixelConstant[27]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum Detail Honor Guard
                                if (ShaderTemplate == "tex_bump_env_illum_detail_honor_guard.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_detail")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[53]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[54]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map_value_scale")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[24]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[20]), anim_color_r: Convert.ToByte(sectionPixelConstant[21]), anim_color_g: Convert.ToByte(sectionPixelConstant[22]), anim_color_b: Convert.ToByte(sectionPixelConstant[23]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum Four Change Color No Lod
                                if (ShaderTemplate == "tex_bump_env_illum_four_change_color_no_lod.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 3);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 13);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionBitmapIndex[3]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[45]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[46]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[47]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[44]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[16]), anim_color_r: Convert.ToByte(sectionPixelConstant[17]), anim_color_g: Convert.ToByte(sectionPixelConstant[18]), anim_color_b: Convert.ToByte(sectionPixelConstant[19]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum Two Change Color
                                if (ShaderTemplate == "tex_bump_env_illum_two_change_color.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 3);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 13);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionBitmapIndex[3]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[16]), anim_color_r: Convert.ToByte(sectionPixelConstant[17]), anim_color_g: Convert.ToByte(sectionPixelConstant[18]), anim_color_b: Convert.ToByte(sectionPixelConstant[19]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Illum Two Change Color Combined
                                if (ShaderTemplate == "tex_bump_env_illum_two_change_color_combined.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[8]), anim_color_r: Convert.ToByte(sectionPixelConstant[9]), anim_color_g: Convert.ToByte(sectionPixelConstant[10]), anim_color_b: Convert.ToByte(sectionPixelConstant[11]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Env Two Change Color Multiply Map Self Illum
                                if (ShaderTemplate == "tex_bump_env_two_change_color_multiply_map_self_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "change_color_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[13]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[14]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "multiply_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 3);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 13);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[19]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[16]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionBitmapIndex[4]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[37]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[38]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[8]), anim_color_r: Convert.ToByte(sectionPixelConstant[9]), anim_color_g: Convert.ToByte(sectionPixelConstant[10]), anim_color_b: Convert.ToByte(sectionPixelConstant[11]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Illum
                                if (ShaderTemplate == "tex_bump_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 5);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part2(bw,anim_type: 13);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[29]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[30]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[31]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[28]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionBitmapIndex[3]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[8]), anim_color_r: Convert.ToByte(sectionPixelConstant[9]), anim_color_g: Convert.ToByte(sectionPixelConstant[10]), anim_color_b: Convert.ToByte(sectionPixelConstant[11]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Illum 3 Channel
                                if (ShaderTemplate == "tex_bump_illum_3_channel.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "base_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[5]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[6]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[7]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[4]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[17]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[18]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[19]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[16]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[16]), anim_color_r: Convert.ToByte(sectionPixelConstant[17]), anim_color_g: Convert.ToByte(sectionPixelConstant[18]), anim_color_b: Convert.ToByte(sectionPixelConstant[19]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[24]), anim_color_r: Convert.ToByte(sectionPixelConstant[25]), anim_color_g: Convert.ToByte(sectionPixelConstant[26]), anim_color_b: Convert.ToByte(sectionPixelConstant[27]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[28]), anim_color_r: Convert.ToByte(sectionPixelConstant[29]), anim_color_g: Convert.ToByte(sectionPixelConstant[30]), anim_color_b: Convert.ToByte(sectionPixelConstant[31]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[9]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[14]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[20]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Tex Bump Illum Alpha Test
                                if (ShaderTemplate == "tex_bump_illum_alpha_test.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "alpha_test_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[13]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[14]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[4]), anim_color_r: Convert.ToByte(sectionPixelConstant[5]), anim_color_g: Convert.ToByte(sectionPixelConstant[6]), anim_color_b: Convert.ToByte(sectionPixelConstant[7]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Tex Bump Illum Bloom
                                if (ShaderTemplate == "tex_bump_illum_bloom.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "base_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[5]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[6]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[7]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[4]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[11]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[8]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[29]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[30]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[31]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[28]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "bloom_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[33]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[34]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[35]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[32]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[8]), anim_color_r: Convert.ToByte(sectionPixelConstant[9]), anim_color_g: Convert.ToByte(sectionPixelConstant[10]), anim_color_b: Convert.ToByte(sectionPixelConstant[11]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "bloom")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[12]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Tex Bump Meter Illum
                                if (ShaderTemplate == "tex_bump_meter_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "meter_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[29]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[30]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[31]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[28]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "meter_on_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[12]), anim_color_r: Convert.ToByte(sectionPixelConstant[13]), anim_color_g: Convert.ToByte(sectionPixelConstant[14]), anim_color_b: Convert.ToByte(sectionPixelConstant[15]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "meter_off_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[16]), anim_color_r: Convert.ToByte(sectionPixelConstant[17]), anim_color_g: Convert.ToByte(sectionPixelConstant[18]), anim_color_b: Convert.ToByte(sectionPixelConstant[19]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "meter_value")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[8]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Tex Bump Plasma One Channel Illum
                                if (ShaderTemplate == "tex_bump_plasma_one_channel_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "bump_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[1]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[2]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "detail_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 2);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[9]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[10]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "multichannel_map")
                                    {
                                        anim_block_part1(bw,anim_block_count: 4);
                                        anim_block_part2(bw,anim_type: 1);
                                        anim_block_part2(bw,anim_type: 2);
                                        anim_block_part2(bw,anim_type: 4);
                                        anim_block_part2(bw,anim_type: 5);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[29]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[30]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[31]), anim_value_y: 1);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionVertexConstant[28]), anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_a_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[20]), anim_color_r: Convert.ToByte(sectionPixelConstant[21]), anim_color_g: Convert.ToByte(sectionPixelConstant[22]), anim_color_b: Convert.ToByte(sectionPixelConstant[23]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[12]), anim_color_r: Convert.ToByte(sectionPixelConstant[13]), anim_color_g: Convert.ToByte(sectionPixelConstant[14]), anim_color_b: Convert.ToByte(sectionPixelConstant[15]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[16]), anim_color_r: Convert.ToByte(sectionPixelConstant[17]), anim_color_g: Convert.ToByte(sectionPixelConstant[18]), anim_color_b: Convert.ToByte(sectionPixelConstant[19]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "time")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[8]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Two Add Env Illum
                                if (ShaderTemplate == "two_add_env_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[12]), anim_color_r: Convert.ToByte(sectionPixelConstant[13]), anim_color_g: Convert.ToByte(sectionPixelConstant[14]), anim_color_b: Convert.ToByte(sectionPixelConstant[15]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Two Add Env Illum 3 Channel
                                if (ShaderTemplate == "two_add_env_illum_3_channel.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "channel_a_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[20]), anim_color_r: Convert.ToByte(sectionPixelConstant[21]), anim_color_g: Convert.ToByte(sectionPixelConstant[22]), anim_color_b: Convert.ToByte(sectionPixelConstant[23]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_a_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[13]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_b_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[24]), anim_color_r: Convert.ToByte(sectionPixelConstant[25]), anim_color_g: Convert.ToByte(sectionPixelConstant[26]), anim_color_b: Convert.ToByte(sectionPixelConstant[27]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_b_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[18]) * b, anim_value_y: 1);
                                    }
                                    if (SectiomItems[i] == "channel_c_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[28]), anim_color_r: Convert.ToByte(sectionPixelConstant[29]), anim_color_g: Convert.ToByte(sectionPixelConstant[30]), anim_color_b: Convert.ToByte(sectionPixelConstant[31]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                    if (SectiomItems[i] == "channel_c_brightness")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 11);
                                        anim_block_part3_bitmap(bw,anim_type: 28, function_type: 1, range_flag: 0, anim_value_x: Convert.ToSingle(sectionPixelConstant[23]) * b, anim_value_y: 1);
                                    }
                                }
                                #endregion

                                #region Two Add Env Illum Active Camo
                                if (ShaderTemplate == "two_add_env_illum_active_camo.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[12]), anim_color_r: Convert.ToByte(sectionPixelConstant[13]), anim_color_g: Convert.ToByte(sectionPixelConstant[14]), anim_color_b: Convert.ToByte(sectionPixelConstant[15]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion

                                #region Two Alpha Env Illum
                                if (ShaderTemplate == "two_alpha_env_illum.shader_template.txt")
                                {
                                    if (SectiomItems[i] == "self_illum_color")
                                    {
                                        anim_block_part1(bw,anim_block_count: 1);
                                        anim_block_part2(bw,anim_type: 12);
                                        anim_block_part3_color(bw,anim_type: 28, function_type: 1, range_flag: 20, anim_color_a: Convert.ToByte(sectionPixelConstant[12]), anim_color_r: Convert.ToByte(sectionPixelConstant[13]), anim_color_g: Convert.ToByte(sectionPixelConstant[14]), anim_color_b: Convert.ToByte(sectionPixelConstant[15]), anim_color_a_range: 0, anim_color_r_range: 255, anim_color_g_range: 255, anim_color_b_range: 255);
                                    }
                                }
                                #endregion
                            }
                        }
                        ms.Position = 0;
                        ds.Position = 0;
                        ms.CopyTo(ds);

                        ms.Close();
                        fs.Close();
                        ds.Close();
                    }
                }
                MessageBox.Show("Finished Creating Tags");
            }
        }
    }
}
