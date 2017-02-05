using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swift.Net.WebAPI.Models
{
    public class FlatUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; }
    }

    public class GetUserTokenViewModel
    {
        public AuthenticationTicket ticket { get; set; }
    }
}
