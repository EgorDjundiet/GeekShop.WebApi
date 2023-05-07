using Microsoft.Extensions.Options;

namespace GeekShop.Domain.Settings
{
    public interface IDbSettings
    {
        public string? ConnectionString { get; }
    }

    public class DbSettings : IDbSettings
    {
        IOptions<DbOptions> _options;

        public DbSettings(IOptions<DbOptions> options)
        {
            _options = options;
        }

        public string? ConnectionString => _options.Value.ConnectionString;
    }
}
