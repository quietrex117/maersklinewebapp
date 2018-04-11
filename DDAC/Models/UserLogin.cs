using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DDAC.Models
{
    public class UserLogin
    {
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        public String Email { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }


    }
}