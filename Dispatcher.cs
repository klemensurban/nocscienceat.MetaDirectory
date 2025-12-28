using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.MetaDirectory.Services.UserSyncService;
using System.Threading;
using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory
{
    public class Dispatcher
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Dispatcher> _logger;
        private readonly IUserSyncService _userSyncService;

        // ReSharper disable once ConvertToPrimaryConstructor
        public Dispatcher(IConfiguration configuration, ILogger<Dispatcher> logger, IUserSyncService userSyncService)
        {
            _configuration = configuration;
            _logger = logger;
            _userSyncService = userSyncService;
        }

        public async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
        {
            await _userSyncService.SyncUsersAsync();

            // Sync Computer-Account Status in the future here as well

            return Task.CompletedTask;
        }
    }
}
