using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SwiftDotNet.WebAPI.Entities
{
    /// <summary>
    /// This is the base class that all root entity classes inherit.  This will 
    /// allow the type of entity to be passed in which is used for the 
    /// Where predicate in the RepositoryBase class.
    /// </summary>
    /// 
    [DataContract]
    public class EntityBase
    {
        private readonly string _docType;

        /// <summary>
        /// All root entities inherit this base class.
        /// </summary>
        /// <param name="docType">The name of the type of entity (lowercase).</param>
        public EntityBase(string docType)
        {
            this._docType = docType;
        }

        /// <summary>
        /// This is need for querying in the RepositoryBase. Used by DocumentDB.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// This docType field will be used to organize the documents by "docType" in 
        /// DocumentDB in a single-collection scenario.  The docType is just the lowercase
        /// name of the derived class.
        /// </summary>
        [JsonProperty(PropertyName = "docType")]
        public string docType { get { return _docType; } }
    }
}
