

using E_Shop.Models.Customes.Contracts.Auth;
using E_Shop.Models.Customes.Contracts.login;
using E_Shop.Service.IServices;
using Microsoft.AspNetCore.Mvc;
using E_Shop.Core.Abstractions;

namespace E_Shop.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IAuthServices _authServices;
    private readonly ILogger<LoginController> _logger;

    public LoginController(IAuthServices authServices, ILogger<LoginController> logger)
    {
        _authServices = authServices;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> LoginAsync([FromBody]LoginRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authServices.GetTokenAsync(request.Email, request.Password, cancellationToken);
        return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
    }
    [HttpPost]
    public async Task<IActionResult> RefreshTokenAsync([FromBody]RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResponse = await _authServices.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
        return authResponse.IsSuccess ?
           Ok() :
           authResponse.ToProblem();
    }
    [HttpPost]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authServices.RevokeRefreshToken(request.Token, request.RefreshToken, cancellationToken);
        return authResult.IsSuccess ?
            Ok() :
            authResult.ToProblem();
    }
    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authServices.RegisterAsync(request, cancellationToken);
        return result.IsSuccess ?
            Ok() :  
            result.ToProblem();
    }
    [HttpGet]
    public async Task<IActionResult> ConfirmEmailGet(ConfirmEmailRequest request)
    {
        var result = await _authServices.ConfirmEmailAsync(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
