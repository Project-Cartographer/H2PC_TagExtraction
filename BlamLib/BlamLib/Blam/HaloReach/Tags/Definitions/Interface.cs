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
	[TI.TagGroup((int)TagGroups.Enumerated.sily, -1, 24)]
	public partial class text_value_pair_definition_group : TI.Definition
	{
		#region text_value_pair_reference_block
		[TI.Definition(-1, 20)]
		public partial class text_value_pair_reference_block : TI.Definition
		{
			public TI.Flags Flags;
			public TI.Enum Type;
			public TI.LongInteger Integer;
			public TI.StringId StringId;
			public TI.StringId Name, Description;
		};
		#endregion

		public TI.Enum Parameter;
		public TI.StringId Title, Description;
		public TI.Block<text_value_pair_reference_block> Values;
	};
	#endregion
}