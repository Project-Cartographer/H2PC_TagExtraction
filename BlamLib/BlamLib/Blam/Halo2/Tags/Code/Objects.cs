/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.Halo2.Tags
{
	#region character_physics_struct
	partial class character_physics_struct
	{
		#region Reconstruct
		internal override bool Reconstruct(BlamLib.Blam.CacheFile c)
		{
			dead_sphere_shapes.DeleteAll();
			pill_shapes.DeleteAll();
			sphere_shapes.DeleteAll();
			return true;
		}
		#endregion
	}
	#endregion

	#region device_group
	partial class device_group
	{
		void Invert_Value(ref float value)
		{
			if (value == 0.0f)
				return;
			value = 1f / value;
		}

		#region Reconstruct
		internal override bool Reconstruct(BlamLib.Blam.CacheFile c)
		{
			Invert_Value(ref power_transition_time.Value);
			Invert_Value(ref power_acceleration_time.Value);
			Invert_Value(ref position_transition_time.Value);
			Invert_Value(ref position_acceleration_time.Value);
			Invert_Value(ref depowered_position_transition_time.Value);
			Invert_Value(ref depowered_position_acceleration_time.Value);
			PredictedResources.DeleteAll();
			return true;
		}
		#endregion
	}
	#endregion

	#region havok_vehicle_physics_struct
	partial class havok_vehicle_physics_struct
	{
		#region Reconstruct
		internal override bool Reconstruct(BlamLib.Blam.CacheFile c)
		{
			shape_phantom_shape.DeleteAll();
			return true;
		}
		#endregion
	}
	#endregion

	#region model_group
	partial class model_group
	{
		#region Reconstruct
		internal override bool Reconstruct(BlamLib.Blam.CacheFile c)
		{
			RuntimeFlags.Value = 0;        
			return true;
		}
		#endregion

		#region global_damage_info_block
		partial class global_damage_info_block
		{
			#region Reconstruct
			internal override bool Reconstruct(BlamLib.Blam.CacheFile c)
			{
				nodes.DeleteAll();
				return true;
			}
			#endregion
		}
		#endregion
	}
	#endregion

	#region weapon_group
	partial class weapon_group
	{
		#region Reconstruct
		internal override bool Reconstruct(BlamLib.Blam.CacheFile c)
		{
			PredictedResources.DeleteAll();
			PredictedBitmaps.DeleteAll();
			WeaponPredictedResources.DeleteAll();
			return true;
		}
		#endregion
	}
	#endregion

	#region unit_seat_accerleration_struct_block
	partial class unit_seat_acceleration_struct
	{
		void Invert_Value(ref float value)
		{
			if (value == 0.0f)
				return;
			value = 1f / value;
		}

		#region Reconstruct
		internal override bool Reconstruct(BlamLib.Blam.CacheFile c)
		{
			Invert_Value(ref acceleration_range.I);
			Invert_Value(ref acceleration_range.J);
			Invert_Value(ref acceleration_range.K);
			return true;
		}
		#endregion
	}
	#endregion
}
