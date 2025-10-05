using E_Shop.Core.Abstractions;
using E_Shop.Models.Customes.Contracts.Auth;
using E_Shop.Models.Customes.Contracts.login;

namespace E_Shop.Service.IServices;
public interface IAuthServices
{
    Task<Result<AuthResponse>> GetTokenAsync(string email , string password , CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken);
    Task<Result> RevokeRefreshToken(string Token, string refreshToken, CancellationToken cancellationToken);
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
}
