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
            CudManager<string, IdmUser, AdUser> userCudManager = new(new UserCudDataAdapter(), idmUsers, adUsers);
            foreach (var itemLink in userCudManager.Items2Update)
            {
                _adService.UpdateAdUser(itemLink.Sync2ItemUpdated, itemLink.DifferingProperties);
            }

            foreach ( AdUser? item in userCudManager.Items2Delete)
            {
                _logger.LogWarning(" AD user not found in IDM: {SamAccountName}, DistinguishedName: {DistinguishedName}, SapPersNr: {SapPersNr}", item.SamAccountName, item.DistinguishedName, item.SapPersNr);

            }
            foreach (IdmUser? item in userCudManager.Items2Create)
            {
                _logger.LogWarning(" IDM user not found in OU WF or GAW: {Sn} {GivenName}, SapPersNr: {SapPersNr}", item.Sn, item.GivenName, item.SapPersNr);
            }
            var a = userCudManager.Items2Create;


        }
    }
}
