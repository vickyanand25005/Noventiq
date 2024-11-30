using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NoventiqApplication.Interface;
using NoventiqApplication.Services;

namespace NoventiqApplication.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public AuthenticationController(IUserService userService, ITokenService tokenService, IUserRepository userRepository, IMapper mapper)
        {
            _userService = userService;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var users = await _userService.GetAllUsersAsync();
            var userDto = users.FirstOrDefault(u => u.Username.ToLower() == request.Username.ToLower());
            var user = _mapper.Map<User>(userDto);
            if (user == null || !_userService.VerifyPassword(user.PasswordHash, request.Password))
                return Unauthorized("Invalid credentials.");

            var token = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);
            await _userRepository.AddRefreshTokenAsync(refreshToken);

            return Ok(new TokenResponse { Token = token, RefreshToken = refreshToken.Token });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var refreshToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken);

            if (refreshToken == null || refreshToken.IsExpired)
                return Unauthorized("Invalid or expired refresh token.");

            var newToken = _tokenService.GenerateToken(refreshToken.User);
            var newRefreshToken = _tokenService.GenerateRefreshToken(refreshToken.UserId);
            await _userRepository.AddRefreshTokenAsync(newRefreshToken);

            return Ok(new TokenResponse { Token = newToken, RefreshToken = newRefreshToken.Token });
        }
    }
}
