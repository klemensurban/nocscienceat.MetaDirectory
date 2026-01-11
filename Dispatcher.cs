using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.MetaDirectory.Services.UserSyncService;
using System.Threading;
using System.Threading.Tasks;
using nocscienceat.MetaDirectory.Services.ComputerSyncService;

namespace nocscienceat.MetaDirectory
{
    /// <summary>
    /// Central coordinator that triggers all registered sync services in sequence.
    /// </summary>
    public class Dispatcher
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Dispatcher> _logger;
        private readonly IUserSyncService _userSyncService;
        private readonly IComputerSyncService _computerSyncService;

        // ReSharper disable once ConvertToPrimaryConstructor
        /// <summary>
        /// Captures infrastructure services and concrete sync pipelines.
        /// </summary>
        public Dispatcher(IConfiguration configuration, ILogger<Dispatcher> logger, IUserSyncService userSyncService, IComputerSyncService computerSyncService)
        {
            _configuration = configuration;
            _logger = logger;
            _userSyncService = userSyncService;
            _computerSyncService = computerSyncService;
        }

        /// <summary>
        /// Executes user and computer synchronization jobs; currently sequential with no cancellation logic.
        /// </summary>
        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _userSyncService.SyncUsersAsync();

            await _computerSyncService.SyncComputersAsync();

        }
    }
}
