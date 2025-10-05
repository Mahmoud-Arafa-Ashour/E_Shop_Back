using E_Shop.Core.Abstractions;
using E_Shop.Core.Authentications;
using E_Shop.Core.Persistent;
using E_Shop.Models.Customes.Contracts.Auth;
using E_Shop.Models.Entities;
using E_Shop.Service.IServices;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using static E_Shop.Core.Abstractions.Errors;
using RegisterRequest = E_Shop.Models.Customes.Contracts.login.RegisterRequest;

namespace E_Shop.Service.Services;

public class AuthServices(UserManager<ApplicationUser> userManager
    , IJwtProvidor jwtProvidor
    , ILogger<ApplicationUser> logger
    , SignInManager<ApplicationUser> signInManager
    , IHttpContextAccessor httpContextAccessor
    , ApplicationDbContext dbContext) : IAuthServices
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvidor _jwtProvidor = jwtProvidor;
    private readonly ILogger<ApplicationUser> _logger = logger;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly int _refreshTokenExpiration = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        //check user existance 
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentionals);
        //check if the user is disabled
        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);
        //check password 
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
        if (result.Succeeded)
        {
            user.LastLogin = DateTime.UtcNow;
            var (userRoles, userPermissions) = await GetRolesAndPermissions(user, cancellationToken);
            //generate token
            var (token, expiresin) = _jwtProvidor.GenerateToken(user, userRoles, userPermissions);
            //generate Refresh Token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(_refreshTokenExpiration);
            //save refresh token to database 
            user.RefreshTokens.Add(new RefreshTokens
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpirationDate
            });
            await _userManager.UpdateAsync(user);
            var response = new AuthResponse(user.Id, user.Email!, user.Name, user.Adress, user.PhoneNumber, token, expiresin, refreshToken, refreshTokenExpirationDate , DateTime.UtcNow);
            return Result.Success(response);
        }
        var error = result.IsNotAllowed ?
            UserErrors.EmailNotConfirmed :
            result.IsLockedOut ?
            UserErrors.UserLocked :
            UserErrors.InvalidCredentionals;

        return Result.Failure<AuthResponse>(error);
    }
    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken)
    {
        //check for the valid token 
        var userId = jwtProvidor.ValidateToken(token);
        if (userId is null) return Result.Failure<AuthResponse>(TokenErrors.EmptyToken);
        //check for the id
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Failure<AuthResponse>(TokenErrors.EmptyToken);
        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.UserLocked);
        //check for the refresh token 
        var UserrefreshToken = user.RefreshTokens?.FirstOrDefault(x => x.Token == refreshToken && x.IsActive);
        if (UserrefreshToken == null) return Result.Failure<AuthResponse>(TokenErrors.EmptyToken);
        //make it valid for one time 
        UserrefreshToken.RevokedOn = DateTime.UtcNow;
        //Make the creation of both refreshtoken and token
        var (userRoles, userPermissions) = await GetRolesAndPermissions(user, cancellationToken);
        var (newtoken, expiresin) = _jwtProvidor.GenerateToken(user, userRoles, userPermissions);
        var newrefreshToken = GenerateRefreshToken();
        var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(_refreshTokenExpiration);
        //save refresh token to database 
        user.RefreshTokens!.Add(new RefreshTokens
        {
            Token = newrefreshToken,
            ExpiresOn = refreshTokenExpirationDate
        });
        await _userManager.UpdateAsync(user);
        var response = new AuthResponse(user.Id, user.Email!, user.Name, user.Adress, user.PhoneNumber, newtoken, expiresin, newrefreshToken, refreshTokenExpirationDate , DateTime.UtcNow);
        return Result.Success(response);
    }
    public async Task<Result> RevokeRefreshToken(string Token, string refreshToken, CancellationToken cancellationToken)
    {
        //check for the valid token 
        var userId = jwtProvidor.ValidateToken(Token);
        if (userId is null) return Result.Failure(TokenErrors.EmptyToken);
        //check for the id
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Result.Failure(TokenErrors.EmptyToken);
        //check for the refresh token 
        var UserrefreshToken = user.RefreshTokens?.FirstOrDefault(x => x.Token == refreshToken && x.IsActive);
        if (UserrefreshToken == null) return Result.Failure(TokenErrors.EmptyToken);
        //make it valid for one time 
        UserrefreshToken.RevokedOn = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        return Result.Success();
    }
    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        //check if the email is Duplicated
        var IsExistedEmail = await _userManager.Users.AnyAsync(x => x.Email == request.Email);
        if (IsExistedEmail)
            return Result.Failure(UserErrors.DuplicateEmail);
        var user = request.Adapt<ApplicationUser>();
        user.UserName = request.Email;
        user.Adress = request.Address;
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        //check if the user exist

        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);
        //check if the email is already confirmed
        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicateConfirmation);
        //getting code
        var code = request.Code;
        //Encode the code
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }
        //confirm the email
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            _logger.LogInformation("User with email {Email} has been confirmed", user.Email);
            await _userManager.AddToRoleAsync(user, DefaultRoles.Owner);
            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    private static string GenerateRefreshToken() =>
         Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    private async Task<(IEnumerable<string> Roles, IEnumerable<string> Permissions)> GetRolesAndPermissions(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var userPermissions = await (from r in _dbContext.Roles
                                     join p in _dbContext.RoleClaims
                                     on r.Id equals p.RoleId
                                     where userRoles.Contains(r.Name!)
                                     select p.ClaimValue!)
                                     .Distinct()
                                     .ToListAsync(cancellationToken);
        return (userRoles, userPermissions);
    }
}