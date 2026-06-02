using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartDelivery.Application.Common;
using SmartDelivery.Application.DTOs.Auth;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Application.Interfaces.Services;
using SmartDelivery.Domain.Entities;

namespace SmartDelivery.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IPasswordHasher<User> passwordHasher,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
                    return Result<AuthResponse>.Failure("Email is already registered.");

                var role = await _unitOfWork.Roles.GetByNameAsync(request.Role, cancellationToken);
                if (role == null)
                    return Result<AuthResponse>.Failure($"Role {request.Role} not found.");

                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email.ToLowerInvariant(),
                    PhoneNumber = request.PhoneNumber
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

                await _unitOfWork.Users.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.Users.AssignRoleAsync(user.Id, role.Id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var roles = new[] { request.Role };
                return await GenerateAuthResponseAsync(user, roles, cancellationToken);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                return Result<AuthResponse>.Failure("Registration failed. Please try again.", 500);
            }
        }
        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
                if (user == null)
                    return Result<AuthResponse>.Failure("Invalid email or password.", 401);

                if (!user.IsActive)
                    return Result<AuthResponse>.Failure("Account is deactivated. Plesae contact support", 403);

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
                if (result == PasswordVerificationResult.Failed)
                    return Result<AuthResponse>.Failure("Invalid email or password", 401);

                var roles = await _unitOfWork.Users.GetUserRolesAsync(user.Id, cancellationToken);
                return await GenerateAuthResponseAsync(user, roles, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return Result<AuthResponse>.Failure("Login failed. Please try again.", 500);
            }
        }
        public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
                if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
                    return Result<AuthResponse>.Unauthorized("Invalid or expired refresh token.");

                var roles = await _unitOfWork.Users.GetUserRolesAsync(user.Id, cancellationToken);
                return await GenerateAuthResponseAsync(user, roles, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token.");
                return Result<AuthResponse>.Failure("Token refresh failed.", 500);
            }
        }
        public async Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user is null)
                return Result.NotFound();

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Logged out successfully.");
        }
        public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null)
                return Result.NotFound("User not found.");

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
            if (verify == PasswordVerificationResult.Failed)
                return Result.Failure("Current password is incorrect.");

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Password changed successfully.");
        }

        private async Task<Result<AuthResponse>> GenerateAuthResponseAsync(User user, IEnumerable<string> roles, CancellationToken cancellationToken)
        {
            var roleList = roles.ToList();
            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, roleList);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(60);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new AuthResponse
            (
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                ExpiresAt: expiresAt,
                User: new UserTokenDto(user.Id, user.FullName, user.Email, user.PhoneNumber, roleList)
            );
            return Result<AuthResponse>.Success(response);
        }

    }


}
