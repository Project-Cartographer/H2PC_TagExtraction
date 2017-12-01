/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using System.Collections.Generic;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.HaloReach.Tags
{
	#region multilingual_unicode_string_list
	[TI.TagGroup((int)TagGroups.Enumerated.unic, -1, 88)]
	public partial class multilingual_unicode_string_list_group : TI.Definition, Cache.IMultilingualUnicodeStringList
	{
		#region multilingual_unicode_string_reference_block
		[TI.Definition(-1, 72)]
		public partial class multilingual_unicode_string_reference_block : TI.Definition, Cache.IMultilingualUnicodeStringReference
		{
			public TI.StringId StringId;
			public TI.LongInteger[] LanguageOffsets;

			#region IMultilingualUnicodeStringReference Members
			TI.StringId Cache.IMultilingualUnicodeStringReference.Name
			{ get { return StringId; } }
			TI.LongInteger[] Cache.IMultilingualUnicodeStringReference.LanguageOffsets
			{ get { return LanguageOffsets; } }
			#endregion
		}
		#endregion

		#region multilingual_unicode_string_list_C_block
		[TI.Definition(-1, 8)]
		public partial class multilingual_unicode_string_list_C_block : TI.Definition
		{
			public TI.StringId Component, Property;
		}
		#endregion

		public TI.Block<multilingual_unicode_string_reference_block> StringRefs;
		public TI.Block<multilingual_unicode_string_list_C_block> BlockC;
		public TI.Data StringData;
		public TI.LongInteger[] LanguageHandles;

		#region IMultilingualUnicodeStringList Members
		Cache.IMultilingualUnicodeStringReference Cache.IMultilingualUnicodeStringList.NewReference()
		{
			multilingual_unicode_string_reference_block sr;
			StringRefs.Add(out sr);

			return sr;
		}

		Cache.IMultilingualUnicodeStringReference Cache.IMultilingualUnicodeStringList.GetReference(int element_index)
		{
			return StringRefs[element_index];
		}

		IEnumerable<Cache.IMultilingualUnicodeStringReference> Cache.IMultilingualUnicodeStringList.StringRefs
		{ get { foreach(var sr in StringRefs) yield return sr; } }
		TI.Data Cache.IMultilingualUnicodeStringList.StringData
		{ get { return StringData; } }
		TI.LongInteger[] Cache.IMultilingualUnicodeStringList.LanguageHandles
		{ get { return LanguageHandles; } }
		#endregion
	};
	#endregion
}