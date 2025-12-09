namespace NorthWind.Membership.Backend.Core.Interfaces.Common
{
    public interface ITokenBlacklistService
    {
        Task AddToBlacklist(string token, TimeSpan? expiration = null);
        Task<bool> IsBlacklisted(string token);
        Task RemoveExpiredTokens();
    }
}
