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
		void Invert_value(ref float value)
		{
			if (value == 0.0f)
				return;
			value = 1f / value;
		}

		#region Reconstruct
		internal override bool Reconstruct(BlamLib.Blam.CacheFile c)
		{
			Invert_value(ref acceleration_range.I);
			Invert_value(ref acceleration_range.J);
			Invert_value(ref acceleration_range.K);
			return true;
		}
		#endregion
	}
    #endregion

    #region device_group
    partial class device_group
    {
        void Invert_value(ref float value)
        {
            if (value == 0.0f)
                return;
            value = 1f / value;
        }

        #region Reconstruct
        internal override bool Reconstruct(BlamLib.Blam.CacheFile c)
        {
            Invert_value(ref power_transition_time.Value);
            Invert_value(ref power_acceleration_time.Value);
            Invert_value(ref position_transition_time.Value);
            Invert_value(ref position_acceleration_time.Value);
            Invert_value(ref depowered_position_transition_time.Value);
            Invert_value(ref depowered_position_acceleration_time.Value);
            return true;
        }
        #endregion
    }
    #endregion
}