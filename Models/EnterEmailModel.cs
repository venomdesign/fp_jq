using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetEasyPay.Models
{
    public class EnterEmailModel
    {
        [Required]
        [RegularExpression(@"([\w.-]+@([\w-]+)\.+\w{2,})", ErrorMessage = "Please enter a valid Email Address.")]
        [Display(Name = "Email Address")]
        [MaxLength(50, ErrorMessage = "The Email Address cannot be longer than 50 characters.")]
        public string EmailAddress { get; set; }
    }
}