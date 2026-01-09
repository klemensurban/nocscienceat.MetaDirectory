using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.CudManager2;
using nocscienceat.MetaDirectory.Services.AdService;
using nocscienceat.MetaDirectory.Services.AdService.Models;
using nocscienceat.MetaDirectory.Services.IdmUserService;
using nocscienceat.MetaDirectory.Services.IdmUserService.Models;
using nocscienceat.MetaDirectory.Services.UserSyncService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory.Services.UserSyncService
{
    public class UserSyncService : IUserSyncService
    {
        private readonly IIdmUserService _idmUserService;
        private readonly IAdService _adService;
        private readonly ILogger<UserSyncService> _logger;
        private readonly UserSyncServiceSettings _userSyncServiceSettings;

        public UserSyncService(IIdmUserService idmUserService, IAdService adService, IConfiguration configuration, ILogger<UserSyncService> logger)
        {
            _idmUserService = idmUserService;
            _adService = adService;
            _logger = logger;
            _userSyncServiceSettings = configuration.GetSection("UserSyncService").Get<UserSyncServiceSettings>() ??
                                       throw new System.ArgumentNullException(nameof(UserSyncServiceSettings));
        }
        public async Task SyncUsersAsync()
        {
            HashSet<string> samAccountNamesToIgnore = new(_userSyncServiceSettings.SamAccountNamesToIgnore);
            HashSet<string> sapPersNumbersToIgnore = new(_userSyncServiceSettings.SapPersNumbersToIgnore);
            HashSet<string> seenSapPersNr = new();

            IEnumerable<IdmUser> idmUsers = await _idmUserService.GetUsersAsync();
            List<AdUser> adUsers = _adService.GetAdUsers();

            bool hasDuplicateSapPersNr = false;

            for (int i = adUsers.Count - 1; i >= 0; i--)
            {
                AdUser? adUser = adUsers[i];
                string? action = adUser switch
                {
                    _ when !string.IsNullOrWhiteSpace(adUser.SamAccountName) && samAccountNamesToIgnore.Contains(adUser.SamAccountName!) => "IgnoreSamAccountName",
                    _ when string.IsNullOrWhiteSpace(adUser.SapPersNr) => "NoSapPersNr",
                    _ when !string.IsNullOrWhiteSpace(adUser.SapPersNr) && adUser.SapPersNr!.Trim().ToUpperInvariant() == "NOSAP" => "NoSapPersNr",
                    _ when !string.IsNullOrWhiteSpace(adUser.SapPersNr) && !seenSapPersNr.Add(adUser.SapPersNr!) => "DuplicateSapPersNr",
                    _ => null
                };

                switch (action)
                {
                    case "IgnoreSamAccountName":
                        _logger.LogDebug("AD user with SamAccountName in ignore list will be ignored: {SamAccountName}, DistinguishedName: {DistinguishedName}", adUser.SamAccountName, adUser.DistinguishedName);
                        adUsers.RemoveAt(i);
                        break;
                    case "NoSapPersNr":
                        _logger.LogWarning("AD user without SapPersNr will be ignored: {SamAccountName}, DistinguishedName: {DistinguishedName}", adUser.SamAccountName, adUser.DistinguishedName);
                        adUsers.RemoveAt(i);
                        break;
                    case "DuplicateSapPersNr":
                        hasDuplicateSapPersNr = true;
                        _logger.LogError("Duplicate SapPersNr found in AD users: {SapPersNr}, SamAccountName: {SamAccountName}, DistinguishedName: {DistinguishedName}", adUser.SapPersNr, adUser.SamAccountName, adUser.DistinguishedName);
                        break;
                }
            }

            if (hasDuplicateSapPersNr)
            {
                _logger.LogError("User synchronization aborted due to duplicate SapPersNr entries in AD users.");
                return;
            }

            CudManager<string, IdmUser, AdUser> userCudManager = new(new UserCudDataAdapter(_userSyncServiceSettings.CudAdapter, _logger), sourceItems: idmUsers, sync2Items: adUsers);
            userCudManager.CheckItems();
            foreach (var itemLink in userCudManager.Items2Update)
            {
                _adService.UpdateAdUser(itemLink.Sync2ItemUpdated, itemLink.DifferingProperties);
            }

            foreach ( AdUser? item in userCudManager.Items2Delete)
            {
                _logger.LogWarning("AD user not found in IDM: {SamAccountName}, DistinguishedName: {DistinguishedName}, SapPersNr: {SapPersNr}", item.SamAccountName, item.DistinguishedName, item.SapPersNr);
            }

            foreach (IdmUser? item in userCudManager.Items2Create)
            {
                if (sapPersNumbersToIgnore.Contains(item.SapPersNr)) 
                    _logger.LogDebug("IDM user not found in specified AD OUs: {Sn} {GivenName}, SapPersNr: {SapPersNr}", item.Sn, item.GivenName, item.SapPersNr);
                else
                    _logger.LogWarning("IDM user not found in specified AD OUs: {Sn} {GivenName}, SapPersNr: {SapPersNr}", item.Sn, item.GivenName, item.SapPersNr);
            }
            _logger.LogInformation("Number of accounts in sync: {insync}", userCudManager.ItemsInSyncCount);
            _logger.LogInformation("Number of accounts updated: {updated}", userCudManager.Items2UpdateCount);

        }
    }
}
