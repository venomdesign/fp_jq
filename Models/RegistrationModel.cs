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
        [RegularExpression(@"([\w.-]+@([\w-]+)\.+\w{2,3})", ErrorMessage = "Please enter a valid Email Address.")]
        [MaxLength(50, ErrorMessage = "The Email Address cannot be longer than 50 characters.")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(18, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?\d)(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?\d)(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Password should meet the displayed requirements.")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "First Name cannot be longer then 50 characters.")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "First Name cannot have any numbers or special characters.")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "Last Name cannot be longer then 50 characters.")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Last Name cannot have any numbers or special characters.")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        [MaxLength(50, ErrorMessage = "Company Name cannot be longer then 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9 \-]*$", ErrorMessage = "Company Name cannot have any special characters.")]
        public string CompanyID { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$", ErrorMessage = "Phone Number must be (999) 999-9999")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Ext")]
        [Range(0, 9999999999, ErrorMessage = "The Phone Extension cannot exceed 10 digits.")]
        public int? PhoneExtension { get; set; }

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
        [MaxLength(4000, ErrorMessage = "Cannot have more than 4000 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9 \-\,\.\$\(\)\r\n\t]*$", ErrorMessage = "Comments cannot have any special characters.")]
        public string Notes { get; set; }

        [Display(Name = "Enable Multi-Factor Authentication")]
        public bool MfaEnabled { get; set; }

        public bool AddToAuth0 { get; set; }

        public bool HasSSO { get; set; }
    }
}