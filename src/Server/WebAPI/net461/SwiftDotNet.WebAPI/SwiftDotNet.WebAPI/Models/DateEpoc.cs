using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwiftDotNet.WebAPI.Helpers;

namespace SwiftDotNet.WebAPI.Models
{
    public class DateEpoch
    {
        public DateTime Date { get; set; }
        public int Epoch
        {
            get
            {
                return (this.Date.Equals(null) || this.Date.Equals(DateTime.MinValue)) ? int.MinValue : this.Date.ToEpoch();
            }
        }
    }
}
