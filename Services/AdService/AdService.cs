using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.MetaDirectory.Services.AdService.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Reflection;


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

        public IEnumerable<AdUser> GetAdUsers()
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
                        if (!string.IsNullOrWhiteSpace(adUser.SapPersNr))
                        {
                            adUsers.Add(adUser);
                        }
                        else
                        {
                            _logger.LogDebug(
                                "AD user with SamAccountName '{SamAccountName}' has null or whitespace SapPersNr. SearchResult DN: '{DistinguishedName}'",
                                adUser.SamAccountName, adUser.DistinguishedName);
                        }
                    }

                }
                // Placeholder for actual AD user retrieval logic

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

        public void UpdateAdUser(AdUser adUser, IEnumerable<string> attributeNames)
        {
            foreach (string attributeName in attributeNames)
            {
                string attributeValue = string.Empty;
                switch (attributeName)
                {

                    case nameof(AdUser.SapPersNr):
                        break;
                    case nameof(AdUser.BpkBf):
                        break;
                    case nameof(AdUser.Sn):
                        attributeValue = adUser.Sn ?? string.Empty;
                        break;
                    case nameof(AdUser.GivenName):
                        attributeValue = adUser.GivenName ?? string.Empty;
                        break;
                    case nameof(AdUser.Mail):
                        // Update logic for Mail
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
                        // Update logic for StreetAddress
                        break;
                    case nameof(AdUser.Room):
                        // Update logic for Room
                        break;
                    case nameof(AdUser.TopLevelUnits):
                        attributeValue = adUser.TopLevelUnits ?? string.Empty;
                        break;
                    case nameof(AdUser.JobRole):
                        // Update logic for JobRole
                        break;
                    default:
                        _logger.LogWarning("Unknown attribute '{AttributeName}' for AD user '{SamAccountName}'", attributeName, adUser.SamAccountName);
                        break;
                }

                _logger.LogInformation("Updating AD user '{DN}': Attribute '{AttributeName}' with Attribute Value '{AttributeValue}'",
                    adUser.DistinguishedName, attributeName, attributeValue);
                // Placeholder for actual AD user update logic
            }
        }
    }
}
