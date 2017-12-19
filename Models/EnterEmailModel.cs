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
        [EmailAddress]
        [Display(Name = "Email Address")]
        [MaxLength(50)]
        public string EmailAddress { get; set; }
    }
}