﻿using System;
using System.ComponentModel.DataAnnotations;
using GamexApi.Utilities;
using Newtonsoft.Json;

namespace GamexApi.Models
{
    // Models used as parameters to AccountController actions.

    #region Unused binding model

    //    public class AddExternalLoginBindingModel
    //    {
    //        [Required]
    //        [Display(Name = "External access token")]
    //        public string ExternalAccessToken { get; set; }
    //    }
    //
    //    public class RemoveLoginBindingModel
    //    {
    //        [Required]
    //        [Display(Name = "Login provider")]
    //        public string LoginProvider { get; set; }
    //
    //        [Required]
    //        [Display(Name = "Provider key")]
    //        public string ProviderKey { get; set; }
    //    }
    //
    //    public class SetPasswordBindingModel
    //    {
    //        [Required]
    //        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //        [DataType(DataType.Password)]
    //        [Display(Name = "New password")]
    //        public string NewPassword { get; set; }
    //
    //        [DataType(DataType.Password)]
    //        [Display(Name = "Confirm new password")]
    //        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    //        public string ConfirmPassword { get; set; }
    //    }

    #endregion

    public class ChangePasswordBindingModel
    {
        
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required] 
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }

    public class UpdateProfileBindingModel
    {
        [Required]
        [Username(ErrorMessage = "At lease 6 characters and only letters, digit, underscore, hyphen and dot allowed")]
        [StringLength(256, ErrorMessage = "Maximum 256 characters")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Maximum 100 characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Maximum 100 characters")]
        public string LastName { get; set; }

    }
}
