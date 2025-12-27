using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TheOfficeApi.Data;
using TheOfficeApi.DTOs;
using TheOfficeApi.Interfaces;






namespace InventoryApi.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<IdentityUser> userManager, IConfiguration configuration, ILogger<AuthController> logger
        ,AppDbContext context, IEmailService emailService, SignInManager<IdentityUser> signInManager){
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _emailService = emailService;
            _signInManager =  signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto){
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if(existingUser != null){
                return BadRequest("User with this email already exists");
            }

            var user = new IdentityUser{
                UserName = registerDto.Email,
                Email = registerDto.Email,
                // EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if(!result.Succeeded){
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "User");

            // var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", new {userId = user.Id, code = token}, Request.Scheme);

            // if(confirmationLink == null){
            //     _logger.LogError("Could not generate confirmation link");
            //     return StatusCode(500, "Could not generate confirmation link");
            // }

            // var emailBody = $"Please confirm your email by <a href='{confirmationLink}'> clicking here.";
            // await _emailService.SendEmailAsync(user.Email, "Confirm your email", emailBody);

            _logger.LogInformation("User {Email} registered successfully", registerDto.Email);
            return Ok(new {message = "User registered successfully. Please check your email to confirm your account"});
        }

        // [HttpGet("confirm-email")]
        // public async Task<IActionResult> ConfirmEmail(string userId, string code){
        //     if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code)){
        //         return BadRequest("Invalid email confirmation URL");
        //     }

        //     var user = await _userManager.FindByIdAsync(userId);

        //     if(user == null){
        //         return NotFound("user not found");
        //     }

        //     // decode the url
        //     var decodedCode = System.Net.WebUtility.UrlDecode(code);
        //     var result = await _userManager.ConfirmEmailAsync(user, decodedCode);

        //     if(result.Succeeded){
        //         _logger.LogInformation("Email confirmed for user {Email}", user.Email);
        //         return Ok("Thank you for confirming your email. You can now log in");
        //     }

        //     return BadRequest("Failed to confirm email");
        // }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto){
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if(user == null){
                _logger.LogInformation("Password reset requested for non-existend user {Email}", forgotPasswordDto.Email);
                return Ok(new {message = "if an account with this email exists, a password reset link has been sent"});
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = System.Net.WebUtility.UrlEncode(token);

            var resetLink = $"https://frontendapp.com/reset-password?email={user.Email}&code={encodedToken};";

            var emailBody = $"Reset your password by <a href='{resetLink}'>clicking here</a>";
            await _emailService.SendEmailAsync(user.Email!, "Reset your password", emailBody);

            _logger.LogInformation("password reset link sent to {Email}", user.Email);
            return Ok(new {message = "if this acccount exists, you'll receive a link to reset your password"});
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto){
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if(user == null){
                return BadRequest("password request failed");
            }

            var decodedToken = System.Net.WebUtility.UrlDecode(resetPasswordDto.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);

            if(result.Succeeded){
                _logger.LogInformation("password reset successfully for user {Email}", resetPasswordDto.Email);

                return Ok(new {message = "Password has been reset successfully"});
            }

            foreach(var error in result.Errors){
                _logger.LogWarning("password reset failed for {Email}: {Error}", resetPasswordDto.Email, error.Description);
            }

            return BadRequest("password reset failed");
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto){
            // validate credentials

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if(user == null) return Unauthorized("Invalid email or password");

            if(!user.EmailConfirmed){
                return Unauthorized("Email not confirmed. Please check your inbox");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: false, lockoutOnFailure: true);

            if(result.Succeeded){
            var jwtId = Guid.NewGuid().ToString();
            var accessToken = GenerateJwtToken(user, jwtId);
            var refreshToken = await GenerateAndSaveRefreshToken(user, jwtId);

            _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
            return Ok(new TokenResponseDto {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            });
            }

            if(result.IsLockedOut){
                _logger.LogWarning("User account {Email locked out}", user.Email);
                return StatusCode(StatusCodes.Status403Forbidden, "This account has been locked, try again later");
            }

            return Unauthorized("Invalid email or password");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request){
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtToken;

            try{
                jwtToken = tokenHandler.ReadJwtToken(request.AccessToken);
            }
            catch{
                return BadRequest("Invalid access token format");
            }

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;   

            if (string.IsNullOrEmpty(jti)){
                return BadRequest("Invalid access token");
            }

            var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.JwtId == jti);
            if(storedRefreshToken == null){
                return BadRequest("Refresh token not found");
            }

            if(storedRefreshToken.IsUsed){
                await RevokeAllUserTokens(storedRefreshToken.UserId);
                return BadRequest("Refresh token has already been used. All tokens revoked for security");
            }

            if(storedRefreshToken.IsRevoked){
                return BadRequest("Refresh token has been revoked");
            }

            if(storedRefreshToken.ExpiryDate < DateTime.UtcNow){
                return BadRequest("refresh token has expired");
            }

            storedRefreshToken.IsUsed = true;
            await _context.SaveChangesAsync();


            var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
            if(user == null){
                return BadRequest("User not found");
            }

            var newJti = Guid.NewGuid().ToString();
            var newAccessToken = GenerateJwtToken(user, newJti);
            var newRefreshToken = await GenerateAndSaveRefreshToken(user, newJti);

            _logger.LogInformation("Tokens refreshed for user {Email}", user.Email);

            return Ok(new TokenResponseDto{
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            });
        }

        private string GenerateJwtToken(IdentityUser user, string jti){
            var claims = new[]{
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(7),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    
        private async Task<RefreshToken> GenerateAndSaveRefreshToken(IdentityUser user, string jwtId){
            var refreshToken = new RefreshToken{
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                JwtId = jwtId,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                IsUsed = false,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private async Task RevokeAllUserTokens(string userId){
            var userTokens = await _context.RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked).ToListAsync();

            foreach(var token in userTokens
            ){
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
            _logger.LogWarning("All tokens revoked for user {UserId} due to security measures", userId);
        }
    }
    }