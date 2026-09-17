using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietySaaS.Application.Common.DTOs;
using SocietySaaS.Application.Common.Interfaces;
using SocietySaaS.Application.Services;
using SocietySaaS.Shared;

namespace SocietySaaS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUser;

    public AuthController(IAuthService authService, ICurrentUserService currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success)
                return Unauthorized(ApiResponse<object>.Fail(result.Message));

            return Ok(ApiResponse<LoginResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] OtpRequestDto request)
    {
        try
        {
            var result = await _authService.RequestOtpAsync(request.Email);
            if (!result.Success)
                return BadRequest(ApiResponse<object>.Fail(result.Message));

            return Ok(ApiResponse<LoginResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        try
        {
            var result = await _authService.VerifyOtpAsync(request.Email, request.Code);
            if (!result.Success)
                return Unauthorized(ApiResponse<object>.Fail(result.Message));

            return Ok(ApiResponse<LoginResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (!result.Success)
                return Unauthorized(ApiResponse<object>.Fail(result.Message));

            return Ok(ApiResponse<LoginResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.Success)
                return Conflict(ApiResponse<object>.Fail(result.Message));

            return Ok(ApiResponse<LoginResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            if (!_currentUser.IsAuthenticated)
                return Unauthorized();

            var user = await _authService.GetMeAsync(_currentUser.UserId?.ToString() ?? "");
            if (user == null) return NotFound(ApiResponse<object>.Fail("User not found"));

            return Ok(ApiResponse<object>.Ok(user));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            if (!_currentUser.IsAuthenticated) return Unauthorized();

            var result = await _authService.ChangePasswordAsync(_currentUser.UserId?.ToString() ?? "", request);
            if (!result.Success)
                return BadRequest(ApiResponse<object>.Fail(result.Message));

            return Ok(ApiResponse<object>.Ok(new { message = result.Message }));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }
}

public record OtpRequestDto(string Email);
public record VerifyOtpRequest(string Email, string Code);
public record RefreshTokenRequest(string RefreshToken);
