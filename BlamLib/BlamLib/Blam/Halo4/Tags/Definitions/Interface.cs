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
	[TI.TagGroup((int)TagGroups.Enumerated.goof, -1, 16)]
	public partial class multiplayer_variant_settings_interface_definition_group : TI.Definition
	{
		// variant_setting_edit_reference_block
		#region multiplayer_variant_settings_interface_definition_0_block
		[TI.Definition(-1, 20)]
		public partial class multiplayer_variant_settings_interface_definition_0_block : TI.Definition
		{
			// variant_setting_edit_reference_options_block
			#region unknown_4_block
			[TI.Definition(-1, 60)]
			public partial class unknown_4_block : TI.Definition
			{
				public TI.TagReference Settings, Template;
				public TI.Enum Unknown20;
				public TI.StringId Title, Description;
				public TI.TagReference ValuePairs;
			}
			#endregion

			public TI.StringId Name;
			public TI.Enum SettingCategory;
			public TI.Block<unknown_4_block> Options;
		};
		#endregion

		public TI.LongInteger Unknown0;
		public TI.Block<multiplayer_variant_settings_interface_definition_0_block> Categories;
	};
	#endregion

	// see: s_text_value_pair_blocks_block_UNUSED in user_interface_screen_widget_definition
	#region text_value_pair_definition_group
	[TI.TagGroup((int)TagGroups.Enumerated.sily, -1, 28)]
	public partial class text_value_pair_definition_group : TI.Definition
	{
		#region text_value_pair_reference_block
		[TI.Definition(-1, 24)]
		public partial class text_value_pair_reference_block : TI.Definition
		{
			public TI.Flags Flags;
			public TI.LongInteger Integer;
			public TI.Real Real;
			public TI.StringId StringId;
			public TI.StringId Name, Description;
		};
		#endregion

		public TI.Enum Parameter;
		public TI.StringId Title, Description;
		public TI.Enum Type;
		public TI.Block<text_value_pair_reference_block> Values;
	};
	#endregion

	#region user_interface_globals_definition
	[TI.TagGroup((int)TagGroups.Enumerated.wgtz, -1, 304)]
	public partial class user_interface_globals_definition_group : TI.Definition
	{
	};
	#endregion

	#region user_interface_shared_globals_definition
	[TI.TagGroup((int)TagGroups.Enumerated.wigl, -1, 380)]
	public partial class user_interface_shared_globals_definition_group : TI.Definition
	{
	};
	#endregion

	/*
<field type="LongEnum" name="parameter" />
<field type="StringId" name="title text" />
<field type="StringId" name="header text" />
<field type="StringId" name="description text" />
<field type="ByteFlags" name="flags?">	// or type
	// respawn time = 0
	// shields multipler = 3
	// primary weapon = 1
<field type="Pad" name="?" definition="3" />
<field type="Block" name="text value pairs">
	<field type="ByteFlags" name="flags">
		<definition count="1">
			<entry>default setting</entry>
			<entry>unknown</entry> // unchanged?
		</definition>
	</field>
	<field type="Pad" name="?" definition="3" />
	<field type="LongInteger" name="value" />
	<field type="Real" name="value" />
	<field type="StringId" name="value" />
	<field type="StringId" name="label string id" />
	<field type="StringId" name="description string id" />
	*/
}