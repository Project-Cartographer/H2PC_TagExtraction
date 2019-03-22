/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.HaloReach.Tags
{
	#region multiplayer_variant_settings_interface_definition
	partial class multiplayer_variant_settings_interface_definition_group
	{
		#region multiplayer_variant_settings_interface_definition_0_block
		partial class multiplayer_variant_settings_interface_definition_0_block
		{
			#region unknown_4_block
			partial class unknown_4_block
			{
				public unknown_4_block() : base(6)
				{
					Add(Settings = new TI.TagReference(this, TagGroups.goof));
					Add(Template = new TI.TagReference(this, TagGroups.goof));
					Add(Unknown20 = new TI.Enum(TI.FieldType.LongEnum));
					Add(Title = new TI.StringId());
					Add(Description = new TI.StringId());
					Add(ValuePairs = new TI.TagReference(this, TagGroups.sily));
				}
			}
			#endregion

			public multiplayer_variant_settings_interface_definition_0_block() : base(2)
			{
				Add(Name = new TI.StringId());
				Add(SettingCategory = new TI.Enum(TI.FieldType.LongEnum));
				Add(Options = new TI.Block<unknown_4_block>(this, 0));
			}
		};
		#endregion

		public multiplayer_variant_settings_interface_definition_group() : base(2)
		{
			Add(Unknown0 = new TI.LongInteger());
			Add(Categories = new TI.Block<multiplayer_variant_settings_interface_definition_0_block>(this, 0));
		}
	};
	#endregion

	#region text_value_pair_definition_group
	partial class text_value_pair_definition_group
	{
		#region text_value_pair_reference_block
		partial class text_value_pair_reference_block
		{
			public text_value_pair_reference_block() : base(7)
			{
				Add(Flags = new TI.Flags(TI.FieldType.ByteFlags));
				// 0 = integer
				// 1 = string id
				Add(Type = new TI.Enum(TI.FieldType.ByteEnum));
				Add(TI.Pad.Word);
				Add(Integer = new TI.LongInteger());
				Add(StringId = new TI.StringId());
				Add(Name = new TI.StringId());
				Add(Description = new TI.StringId());
			}
		};
		#endregion

		public text_value_pair_definition_group() : base(4)
		{
			Add(Parameter = new TI.Enum(TI.FieldType.LongEnum));
			Add(Title = new TI.StringId());
			Add(Description = new TI.StringId());
			Add(Values = new TI.Block<text_value_pair_reference_block>(this, 0));
		}
	};
	#endregion
}