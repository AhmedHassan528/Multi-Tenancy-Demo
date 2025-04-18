﻿using System.ComponentModel.DataAnnotations;

namespace MultiTenancy.Models.AuthModels
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
