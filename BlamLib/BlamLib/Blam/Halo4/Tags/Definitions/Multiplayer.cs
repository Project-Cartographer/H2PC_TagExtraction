/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.Halo4.Tags
{
	// TODO: move to Globals.cs
	[TI.Definition]
	public sealed class field_block<FieldType> : TI.field_block<FieldType> where FieldType : TI.Field, new() { };


	#region custom_app_globals
	[TI.TagGroup((int)TagGroups.Enumerated.capg, -1, 20)]
	public partial class custom_app_globals_group : TI.Definition
	{
		#region custom_app_block
		[TI.Definition(-1, 60)]
		public partial class custom_app_block : TI.Definition
		{
			public TI.StringId Name, Icon;
		};
		#endregion

		public TI.Block<custom_app_block> Apps;
	};
	#endregion

	#region game_globals_ordnance_list
	[TI.TagGroup((int)TagGroups.Enumerated.ggol, -1, 52)]
	public partial class game_globals_ordnance_list_group : TI.Definition
	{
		#region ordnance_block
		[TI.Definition(-1, 76)]
		public partial class ordnance_block : TI.Definition
		{
			public TI.StringId Name;
			public TI.String LookupName;
			public TI.StringId ObjectType;
		};
		#endregion

		#region ordnance_set_block
		[TI.Definition(-1, 48)]
		public partial class ordnance_set_block : TI.Definition
		{
			#region remapping_block
			[TI.Definition(-1, 8)]
			public partial class remapping_block : TI.Definition
			{
				public TI.StringId OldType, NewType;
			};
			#endregion

			public TI.StringId Name;
			public TI.String LookupName;
			public TI.Block<remapping_block> Remappings;
		};
		#endregion

		public TI.Block<ordnance_block> OrdnanceList;
		public TI.Block<ordnance_set_block> OrdnanceSets;
	};
	#endregion

	#region game_engine_globals
	[TI.TagGroup((int)TagGroups.Enumerated.gegl, -1, 44)]
	public partial class game_engine_globals_group : TI.Definition
	{
	};
	#endregion

	#region game_engine_settings_definition
	[TI.TagGroup((int)TagGroups.Enumerated.wezr, -1, 76)]
	public partial class game_engine_settings_definition_group : TI.Definition
	{
	};
	#endregion

	#region game_medal_globals
	[TI.TagGroup((int)TagGroups.Enumerated.gmeg, -1, 24)]
	public partial class game_medal_globals_group : TI.Definition
	{
		#region game_medal_globals_category_block
		[TI.Definition(-1, 12)]
		public partial class game_medal_globals_category_block : TI.Definition
		{
			public TI.StringId Name, Unknown4;
			public TI.ShortInteger Unknown8, Points;
		};
		#endregion

		#region game_medal_globals_medals_block
		[TI.Definition(-1, 20)]
		public partial class game_medal_globals_medals_block : TI.Definition
		{
			public TI.StringId Name, Description;
			public TI.ShortInteger Unknown8;
			public TI.Flags UnknownA;
			public TI.LongInteger UnknownC; // appears to be point related as well
			public TI.BlockIndex CategoryIndex;
			public TI.ShortInteger Points;
		};
		#endregion

		public TI.Block<game_medal_globals_category_block> Categories;
		public TI.Block<game_medal_globals_medals_block> Medals;
	};
	#endregion

	#region incident_globals_definition
	[TI.TagGroup((int)TagGroups.Enumerated.ingd, -1, 44)]
	public partial class incident_globals_definition_group : TI.Definition
	{
		#region incident_globals_definition_0_block
		[TI.Definition(-1, 84)]
		public partial class incident_globals_definition_0_block : TI.Definition
		{
			// parent categories?
			#region unknown_C_block
			[TI.Definition(-1, 8)]
			public partial class unknown_C_block : TI.Definition
			{
				public TI.StringId Unknown0;
				public TI.Flags Flags;
			}
			#endregion

			#region unknown_18_block
			[TI.Definition(-1, 68)]
			public partial class unknown_18_block : TI.Definition
			{
				public TI.TagReference SuppressedIncident;
				public TI.LongInteger Unknown10, Unknown14;
				public TI.StringId Unknown18;
				public TI.LongInteger Unknown1C;
				public TI.TagReference CuiScreen, SoundResponse;
				public TI.LongInteger Unknown40;
			}
			#endregion

			#region unknown_24_block
			[TI.Definition(-1, 100)]
			public partial class unknown_24_block : TI.Definition
			{
				public TI.StringId Unknown0;
			}
			#endregion

			// filter of some sort?
			#region unknown_30_block
			[TI.Definition(-1, 68)]
			public partial class unknown_30_block : TI.Definition
			{
				#region unknown_4_block
				[TI.Definition(-1, 20)]
				public partial class unknown_4_block : TI.Definition
				{
					public TI.Flags Flags;
					public TI.LongInteger Unknown4;
					public TI.StringId Unknown8;
					public TI.LongInteger UnknownC;
					public TI.LongInteger Unknown10;
				}
				#endregion

				public TI.LongInteger Unknown0;
				public TI.Block<unknown_4_block> Block4;
				public TI.LongInteger Unknown10;
				public TI.LongInteger Unknown14;
				public TI.StringId Unknown18;
				public TI.LongInteger Unknown1C;
				public TI.TagReference CuiScreen, Unknown30;
				public TI.LongInteger Unknown44;
			}
			#endregion

			#region unknown_3C_block
			[TI.Definition(-1, 124)]
			public partial class unknown_3C_block : TI.Definition
			{
				// seems to possibly be a megalo category filter (eg, regicide)
				#region unknown_8_block
				[TI.Definition(-1, 12)]
				public partial class unknown_8_block : TI.Definition
				{
					public TI.StringId Unknown0;
					public TI.LongInteger Unknown4;
					public TI.LongInteger Unknown8;
				}
				#endregion

				public TI.Flags Unknown0, Unknown1;

				public TI.LongInteger Unknown4;
				public TI.Block<unknown_8_block> Block8;
				public TI.LongInteger Unknown14;
				public TI.LongInteger Unknown18;
				public TI.StringId Unknown1C;
				public TI.LongInteger Unknown20;
				public TI.TagReference CuiScreen, SoundResponse;
				public TI.LongInteger Unknown44, Unknown48;
				public TI.Flags Unknown4C, Unknown4E;
				public TI.StringId Unknown50;
				public TI.LongInteger Unknown54;
				public TI.TagReference CuiScreen2, Unknown68;
				public TI.LongInteger Unknown78;
			}
			#endregion

			public TI.StringId Name;
			public TI.Flags Unknown4, Unknown8;
			public TI.Block<unknown_C_block> BlockC;
			public TI.Block<unknown_18_block> Block18;
			public TI.Block<unknown_24_block> Block24;
			public TI.Block<unknown_30_block> Block30;
			public TI.Block<unknown_3C_block> Block3C;
		};
		#endregion

		public TI.Block<incident_globals_definition_0_block> Block0;
		public TI.TagReference UnknownC, Unknown1C;
	};
	#endregion

	#region loadout_globals_definition
	[TI.TagGroup((int)TagGroups.Enumerated.lgtd, -1, 48)]
	public partial class loadout_globals_definition_group : TI.Definition
	{
		#region loadout_block
		[TI.Definition(-1, 36)]
		public partial class loadout_block : TI.Definition
		{
			public TI.StringId Name, AppSlot1, AppSlot2, PrimaryWeapon, SecondaryWeapon, Equipment;
			public TI.Flags Unk20;
		};
		#endregion

		#region loadout_set_block
		[TI.Definition(-1, 16)]
		public partial class loadout_set_block : TI.Definition
		{
			#region entry_block
			[TI.Definition(-1, 4)]
			public partial class entry_block : TI.Definition
			{
				public TI.BlockIndex LoadoutIndex;
			};
			#endregion

			public TI.StringId Name;
			public TI.Block<entry_block> Loadouts;
		};
		#endregion

		public TI.Block<loadout_block> Loadouts;
		public TI.Block<loadout_set_block> LoadoutSets;
		public TI.Block<field_block<TI.StringId>> LoadoutNames;
		public TI.Block<loadout_block> LoadoutDefaults;
	};
	#endregion

	#region megalogamengine_sounds
	[TI.TagGroup((int)TagGroups.Enumerated.mgls, -1, 3840)]
	public partial class megalogamengine_sounds_group : TI.Definition
	{
		const int kNumberOfSounds = 240;

		public TI.TagReference[] Sounds;
	};
	#endregion

	#region megalo_string_id_table
	[TI.TagGroup((int)TagGroups.Enumerated.msit, -1, 24)]
	public partial class megalo_string_id_table_group : TI.Definition
	{
		#region megalo_string_id_table_C_block
		[TI.Definition(-1, 8)]
		public partial class megalo_string_id_table_C_block : TI.Definition
		{
			public TI.LongInteger Unknown0, Unknown4;
		};
		#endregion

		public TI.Block<field_block<TI.StringId>> Names;
		public TI.Block<megalo_string_id_table_C_block> BlockC;
	};
	#endregion

	#region MultiplayerEffects
	[TI.TagGroup((int)TagGroups.Enumerated.mgee, -1, 12)]
	public partial class multiplayer_effects_group : TI.Definition
	{
		#region multiplayer_effect_block
		[TI.Definition(-1, 20)]
		public partial class multiplayer_effect_block : TI.Definition
		{
			public TI.StringId Name;
			public TI.TagReference Definition;
		};
		#endregion

		public TI.Block<multiplayer_effect_block> Effects;
	};
	#endregion

	#region multiplayer_object_type_list
	[TI.TagGroup((int)TagGroups.Enumerated.motl, -1, 132)]
	public partial class multiplayer_object_type_list_group : TI.Definition
	{
		#region multiplayer_object_type_block
		[TI.Definition(-1, 20)]
		public partial class multiplayer_object_type_block : TI.Definition
		{
			public TI.StringId Name;
			public TI.TagReference Definition;
		};
		#endregion

		#region multiplayer_object_type_mapping_block
		[TI.Definition(-1, 28)]
		public partial class multiplayer_object_type_mapping_block : TI.Definition
		{
			public TI.BlockIndex TypeIndex;
			public TI.StringId DescriptionText, HeaderText, HelpText;
			public TI.Real Unknown10;
			public TI.LongInteger Unknown14;
			public TI.StringId Icon;
		};
		#endregion

		#region multiplayer_object_type_set_block
		[TI.Definition(-1, 16)]
		public partial class multiplayer_object_type_set_block : TI.Definition
		{
			#region multiplayer_object_type_set_entry_block
			[TI.Definition(-1, 8)]
			public partial class multiplayer_object_type_set_entry_block : TI.Definition
			{
				public TI.LongInteger Unknown0;
				public TI.BlockIndex ObjectMappingIndex;
			};
			#endregion

			public TI.StringId Name;
			public TI.Block<multiplayer_object_type_set_entry_block> Entries;
		};
		#endregion

		public TI.Block<multiplayer_object_type_block> TypeList;
		public TI.Block<multiplayer_object_type_mapping_block> WeaponsList,
			VehiclesList, GrenadesList, EquipmentList;
		public TI.Block<multiplayer_object_type_set_block> WeaponSets, VehicleSets,
			EquipmentSets;
		public TI.LongInteger Unknown60, Unknown64;
		public TI.StringId Unknown68, Unknown6C;
		public TI.LongInteger Unknown70;
		public TI.Real Unknown74;
	};
	#endregion
}