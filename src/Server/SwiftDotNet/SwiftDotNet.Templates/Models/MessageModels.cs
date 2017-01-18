using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SwiftDotNet.Templates.Models
{
    public class ContactFormModel
    {

        [Required(ErrorMessage = "Your Name is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //[Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [Display(Name = "Destination")]
        public string Destination { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Intent")]
        public string Intent { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(4000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 25)]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [Required]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [Required]
        [Display(Name = "FQCN")]
        public string FQCN { get; set; }

    }
}
