using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.MetaDirectory.Services.UserSyncService;
using System.Threading;
using System.Threading.Tasks;
using nocscienceat.MetaDirectory.Services.ComputerSyncService;

namespace nocscienceat.MetaDirectory
{
    public class Dispatcher
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Dispatcher> _logger;
        private readonly IUserSyncService _userSyncService;
        private readonly IComputerSyncService _computerSyncService;

        // ReSharper disable once ConvertToPrimaryConstructor
        public Dispatcher(IConfiguration configuration, ILogger<Dispatcher> logger, IUserSyncService userSyncService, IComputerSyncService computerSyncService)
        {
            _configuration = configuration;
            _logger = logger;
            _userSyncService = userSyncService;
            _computerSyncService = computerSyncService;
        }

        public async Task<Task> ExecuteAsync(CancellationToken stoppingToken)
        {
            await _userSyncService.SyncUsersAsync();

            // Sync Computer-Account Status in the future here as well

            await _computerSyncService.SyncComputersAsync();

            return Task.CompletedTask;
        }
    }
}
