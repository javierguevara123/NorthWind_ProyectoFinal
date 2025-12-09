using Microsoft.Extensions.Caching.Distributed;
using NorthWind.Membership.Backend.Core.Interfaces.Common;

namespace NorthWind.Membership.Backend.Core.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IDistributedCache _cache;
        private const string BlacklistPrefix = "blacklist_token_";

        public TokenBlacklistService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task AddToBlacklist(string token, TimeSpan? expiration = null)
        {
            var key = $"{BlacklistPrefix}{token}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(24)
            };

            await _cache.SetStringAsync(key, "blacklisted", options);
        }

        public async Task<bool> IsBlacklisted(string token)
        {
            var key = $"{BlacklistPrefix}{token}";
            var value = await _cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(value);
        }

        public async Task RemoveExpiredTokens()
        {
            // La cache distribuida maneja esto automáticamente
            await Task.CompletedTask;
        }
    }
}
