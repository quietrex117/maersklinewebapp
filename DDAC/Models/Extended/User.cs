using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DDAC.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        //add validation
        public string ConfirmPassword { get; set; }
    }

    public class UserMetadata
    {
        [Display(Name ="Username")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username required")]
        public string Username { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage ="Minimum 6 characters required")]
        public string Password { get; set; }

        [Display(Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address required")]
        public string Address { get; set; }

        [Display(Name = "PhoneNumber")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number required")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Confirm password and password do not match")]
        public string ConfirmPassword { get; set; }



    }
}