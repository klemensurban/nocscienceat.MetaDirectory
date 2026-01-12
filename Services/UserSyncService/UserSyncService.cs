using System;
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
    /// <summary>
    /// Coordinates IDM-to-AD user synchronization, applying custom filtering rules before diffing.
    /// </summary>
    public class UserSyncService : IUserSyncService
    {
        private readonly IIdmUserService _idmUserService;
        private readonly IAdService _adService;
        private readonly ILogger<UserSyncService> _logger;
        private readonly UserSyncServiceSettings _userSyncServiceSettings;

        /// <summary>
        /// Initializes the service and loads the strongly typed configuration section.
        /// </summary>
        public UserSyncService(IIdmUserService idmUserService, IAdService adService, IConfiguration configuration, ILogger<UserSyncService> logger)
        {
            _idmUserService = idmUserService;
            _adService = adService;
            _logger = logger;
            _userSyncServiceSettings = configuration.GetSection("UserSyncService").Get<UserSyncServiceSettings>() ??
                                       throw new System.ArgumentNullException(nameof(UserSyncServiceSettings));
        }

        /// <summary>
        /// Applies filtering rules, detects duplicates, and executes CUD operations between IDM and AD.
        /// </summary>
        public async Task SyncUsersAsync()
        {
            // Prepare lookup helpers for ignore lists and duplicate detection.
            HashSet<string> samAccountNamesToIgnore = new(_userSyncServiceSettings.SamAccountNamesToIgnore, StringComparer.OrdinalIgnoreCase);
            HashSet<string> sapPersNumbersToIgnore = new(_userSyncServiceSettings.SapPersNumbersToIgnore, StringComparer.OrdinalIgnoreCase);
            HashSet<string> seenSapPersNr = new();

            // Retrieve user collections from IDM and AD.
            IEnumerable<IdmUser> idmUsers = await _idmUserService.GetUsersAsync();
            List<AdUser> adUsers = _adService.GetAdUsers();

            bool hasDuplicateSapPersNr = false;

            // Iterate backwards so removal does not shift the yet-to-be-processed items.
            for (int i = adUsers.Count - 1; i >= 0; i--)
            {
                AdUser? adUser = adUsers[i];

                // Classify each AD user to decide whether to skip, warn, or flag errors before synchronization.
                string? action = adUser switch
                {
                    _ when !string.IsNullOrWhiteSpace(adUser.SamAccountName) && samAccountNamesToIgnore.Contains(adUser.SamAccountName!) => "IgnoreSamAccountName",
                    _ when string.IsNullOrWhiteSpace(adUser.SapPersNr) => "NoSapPersNr",
                    _ when !string.IsNullOrWhiteSpace(adUser.SapPersNr) && adUser.SapPersNr!.Trim().ToUpperInvariant() == "NOSAP" => "NoSapPersNr",
                    _ when !string.IsNullOrWhiteSpace(adUser.SapPersNr) && !seenSapPersNr.Add(adUser.SapPersNr!) => "DuplicateSapPersNr",
                    _ => null
                };

                // Execute the decided action and log the rationale.
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

            // Abort synchronization to avoid inconsistent updates when duplicates were detected.
            if (hasDuplicateSapPersNr)
            {
                _logger.LogError("User synchronization aborted due to duplicate SapPersNr entries in AD users.");
                return;
            }

            // Instantiate the generic CUD manager that finds delta sets between IDM and AD collections.
            CudManager<string, IdmUser, AdUser> userCudManager = new(new UserCudDataAdapter(_userSyncServiceSettings.CudAdapter, _logger), sourceItems: idmUsers, sync2Items: adUsers);
            userCudManager.CheckItems();

            // Push attribute differences back to AD for each matched user.
            foreach (var itemLink in userCudManager.Items2Update)
            {
                _adService.UpdateAdUser(itemLink.Sync2ItemUpdated, itemLink.DifferingProperties);
            }

            // Warn about AD accounts missing in IDM (potential cleanup candidates).
            foreach (AdUser? item in userCudManager.Items2Delete)
            {
                _logger.LogWarning("AD user not found in IDM: {SamAccountName}, DistinguishedName: {DistinguishedName}, SapPersNr: {SapPersNr}", item.SamAccountName, item.DistinguishedName, item.SapPersNr);
            }

            // Report IDM users missing in AD OUs, downgrading severity when explicitly ignored.
            foreach (IdmUser? item in userCudManager.Items2Create)
            {
                if (sapPersNumbersToIgnore.Contains(item.SapPersNr))
                    _logger.LogDebug("IDM user not found in specified AD OUs: {Sn} {GivenName}, SapPersNr: {SapPersNr}", item.Sn, item.GivenName, item.SapPersNr);
                else
                    _logger.LogWarning("IDM user not found in specified AD OUs: {Sn} {GivenName}, SapPersNr: {SapPersNr}", item.Sn, item.GivenName, item.SapPersNr);
            }

            // Summarize synchronization statistics for observability.
            _logger.LogInformation("Number of accounts in sync: {insync}", userCudManager.ItemsInSyncCount);
            _logger.LogInformation("Number of accounts updated: {updated}", userCudManager.Items2UpdateCount);
        }
    }
}
