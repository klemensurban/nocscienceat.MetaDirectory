using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.CudManager2;
using nocscienceat.MetaDirectory.Services.AdService;
using nocscienceat.MetaDirectory.Services.AdService.Models;
using nocscienceat.MetaDirectory.Services.IdmUserService;
using nocscienceat.MetaDirectory.Services.IdmUserService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            CudManager<string, IdmUser, AdUser> userCudManager = new CudManager<string, IdmUser, AdUser>(new UserCudDataAdapter(), idmUsers, adUsers);
            var a = userCudManager.Items2Create;


        }
    }
}
