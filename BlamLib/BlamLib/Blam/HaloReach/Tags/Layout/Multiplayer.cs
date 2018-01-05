/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.HaloReach.Tags
{
	// TODO: Reach-ify
	#region game_engine_globals
	partial class game_engine_globals_group
	{
		public game_engine_globals_group() : base(3)
		{
			Add(/*settings = */ new TI.TagReference(this, TagGroups.wezr));
			Add(/*text = */ new TI.TagReference(this, TagGroups.unic));
			Add(TI.Pad.BlockHalo3); // events 0x58
				// string id, event name
				// byte_enum audience
					// 0 cause player
					// 1 cause team
					// 2 effect player
					// 3 effect team
					// 4 all
				// PAD24
				// string id, display name
				// word_flags
				// byte (required field?)
				// PAD8? (or excluded audience?)
				// byte[0xC], zeros (primary string, duration, sound delay?)
				// byte (sound flags?)
				// PAD24?
				// tag reference, unused (sound?)
				// byte[0x1C], zeros
				// tag block, unused (sound permutations?)
		}
	};
	#endregion

	// TODO: Reach-ify
	#region game_engine_settings_definition
	partial class game_engine_settings_definition_group
	{
		public game_engine_settings_definition_group() : base(7)
		{
			Add(TI.Pad.DWord);
			Add(TI.Pad.BlockHalo3); // player traits
				// string id
				// tag block
				// tag block
				// tag block
				// tag block
				// tag block
			Add(TI.Pad.BlockHalo3); // survival related
				// string id
				// byte
				// byte
				// byte
				// dword?
				// short
				// short?
			Add(TI.Pad.BlockHalo3); // sandbox variants?
				// string id, variant name
				// string id, variant description
				// tag block
				// tag block
				// tag block
				// tag block
				// tag block
				// tag block
				// tag block
				// tag block
				// dword
				// dword
				// string
			Add(TI.Pad.BlockHalo3); // survival variants, 0xD0
			Add(TI.Pad.BlockHalo3); // firefight variants
				// tag reference, ffgt
			Add(TI.Pad.BlockHalo3); // haven't seen used
		}
	};
	#endregion

	#region game_medal_globals
	partial class game_medal_globals_group
	{
		#region game_medal_globals_medals_block
		partial class game_medal_globals_medals_block
		{
			public game_medal_globals_medals_block() : base(7)
			{
				Add(Name = new TI.StringId());
				Add(Description = new TI.StringId());
				Add(Unknown8 = new TI.Real());
				Add(UnknownC = new TI.Real());
				Add(Unknown10 = new TI.ShortInteger());
				Add(Unknown12 = new TI.Flags(TI.FieldType.WordFlags));
				Add(Unknown14 = new TI.LongInteger());
			}
		};
		#endregion

		public game_medal_globals_group() : base(1)
		{
			Add(Medals = new TI.Block<game_medal_globals_medals_block>(this, 0));
		}
	};
	#endregion

	#region incident_globals_definition
	partial class incident_globals_definition_group
	{
		#region incident_globals_definition_0_block
		partial class incident_globals_definition_0_block
		{
			#region unknown_C_block
			partial class unknown_C_block
			{
				public unknown_C_block() : base(3)
				{
					Add(Unknown0 = new TI.StringId());
					Add(Flags = new TI.Flags(TI.FieldType.ByteFlags));
					Add(TI.Pad._24);
				}
			}
			#endregion

			#region unknown_30_block
			partial class unknown_30_block
			{
				#region unknown_4_block
				partial class unknown_4_block
				{
					public unknown_4_block() : base(4)
					{
						Add(Flags = new TI.Flags(TI.FieldType.ByteFlags));
						Add(TI.Pad._24);
						Add(Unknown4 = new TI.LongInteger());	// for multikills this is the count
						Add(Unknown8 = new TI.StringId());		// for multikills this is 'kill', so it's probably the incident filter
					}
				}
				#endregion

				public unknown_30_block() : base(3)
				{
					Add(Unknown0 = new TI.Flags(TI.FieldType.ByteFlags));
					Add(Unknown1 = new TI.Flags(TI.FieldType.ByteFlags));
					Add(TI.Pad.Word);
					Add(Block4 = new TI.Block<unknown_4_block>(this, 0));
				}
			}
			#endregion

			public incident_globals_definition_0_block() : base(9)
			{
				Add(Name = new TI.StringId());
				Add(Unknown4 = new TI.Flags(TI.FieldType.ByteFlags));
				Add(TI.Pad._24);
				Add(Unknown8 = new TI.Flags(TI.FieldType.ByteFlags));
				Add(TI.Pad._24);
				Add(BlockC = new TI.Block<unknown_C_block>(this, 0));
				Add(TI.Pad.BlockHalo3); // 0x38?
				// Actually 0x24
				Add(Block30 = new TI.Block<unknown_30_block>(this, 0));
				// Actually 0x30
				Add(TI.Pad.BlockHalo3); // 0x30
					// byte_flags
					// byte_flags
					// PAD16
					// string_id?
					// string_id
					// string_id
					// unknown[0x1C]
					// int16
					// PAD16?
			}
		};
		#endregion

		public incident_globals_definition_group() : base(2)
		{
			Add(new TI.Pad(4)); // only seen as zero
			Add(Block0 = new TI.Block<incident_globals_definition_0_block>(this, 0));
		}
	};
	#endregion

	#region loadout_globals_definition
	[TI.TagGroup((int)TagGroups.Enumerated.lgtd, -1, 36)]
	public partial class loadout_globals_definition_group : TI.Definition
	{
		#region loadout_block
		[TI.Definition(-1, 20)]
		public partial class loadout_block : TI.Definition
		{
			public TI.StringId Name, PrimaryWeapon, SecondaryWeapon, Equipment;
			public TI.Flags Unk10;
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
	};
	#endregion

	#region megalogamengine_sounds
	public partial class megalogamengine_sounds_group : TI.Definition
	{
		public megalogamengine_sounds_group() : base(kNumberOfSounds)
		{
			Sounds = new TI.TagReference[kNumberOfSounds];
			for (int x = 0; x < Sounds.Length; x++)
				Add(Sounds[x] = new TI.TagReference(this, TagGroups.snd_));
		}
	};
	#endregion

	#region megalo_string_id_table
	partial class megalo_string_id_table_group
	{
		public megalo_string_id_table_group() : base(1)
		{
			Add(Names = new TI.Block<field_block<TI.StringId>>(this, 0));
		}
	};
	#endregion

	#region multiplayer_object_type_list
	partial class multiplayer_object_type_list_group
	{
		#region multiplayer_object_type_block
		partial class multiplayer_object_type_block
		{
			public multiplayer_object_type_block() : base(2)
			{
				Add(Name = new TI.StringId());
				Add(Definition = new TI.TagReference(this));
			}
		};
		#endregion

		#region multiplayer_object_type_mapping_block
		partial class multiplayer_object_type_mapping_block
		{
			public multiplayer_object_type_mapping_block() : base(7)
			{
				Add(TypeIndex = new TI.BlockIndex(TI.FieldType.LongBlockIndex));
				Add(TI.Pad.DWord); // Only ever seen as zero. Probably an unused string id
				Add(Unknown10 = new TI.Real());
				Add(Unknown14 = new TI.LongInteger());
			}
		};
		#endregion

		#region multiplayer_object_type_set_block
		partial class multiplayer_object_type_set_block
		{
			#region multiplayer_object_type_set_entry_block
			partial class multiplayer_object_type_set_entry_block
			{
				public multiplayer_object_type_set_entry_block() : base(2)
				{
					Add(Unknown0 = new TI.LongInteger());
					Add(ObjectMappingIndex = new TI.BlockIndex(TI.FieldType.LongBlockIndex));
				}
			};
			#endregion

			public multiplayer_object_type_set_block() : base(2)
			{
				Add(Name = new TI.StringId());
				Add(Entries = new TI.Block<multiplayer_object_type_set_entry_block>(this, 0));
			}
		};
		#endregion

		public multiplayer_object_type_list_group() : base(9)
		{
			Add(TypeList = new TI.Block<multiplayer_object_type_block>(this, 0));
			Add(WeaponsList = new TI.Block<multiplayer_object_type_mapping_block>(this, 0));
			Add(VehiclesList = new TI.Block<multiplayer_object_type_mapping_block>(this, 0));
			Add(GrenadesList = new TI.Block<multiplayer_object_type_mapping_block>(this, 0));
			Add(EquipmentList = new TI.Block<multiplayer_object_type_mapping_block>(this, 0));
			Add(WeaponSets = new TI.Block<multiplayer_object_type_set_block>(this, 0));
			Add(VehicleSets = new TI.Block<multiplayer_object_type_set_block>(this, 0));
			Add(Unknown60 = new TI.LongInteger());
			Add(Unknown64 = new TI.LongInteger());
		}
	};
	#endregion
}