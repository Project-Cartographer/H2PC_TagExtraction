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

                    var sectionBitmap = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### BITMAPS ###")               // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### BITMAPS END ###")           // Take lines until the end
                        .ToList();                                            // Convert the result to List<string>

                    var sectionLightmap = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### LIGHTMAP ###")              // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### LIGHTMAP END ###")          // Take lines until the end
                        .ToList();

                    var sectionPixelConstant = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### PIXEL CONSTANTS ###")       // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### PIXEL CONSTANTS END ###")   // Take lines until the end
                        .ToList();                                            // Convert the result to List<string>

                    var sectionVertexConstant = File.ReadAllLines(Path.Combine(shaderpath, line))
                        .SkipWhile(s => s != "### VERTEX CONSTANTS ###")      // Skip up to the header
                        .Skip(1)                                              // Skip the header
                        .TakeWhile(s => s != "### VERTEX CONSTANTS END ###")  // Take lines until the end
                        .ToList();

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
                    SupportedShaders.Add("cortana_holographic_active_camo.shader_template.txt");
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
                    SupportedShaders.Add("tex_bump_env_dbl_spec.shader_template.txt");
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

                    string ShaderTemplate = Path.GetFileName(sectionShaderParameter[0]) + ".shader_template.txt";

                    string PixelTemplate = Path.GetFileName(sectionShaderParameter[0]) + ".shader_template_pixel.txt";

                    string[] BitmapLabels = File.ReadAllLines(Path.Combine(workingdirectory, "plugins", "shaderstemplates", ShaderTemplate));

                    string[] PixelLabels = File.ReadAllLines(Path.Combine(workingdirectory, "plugins", "pixeltemplates", PixelTemplate));

                    int ShaderParameterCount = BitmapLabels.GetLength(0);

                    int PixelParameterCount = PixelLabels.GetLength(0);

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
                        bw.Write(Convert.ToInt32(ShaderParameterCount + PixelParameterCount)); //Write parameter count
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
                        bw.Write(Convert.ToInt32(ShaderParameterCount + PixelParameterCount)); //Write parameter count
                        bw.Write(Convert.ToInt32(52));

                        for (int i = 0; i < ShaderParameterCount; i++)
                        {
                            bw.Write(Convert.ToInt16(0));

                            if (sectionBitmap[i] == " ")
                            {
                                sectionBitmap[i] = "";
                            }

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
                            //Look I don't design the tags I just write them to the file. I need to write 10088844 and I don't care why.
                        }

                        for (int i = 0; i < PixelParameterCount; i++)
                        {
                            bw.Write(Convert.ToInt16(0));

                            if (PixelLabels[i] == " ")
                            {
                                PixelLabels[i] = "";
                            }

                            byte[] flip = new byte[2];
                            flip = BitConverter.GetBytes(Convert.ToInt16(PixelLabels[i].Length));
                            Array.Reverse(flip);

                            bw.Write(BitConverter.ToInt16(flip, 0));

                            #region Shader Specific Fixes

                            float b = 0.00392156862f;

                            #region Active Camo Opaque
                            if (ShaderTemplate == "active_camo_opaque.shader_template.txt")
                            {
                                if (PixelLabels[i] == "refraction_bump_amount")     //vertex constant index 2 x value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "refraction_geometry_amount") //vertex constant index 3 y value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Active Camo Transparent
                            if (ShaderTemplate == "active_camo_transparent.shader_template.txt")
                            {
                                if (PixelLabels[i] == "refraction_bump_amount")     //vertex constant index 2 x value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "refraction_geometry_amount") //vertex constant index 3 y value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Add Illum Detail
                            if (ShaderTemplate == "add_illum_detail.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")        //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_color") //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Ammo Meter
                            if (ShaderTemplate == "ammo_meter.shader_template.txt")
                            {
                                if (PixelLabels[i] == "meter_gradient_min") //pixel constants index 1 value - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));          //A
                                    bw.Write(Convert.ToSingle(0));          //R
                                    bw.Write(Convert.ToSingle(0));          //G
                                    bw.Write(Convert.ToSingle(0));          //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "meter_gradient_max") //pixel constants index 2 value - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));          //A
                                    bw.Write(Convert.ToSingle(0));          //R
                                    bw.Write(Convert.ToSingle(0));          //G
                                    bw.Write(Convert.ToSingle(0));          //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "meter_empty_color")  //pixel constants index 3 value - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));          //A
                                    bw.Write(Convert.ToSingle(0));          //R
                                    bw.Write(Convert.ToSingle(0));          //G
                                    bw.Write(Convert.ToSingle(0));          //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "meter_amount")       //pixel constants index 0 value - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));          //A
                                    bw.Write(Convert.ToSingle(0));          //R
                                    bw.Write(Convert.ToSingle(0));          //G
                                    bw.Write(Convert.ToSingle(0));          //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bloom
                            if (ShaderTemplate == "bloom.shader_template.txt")
                            {
                                if (PixelLabels[i] == "lightmap_emissive_color") //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power") //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "bloom")                   //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bumped Environment Additive
                            if (ShaderTemplate == "bumped_environment_additive.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")          //vertex constant index 0 x,y,z rgb floats - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color") //vertex constant index 1 x,y,z rgb floats - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")          //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness") //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")          //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));               //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")          //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToSingle(0));               //R
                                    bw.Write(Convert.ToSingle(0));               //G
                                    bw.Write(Convert.ToSingle(0));               //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bumped Environment Blended
                            if (ShaderTemplate == "bumped_environment_blended.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")             //vertex constant index 0 x,y,z rgb floats - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")    //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")             //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")    //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")             //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")             //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_translucent_color") //runtime properites tag block lightmap transparent colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));                  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[6]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[7]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[8]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_translucent_alpha") //runtime properties tag block lightmap transparent alpha - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[5]))); //A
                                    bw.Write(Convert.ToSingle(0));                  //R
                                    bw.Write(Convert.ToSingle(0));                  //G
                                    bw.Write(Convert.ToSingle(0));                  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bumped Environment Darkened
                            if (ShaderTemplate == "bumped_environment_darkened.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //vertex constant index 0 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bumped Environment Mask Colored
                            if (ShaderTemplate == "bumped_environment_mask_colored.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //vertex constant index 0 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "mask_color0")                //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "mask_color1")                //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "mask_color2")                //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Bumped Environment Masked
                            if (ShaderTemplate == "bumped_environment_masked.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //vertex constant index 0 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //vertex constant index 1 x,y,z rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //vertex constant index 2 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //vertex constant index 3 w value - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                                         //runtime properites tag block lightmap emissive colors value - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                         //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Cortana
                            if (ShaderTemplate == "cortana.shader_template.txt")
                            {
                                if (PixelLabels[i] == "flat_environment_color")     //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")          //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")        //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")                 //vertex constant index 0 rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")        //vertex constant index 1 rgb floats - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")                 //vertex constant index 2 alpha float - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")        //vertex constant index 3 alpha float - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "color_wide")                 //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "color_medium")               //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "color_sharp")                //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0)); //A
                                    bw.Write(Convert.ToSingle(0)); //R
                                    bw.Write(Convert.ToSingle(0)); //G
                                    bw.Write(Convert.ToSingle(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "perpendicular_brightness")   //vertex constant index 21 alpha float - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[84]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[85]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[86]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[87]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "fade_bias")                  //pixel constant index 7 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[28]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[29]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[30]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[31]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "bloom")                      //pixel constant index 4 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_tint_color")             //vertex constant index 23 rgb float - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[92]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[93]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[94]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[95]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")    //vertex constant index 24 rgb float - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[96]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[97]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[98]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[99]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")             //vertex constant index 25 rgb float - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[100]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[101]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[102]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[103]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")    //vertex constant index 26 rgb float - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[104]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[105]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[106]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[107]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor")   //pixel constant index 9 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[36]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[37]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[38]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[39]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color")    //pixel constant index 10 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[40]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[41]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[42]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[43]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Cortana Holographic Active Camo
                            if (ShaderTemplate == "cortana_holographic_active_camo.shader_template.txt")
                            {
                                if (PixelLabels[i] == "refraction_bump_amount")                                        //unused? moved from the shad file to some other file on package? Only used on lighting and thrown away on package? - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));                                             //A
                                    bw.Write(Convert.ToSingle(0));                                             //R
                                    bw.Write(Convert.ToSingle(0));                                             //G
                                    bw.Write(Convert.ToSingle(0));                                             //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "refraction_geometry_amount")                                        //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "refraction_x_offset")                               //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Illum
                            if (ShaderTemplate == "illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Illum 3 Channel Opaque
                            if (ShaderTemplate == "illum_3_channel_opaque.shader_template.txt")
                            {
                                if (PixelLabels[i] == "channel_a_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_brightness")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_brightness")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_brightness")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region illum Bloom
                            if (ShaderTemplate == "illum_bloom.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "bloom")                                        //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region illum Bloom Opaque
                            if (ShaderTemplate == "illum_bloom_opaque.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "bloom")                                        //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region illum Clamped
                            if (ShaderTemplate == "illum_clamped.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region illum Detail
                            if (ShaderTemplate == "illum_detail.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region illum Opaque
                            if (ShaderTemplate == "illum_opaque.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region One Add Illum
                            if (ShaderTemplate == "one_add_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")                                   //vertex constant index 2 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")                          //vertex constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")                                   //vertex constant index 4 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[16]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[17]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[18]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[19]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")                          //vertex constant index 5 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[20]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[21]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[22]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[23]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emmisive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_emmisive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region One Add Illum Detail
                            if (ShaderTemplate == "one_add_illum_detail.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 0 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")                                   //vertex constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")                          //vertex constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")                                   //vertex constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")                          //vertex constant index 7 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region One Alpha Env Illum
                            if (ShaderTemplate == "one_alpha_env_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "environment_color")                             //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")                            //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")                          //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")                                          //vertex constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")                                 //vertex constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")                                          //vertex constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")                                 //vertex constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region One Alpha Env Illum Specular Mask
                            if (ShaderTemplate == "one_alpha_env_illum_specular_mask.shader_template.txt")
                            {
                                if (PixelLabels[i] == "environment_color")                             //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")                            //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")                          //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color")                             //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")                                          //vertex constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")                                 //vertex constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")                                          //vertex constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")                                 //vertex constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                      //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                                     //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                      //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0));                                     //R
                                    bw.Write(Convert.ToInt32(0));                                     //G
                                    bw.Write(Convert.ToInt32(0));                                     //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump
                            if (ShaderTemplate == "tex_bump.shader_template.txt")
                            {
                                if (PixelLabels[i] == "ambient_factor")                                        //unused? moved from the shad file to some other file on package? Only used on lighting and thrown away on package? - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(0));                                             //A
                                    bw.Write(Convert.ToSingle(0));                                             //R
                                    bw.Write(Convert.ToSingle(0));                                             //G
                                    bw.Write(Convert.ToSingle(0));                                             //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                        //pixel constant index 0 - written as rgb float in mitb block
                                {

                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                               //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Dprs Env Illum
                            if (ShaderTemplate == "tex_bump_dprs_env_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")             //vertex constant index 8 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[32])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[33])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[34])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[35])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //vertex constant index 9 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[36])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[37])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[38])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[39])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //vertex constant index 10 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[40])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[41])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[42])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[43])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")    //vertex constant index 11 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[44])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[45])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[46])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[47])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //vertex constant index 12 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[48])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[49])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[50])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[51])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //vertex constant index 13 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[52])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[53])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[54])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[55])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env
                            if (ShaderTemplate == "tex_bump_env.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Dbl Spec
                            if (ShaderTemplate == "tex_bump_env_dbl_spec.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum
                            if (ShaderTemplate == "tex_bump_env_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //Not written to packaged shad tag? - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                 //A
                                    bw.Write(Convert.ToInt32(0));                 //R
                                    bw.Write(Convert.ToInt32(0));                 //G
                                    bw.Write(Convert.ToInt32(0));                 //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //Not written to packaged shad tag? - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));                //A
                                    bw.Write(Convert.ToInt32(0));                //R
                                    bw.Write(Convert.ToInt32(0));                //G
                                    bw.Write(Convert.ToInt32(0));                //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum 3 Channel
                            if (ShaderTemplate == "tex_bump_env_illum_3_channel.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 10 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[40]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[41]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[42]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[43]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 11 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[44]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[45]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[46]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[47]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 12 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[48]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[49]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[50]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[51]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 13 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[52]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[53]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[54]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[55]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color") //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_brightness") //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color") //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_brightness") //pixel constant index 9 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum 3 Channel Combined
                            if (ShaderTemplate == "tex_bump_env_illum_3_channel_combined.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color") //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_brightness") //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color") //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_brightness") //pixel constant index 9 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum 3 Channel Occlusion Combined
                            if (ShaderTemplate == "tex_bump_env_illum_3_channel_occlusion_combined.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 13 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[52]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[53]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[54]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[55]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 14 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[56]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[57]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[58]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[59]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 15 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[60]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[61]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[62]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[63]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 16 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[64]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[65]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[66]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[67]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color") //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_brightness") //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color") //pixel constant index 8 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_brightness") //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "occlusion_a_color") //pixel constant index 10 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "occlusion_b_color") //pixel constant index 11 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "occlusion_c_color") //pixel constant index 12 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum Combined
                            if (ShaderTemplate == "tex_bump_env_illum_combined.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap emissive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum Detail Honor Guard
                            if (ShaderTemplate == "tex_bump_env_illum_detail_honor_guard.shader_template.txt")
                            {
                                if (PixelLabels[i] == "detail_map_value_scale")                                 //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "burn_scale")                                             //pixel constant index 0 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap half life - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[4]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_tint_color")                                         //vertex constant index 6 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[24])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[25])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[26])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[27])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                                //vertex constant index 7 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[28])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[29])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[30])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[31])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                         //vertex constant index 8 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[32])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[33])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[34]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[35]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                                //vertex constant index 9 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[36]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[37]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[38]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[39]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[16]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[17]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[18]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[19]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 2 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum Four Change Color No Lod
                            if (ShaderTemplate == "tex_bump_env_illum_four_change_color_no_lod.shader_template.txt")
                            {
                                if (PixelLabels[i] == "ambient_factor")   //Not written to shad file - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_tint_color")                                       //vertex constant index 4 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[16])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[17])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[18])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[19])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                              //vertex constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[20])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[21])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[22])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[23])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                      //vertex constant index 6 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[24]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[25]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[26]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[27]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                             //vertex constant index 7 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[28]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[29]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[30]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[31]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum Two Change Color
                            if (ShaderTemplate == "tex_bump_env_illum_two_change_color.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                       //vertex constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                              //vertex constant index 6 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[24]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[25]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[26]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[27]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                      //vertex constant index 7 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[28]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[29]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[30]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[31]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                             //vertex constant index 8 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[32]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[33]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[34]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[35]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //pixel constant index 3 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[12]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[13]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[14]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[15]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Illum Two Change Color Combined
                            if (ShaderTemplate == "tex_bump_env_illum_two_change_color_combined.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                       //vertex constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[20])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[21])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[22])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[23])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                              //vertex constant index 6 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[24])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[25])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[26])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[27])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                      //vertex constant index 7 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[28]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[29]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[30]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[31]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                             //vertex constant index 8 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[32]))); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[33]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[34]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[35]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_factor") //Not written to shad file - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_midrange_tint_color") //Not written to shad file - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Env Two Change Color Multiply Map Self Illum
                            if (ShaderTemplate == "tex_bump_env_two_change_color_multiply_map_self_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "env_tint_color")                                       //vertex constant index 5 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[20]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[21]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[22]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[23]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_tint_color")                              //vertex constant index 6 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[24]) * b));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[25]) * b));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[26]) * b));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[27]) * b));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_brightness")                                      //vertex constant index 7 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[28]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[29]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[30]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[31]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "env_glancing_brightness")                             //vertex constant index 8 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[32]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[33]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[34]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[35]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")             //Not written to packaged shad file - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")             //Not written to packaged shad file  - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Illum
                            if (ShaderTemplate == "tex_bump_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Illum 3 Channel
                            if (ShaderTemplate == "tex_bump_illum_3_channel.shader_template.txt")
                            {
                                if (PixelLabels[i] == "channel_a_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color") //pixel constant index 6 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color") //pixel constant index 7 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_brightness") //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_brightness") //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_brightness") //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Illum Alpha Test
                            if (ShaderTemplate == "tex_bump_illum_alpha_test.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 1 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 2 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Illum Bloom
                            if (ShaderTemplate == "tex_bump_illum_bloom.shader_template.txt")
                            {
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                                //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "bloom") //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Meter Illum
                            if (ShaderTemplate == "tex_bump_meter_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "meter_on_color")   //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "meter_off_color")  //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "meter_value")      //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Tex Bump Plasma One Channel Illum
                            if (ShaderTemplate == "tex_bump_plasma_one_channel_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "channel_a_color")  //pixel constant index 5 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color")  //pixel constant index 3 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color")  //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "time")             //pixel constant index 2 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                               //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_color")                                         //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "specular_glancing_color")                                //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Two Add Env Illum
                            if (ShaderTemplate == "two_add_env_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "environment_color")  //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")  //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")  //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")  //vertex constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")             //vertex constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")  //vertex constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")             //vertex constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                               //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Two Add Env Illum 3 Channel
                            if (ShaderTemplate == "two_add_env_illum_3_channel.shader_template.txt")
                            {
                                if (PixelLabels[i] == "environment_color")  //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")  //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")  //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_a_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_b_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "channel_c_brightness") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")  //vertex constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")             //vertex constant index 2 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")  //vertex constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")             //vertex constant index 4 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[16])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[17])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[18])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[19])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                               //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Two Add Env Illum Active Camo
                            if (ShaderTemplate == "two_add_env_illum_active_camo.shader_template.txt")
                            {
                                if (PixelLabels[i] == "environment_color")  //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")  //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")  //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")  //vertex constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")             //vertex constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")  //vertex constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")             //vertex constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                               //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            #region Two Alpha Env Illum
                            if (ShaderTemplate == "two_alpha_env_illum.shader_template.txt")
                            {
                                if (PixelLabels[i] == "environment_color")  //pixel constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[0]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[1]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[2]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[3]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_color")  //pixel constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[4]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[5]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[6]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[7]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "alpha_blend_opacity")  //pixel constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[8]) * b)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[9]) * b)); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[10]) * b)); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionPixelConstant[11]) * b)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "self_illum_color") //pixel constant index 4 - Written as hex color value in a dfbt next to parameter name
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0));         //A
                                    bw.Write(Convert.ToInt32(0));         //R
                                    bw.Write(Convert.ToInt32(0));         //G
                                    bw.Write(Convert.ToInt32(0));         //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "tint_color")  //vertex constant index 0 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[0])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[1])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[2])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[3])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_tint_color")             //vertex constant index 1 - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[4])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[5])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[6])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[7])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "brightness")  //vertex constant index 2 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[8])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[9])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[10])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[11])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "glancing_brightness")             //vertex constant index 3 - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[12])));  //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[13])));  //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[14])));  //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionVertexConstant[15])));  //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_translucent_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[6]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[7]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[8]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "lightmap_translucent_alpha")                               //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[5]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_color")                               //runtime properties tag block lightmap emissive color - written as rgb float in mitb block
                                {
                                    bw.Write(Convert.ToByte(2));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToInt32(0)); //A
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[1]))); //R
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[2]))); //G
                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[3]))); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                                if (PixelLabels[i] == "emissive_power")                               //runtime properties tag block lightmap emmisive power - written as alpha float in mitb block
                                {
                                    bw.Write(Convert.ToByte(1));                    //Unknown Value
                                    bw.Write(Convert.ToByte(0));
                                    bw.Write(Convert.ToInt16(0));
                                    bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                    bw.Write(Convert.ToInt32(0));
                                    bw.Write((Convert.ToInt32(0)));
                                    bw.Write(Convert.ToInt32(-1));

                                    bw.Write(Convert.ToSingle(Convert.ToSingle(sectionLightmap[0]))); //A
                                    bw.Write(Convert.ToInt32(0)); //R
                                    bw.Write(Convert.ToInt32(0)); //G
                                    bw.Write(Convert.ToInt32(0)); //B
                                    bw.Write(Convert.ToInt32(0));
                                }
                            }
                            #endregion

                            if (!SupportedShaders.Contains(ShaderTemplate))
                            {
                                bw.Write(Convert.ToByte(0));                    //Unknown Value
                                bw.Write(Convert.ToByte(0));
                                bw.Write(Convert.ToInt16(0));
                                bw.Write(Encoding.UTF8.GetBytes("mtib"));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write((Convert.ToInt32(0)));
                                bw.Write(Convert.ToInt32(-1));

                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                            }

                            #endregion
                            // 5 time 5 time 5 time 5 time 5 time WCW Champion

                            bw.Write(Convert.ToInt32(-1));

                            bw.Write(Convert.ToInt32(10088844));
                            //Look I don't design the tags I just write them to the file. I need to write 10088844 and I don't care why.
                        }
                        for (int i = 0; i < ShaderParameterCount; i++)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(BitmapLabels[i]));
                            bw.Write(Encoding.UTF8.GetBytes(sectionBitmap[i]));
                            if (sectionBitmap[i].Length > 0)
                            {
                                bw.Write(Convert.ToByte(0));
                            }
                            /*if (BitmapLabels[i] == "detail_map")
                            {
                                bw.Write(Encoding.UTF8.GetBytes("dfbt"));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                                bw.Write(Convert.ToInt32(0));
                            }*/
                        }
                        for (int i = 0; i < PixelParameterCount; i++)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(PixelLabels[i]));
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
