using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TheOfficeApi.DTOs
{
   public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email {get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email {get; set;} = string.Empty;
        
        [Required]
        public string Password {get; set;} = string.Empty;
    }

    public class RefreshToken
    {
        public int Id {get; set;}
        public string Token {get; set;} = string.Empty;
        public string JwtId {get; set;} = string.Empty;
        public DateTime AddedDate {get; set;}
        public DateTime ExpiryDate {get; set;}
        public bool IsUsed {get; set;}
        public bool IsRevoked {get; set;}
        public string UserId {get; set;}

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }


    public class RefreshTokenRequestDto
    {
    [Required]
     public string AccessToken {get; set; } = string.Empty;
     [Required]
     public string RefreshToken { get; set; } = string.Empty;   
    }

    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email {get; set;} = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password {get; set;} = string.Empty;
    }

     public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email {get; set;} = string.Empty;
        [Required]
        public string Token { get;set;}  = string.Empty;
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }

     public class TokenResponseDto
    {
        public string AccessToken {get; set; } = string.Empty;
        public string RefreshToken {get; set;} = string.Empty;
        public DateTime ExpiresAt {get; set;}
    }
}