using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDotNet.WebAPI.PartitionResolvers
{
    /// <summary>
    /// Specifies how to handle requests to partitions in transition.
    /// </summary>
    public enum TransitionReadMode
    {
        /// <summary>
        /// Perform reads using the current PartitionResolver.
        /// </summary>
        ReadCurrent,

        /// <summary>
        /// Perform reads using the targeted PartitionResolver.
        /// </summary>
        ReadNext,

        /// <summary>
        /// Perform reads using partitions from both current and targeted PartitionResolvers, and 
        /// return the union of results.
        /// </summary>
        ReadBoth,

        /// <summary>
        /// Throw an transient Exception when reads are attempted during migration.
        /// </summary>
        None
    }
}
