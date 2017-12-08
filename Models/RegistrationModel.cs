using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetEasyPay.Models
{
    public class RegistrationModel
    {
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        public string CompanyID { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Ext")]
        public string PhoneExtension { get; set; }

        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$")]
        public string MobileNumber { get; set; }

        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$")]
        public string MfaPhoneNumber { get; set; }

        [Required]
        [Display(Name = "Time Zone")]
        public string TimeZone { get; set; }

        [Display(Name = "Comments")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(Name = "Enable Multi-Factor Authentication")]
        public bool MfaEnabled { get; set; }

        public bool AddToAuth0 { get; set; }

        public bool HasSSO { get; set; }
    }
}