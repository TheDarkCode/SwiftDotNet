using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Swift.Net.WebAPI.Entities.Analytics
{
    [DataContract]
    public class Analytic : EntityBase
    {
        /// <summary>
        /// Pass the lowercase string name of the class to the base class.
        /// This is used in the repository for storage and querying, to organize
        /// documents by this type name.
        /// </summary>
        public Analytic()
            : base("analytic")
        {

        }

        [DataMember]
        [Required]
        public string ForwarderFor { get; set; }

        [DataMember]
        [Required]
        public string RemoteIP { get; set; }

        [DataMember]
        [Required]
        public string IPAddress { get; set; }

        [DataMember]
        [Required]
        public string Certainty { get; set; }

        [DataMember]
        [Required]
        public string Internet { get; set; }

        [DataMember]
        [Required]
        public string City { get; set; }

        [DataMember]
        [Required]
        public string Region { get; set; }

        [DataMember]
        [Required]
        public string Country { get; set; }

        [DataMember]
        [Required]
        public string FQCN { get; set; }

        [DataMember]
        [Required]
        public string ConvertsOn { get; set; }

        [DataMember]
        [Required]
        public decimal Latitude { get; set; }

        [DataMember]
        [Required]
        public decimal Longitude { get; set; }

    }
}
