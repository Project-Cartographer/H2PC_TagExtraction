/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.HaloReach.Tags
{
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
	[TI.TagGroup((int)TagGroups.Enumerated.gmeg, -1, 12)]
	public partial class game_medal_globals_group : TI.Definition
	{
		#region game_medal_globals_medals_block
		[TI.Definition(-1, 24)]
		public partial class game_medal_globals_medals_block : TI.Definition
		{
			public TI.StringId Name, Description;
			public TI.Real Unknown8, UnknownC;
			public TI.ShortInteger Unknown10;
			public TI.Flags Unknown12;
			public TI.LongInteger Unknown14;
		};
		#endregion

		public TI.Block<game_medal_globals_medals_block> Medals;
	};
	#endregion

	#region incident_globals_definition
	[TI.TagGroup((int)TagGroups.Enumerated.ingd, -1, 16)]
	public partial class incident_globals_definition_group : TI.Definition
	{
		#region incident_globals_definition_0_block
		[TI.Definition(-1, 60)]
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

			// filter of some sort?
			#region unknown_30_block
			[TI.Definition(-1, 68)]
			public partial class unknown_30_block : TI.Definition
			{
				#region unknown_4_block
				[TI.Definition(-1, 12)]
				public partial class unknown_4_block : TI.Definition
				{
					public TI.Flags Flags;
					public TI.LongInteger Unknown4;
					public TI.StringId Unknown8;
				}
				#endregion

				public TI.Flags Unknown0, Unknown1;
				public TI.Block<unknown_4_block> Block4;
			}
			#endregion

			public TI.StringId Name;
			public TI.Flags Unknown4, Unknown8;
			public TI.Block<unknown_C_block> BlockC;

			public TI.Block<unknown_30_block> Block30;
		};
		#endregion

		public TI.Block<incident_globals_definition_0_block> Block0;
	};
	#endregion

	#region loadout_globals_definition
	partial class loadout_globals_definition_group
	{
		#region loadout_block
		partial class loadout_block
		{
			public loadout_block() : base(6)
			{
				Add(Name = new TI.StringId());

				Add(PrimaryWeapon = new TI.StringId());

				Add(SecondaryWeapon = new TI.StringId());

				Add(Equipment = new TI.StringId());
				Add(Unk10 = new TI.Flags(TI.FieldType.ByteFlags));
				Add(TI.Pad._24);
			}
		};
		#endregion

		#region loadout_set_block
		partial class loadout_set_block
		{
			#region entry_block
			partial class entry_block
			{
				public entry_block() : base(2)
				{
					Add(LoadoutIndex = new TI.BlockIndex());
					Add(TI.Pad.Word);
				}
			};
			#endregion

			public loadout_set_block() : base(2)
			{
				Add(Name = new TI.StringId());
				Add(Loadouts = new TI.Block<entry_block>(this, 0));
			}
		};
		#endregion

		public loadout_globals_definition_group() : base(3)
		{
			Add(Loadouts = new TI.Block<loadout_block>(this, 0));
			Add(LoadoutSets = new TI.Block<loadout_set_block>(this, 0));
			Add(LoadoutNames = new TI.Block<field_block<TI.StringId>>(this, 0));
		}
	};
	#endregion

	#region megalogamengine_sounds
	[TI.TagGroup((int)TagGroups.Enumerated.mgls, -1, 1520)]
	public partial class megalogamengine_sounds_group : TI.Definition
	{
		const int kNumberOfSounds = 95;

		public TI.TagReference[] Sounds;
	};
	#endregion

	#region megalo_string_id_table
	[TI.TagGroup((int)TagGroups.Enumerated.msit, -1, 12)]
	public partial class megalo_string_id_table_group : TI.Definition
	{
		public TI.Block<field_block<TI.StringId>> Names;
	};
	#endregion

	#region multiplayer_object_type_list
	[TI.TagGroup((int)TagGroups.Enumerated.motl, -1, 92)]
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
		[TI.Definition(-1, 16)]
		public partial class multiplayer_object_type_mapping_block : TI.Definition
		{
			public TI.BlockIndex TypeIndex;
			public TI.Real Unknown10;
			public TI.LongInteger Unknown14;
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
		public TI.Block<multiplayer_object_type_set_block> WeaponSets, VehicleSets;
		public TI.LongInteger Unknown60, Unknown64;
	};
	#endregion
}