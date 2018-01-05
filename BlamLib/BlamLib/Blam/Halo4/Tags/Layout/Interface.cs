/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.Halo4.Tags
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
				Add(TI.Pad._24);
				Add(Integer = new TI.LongInteger());
				Add(Real = new TI.Real());
				Add(StringId = new TI.StringId());
				Add(Name = new TI.StringId());
				Add(Description = new TI.StringId());
			}
		};
		#endregion

		public text_value_pair_definition_group() : base(6)
		{
			Add(Parameter = new TI.Enum(TI.FieldType.LongEnum));
			Add(Title = new TI.StringId());
			Add(Description = new TI.StringId());
			Add(Type = new TI.Enum(TI.FieldType.ByteEnum));
			Add(TI.Pad._24);
			Add(Values = new TI.Block<text_value_pair_reference_block>(this, 0));
		}
	};
	#endregion

	#region user_interface_globals_definition
	partial class user_interface_globals_definition_group
	{
		public user_interface_globals_definition_group() : base(23)
		{
			Add(/*shared globals = */ new TI.TagReference(this, TagGroups.wigl));
			Add(/*variant settings ui = */ new TI.TagReference(this, TagGroups.goof));
			Add(/*game hopper descriptions = */ new TI.TagReference(this, TagGroups.unic));
			Add(TI.Pad.BlockHalo3); // screen widgets
				// string id
				// cusc
			// only saw these used in ui\main_menu
			Add(TI.Pad.BlockHalo3);
			Add(TI.Pad.BlockHalo3);
			Add(TI.Pad.BlockHalo3);
			Add(TI.Pad.BlockHalo3);
			Add(/*cookie purchase globals = */ new TI.TagReference(this, TagGroups.cpgd));
			Add(/*infinity images = */ new TI.TagReference(this, TagGroups.iuii));
			Add(TI.Pad.BlockHalo3); // only saw this used in ui\main_menu
			Add(/*enemy to category mapping = */ new TI.TagReference(this, TagGroups.pcec));
			Add(/*damage type image mapping = */ new TI.TagReference(this, TagGroups.pdti));
			Add(TI.Pad.BlockHalo3);
			Add(TI.Pad.DWord); // real
			// didn't see any of these used in mainmenu tags
			Add(TI.Pad.BlockHalo3);
			Add(TI.Pad.BlockHalo3);
			Add(TI.Pad.BlockHalo3);
			Add(TI.Pad.BlockHalo3);
			Add(TI.Pad.BlockHalo3);
			Add(/*hud globals = */ new TI.TagReference(this, TagGroups.uihg));
			Add(/*portrait poses = */ new TI.TagReference(this, TagGroups.ppod));
			Add(TI.Pad.BlockHalo3);
		}
	};
	#endregion

	#region user_interface_shared_globals_definition
	partial class user_interface_shared_globals_definition_group
	{
		public user_interface_shared_globals_definition_group() : base(39)
		{
			Add(/*inc. text update period = */ new TI.ShortInteger());
			Add(/*inc. text block character = */ new TI.ShortInteger());
			Add(/*near clip plane distance = */ new TI.Real());
			Add(/*projection plane distance = */ new TI.Real());
			Add(/*far clip plane distance = */ new TI.Real());
			Add(/*unicode string list tag = */ new TI.TagReference(this, TagGroups.unic));
			Add(/*damage types = */ new TI.TagReference(this, TagGroups.unic));
			Add(/*fireteam member names = */ new TI.TagReference(this, TagGroups.unic));
			Add(/*fireteam member service tags = */ new TI.TagReference(this, TagGroups.unic));
			Add(/*main menu music = */ new TI.TagReference(this, TagGroups.lsnd));
			Add(/* = */ new TI.TagReference(this));
			Add(/*music fade time = */ new TI.LongInteger());
			Add(/* = */ new TI.RealColor(TI.FieldType.RealArgbColor));
			Add(/* = */ new TI.RealColor(TI.FieldType.RealArgbColor));
			Add(TI.Pad.BlockHalo3); // text colors?
				// string id
				// real_argb_color
			Add(TI.Pad.BlockHalo3);
				// tag block
					// real[4]
				// tag block
				// tag block
			Add(TI.Pad.DWord); // argb_color?
			Add(/*sounds = */ new TI.TagReference(this, TagGroups.uise));
			Add(TI.Pad.BlockHalo3); // alerts
				// string id, category name
				// byte flags?
				// byte default button?
				// byte_flags
				// PAD8
				// string id, title
				// string id, message
			Add(TI.Pad.BlockHalo3); // dialogs
				// string id, name
				// dword, only seen as zero
				// string id, title
				// string id, same as name (message?)
				// string id, button1 name
				// string id, button2 name
				// string id, button3 name
				// string id, button4 name
				// string id, button key (default?)
				// short (enum?)
				// short (flags?)
			Add(TI.Pad.DWord); // real
			Add(TI.Pad.DWord); // real
			Add(TI.Pad.DWord); // real
			Add(TI.Pad.DWord); // real
			Add(TI.Pad.DWord); // real
			Add(TI.Pad.DWord); // real
			Add(TI.Pad.DWord); // string id, chief
			Add(new TI.Pad(0x24)); // only seen as zeros
			Add(TI.Pad.DWord); // string id, elite
			Add(new TI.Pad(0x24)); // only seen as zeros
			Add(TI.Pad.DWord); // string id, chief portrait
			Add(TI.Pad.DWord); // string id, elite
			Add(TI.Pad.DWord);
			Add(TI.Pad.DWord);
			Add(TI.Pad.DWord); // seems to be the same value as the data's size
			Add(/* = */ new TI.Data(this));
			Add(TI.Pad.DWord);
			Add(TI.Pad.DWord);
			Add(TI.Pad.BlockHalo3); // haven't seen used
		}
	};
	#endregion
}