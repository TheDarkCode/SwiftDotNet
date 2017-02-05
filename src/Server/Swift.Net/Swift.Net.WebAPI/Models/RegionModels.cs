using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swift.Net.WebAPI.Models
{
    /// <summary>
    /// The geographic region a user is in.
    /// </summary>
    public enum Region : int
    {
        /// <summary>
        /// The US East region.
        /// </summary>
        UnitedStatesEast,

        /// <summary>
        /// The West US region.
        /// </summary>
        UnitedStatesWest,

        /// <summary>
        /// The Europe region.
        /// </summary>
        Europe,

        /// <summary>
        /// The Asia Pacific region.
        /// </summary>
        AsiaPacific,

        /// <summary>
        /// Other regions.
        /// </summary>
        Other
    }
}
