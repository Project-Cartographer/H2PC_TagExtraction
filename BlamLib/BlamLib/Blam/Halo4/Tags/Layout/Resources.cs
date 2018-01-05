/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.Halo4.Tags
{
	#region multilingual_unicode_string_list
	partial class multilingual_unicode_string_list_group
	{
		#region multilingual_unicode_string_reference_block
		partial class multilingual_unicode_string_reference_block
		{
			public multilingual_unicode_string_reference_block() : base(18)
			{
				Add(StringId = new TI.StringId());

				LanguageOffsets = new TI.LongInteger[(int)LanguageType.kMax];
				for (int x = 0; x < LanguageOffsets.Length; x++)
					Add(LanguageOffsets[x] = new TI.LongInteger());
			}
		}
		#endregion

		#region multilingual_unicode_string_list_C_block
		partial class multilingual_unicode_string_list_C_block
		{
			public multilingual_unicode_string_list_C_block() : base(3)
			{
				Add(Component = new TI.StringId());
				Add(Property = new TI.StringId());
				Add(Unknown8 = new TI.LongInteger());
			}
		}
		#endregion

		public multilingual_unicode_string_list_group() : base(20)
		{
			Add(StringRefs = new TI.Block<multilingual_unicode_string_reference_block>(this, 0));
			Add(BlockC = new TI.Block<multilingual_unicode_string_list_C_block>(this, 0));
			Add(StringData = new TI.Data(this));

			LanguageHandles = new TI.LongInteger[(int)LanguageType.kMax];
			for (int x = 0; x < LanguageHandles.Length; x++)
				Add(LanguageHandles[x] = new TI.LongInteger());
		}
	};
	#endregion
}