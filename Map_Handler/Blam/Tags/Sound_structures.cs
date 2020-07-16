using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace Map_Handler.Blam.Tags.sound
{
    /// <summary>
    /// Structures originaly from blackdimund's sound injection application and modified by me as per the usage
    /// </summary>
    class snd_tag
    {
        public struct SndTag
        {
            public Int16 Flags;
            public SByte SoundClass;
            public SByte SampleRate;
            public SByte Encoding;
            public SByte Compression;
            public Int16 PlaybackParameterIndex;
            public Int16 PitchRangeIndex;
            public SByte PitchRangeCount;
            public SByte ScaleIndex;
            public SByte PromotionIndex;
            public SByte CustomPlaybackIndex;
            public Int16 ExtraInfoIndex;
            public Int32 MaximumPlayTime;
        }
        public SndTag header;

        public snd_tag()
        {
            header.PlaybackParameterIndex = -1;
            header.PitchRangeIndex = -1;
            header.PitchRangeCount = 0;
            header.ScaleIndex = -1;
            header.PromotionIndex = -1;
            header.CustomPlaybackIndex = -1;
            header.ExtraInfoIndex = -1;
        }
        public snd_tag(snd_tag other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<SndTag>(t);
        }
        public snd_tag(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<SndTag>(t);
        }
        /// <summary>
        /// opens a snd tag of given index
        /// </summary>
        /// <param name="datum_index"></param>
        /// <param name="map_stream"></param>
        /// <returns></returns>
        public static snd_tag open_sound_tag_from_map_stream(int datum_index, ref StreamReader map_stream)
        {
            int tag_offset_mask = DATA_READ.ReadINT_LE(0x20, map_stream);
            int table_off = DATA_READ.ReadINT_LE(0x10, map_stream);
            int table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4, map_stream) + 0x20;

            int tag_mem_offset = DATA_READ.ReadINT_LE(table_start + 0x10 * (datum_index & 0xFFFF) + 0x8, map_stream);
            int file_offset = table_off + tag_mem_offset - tag_offset_mask;

            StreamReader[] sr_array = new StreamReader[0x1];
            sr_array[0] = map_stream;

            return new snd_tag(file_offset, tag_mem_offset, ref sr_array);
        }
    }
    class playback_parameters_block
    {
        public struct PlaybackParameters
        {
            public float minimumDistance;
            public float maximumDistance;
            public float skipFraction;
            public float maximumBlendPerSecond;
            public float gainBase;
            public float gainVariance;
            public short randomPitchBoundsmin;
            public short randomPitchBoundsmax;
            public float innerConeAngle;
            public float outerConeAngle;
            public float outerConeGain;
            public int flags;
            public float positionalGain;
            public float firstPersonGain;
            private int pad;
        }
        public PlaybackParameters header;

        public playback_parameters_block(playback_parameters_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<PlaybackParameters>(t);
        }
        public playback_parameters_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<PlaybackParameters>(t);
        }
    }
    class scales_block
    {
        public struct Scales
        {
            public float gainModifiermin;
            public float gainModifiermax;
            public short pitchModifiermin;
            public short pitchModifiermax;
            public float skipFractionModifiermin;
            public float skipFractionModifiermax;
        }
        public Scales header;

        public scales_block(scales_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<Scales>(t);
        }
        public scales_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<Scales>(t);
        }
    }
    class import_names_block
    {
        public struct ImportNames
        {
            public int string_ID;
        }
        public ImportNames header;

        public import_names_block(import_names_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<ImportNames>(t);
        }
        public import_names_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<ImportNames>(t);
        }
    }
    class pitch_range_parameters_block
    {
        public struct PitchRangeParameters
        {
            public short naturalPitch;
            public short blendBoundsmin;
            public short blendBoundsmax;
            public short maxGainPitchBoundsmin;
            public short maxGainPitchBoundsmax;
        }
        public PitchRangeParameters header;

        public pitch_range_parameters_block(pitch_range_parameters_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<PitchRangeParameters>(t);
        }
        public pitch_range_parameters_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<PitchRangeParameters>(t);
        }
    }
    class pitch_ranges_block
    {
        public struct PitchRanges
        {
            public Int16 ImportNameIndex;
            public Int16 PitchRangeParameterIndex;
            public Int16 EncodedPermutationDataIndex;
            public Int16 EncodedRuntimePermutationFlags;
            public Int16 FirstPermutation;
            public Int16 PermutationCount;
        }
        public PitchRanges header;

        public pitch_ranges_block(pitch_ranges_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<PitchRanges>(t);
        }
        public pitch_ranges_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<PitchRanges>(t);
        }
    }
    class permutations_block
    {
        public struct Permutations
        {
            public Int16 ImportNameIndex;
            public Int16 EncodedSkipFraction;
            public Byte Gain;
            public Byte PermutationInfoIndex;
            public Int16 LanguageNeutralTime;
            public UInt32 SampleSize;
            public Int16 FirstChunk;
            public Int16 ChunkCount;
        }
        public Permutations header;

        public permutations_block(permutations_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<Permutations>(t);
        }
        public permutations_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<Permutations>(t);
        }
    }
    class filter_block
    {
        public struct Filter
        {
            public int filtertype;
            public float filterWidth;
            public float leftFilterFrequenceScaleBoundsmin;
            public float leftFilterFrequenceScaleBoundsmax;
            public float leftFilterFrequenceRandomBase;
            public float leftFilterFrequenceRandomVariance;
            public float leftFilterGainScaleBoundsmin;
            public float leftFilterGainScaleBoundsmax;
            public float leftFilterGainRandomBase;
            public float leftFilterGainRandomVariance;
            public float rightFilterFrequenceScaleBoundsmin;
            public float rightFilterFrequenceScaleBoundsmax;
            public float rightFilterFrequenceRandomBase;
            public float rightFilterFrequenceRandomVariance;
            public float rightFilterGainScaleBoundsmin;
            public float rightFilterGainScaleBoundsmax;
            public float rightFilterGainRandomBase;
            public float rightFilterGainRandomVariance;
        }
        public Filter header;

        public filter_block(filter_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<Filter>(t);
        }
        public filter_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<Filter>(t);
        }
    }
    class custom_playback_block
    {
        public struct CustomPlaybacks
        {
            public int fLags;
            private int pad0;
            private int pad1;
            private int pad2;
            private int pad4;
            public UInt32 FilterCount;
            public UInt32 FilterMemAddr;
            private int pad5;
            private int pad6;
            private int pad7;
            private int pad8;
            private int pad9;
            private int pad10;
        }
        public CustomPlaybacks header; 
        public List<filter_block> list_filter;

        public custom_playback_block(custom_playback_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<CustomPlaybacks>(t);
            list_filter = new List<filter_block>();
            for (int i = 0; i < other.list_filter.Count; i++)
                list_filter.Add(new filter_block(other.list_filter[i]));
        }
        public custom_playback_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        { 
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<CustomPlaybacks>(t);

            list_filter = new List<filter_block>();

            for (int i = 0; i < header.FilterCount; i++)
            {
                long t_block_file_offset = file_offset + header.FilterMemAddr + i * Marshal.SizeOf<filter_block.Filter>() - block_memory_addr;
                long t_block_memory_addr = header.FilterMemAddr + i * Marshal.SizeOf<filter_block.Filter>();
                filter_block t_block = new filter_block(t_block_file_offset, t_block_memory_addr, ref file_stream);

                list_filter.Add(t_block);
            }
        }
        public byte[] get_child_block(int rebase_mem_offset)
        {
            byte[] ret = new byte[list_filter.Count * Marshal.SizeOf<filter_block.Filter>()];
            int start_offset = 0x0;

            header.FilterCount = (uint)list_filter.Count;
            header.FilterMemAddr = (uint)(start_offset + rebase_mem_offset);
            for (int i = 0; i < list_filter.Count; i++)
            {
                var t = DATA_READ.StructureToByteArray(list_filter[i].header);
                DATA_READ.ArrayCpy(ret, t, start_offset, t.Length);

                start_offset += t.Length;
            }

            return ret;
        }
    }
    class runtime_permutation_flags_block
    {
        public struct RuntimePermutationFlags
        {
            public byte unk;
        }
        public RuntimePermutationFlags header;

        public runtime_permutation_flags_block(runtime_permutation_flags_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<RuntimePermutationFlags>(t);
        }
        public runtime_permutation_flags_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<RuntimePermutationFlags>(t);
        }
    }
    class permutation_chunk_block
    {
        public struct PermutationChunks
        {
            public UInt32 FileOffset;
            public UInt16 ChunkSize;
            public SByte Unknown0;
            public SByte Unknown1;
            public Int32 RuntimeIndex;
        }
        public PermutationChunks header;
        public byte[] raw_data;

        public permutation_chunk_block(permutation_chunk_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<PermutationChunks>(t);
            raw_data = new byte[other.raw_data.Length];
            DATA_READ.ArrayCpy(raw_data, other.raw_data, 0x0, raw_data.Length);
        }
        public permutation_chunk_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<PermutationChunks>(t);

            raw_data = new byte[header.ChunkSize];

            StreamReader sr;
            uint block_off = header.FileOffset & 0x3FFFFFFF;
            switch (get_resource_type((uint)header.FileOffset))
            {
                case 0:
                    sr = file_stream[0];
                    sr.BaseStream.Seek(block_off, SeekOrigin.Begin);
                    sr.BaseStream.Read(raw_data, 0x0, header.ChunkSize);
                    break;
                case 1:
                    if (file_stream.Length > 1)
                    {
                        sr = file_stream[1];//mainmenu stream
                        sr.BaseStream.Seek(block_off, SeekOrigin.Begin);
                        sr.BaseStream.Read(raw_data, 0x0, header.ChunkSize);
                    }
                    else
                        raw_data = new byte[0x0];
                    break;
                case 2:
                    if (file_stream.Length > 2)
                    {
                        sr = file_stream[2];//mp shared stream
                        sr.BaseStream.Seek(block_off, SeekOrigin.Begin);
                        sr.BaseStream.Read(raw_data, 0x0, header.ChunkSize);
                    }
                    else
                        raw_data = new byte[0x0];
                    break;
                case 3:
                    if (file_stream.Length > 3)
                    {
                        sr = file_stream[3];//sp shared stream
                        sr.BaseStream.Seek(block_off, SeekOrigin.Begin);
                        sr.BaseStream.Read(raw_data, 0x0, header.ChunkSize);
                    }
                    else
                        raw_data = new byte[0x0];
                    break;
            }
        }
        public byte[] get_raw_block(int rebase_file_offset)
        {
            header.ChunkSize = (ushort)raw_data.Length;
            header.FileOffset = (uint)rebase_file_offset;

            return raw_data;
        }
        private uint get_resource_type(uint block_off)
        {
            uint ret = block_off >> 30;
            return ret;
        }
    }
    class rules_block
    {
        public struct Rules
        {
            public short pitchRangeIndex;
            public short maximumPlayingCount;
            public float suppressionTime;
            private int pad0;
            private int pad1;
        }
        public Rules header;

        public rules_block(rules_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<Rules>(t);
        }
        public rules_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<Rules>(t);
        }
    }
    class runtime_timers_block
    {
        public struct RuntimeTimers
        {
            public int unk;
        }
        public RuntimeTimers header;

        public runtime_timers_block(runtime_timers_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<RuntimeTimers>(t);
        }
        public runtime_timers_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<RuntimeTimers>(t);
        }
    }
    class promotions_block
    {
        public struct Promotions
        {
            public UInt32 PromotionRulesCount;
            public UInt32 PromotionRulesAddr;
            public UInt32 RuntimeTimersCount;
            public UInt32 RuntimeTimersAddr;
            private int pad0;
            private int pad1;
            private int pad2;
        }
        public Promotions header;
        public List<rules_block> list_rules;
        public List<runtime_timers_block> list_runtime_timers;

        public promotions_block(promotions_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<Promotions>(t);
            list_rules = new List<rules_block>();
            for (int i = 0; i < other.list_rules.Count; i++)
                list_rules.Add(new rules_block(other.list_rules[i]));
            list_runtime_timers = new List<runtime_timers_block>();
            for (int i = 0; i < other.list_runtime_timers.Count; i++)
                list_runtime_timers.Add(new runtime_timers_block(other.list_runtime_timers[i]));
        }
        public promotions_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<Promotions>(t);

            list_rules = new List<rules_block>();
            list_runtime_timers = new List<runtime_timers_block>();

            for (int i = 0; i < header.PromotionRulesCount; i++)
            {
                long t_block_file_offset = file_offset + header.PromotionRulesAddr + i * Marshal.SizeOf<rules_block.Rules>() - block_memory_addr;
                long t_block_memory_addr = header.PromotionRulesAddr + i * Marshal.SizeOf<rules_block.Rules>();
                rules_block t_block = new rules_block(t_block_file_offset, t_block_memory_addr, ref file_stream);

                list_rules.Add(t_block);
            }
            for (int i = 0; i < header.RuntimeTimersCount; i++)
            {
                long t_block_file_offset = file_offset + header.RuntimeTimersAddr + i * Marshal.SizeOf<runtime_timers_block.RuntimeTimers>() - block_memory_addr;
                long t_block_memory_addr = header.RuntimeTimersAddr + i * Marshal.SizeOf<runtime_timers_block.RuntimeTimers>();
                runtime_timers_block t_block = new runtime_timers_block(t_block_file_offset, t_block_memory_addr, ref file_stream);

                list_runtime_timers.Add(t_block);
            }
        }
        public byte[] get_child_block(int rebase_mem_offset)
        {
            byte[] ret = new byte[list_rules.Count * Marshal.SizeOf<rules_block.Rules>() + list_runtime_timers.Count * Marshal.SizeOf<runtime_timers_block.RuntimeTimers>()];
            int start_offset = 0x0;

            header.PromotionRulesCount = (uint)list_rules.Count;
            header.PromotionRulesAddr = (uint)(start_offset + rebase_mem_offset);
            for (int i = 0; i < list_rules.Count; i++)
            {
                var t = DATA_READ.StructureToByteArray(list_rules[i].header);
                DATA_READ.ArrayCpy(ret, t, start_offset, t.Length);

                start_offset += t.Length;
            }
            header.RuntimeTimersCount = (uint)list_runtime_timers.Count;
            header.RuntimeTimersAddr = (uint)(start_offset + rebase_mem_offset);
            for (int i = 0; i < list_runtime_timers.Count; i++)
            {
                var t = DATA_READ.StructureToByteArray(list_runtime_timers[i].header);
                DATA_READ.ArrayCpy(ret, t, start_offset, t.Length);

                start_offset += t.Length;
            }

            return ret;
        }
    }
    class resources_block
    {
        public struct Resources
        {
            public byte type;
            private byte pad0;
            private short pad1;
            public short PrimaryLocator;
            public short SecondaryLocator;
            public uint ResourceDataSize;
            public uint ResourceDataOffset;
        }
        public Resources header;

        public resources_block(resources_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<Resources>(t);
        }
        public resources_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<Resources>(t);
        }
    }
    class extra_info_block
    {
        public struct ExtraInfos
        {
            private int pad0;
            private int pad1;
            public int blockoffset;
            public int blocksize;
            public uint sectiondatasize;
            public uint Resourcedatasize;
            public UInt32 ResourcesCount;
            public UInt32 ResoucesMemAddr;
            public int SoundGestaltDatumIndex;
            private int pad3;
            private int pad4;
        }
        public ExtraInfos header;
        public byte[] raw_data;
        public List<resources_block> list_resources;

        public extra_info_block(extra_info_block other)
        {
            var t = DATA_READ.StructureToByteArray(other.header);
            header = DATA_READ.BytesToStructure<ExtraInfos>(t);
            raw_data = new byte[other.raw_data.Length];
            DATA_READ.ArrayCpy(raw_data, other.raw_data, 0x0, raw_data.Length);
            list_resources = new List<resources_block>();
            for (int i = 0; i < other.list_resources.Count; i++)
                list_resources.Add(new resources_block(other.list_resources[i]));
        }
        public extra_info_block(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream[0].BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream[0].BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<ExtraInfos>(t);

            raw_data = new byte[header.blocksize];

            StreamReader sr;//map stream
            int block_off = header.blockoffset & 0x3FFFFFFF;
            switch (get_resource_type((uint)header.blockoffset))
            {
                case 0:
                    sr = file_stream[0];
                    sr.BaseStream.Seek(block_off, SeekOrigin.Begin);
                    sr.BaseStream.Read(raw_data, 0x0, header.blocksize);
                    break;
                case 1:
                    if (file_stream.Length > 1)
                    {
                        sr = file_stream[1];//mainmenu stream
                        sr.BaseStream.Seek(block_off, SeekOrigin.Begin);
                        sr.BaseStream.Read(raw_data, 0x0, header.blocksize);
                    }
                    else
                        raw_data = new byte[0x0];
                    break;
                case 2:
                    if (file_stream.Length > 2)
                    {
                        sr = file_stream[2];//mp shared stream
                        sr.BaseStream.Seek(block_off, SeekOrigin.Begin);
                        sr.BaseStream.Read(raw_data, 0x0, header.blocksize);
                    }
                    else
                        raw_data = new byte[0x0];
                    break;
                case 3:
                    if (file_stream.Length > 3)
                    {
                        sr = file_stream[3];//sp shared stream
                        sr.BaseStream.Seek(block_off, SeekOrigin.Begin);
                        sr.BaseStream.Read(raw_data, 0x0, header.blocksize);
                    }
                    else
                        raw_data = new byte[0x0];
                    break;
            }


            list_resources = new List<resources_block>();

            for (int i = 0; i < header.ResourcesCount; i++)
            {
                long t_block_file_offset = file_offset + header.ResoucesMemAddr + i * Marshal.SizeOf<resources_block.Resources>() - block_memory_addr;
                long t_block_memory_addr = header.ResoucesMemAddr + i * Marshal.SizeOf<resources_block.Resources>();
                resources_block t_block = new resources_block(t_block_file_offset, t_block_memory_addr, ref file_stream);

                list_resources.Add(t_block);
            }
        }
        public byte[] get_child_block(int rebase_mem_offset)
        {
            byte[] ret = new byte[list_resources.Count * Marshal.SizeOf<resources_block.Resources>()];
            int start_offset = 0x0;

            header.ResourcesCount = (uint)list_resources.Count;
            header.ResoucesMemAddr = (uint)(start_offset + rebase_mem_offset);
            for (int i=0;i<list_resources.Count;i++)
            {
                var t = DATA_READ.StructureToByteArray(list_resources[i].header);
                DATA_READ.ArrayCpy(ret, t, start_offset, t.Length);

                start_offset += t.Length;
            }

            return ret;
        }
        public byte[] get_raw_block(int rebase_file_offset)
        {
            header.blockoffset = rebase_file_offset;
            header.blocksize = raw_data.Length;

            return raw_data;
        }
        private uint get_resource_type(uint block_off)
        {
            uint ret = block_off >> 30;
            return ret;
        }
    }
    /// <summary>
    /// sound gestalt class implementation using some of the structs from black dimund's
    /// </summary>
    class sound_gestalt_tag
    {
        public struct UghTagStruct
        {
            public UInt32 PlaybackParametersCount;
            public UInt32 PlaybackParametersMemAddr;
            public UInt32 ScalesCount;
            public UInt32 ScalesMemAddr;
            public UInt32 ImportNamesCount;
            public UInt32 ImportNamesMemAddr;
            public UInt32 PitchRangeParametersCount;
            public UInt32 PitchRangeParametersMemAddr;
            public UInt32 PitchRangesCount;
            public UInt32 PitchRangesMemAddr;
            public UInt32 PermutationsCount;
            public UInt32 PermutationsMemAddr;
            public UInt32 CustomPlaybacksCount;
            public UInt32 CustomPlaybacksMemAddr;
            public UInt32 RuntimePermutationFlagsCount;
            public UInt32 RuntimePermutationFlagsMemAddr;
            public UInt32 PermutationChunksCount;
            public UInt32 PermutationChunksMemAddr;
            public UInt32 PromotionsCount;
            public UInt32 PromotionsMemAddr;
            public UInt32 ExtraInfoCount;
            public UInt32 ExtraInfoMemAddr;
        }
        public UghTagStruct header;
        public List<playback_parameters_block> list_playback_parameters;
        public List<scales_block> list_scales;
        public List<import_names_block> list_import_names;
        public List<pitch_ranges_block> list_pitch_range;
        public List<pitch_range_parameters_block> list_pitch_range_parameters;
        public List<permutations_block> list_permutation;
        public List<custom_playback_block> list_custom_playback;
        public List<runtime_permutation_flags_block> list_runtime_permutation_flags;
        public List<permutation_chunk_block> list_permutation_chunk;
        public List<promotions_block> list_promotions;
        public List<extra_info_block> list_extra_info;


        public sound_gestalt_tag()
        {           
            init_child_blocks();
        }
        public sound_gestalt_tag(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            read_header(file_offset, block_memory_addr,ref file_stream[0]);
            init_child_blocks();
            read_child_blocks(file_offset, block_memory_addr,ref file_stream);
        }
        private void read_header(long file_offset, long block_memory_addr,ref StreamReader file_stream)
        {
            byte[] t = new byte[Marshal.SizeOf(header)];
            file_stream.BaseStream.Seek(file_offset, SeekOrigin.Begin);
            file_stream.BaseStream.Read(t, 0x0, t.Length);
            header = DATA_READ.BytesToStructure<UghTagStruct>(t);
        }
        private void init_child_blocks()
        {
            list_playback_parameters = new List<playback_parameters_block>();
            list_scales = new List<scales_block>();
            list_import_names = new List<import_names_block>();
            list_pitch_range = new List<pitch_ranges_block>();
            list_pitch_range_parameters = new List<pitch_range_parameters_block>();
            list_permutation = new List<permutations_block>();
            list_custom_playback = new List<custom_playback_block>();
            list_runtime_permutation_flags = new List<runtime_permutation_flags_block>();
            list_permutation_chunk = new List<permutation_chunk_block>();
            list_promotions = new List<promotions_block>();
            list_extra_info = new List<extra_info_block>();
        }
        private void read_child_blocks(long file_offset, long block_memory_addr,ref StreamReader[] file_stream)
        {
            for (int i = 0; i < header.PlaybackParametersCount; i++)
            {
                long t_block_file_offset = file_offset + header.PlaybackParametersMemAddr + i * Marshal.SizeOf<playback_parameters_block.PlaybackParameters>() - block_memory_addr;
                long t_block_memory_addr = header.PlaybackParametersMemAddr + i * Marshal.SizeOf<playback_parameters_block.PlaybackParameters>();
                playback_parameters_block t_block = new playback_parameters_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_playback_parameters.Add(t_block);
            }
            for (int i = 0; i < header.ScalesCount; i++)
            {
                long t_block_file_offset = file_offset + header.ScalesMemAddr + i * Marshal.SizeOf<scales_block.Scales>() - block_memory_addr;
                long t_block_memory_addr = header.ScalesMemAddr + i * Marshal.SizeOf<scales_block.Scales>();
                scales_block t_block = new scales_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_scales.Add(t_block);
            }
            for (int i = 0; i < header.ImportNamesCount; i++)
            {
                long t_block_file_offset = file_offset + header.ImportNamesMemAddr + i * Marshal.SizeOf<import_names_block.ImportNames>() - block_memory_addr;
                long t_block_memory_addr = header.ImportNamesMemAddr + i * Marshal.SizeOf<import_names_block.ImportNames>();
                import_names_block t_block = new import_names_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_import_names.Add(t_block);
            }
            for (int i = 0; i < header.PitchRangeParametersCount; i++)
            {
                long t_block_file_offset = file_offset + header.PitchRangeParametersMemAddr + i * Marshal.SizeOf<pitch_range_parameters_block.PitchRangeParameters>() - block_memory_addr;
                long t_block_memory_addr = header.PitchRangeParametersMemAddr + i * Marshal.SizeOf<pitch_range_parameters_block.PitchRangeParameters>();
                pitch_range_parameters_block t_block = new pitch_range_parameters_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_pitch_range_parameters.Add(t_block);
            }
            for (int i = 0; i < header.PitchRangesCount; i++)
            {
                long t_block_file_offset = file_offset + header.PitchRangesMemAddr + i * Marshal.SizeOf<pitch_ranges_block.PitchRanges>() - block_memory_addr;
                long t_block_memory_addr = header.PitchRangesMemAddr + i * Marshal.SizeOf<pitch_ranges_block.PitchRanges>();
                pitch_ranges_block t_block = new pitch_ranges_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_pitch_range.Add(t_block);
            }
            for (int i = 0; i < header.PermutationsCount; i++)
            {
                long t_block_file_offset = file_offset + header.PermutationsMemAddr + i * Marshal.SizeOf<permutations_block.Permutations>() - block_memory_addr;
                long t_block_memory_addr = header.PermutationsMemAddr + i * Marshal.SizeOf<permutations_block.Permutations>();
                permutations_block t_block = new permutations_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_permutation.Add(t_block);
            }
            for (int i = 0; i < header.CustomPlaybacksCount; i++)
            {
                long t_block_file_offset = file_offset + header.CustomPlaybacksMemAddr + i * Marshal.SizeOf<custom_playback_block.CustomPlaybacks>() - block_memory_addr;
                long t_block_memory_addr = header.CustomPlaybacksMemAddr + i * Marshal.SizeOf<custom_playback_block.CustomPlaybacks>();
                custom_playback_block t_block = new custom_playback_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_custom_playback.Add(t_block);
            }
            for (int i = 0; i < header.RuntimePermutationFlagsCount; i++)
            {
                long t_block_file_offset = file_offset + header.RuntimePermutationFlagsMemAddr + i * Marshal.SizeOf<runtime_permutation_flags_block.RuntimePermutationFlags>() - block_memory_addr;
                long t_block_memory_addr = header.RuntimePermutationFlagsMemAddr + i * Marshal.SizeOf<runtime_permutation_flags_block.RuntimePermutationFlags>();
                runtime_permutation_flags_block t_block = new runtime_permutation_flags_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_runtime_permutation_flags.Add(t_block);
            }
            for (int i = 0; i < header.PermutationChunksCount; i++)
            {
                long t_block_file_offset = file_offset + header.PermutationChunksMemAddr + i * Marshal.SizeOf<permutation_chunk_block.PermutationChunks>() - block_memory_addr;
                long t_block_memory_addr = header.PermutationChunksMemAddr + i * Marshal.SizeOf<permutation_chunk_block.PermutationChunks>();
                permutation_chunk_block t_block = new permutation_chunk_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_permutation_chunk.Add(t_block);
            }
            for (int i = 0; i < header.PromotionsCount; i++)
            {
                long t_block_file_offset = file_offset + header.PromotionsMemAddr + i * Marshal.SizeOf<promotions_block.Promotions>() - block_memory_addr;
                long t_block_memory_addr = header.PromotionsMemAddr + i * Marshal.SizeOf<promotions_block.Promotions>();
                promotions_block t_block = new promotions_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_promotions.Add(t_block);
            }
            for (int i = 0; i < header.ExtraInfoCount; i++)
            {
                long t_block_file_offset = file_offset + header.ExtraInfoMemAddr + i * Marshal.SizeOf<extra_info_block.ExtraInfos>() - block_memory_addr;
                long t_block_memory_addr = header.ExtraInfoMemAddr + i * Marshal.SizeOf<extra_info_block.ExtraInfos>();
                extra_info_block t_block = new extra_info_block(t_block_file_offset, t_block_memory_addr,ref file_stream);

                list_extra_info.Add(t_block);
            }
        }
        /// <summary>
        /// Function to extract sound chunk as per the snd tag into arg0
        /// for some reason also modifies the argument snd_tag
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="tag"></param>
        /// <returns>return the snd_tag as per the chunk generated</returns>
        public snd_tag add_to_gestalt_chunk_from_snd(ref sound_gestalt_tag dest, snd_tag tag)
        {
            snd_tag temp_tag = new snd_tag(tag);

            if (tag.header.PlaybackParameterIndex != -1)
            {
                temp_tag.header.PlaybackParameterIndex = (short)dest.list_playback_parameters.Count;

                var t_playback_param = list_playback_parameters[tag.header.PlaybackParameterIndex];
                dest.list_playback_parameters.Add(t_playback_param);
            }
            if (tag.header.PitchRangeIndex != -1)
            {
                temp_tag.header.PitchRangeIndex = (short)dest.list_pitch_range.Count;

                for (int i = 0; i < tag.header.PitchRangeCount; i++)
                {
                    var t_pitch_range = list_pitch_range[tag.header.PitchRangeIndex + i];
                    _add_pitch_ranges_block(t_pitch_range, ref dest);
                }
            }
            if(tag.header.ScaleIndex!=-1)
            {
                var t_scale_block = list_scales[tag.header.ScaleIndex];

                temp_tag.header.ScaleIndex = dest._add_scales_block(t_scale_block);
            }
            if(tag.header.PromotionIndex!=-1)
            {
                temp_tag.header.PromotionIndex = (sbyte)dest.list_promotions.Count;

                var t_promotions_block = list_promotions[tag.header.PromotionIndex];
                _add_promotions_block(t_promotions_block, dest);
            }
            if (tag.header.CustomPlaybackIndex!=-1)
            {
                temp_tag.header.CustomPlaybackIndex = (sbyte)dest.list_custom_playback.Count;

                var t_custom_playback_block = list_custom_playback[tag.header.CustomPlaybackIndex];
                dest.list_custom_playback.Add(t_custom_playback_block);
            }
            if(tag.header.ExtraInfoIndex!=-1)
            {
                temp_tag.header.ExtraInfoIndex = (short)dest.list_extra_info.Count;

                var t_extra_info_block = list_extra_info[tag.header.ExtraInfoIndex];
                dest.list_extra_info.Add(t_extra_info_block);
            }

            return temp_tag;
        }
        private void _add_pitch_ranges_block(pitch_ranges_block block, ref sound_gestalt_tag arg0)
        {
            pitch_ranges_block temp_block = new pitch_ranges_block(block);

            if (block.header.ImportNameIndex != -1 && false)
            {
                temp_block.header.ImportNameIndex = (short)arg0.list_import_names.Count;

                var t_import_names_block = list_import_names[block.header.ImportNameIndex];
                arg0.list_import_names.Add(t_import_names_block);
            }
            if (block.header.PitchRangeParameterIndex != -1)
            {
                temp_block.header.PitchRangeParameterIndex = (short)arg0.list_pitch_range_parameters.Count;

                var t_pitch_range_param_block = list_pitch_range_parameters[block.header.PitchRangeParameterIndex];
                arg0.list_pitch_range_parameters.Add(t_pitch_range_param_block);
            }
            if (block.header.EncodedRuntimePermutationFlags != -1 && false)
            {
                temp_block.header.EncodedRuntimePermutationFlags = (short)arg0.list_runtime_permutation_flags.Count;

                var t_runtime_perm_flag_block = list_runtime_permutation_flags[block.header.EncodedRuntimePermutationFlags];
                arg0.list_runtime_permutation_flags.Add(t_runtime_perm_flag_block);
            }
            if(block.header.FirstPermutation!=-1)
            {
                temp_block.header.FirstPermutation = (short)arg0.list_permutation.Count;

                for (int i = 0; i < block.header.PermutationCount; i++)
                {
                    var t_permutation_block = list_permutation[block.header.FirstPermutation + i];
                    _add_permutation_block(t_permutation_block,ref arg0);
                }
            }

            arg0.list_pitch_range.Add(temp_block);
        }
        private void _add_permutation_block(permutations_block block,ref sound_gestalt_tag arg0)
        {
            permutations_block temp_block = new permutations_block(block);

            if (block.header.ImportNameIndex != -1 && false)
            {
                temp_block.header.ImportNameIndex = (short)arg0.list_import_names.Count;

                var t_import_names_block = list_import_names[block.header.ImportNameIndex];
                arg0.list_import_names.Add(t_import_names_block);
            }
            if(block.header.FirstChunk!=-1)
            {
                temp_block.header.FirstChunk = (short)arg0.list_permutation_chunk.Count;

                for (int i = 0; i < block.header.ChunkCount; i++)
                {
                    var t_permutation_chunk = list_permutation_chunk[block.header.FirstChunk + i];
                    arg0.list_permutation_chunk.Add(t_permutation_chunk);
                }
            }

            arg0.list_permutation.Add(temp_block);
        }
        private void _add_promotions_block(promotions_block block,sound_gestalt_tag arg0)
        {
            promotions_block temp_block = new promotions_block(block);

            for (int i = 0; i < block.list_rules.Count; i++)
            {
                temp_block.list_rules[i].header.pitchRangeIndex = (short)arg0.list_pitch_range.Count;

                var t_pitch_ranges = list_pitch_range[block.list_rules[i].header.pitchRangeIndex];
                _add_pitch_ranges_block(t_pitch_ranges,ref arg0);

            }
            arg0.list_promotions.Add(temp_block);
        }
        private sbyte _add_scales_block(scales_block block)
        {
            for (int i = 0; i < list_scales.Count; i++)
            {
                var t = list_scales[i];
                if (t.header.gainModifiermax != block.header.gainModifiermax)
                    continue;
                if (t.header.gainModifiermin != block.header.gainModifiermin)
                    continue;
                if (t.header.pitchModifiermax != block.header.pitchModifiermax)
                    continue;
                if (t.header.pitchModifiermin != block.header.pitchModifiermin)
                    continue;
                if (t.header.skipFractionModifiermax != block.header.skipFractionModifiermax)
                    continue;
                if (t.header.skipFractionModifiermin != block.header.skipFractionModifiermin)
                    continue;

                return (sbyte)i;
            }
            list_scales.Add(block);
            return (sbyte)(list_scales.Count - 1);
        }
        /// <summary>
        /// return the meta data with rebased to the supplied mem_offset
        /// </summary>
        /// <returns></returns>
        public byte[] Generate_meta_data(int rebase_mem_offset)
        {
            List<byte[]> array_chunk = new List<byte[]>();
            List<byte[]> child_array_chunk = new List<byte[]>();

            int tag_offset = 0x0;//basically tag meta size
            tag_offset += Marshal.SizeOf(header);

            header.PlaybackParametersCount = (uint)list_playback_parameters.Count;
            header.PlaybackParametersMemAddr = (uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<playback_parameters_block.PlaybackParameters>() * list_playback_parameters.Count;
            foreach (var playback_param_iter in list_playback_parameters)
                array_chunk.Add(DATA_READ.StructureToByteArray(playback_param_iter.header));
            

            header.ScalesCount = (uint)list_scales.Count;
            header.ScalesMemAddr = (uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<scales_block.Scales>() * list_scales.Count;
            foreach (var scales_iter in list_scales)
                array_chunk.Add(DATA_READ.StructureToByteArray(scales_iter.header));


            header.ImportNamesCount = (uint)list_import_names.Count;
            header.ImportNamesMemAddr = (uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<import_names_block.ImportNames>() * list_import_names.Count;
            foreach (var inames_iter in list_import_names)
                array_chunk.Add(DATA_READ.StructureToByteArray(inames_iter.header));


            header.PitchRangeParametersCount = (uint)list_pitch_range_parameters.Count;
            header.PitchRangeParametersMemAddr = (uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<pitch_range_parameters_block.PitchRangeParameters>() * list_pitch_range_parameters.Count;
            foreach (var pitchrangeparam_iter in list_pitch_range_parameters)
                array_chunk.Add(DATA_READ.StructureToByteArray(pitchrangeparam_iter.header));


            header.PitchRangesCount = (uint)list_pitch_range.Count;
            header.PitchRangesMemAddr=(uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<pitch_ranges_block.PitchRanges>() * list_pitch_range.Count;
            foreach (var pitchrange_iter in list_pitch_range)
                array_chunk.Add(DATA_READ.StructureToByteArray(pitchrange_iter.header));


            header.PermutationsCount = (uint)list_permutation.Count;
            header.PermutationsMemAddr = (uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<permutations_block.Permutations>() * list_permutation.Count;
            foreach (var perm_iter in list_permutation)
                array_chunk.Add(DATA_READ.StructureToByteArray(perm_iter.header));


            //custom play backs,these nested blocks are a mess now, owing to my lack of foresight
            //nevertheless lets get the job done
            header.CustomPlaybacksCount = (uint)list_custom_playback.Count;
            header.CustomPlaybacksMemAddr = (uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<custom_playback_block.CustomPlaybacks>() * list_custom_playback.Count;
            foreach(var custom_iter in list_custom_playback)
            {
                var t_child_block = custom_iter.get_child_block(tag_offset + rebase_mem_offset);
                child_array_chunk.Add(t_child_block);
                array_chunk.Add(DATA_READ.StructureToByteArray(custom_iter.header));
                tag_offset += t_child_block.Length;
            }
            array_chunk.AddRange(child_array_chunk);
            child_array_chunk.Clear();


            header.RuntimePermutationFlagsCount = (uint)list_runtime_permutation_flags.Count;
            header.RuntimePermutationFlagsMemAddr = (uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<runtime_permutation_flags_block.RuntimePermutationFlags>() * list_runtime_permutation_flags.Count;
            foreach (var runtimepermflag_iter in list_runtime_permutation_flags)
                array_chunk.Add(DATA_READ.StructureToByteArray(runtimepermflag_iter.header));


            header.PermutationChunksCount = (uint)list_permutation_chunk.Count;
            header.PermutationChunksMemAddr = (uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<permutation_chunk_block.PermutationChunks>() * list_permutation_chunk.Count;
            foreach (var permchunk_iter in list_permutation_chunk)
                array_chunk.Add(DATA_READ.StructureToByteArray(permchunk_iter.header));


            header.PromotionsCount = (uint)list_promotions.Count;
            header.PromotionsMemAddr = (uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<promotions_block.Promotions>() * list_promotions.Count;
            foreach (var promotion_iter in list_promotions)
            {
                var t_runtime_block = promotion_iter.get_child_block(tag_offset + rebase_mem_offset);
                child_array_chunk.Add(t_runtime_block);
                array_chunk.Add(DATA_READ.StructureToByteArray(promotion_iter.header));
                tag_offset += t_runtime_block.Length;
            }
            array_chunk.AddRange(child_array_chunk);
            child_array_chunk.Clear();


            header.ExtraInfoCount = (uint)(list_extra_info.Count);
            header.ExtraInfoMemAddr = (uint)(tag_offset + rebase_mem_offset);
            tag_offset += Marshal.SizeOf<extra_info_block.ExtraInfos>() * list_extra_info.Count;
            foreach(var extrainfo_iter in list_extra_info)
            {
                var t_resource_block = extrainfo_iter.get_child_block(tag_offset + rebase_mem_offset);
                child_array_chunk.Add(t_resource_block);
                array_chunk.Add(DATA_READ.StructureToByteArray(extrainfo_iter.header));
                tag_offset += t_resource_block.Length;
            }
            array_chunk.AddRange(child_array_chunk);
            child_array_chunk.Clear();

            byte[] ret = new byte[tag_offset];
            int off = 0x0;
            byte[] t_header = DATA_READ.StructureToByteArray(header);
            DATA_READ.ArrayCpy(ret, t_header, off, t_header.Length);
            off += t_header.Length;
            foreach(var t_array_chunk in array_chunk)
            {
                DATA_READ.ArrayCpy(ret, t_array_chunk, off, t_array_chunk.Length);
                off += t_array_chunk.Length;
            }

            return ret;
        }
        /// <summary>
        /// returns the raw data with rebased to supplied file offset
        /// <CAUTION>Its important to generate raw prior to generating meta as subfields in the meta data refer to the RAW block
        /// and requires correction wrt to the newly offsetted raw blocks</CAUTION>
        /// </summary>
        /// <param name="rebase_file_offset"></param>
        /// <returns></returns>
        public byte[] Generate_raw_data(int rebase_file_offset)
        {
            List<byte[]> array_chunk = new List<byte[]>();

            int start_offset = 0x0;

            foreach(var t_permchunk in list_permutation_chunk)
            {
                byte[] t_raw = t_permchunk.get_raw_block(start_offset + rebase_file_offset);
                array_chunk.Add(t_raw);
                start_offset += t_raw.Length;
            }
            foreach(var t_extrainfo in list_extra_info)
            {
                byte[] t_raw = t_extrainfo.get_raw_block(start_offset + rebase_file_offset);
                array_chunk.Add(t_raw);
                start_offset += t_raw.Length;
            }
            byte[] ret = new byte[start_offset];
            int off = 0x0;
            foreach(var chunk in array_chunk)
            {
                DATA_READ.ArrayCpy(ret, chunk, off, chunk.Length);
                off += chunk.Length;
            }

            return ret;
        }
        /// <summary>
        /// i know its lame,but we need it
        /// </summary>
        /// <returns></returns>
        public int Get_meta_size()
        {
            var t = Generate_meta_data(0x0);
            return t.Length;
        }
        /// <summary>
        /// as the name suggests
        /// </summary>
        /// <returns></returns>
        public int Get_raw_size()
        {
            var t = Generate_raw_data(0x0);
            return t.Length;
        }
        /// <summary>
        /// Fixes the owner index mentioned in the extra info blocks
        /// </summary>
        /// <param name="sound_gestalt_new_index"></param>
        public void Update_extra_info_owner_index(int sound_gestalt_new_index)
        {
            foreach(var extrainfo_iter in list_extra_info)
            {
                extrainfo_iter.header.SoundGestaltDatumIndex = sound_gestalt_new_index;
            }
        }
        /// <summary>
        /// as the name suggests opens the gestalt of the particular map
        /// </summary>
        /// <param name="map_stream"></param>
        /// <returns></returns>
        public static sound_gestalt_tag open_sound_gestalt_tag_from_map_stream(ref StreamReader[] sr_array)
        {
            int tag_offset_mask = DATA_READ.ReadINT_LE(0x20, sr_array[0]);
            int table_off = DATA_READ.ReadINT_LE(0x10, sr_array[0]);
            int table_start = table_off + 0xC * DATA_READ.ReadINT_LE(table_off + 4, sr_array[0]) + 0x20;
            int tag_count = DATA_READ.ReadINT_LE(table_off + 0x18, sr_array[0]);

            int ugh_index = -1;
            for (int i = 0; i < tag_count; i++)
            {
                string type = DATA_READ.ReadTAG_TYPE(table_start + 0x10 * i, sr_array[0]);
                if (type.CompareTo("ugh!") == 0)
                {
                    ugh_index = i;
                    break;
                }
            }

            int tag_mem_offset = DATA_READ.ReadINT_LE(table_start + 0x10 * (ugh_index & 0xFFFF) + 0x8, sr_array[0]);
            int file_offset = table_off + tag_mem_offset - tag_offset_mask;

            sound_gestalt_tag ret = new sound_gestalt_tag(file_offset, tag_mem_offset, ref sr_array);
            return ret;
        }
    }

}
/*
 * AUTHOR'S NOTE:<yeah i typed in such a thing>
 * Himanshu01:The gestalt structure implementation may not be the best and i am quite sure that something better could be achieved.
 *            But i dont want to delay sound injection stuff and had to do with this somewhat crude implementation.
 */
