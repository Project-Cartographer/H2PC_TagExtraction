/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.Halo4.Tags
{
	#region custom_app_globals
	partial class custom_app_globals_group
	{
		#region custom_app_block
		partial class custom_app_block
		{
			public custom_app_block() : base(7)
			{
				Add(Name = new TI.StringId());
				Add(new TI.Pad(8)); // unused; description, help text?
				Add(Icon = new TI.StringId());
				Add(new TI.TagReference(this));
				Add(TI.Pad.DWord); // '0'
				Add(TI.Pad.BlockHalo3); // 0x3C
					// tag block
					// tag block
					// tag block
					// tag block
					// tag block
				Add(TI.Pad.BlockHalo3); // 0x1C?
					// byte[0x8]
					// string id?
					// tag reference
			}
		};
		#endregion

		public custom_app_globals_group() : base(4)
		{
			Add(TI.Pad.DWord); // '1'
			Add(TI.Pad.Word); // '1'
			Add(TI.Pad.Word); // '0'
			Add(Apps = new TI.Block<custom_app_block>(this, 0));
		}
	};
	#endregion

	#region game_globals_ordnance_list
	partial class game_globals_ordnance_list_group
	{
		#region ordnance_block
		partial class ordnance_block
		{
			public ordnance_block() : base(10)
			{
				Add(Name = new TI.StringId());
				Add(LookupName = new TI.String());
				Add(new TI.LongInteger());
				Add(ObjectType = new TI.StringId());
				Add(new TI.TagReference(this));
				Add(new TI.ByteInteger());
				Add(new TI.ByteInteger()); // amount?
				Add(new TI.Flags(TI.FieldType.ByteFlags));
				Add(TI.Pad.Byte);
				Add(new TI.Pad(12));
			}
		};
		#endregion

		#region ordnance_set_block
		partial class ordnance_set_block
		{
			#region remapping_block
			partial class remapping_block
			{
				public remapping_block() : base(2)
				{
					Add(OldType = new TI.StringId());
					Add(NewType = new TI.StringId());
				}
			};
			#endregion

			public ordnance_set_block() : base(3)
			{
				Add(Name = new TI.StringId());
				Add(LookupName = new TI.String());
				Add(Remappings = new TI.Block<remapping_block>(this, 0));
			}
		};
		#endregion

		public game_globals_ordnance_list_group() : base(7)
		{
			Add(new TI.Real());
			Add(new TI.Real());
			Add(new TI.TagReference(this, TagGroups.effe));
			Add(OrdnanceList = new TI.Block<ordnance_block>(this, 0));
			Add(OrdnanceSets = new TI.Block<ordnance_set_block>(this, 0));
			Add(new TI.Flags(TI.FieldType.ByteFlags));
			Add(TI.Pad._24);
		}
	};
	#endregion

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
		#region game_medal_globals_category_block
		partial class game_medal_globals_category_block
		{
			public game_medal_globals_category_block() : base(4)
			{
				Add(Name = new TI.StringId());
				Add(Unknown4 = new TI.StringId());
				Add(Unknown8 = new TI.ShortInteger());
				Add(Points = new TI.ShortInteger());
			}
		};
		#endregion

		#region game_medal_globals_medals_block
		partial class game_medal_globals_medals_block
		{
			public game_medal_globals_medals_block() : base(8)
			{
				Add(Name = new TI.StringId());
				Add(Description = new TI.StringId());
				Add(Unknown8 = new TI.ShortInteger());
				Add(UnknownA = new TI.Flags(TI.FieldType.ByteFlags));
				Add(TI.Pad.Byte);
				Add(UnknownC = new TI.LongInteger());
				Add(CategoryIndex = new TI.BlockIndex());
				Add(Points = new TI.ShortInteger());
			}
		};
		#endregion

		public game_medal_globals_group() : base(2)
		{
			Add(Categories = new TI.Block<game_medal_globals_category_block>(this, 0));
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

			#region unknown_18_block
			partial class unknown_18_block
			{
				public unknown_18_block() : base(8)
				{
					Add(SuppressedIncident = new TI.TagReference(this, TagGroups.sigd));
					Add(Unknown10 = new TI.LongInteger());
					Add(Unknown14 = new TI.LongInteger()); // unknown
					Add(Unknown18 = new TI.StringId());
					Add(Unknown1C = new TI.LongInteger()); // unknown
					Add(CuiScreen = new TI.TagReference(this, TagGroups.cusc));
					Add(SoundResponse = new TI.TagReference(this, TagGroups.sgrp));
					Add(Unknown40 = new TI.LongInteger()); // unknown
				}
			}
			#endregion

			#region unknown_24_block
			partial class unknown_24_block
			{
				public unknown_24_block() : base(9)
				{
					Add(Unknown0 = new TI.StringId());
					Add(TI.Pad.BlockHalo3);
						// byte_flags
						// PAD24
					Add(TI.Pad.BlockHalo3);
					Add(TI.Pad.BlockHalo3);
					Add(TI.Pad.BlockHalo3);
					Add(TI.Pad.BlockHalo3);
					Add(TI.Pad.BlockHalo3);
					Add(TI.Pad.BlockHalo3);
					Add(TI.Pad.BlockHalo3);
				}
			}
			#endregion

			#region unknown_30_block
			partial class unknown_30_block
			{
				#region unknown_4_block
				partial class unknown_4_block
				{
					public unknown_4_block() : base(6)
					{
						Add(Flags = new TI.Flags(TI.FieldType.ByteFlags));
						Add(TI.Pad._24);
						Add(Unknown4 = new TI.LongInteger());	// for multikills this is the count
						Add(Unknown8 = new TI.StringId());		// for multikills this is 'kill', so it's probably the incident filter
						Add(UnknownC = new TI.LongInteger()); // unknown
						Add(Unknown10 = new TI.LongInteger());// unknown
					}
				}
				#endregion

				public unknown_30_block() : base(3)
				{
					Add(Unknown0 = new TI.LongInteger()); // unknown
					Add(Block4 = new TI.Block<unknown_4_block>(this, 0));
					Add(Unknown10 = new TI.LongInteger());
					Add(Unknown14 = new TI.LongInteger()); // unknown
					Add(Unknown18 = new TI.StringId());
					Add(Unknown1C = new TI.LongInteger()); // unknown
					Add(CuiScreen = new TI.TagReference(this, TagGroups.cusc));
					Add(Unknown30 = new TI.TagReference(this));
					Add(Unknown44 = new TI.LongInteger()); // unknown
				}
			}
			#endregion

			#region unknown_3C_block
			partial class unknown_3C_block
			{
				#region unknown_8_block
				partial class unknown_8_block
				{
					public unknown_8_block() : base(3)
					{
						Add(Unknown0 = new TI.StringId());
						Add(Unknown4 = new TI.LongInteger()); // unknown
						Add(Unknown8 = new TI.LongInteger()); // unknown
					}
				}
				#endregion

				public unknown_3C_block() : base(22)
				{
					Add(Unknown0 = new TI.Flags(TI.FieldType.ByteFlags));
					Add(Unknown1 = new TI.Flags(TI.FieldType.ByteFlags));
					Add(TI.Pad.Word);
					Add(Unknown4 = new TI.LongInteger());
					Add(Block8 = new TI.Block<unknown_8_block>(this, 0));
					Add(Unknown14 = new TI.LongInteger()); // unknown
					Add(Unknown18 = new TI.LongInteger()); // unknown
					Add(Unknown1C = new TI.StringId());
					Add(Unknown20 = new TI.LongInteger()); // unknown
					Add(CuiScreen = new TI.TagReference(this, TagGroups.cusc));
					Add(SoundResponse = new TI.TagReference(this, TagGroups.sgrp));
					Add(Unknown44 = new TI.LongInteger()); // unknown
					Add(Unknown48 = new TI.LongInteger()); // unknown
					Add(Unknown4C = new TI.Flags(TI.FieldType.ByteFlags));
					Add(TI.Pad.Byte);
					Add(Unknown4E = new TI.Flags(TI.FieldType.ByteFlags));
					Add(TI.Pad.Byte);
					Add(Unknown50 = new TI.StringId());
					Add(Unknown54 = new TI.LongInteger()); // unknown
					Add(CuiScreen2 = new TI.TagReference(this, TagGroups.cusc));
					Add(Unknown68 = new TI.TagReference(this));
					Add(Unknown78 = new TI.LongInteger()); // unknown
				}
			}
			#endregion

			public incident_globals_definition_0_block() : base(11)
			{
				Add(Name = new TI.StringId());
				Add(Unknown4 = new TI.Flags(TI.FieldType.ByteFlags));
				Add(TI.Pad._24);
				Add(Unknown8 = new TI.Flags(TI.FieldType.ByteFlags));
				Add(TI.Pad._24);
				Add(BlockC = new TI.Block<unknown_C_block>(this, 0));
				Add(Block18 = new TI.Block<unknown_18_block>(this, 0));
				Add(Block24 = new TI.Block<unknown_24_block>(this, 0));
				Add(Block30 = new TI.Block<unknown_30_block>(this, 0));
				Add(Block3C = new TI.Block<unknown_3C_block>(this, 0));
				Add(TI.Pad.BlockHalo3); // 0x54
					// byte_flags
					// byte_flags
					// PAD16
					// unknown[0xC]
					// string_id
					// string_id
					// string_id
					// unknown[0x8]
					// tag_block
					// unknown[0x4]
					// int16
					// PAD16?
					// tag_block
					// tag_reference<sirg> sound_incident_response
			}
		};
		#endregion

		public incident_globals_definition_group() : base(3)
		{
			Add(Block0 = new TI.Block<incident_globals_definition_0_block>(this, 0));
			Add(UnknownC = new TI.TagReference(this, TagGroups.unic));
			Add(Unknown1C = new TI.TagReference(this, TagGroups.unic));
		}
	};
	#endregion

	#region loadout_globals_definition
	partial class loadout_globals_definition_group
	{
		#region loadout_block
		partial class loadout_block
		{
			public loadout_block() : base(10)
			{
				Add(Name = new TI.StringId());
				Add(AppSlot1 = new TI.StringId());
				Add(AppSlot2 = new TI.StringId());

				Add(PrimaryWeapon = new TI.StringId());
				Add(TI.Pad.DWord);

				Add(SecondaryWeapon = new TI.StringId());
				Add(TI.Pad.DWord);

				Add(Equipment = new TI.StringId());
				Add(Unk20 = new TI.Flags(TI.FieldType.ByteFlags));
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

		public loadout_globals_definition_group() : base(4)
		{
			Add(Loadouts = new TI.Block<loadout_block>(this, 0));
			Add(LoadoutSets = new TI.Block<loadout_set_block>(this, 0));
			Add(LoadoutNames = new TI.Block<field_block<TI.StringId>>(this, 0));
			Add(LoadoutDefaults = new TI.Block<loadout_block>(this, 0));
		}
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
		#region megalo_string_id_table_C_block
		partial class megalo_string_id_table_C_block
		{
			public megalo_string_id_table_C_block() : base(2)
			{
				Add(Unknown0 = new TI.LongInteger());
				Add(Unknown4 = new TI.LongInteger());
			}
		};
		#endregion

		public megalo_string_id_table_group() : base(2)
		{
			Add(Names = new TI.Block<field_block<TI.StringId>>(this, 0));
			Add(BlockC = new TI.Block<megalo_string_id_table_C_block>(this, 0));
		}
	};
	#endregion

	#region MultiplayerEffects
	partial class multiplayer_effects_group
	{
		#region multiplayer_effect_block
		partial class multiplayer_effect_block
		{
			public multiplayer_effect_block() : base(2)
			{
				Add(Name = new TI.StringId());
				Add(Definition = new TI.TagReference(this));
			}
		};
		#endregion

		public multiplayer_effects_group() : base(1)
		{
			Add(Effects = new TI.Block<multiplayer_effect_block>(this, 0));
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
				Add(DescriptionText = new TI.StringId());
				Add(HeaderText = new TI.StringId());
				Add(HelpText = new TI.StringId());
				Add(Unknown10 = new TI.Real());
				Add(Unknown14 = new TI.LongInteger());
				Add(Icon = new TI.StringId());
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

		public multiplayer_object_type_list_group() : base(15)
		{
			Add(TypeList = new TI.Block<multiplayer_object_type_block>(this, 0));
			Add(WeaponsList = new TI.Block<multiplayer_object_type_mapping_block>(this, 0));
			Add(VehiclesList = new TI.Block<multiplayer_object_type_mapping_block>(this, 0));
			Add(GrenadesList = new TI.Block<multiplayer_object_type_mapping_block>(this, 0));
			Add(EquipmentList = new TI.Block<multiplayer_object_type_mapping_block>(this, 0));
			Add(WeaponSets = new TI.Block<multiplayer_object_type_set_block>(this, 0));
			Add(VehicleSets = new TI.Block<multiplayer_object_type_set_block>(this, 0));
			Add(EquipmentSets = new TI.Block<multiplayer_object_type_set_block>(this, 0));
			Add(Unknown60 = new TI.LongInteger());
			Add(Unknown64 = new TI.LongInteger());
			Add(Unknown68 = new TI.StringId());
			Add(Unknown6C = new TI.StringId());
			Add(Unknown70 = new TI.LongInteger());
			Add(Unknown74 = new TI.Real());
			Add(new TI.Pad(12));
		}
	};
	#endregion
}