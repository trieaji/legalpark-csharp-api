using LegalPark.Exception;
using LegalPark.Models.DTOs.Request.Notification;
using LegalPark.Models.DTOs.Request.User;
using LegalPark.Models.Entities;
using LegalPark.Repositories.LogVerification;
using LegalPark.Repositories.User;
using LegalPark.Security.Jwt;
using LegalPark.Services.Auth;
using LegalPark.Services.Notification;
using LegalPark.Services.Template;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace LegalPark.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ILogVerificationRepository> _logVerificationRepositoryMock;
        private readonly Mock<ITemplateService> _templateServiceMock;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _logVerificationRepositoryMock = new Mock<ILogVerificationRepository>();
            _templateServiceMock = new Mock<ITemplateService>();
        }

        [Fact]
        public async Task Register_ShouldReturnSuccess_WhenUserCreated()
        {
            // Arrange
            var userManagerMock = MockHelpers.MockUserManager<User>();
            var roleManagerMock = MockHelpers.MockRoleManager<IdentityRole<Guid>>();

            // Setup UserManager untuk sukses create user
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            // Setup RoleManager role sudah ada
            roleManagerMock.Setup(x => x.RoleExistsAsync("User"))
                .ReturnsAsync(true);

            // Setup JWT Service
            _jwtServiceMock.Setup(x =>
    x.generateToken(
        It.IsAny<User>(),
        It.IsAny<IList<Claim>>() // untuk optional param
    ))
    .ReturnsAsync("fake-jwt-token");


            // Setup LogVerification Repository
            _logVerificationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<LogVerification>()))
                .Returns(Task.CompletedTask);

            _logVerificationRepositoryMock.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Setup Template Service
            _templateServiceMock.Setup(x =>
                x.ProcessEmailTemplateAsync("email_verification", It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync("fake-email-body");

            // Setup Notification Service
            var fakeResult = ResponseHandler.GenerateResponseSuccess(
    HttpStatusCode.OK,
    "Fake email sent",
    null);

            _notificationServiceMock.Setup(x =>
                x.SendEmailNotification(It.IsAny<EmailNotificationRequest>()))
                .ReturnsAsync(fakeResult);


            // Buat AuthService
            var authService = new AuthService(
                _userRepositoryMock.Object,
                _jwtServiceMock.Object,
                userManagerMock.Object,
                roleManagerMock.Object,
                _notificationServiceMock.Object,
                _logVerificationRepositoryMock.Object,
                _templateServiceMock.Object
            );

            // Buat RegisterRequest dummy
            var registerRequest = new RegisterRequest
            {
                Email = "test@example.com",
                AccountName = "TestUser",
                Password = "Password123!"
            };

            // Act
            var result = await authService.Register(registerRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // pastikan return OkObjectResult
            Assert.Equal(200, okResult.StatusCode); // status code 200
            Assert.Contains("Registration successful", okResult.Value.ToString()); // cek isi response
        }




        public async Task Login_ShouldReturnSuccess_WhenCredentialsValid()
        {
            // Arrange
            var userManagerMock = MockHelpers.MockUserManager<User>();
            var roleManagerMock = MockHelpers.MockRoleManager<IdentityRole<Guid>>();

            // Buat user dummy
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                UserName = "test",
                Role = Role.USER
            };

            // Mock userRepository.findByEmail
            _userRepositoryMock.Setup(x => x.findByEmail(It.IsAny<string>()))
                .ReturnsAsync(user);

            // Mock check password di UserManager
            userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Mock JWT service generate token
            _jwtServiceMock.Setup(x =>
   x.generateToken(
       It.IsAny<User>(),
       It.IsAny<IList<Claim>>() // untuk optional param
   ))
   .ReturnsAsync("fake-jwt-token");

            // Mock JWT service generate refresh token
            _jwtServiceMock.Setup(x => x.generateRefreshToken(It.IsAny<Guid>()))
                .ReturnsAsync("fake-refresh-token");

            // Instansiasi AuthService
            var authService = new AuthService(
                _userRepositoryMock.Object,
                _jwtServiceMock.Object,
                userManagerMock.Object,
                roleManagerMock.Object,
                _notificationServiceMock.Object,
                _logVerificationRepositoryMock.Object,
                _templateServiceMock.Object
            );

            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = await authService.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result); // karena ResponseHandler.GenerateResponseSuccess -> ObjectResult
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);

            // Kamu juga bisa cek data di Value-nya
            // dynamic value = okResult.Value;
            // Assert.Equal("test@example.com", value.Email);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
        {
            // Arrange
            var userManagerMock = MockHelpers.MockUserManager<User>();
            var roleManagerMock = MockHelpers.MockRoleManager<IdentityRole<Guid>>();

            // Mock userRepository.findByEmail ? user tidak ditemukan
            _userRepositoryMock.Setup(x => x.findByEmail(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            var authService = new AuthService(
                _userRepositoryMock.Object,
                _jwtServiceMock.Object,
                userManagerMock.Object,
                roleManagerMock.Object,
                _notificationServiceMock.Object,
                _logVerificationRepositoryMock.Object,
                _templateServiceMock.Object
            );

            var loginRequest = new LoginRequest
            {
                Email = "wrong@example.com",
                Password = "wrongpassword"
            };

            // Act
            var result = await authService.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, unauthorizedResult.StatusCode);
        }
    }
}
