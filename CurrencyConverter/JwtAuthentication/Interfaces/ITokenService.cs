namespace CurrencyConverter.API.JwtAuthentication.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userId, List<string> roles);
        string GenerateRefreshToken();
        void RemoveRefreshToken(string refreshToken);
        void RemoveRefreshTokenByUserId(string userId);
        string GetUserIdFromRefreshToken(string refreshToken);
    }
}
