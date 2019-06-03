/*
	BlamLib: .NET SDK for the Blam Engine

	See license\BlamLib\BlamLib for specific license information
*/
#pragma warning disable 1591 // "Missing XML comment for publicly visible type or member"
using System;
using TI = BlamLib.TagInterface;

namespace BlamLib.Blam.Halo2.Tags
{
	#region unit_seat_accerleration_struct_block
	partial class unit_seat_acceleration_struct
	{
		void invert_value(ref float value)
		{
			if (value == 0.0f)
				return;
			value = 1f / value;
		}

		#region Reconstruct
		internal override bool Reconstruct(BlamLib.Blam.CacheFile c)
		{
			invert_value(ref acceleration_range.I);
			invert_value(ref acceleration_range.J);
			invert_value(ref acceleration_range.K);
			return true;
		}
		#endregion
	}
	#endregion
}