using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.MetaDirectory.Services.IdmUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nocscienceat.MetaDirectory.Services.IdmUserService.Models;
using nocscienceat.CudManager2;
using nocscienceat.MetaDirectory.Services.AdService;
using nocscienceat.MetaDirectory.Services.AdService.Models;

namespace nocscienceat.MetaDirectory.Services.UserSyncService
{
    public class UserSyncService : IUserSyncService
    {
        private readonly IIdmUserService _idmUserService;
        private readonly IAdService _adService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Dispatcher> _logger;

        public UserSyncService(IIdmUserService idmUserService, IAdService adService, IConfiguration configuration, ILogger<Dispatcher> logger)
        {
            _idmUserService = idmUserService;
            _adService = adService;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task SyncUsersAsync()
        {
            IEnumerable<IdmUser> idmUsers = await _idmUserService.GetUsersAsync();
            IEnumerable<AdUser> adUsers = _adService.GetAdUsers();
            var a = 1; // Placeholder for further implementation
        }
    }
}
