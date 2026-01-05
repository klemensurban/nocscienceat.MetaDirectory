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

        private const string AttributeNameStreetAddress = "extensionAttribute5";
        private const string AttributeNameRoom = "extensionAttribute6";
        private const string AttributeNameTopLevelUnits = "extensionAttribute7";
        private const string AttributeNameJobRole = "extensionAttribute14";
        private const string AttributeNameBpkBf = "bPKBF";
        private const string AttributeNameSapPersNr = "SAPPersNr";
        private const string AttributeNameTelephoneNumber = "telephonenumber";
        private const string AttributeNameSamAccountName = "SamAccountName";
        private const string AttributeNameDistinguishedName = "distinguishedname";
        private const string AttributeNameSn = "sn";
        private const string AttributeNameGivenName = "GivenName";
        private const string AttributeNameMobile = "mobile";
        private const string AttributeNameTitle = "title";
        private const string AttributeNameMail = "mail";

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
                    searcher.SearchScope = directorySearch.SearchScopeSubtree ? SearchScope.Subtree : SearchScope.OneLevel;
                    searcher.PageSize = 1000;
                    searcher.PropertiesToLoad.Clear();
                    searcher.PropertiesToLoad.AddRange(new[]
                    {
                        AttributeNameSamAccountName, AttributeNameDistinguishedName, AttributeNameSapPersNr, AttributeNameBpkBf,
                        AttributeNameSn, AttributeNameGivenName, AttributeNameMail, AttributeNameTelephoneNumber, AttributeNameMobile, 
                        AttributeNameTitle, AttributeNameStreetAddress, AttributeNameRoom, AttributeNameTopLevelUnits, AttributeNameJobRole
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
                SamAccountName = GetPropertyValue(searchResult, AttributeNameSamAccountName),
                DistinguishedName = GetPropertyValue(searchResult, AttributeNameDistinguishedName),
                SapPersNr = GetPropertyValue(searchResult, AttributeNameSapPersNr),
                BpkBf = GetPropertyValue(searchResult, AttributeNameBpkBf),
                Sn = GetPropertyValue(searchResult, AttributeNameSn),
                GivenName = GetPropertyValue(searchResult, AttributeNameGivenName),
                Mail = GetPropertyValue(searchResult, AttributeNameMail),
                TelephoneNumber = GetPropertyValue(searchResult, AttributeNameTelephoneNumber),
                Mobile = GetPropertyValue(searchResult, AttributeNameMobile),
                Title = GetPropertyValue(searchResult, AttributeNameTitle),
                StreetAddress = GetPropertyValue(searchResult, AttributeNameStreetAddress),
                Room = GetPropertyValue(searchResult, AttributeNameRoom),
                TopLevelUnits = GetPropertyValue(searchResult, AttributeNameTopLevelUnits),
                JobRole = GetPropertyValue(searchResult, AttributeNameJobRole)
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
                        switch (attributeName)
                        {
                            case nameof(AdUser.BpkBf):
                                updateRequired = true;
                                UpdateUserAttribute(directoryEntry, AttributeNameBpkBf, adUserUpdated.BpkBf);
                                break;
                            case nameof(AdUser.Sn):
                                updateRequired = true;
                                UpdateUserAttribute(directoryEntry, AttributeNameSn, adUserUpdated.Sn);
                                break;
                            case nameof(AdUser.GivenName):
                                UpdateUserAttribute(directoryEntry, AttributeNameGivenName, adUserUpdated.GivenName);
                                updateRequired = true;
                                break;
                            case nameof(AdUser.Mail):
                                _logger.LogWarning("AD user '{Dn}': AD Attribute Mail can not be synced in an Exchange Environment", adUserCurrent.DistinguishedName);
                                break;
                            case nameof(AdUser.TelephoneNumber):
                                UpdateUserAttribute(directoryEntry, AttributeNameTelephoneNumber, adUserUpdated.TelephoneNumber);
                                updateRequired = true;
                                break;
                            case nameof(AdUser.Mobile):
                                UpdateUserAttribute(directoryEntry, AttributeNameMobile, adUserUpdated.Mobile);
                                break;
                            case nameof(AdUser.Title):
                                UpdateUserAttribute(directoryEntry, AttributeNameTitle, adUserUpdated.Title);
                                break;
                            case nameof(AdUser.StreetAddress):
                                updateRequired = true;
                                UpdateUserAttribute(directoryEntry, AttributeNameStreetAddress, adUserUpdated.StreetAddress);
                                break;
                            case nameof(AdUser.Room):
                                updateRequired = true;
                                UpdateUserAttribute(directoryEntry, AttributeNameRoom, adUserUpdated.Room);
                                break;
                            case nameof(AdUser.TopLevelUnits):
                                updateRequired = true;
                                UpdateUserAttribute(directoryEntry, AttributeNameTopLevelUnits, adUserUpdated.TopLevelUnits);
                                break;
                            case nameof(AdUser.JobRole):
                                updateRequired = true;
                                UpdateUserAttribute(directoryEntry, AttributeNameJobRole, adUserUpdated.JobRole);
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
            if (attributeValue is not null)
                directoryEntry.Properties[attributeName].Value = attributeValue;
            else
                directoryEntry.Properties[attributeName].Clear();
        }

    } // AD Service class
}
