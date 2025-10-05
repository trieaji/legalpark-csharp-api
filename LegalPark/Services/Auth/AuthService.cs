using LegalPark.Exception;
using LegalPark.Helpers;
using LegalPark.Models.DTOs.Request.Notification;
using LegalPark.Models.DTOs.Request.User;
using LegalPark.Models.DTOs.Response.User;
using LegalPark.Models.Entities;
using LegalPark.Repositories.LogVerification;
using LegalPark.Repositories.User;
using LegalPark.Security.Jwt;
using LegalPark.Services.Notification;
using LegalPark.Services.Template;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LegalPark.Services.Auth
{
    public class AuthService : IAuthService
    {
        
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly UserManager<LegalPark.Models.Entities.User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly INotificationService _notificationService;
        private readonly ILogVerificationRepository _logVerificationRepository;
        private readonly ITemplateService _templateService;

        
        public AuthService(IUserRepository userRepository, IJwtService jwtService, UserManager<LegalPark.Models.Entities.User> userManager, RoleManager<IdentityRole<Guid>> roleManager,
            INotificationService notificationService,
            ILogVerificationRepository logVerificationRepository, ITemplateService templateService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _userManager = userManager;
            _roleManager = roleManager;
            _notificationService = notificationService;
            _logVerificationRepository = logVerificationRepository;
            _templateService = templateService;
        }

        
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // 1. Search for users by email.
            var user = await _userRepository.findByEmail(request.Email);

            // 2. Check whether the user is found and the password matches.
            // If the user is not found or the password is incorrect, return an error response.
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                
                return ResponseHandler.GenerateResponseError(
                    HttpStatusCode.Unauthorized,
                    new { Message = "Invalid email or password." },
                    "Unauthorized"
                );
            }

            // 3. If authentication is successful, create a token and refresh token.
            
            string token = await _jwtService.generateToken(user);
            string refreshToken = await _jwtService.generateRefreshToken(user.Id);

            // 4. Create a SignResponse object for a successful response.
            
            var response = new SignResponse
            {
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                RefreshToken = refreshToken
            };

            
            return ResponseHandler.GenerateResponseSuccess(response);
        }


        
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var user = new LegalPark.Models.Entities.User
            {
                UserName = request.AccountName,
                Email = request.Email,
                AccountName = request.AccountName,
                //Role = Role.USER,
                AccountStatus = AccountStatus.PENDING_VERIFICATION,
                Balance = 100000.00m
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            
            var roleExists = await _roleManager.RoleExistsAsync("User");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("User"));
            }

            
            await _userManager.AddToRoleAsync(user, "User");
            

            
            var verification = new LogVerification
            {
                Code = GenerateOtp.GenerateRandomNumber(),
                UserId = user.Id,
                Expired = GenerateOtp.GetExpiryDate(),
                IsVerify = false
            };

            
            await _logVerificationRepository.AddAsync(verification);

            
            await _logVerificationRepository.SaveChangesAsync();

            // ============= Send Email (using Template Service) =================
            // Create a dictionary for template variables
            var templateVariables = new Dictionary<string, object>
            {
                { "otp", verification.Code },
                { "name", user.AccountName }
            };

            // Call the template service to process the template
            string emailBody = await _templateService.ProcessEmailTemplateAsync("email_verification", templateVariables);

            // Send an email with a processed body
            var emailRequest = new EmailNotificationRequest
            {
                To = user.Email,
                Subject = "Account Verification",
                Body = emailBody
            };

            await _notificationService.SendEmailNotification(emailRequest);

            // Generate a token after the user has been successfully created
            var token = await _jwtService.generateToken(user);

            var response = new RegisterResponse
            {
                Email = user.Email,
                Token = token
            };

            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Registration successful.", response);
        }


        
        public async Task<IActionResult> UpdateUserRole(UpdateRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
            }

            // Check whether the new role is valid (Admin or User)
            if (request.Role.ToLower() != "admin" && request.Role.ToLower() != "user")
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.BadRequest, "FAILED", "Invalid role specified.");
            }

            // Get all current user roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Delete old roles
            if (currentRoles.Any())
            {
                var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!result.Succeeded)
                {
                    return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // Add a new role
            var addRoleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!addRoleResult.Succeeded)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.InternalServerError, "FAILED", string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            }

            // Generate a new token
            var claims = (await _userManager.GetClaimsAsync(user)).ToList();
            var newClaims = (await _userManager.GetClaimsAsync(user)).ToList();
            newClaims.Add(new Claim(ClaimTypes.Role, request.Role));

            var newJwtToken = await _jwtService.generateToken(user, newClaims);

            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "SUCCESS", new { Message = "User role updated successfully.", Token = newJwtToken });
        }

        
        public async Task<IActionResult> Logout(string email)
        {
            var user = await _userRepository.findByEmail(email);
            if (user == null)
            {
                return ResponseHandler.GenerateResponseError(HttpStatusCode.NotFound, "FAILED", "User not found.");
            }

            user.Balance = 100000.00m;
            user.UpdatedAt = DateTime.Now;

            
            _userRepository.Update(user);

            
            await _userRepository.SaveChangesAsync();

            return ResponseHandler.GenerateResponseSuccess(HttpStatusCode.OK, "Logout successful. Your virtual balance has been reset to 100k.", null);
        }

    }
}
