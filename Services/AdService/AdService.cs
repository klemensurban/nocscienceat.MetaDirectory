using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.MetaDirectory.Services.AdService.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace nocscienceat.MetaDirectory.Services.AdService
{
    public class AdService : IAdService
    {
        private readonly AdServiceSettings _adServiceSettings;
        private readonly ILogger<AdService> _logger;

        public AdService(IConfiguration configuration, ILogger<AdService> logger)
        {
            _adServiceSettings = configuration.GetSection("AdService").Get<AdServiceSettings>() ??
                                 throw new ArgumentNullException(nameof(AdServiceSettings));
            _logger = logger;
        }

        public List<AdUser> GetAdUsers()
        {
            List<AdUser> adUsers = new();
            try
            {
                foreach (DirectorySearch directorySearch in _adServiceSettings.UserOUs)
                {
                    using PrincipalContext ctx = new(ContextType.Domain, null, directorySearch.Dn);
                    using PrincipalSearcher principalSearcher = new();
                    principalSearcher.QueryFilter = new UserPrincipal(ctx);
                    using DirectorySearcher searcher = (DirectorySearcher) principalSearcher.GetUnderlyingSearcher();
                    searcher.SearchScope =
                        directorySearch.SearchScopeSubtree ? SearchScope.Subtree : SearchScope.OneLevel;
                    searcher.PageSize = 1000;
                    searcher.PropertiesToLoad.Clear();
                    searcher.PropertiesToLoad.AddRange(new[]
                    {
                        "SamAccountName", "distinguishedname", "SAPPersNr", "bPKBF",
                        "sn", "GivenName", "mail", "telephonenumber", "mobile", "title", "extensionAttribute5",
                        "extensionAttribute6", "extensionAttribute7", "extensionAttribute14"
                    });
                    using SearchResultCollection resultCollection = searcher.FindAll();
                    foreach (SearchResult searchResult in resultCollection)
                    {
                        AdUser adUser = MapSearchResult2AdUser(searchResult);
                        adUsers.Add(adUser);
                    }
                }
                return adUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching AD users");
                throw;
            }
        }

        private static string? GetPropertyValue(SearchResult searchResult, string propertyName)
        {
            return searchResult.Properties[propertyName]?.Count > 0
                ? searchResult.Properties[propertyName][0]?.ToString()
                : null;
        }

        private static AdUser MapSearchResult2AdUser(SearchResult searchResult)
        {
            AdUser adUser = new()
            {
                SamAccountName = GetPropertyValue(searchResult, "SamAccountName"),
                DistinguishedName = GetPropertyValue(searchResult, "distinguishedname"),
                SapPersNr = GetPropertyValue(searchResult, "SAPPersNr"),
                BpkBf = GetPropertyValue(searchResult, "bPKBF"),
                Sn = GetPropertyValue(searchResult, "sn"),
                GivenName = GetPropertyValue(searchResult, "GivenName"),
                Mail = GetPropertyValue(searchResult, "mail"),
                TelephoneNumber = GetPropertyValue(searchResult, "telephonenumber"),
                Mobile = GetPropertyValue(searchResult, "mobile"),
                Title = GetPropertyValue(searchResult, "title"),
                StreetAddress = GetPropertyValue(searchResult, "extensionAttribute5"),
                Room = GetPropertyValue(searchResult, "extensionAttribute6"),
                TopLevelUnits = GetPropertyValue(searchResult, "extensionAttribute7"),
                JobRole = GetPropertyValue(searchResult, "extensionAttribute14")
            };
            return adUser;
        }

        public void UpdateAdUser(AdUser adUserUpdated, AdUser adUserCurrent, IEnumerable<string> attributeNames)
        {
            try
            {
                using PrincipalContext context = new(ContextType.Domain);
                using UserPrincipal? userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName,
                    adUserUpdated.SamAccountName!);
                if (userPrincipal != null)
                {
                    bool updateRequired = false;

                    using DirectoryEntry directoryEntry = (DirectoryEntry) userPrincipal.GetUnderlyingObject();

                    foreach (string attributeName in attributeNames)
                    {


                        string attributeValue = string.Empty;
                        switch (attributeName)
                        {

                            case nameof(AdUser.BpkBf):
                                // updateRequired = true;

                                break;
                            case nameof(AdUser.Sn):
                                _logger.LogDebug("AD user '{Dn}': sn '{Sn}' differs from Synchronization Source '{SnSource}'",
                                    adUserCurrent.DistinguishedName, adUserCurrent.Sn ?? string.Empty, adUserUpdated.Sn ?? string.Empty);
                                break;
                            case nameof(AdUser.GivenName):
                                _logger.LogDebug("AD user '{Dn}': GivenName'{Gn}' differs from Synchronization Source '{GnSource}'",
                                    adUserCurrent.DistinguishedName, adUserCurrent.GivenName ?? string.Empty, adUserUpdated.GivenName ?? string.Empty);
                                break;
                            case nameof(AdUser.Mail):
                                _logger.LogWarning("AD user '{Dn}': email-Address '{Mail}' differs from Synchronization Source '{MailSource}'",
                                    adUserCurrent.DistinguishedName, adUserCurrent.Mail ?? string.Empty, adUserUpdated.Mail ?? string.Empty);
                                break;
                            case nameof(AdUser.TelephoneNumber):
                                // Update logic for TelephoneNumber
                                break;
                            case nameof(AdUser.Mobile):
                                // Update logic for Mobile
                                break;
                            case nameof(AdUser.Title):
                                // Update logic for Title
                                break;
                            case nameof(AdUser.StreetAddress):
                                updateRequired = true;
                                UpdateUserAttribute(directoryEntry, "extensionAttribute5", adUserUpdated.StreetAddress);
                                break;
                            case nameof(AdUser.Room):
                                updateRequired = true;
                                UpdateUserAttribute(directoryEntry, "extensionAttribute6", adUserUpdated.Room);
                                break;
                            case nameof(AdUser.TopLevelUnits):
                                updateRequired = true;
                                _logger.LogWarning("AD user '{Dn}': TopLevelUnits '{TopLevelUnits}' differs from Synchronization Source '{TopLevelUnitsSource}'",
                                    adUserCurrent.DistinguishedName, adUserCurrent.TopLevelUnits ?? string.Empty, adUserUpdated.TopLevelUnits ?? string.Empty);
                                //UpdateUserAttribute(directoryEntry, "extensionAttribute7", adUserUpdated.TopLevelUnits);
                                break;
                            case nameof(AdUser.JobRole):
                                updateRequired = true;
                                UpdateUserAttribute(directoryEntry, "extensionAttribute14", adUserUpdated.JobRole);
                                break;
                            default:
                                _logger.LogWarning("Unknown attribute '{AttributeName}' for AD user '{DN}'", attributeName, adUserUpdated.DistinguishedName);
                                break;
                        }

                    }

                    if (updateRequired)
                    {
                        directoryEntry.CommitChanges();
                        _logger.LogDebug("AD User '{Dn}' updated ", adUserUpdated.DistinguishedName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AD user '{SamAccountName}'", adUserUpdated.SamAccountName);
                throw;
            }
        } // UpdateAdUser method

        private static void UpdateUserAttribute(DirectoryEntry directoryEntry, string attributeName, string? attributeValue)
        {
            if (attributeValue == null)
            {
                directoryEntry.Properties[attributeName].Clear();
            }
            else
            {
                directoryEntry.Properties[attributeName].Value = attributeValue;
            }
        }

    } // AD Service class
}
