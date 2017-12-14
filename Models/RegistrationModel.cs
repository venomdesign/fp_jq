using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace NetEasyPay.Models
{
    public class RegistrationModel
    {
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?\d)(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?\d)(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords should meet the displayed requirments.")]
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
        [RegularExpression(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$", ErrorMessage = "Phone Number must be (999) 999-9999")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Ext")]
        public string PhoneExtension { get; set; }

        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$", ErrorMessage = "Mobile Number must be (999) 999-9999")]
        public string MobileNumber { get; set; }

        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$", ErrorMessage = "Mobile Number must be (999) 999-9999")]
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