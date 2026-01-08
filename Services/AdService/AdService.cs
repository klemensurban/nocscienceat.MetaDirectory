using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.MetaDirectory.Services.AdService.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Text;

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
        private const string AttributeNameComputerStatus = "extensionAttribute10";
        private const string AttributeNameComputerName = "name";

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

        public void UpdateAdUser(AdUser adUserUpdated, IEnumerable<string> attributeNames)
        {
            try
            {
                using PrincipalContext context = new(ContextType.Domain);
                using UserPrincipal? userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, adUserUpdated.SamAccountName!);
                if (userPrincipal != null)
                {
                    bool updateRequired = false;
                    StringBuilder attributes = new();

                    using DirectoryEntry directoryEntry = (DirectoryEntry) userPrincipal.GetUnderlyingObject();

                    foreach (string attributeName in attributeNames)
                    {
                        switch (attributeName)
                        {
                            case nameof(AdUser.BpkBf):
                                updateRequired = true;
                                UpdateAttribute(directoryEntry, AttributeNameBpkBf, adUserUpdated.BpkBf);
                                break;
                            case nameof(AdUser.Sn):
                                updateRequired = true;
                                UpdateAttribute(directoryEntry, AttributeNameSn, adUserUpdated.Sn);
                                break;
                            case nameof(AdUser.GivenName):
                                UpdateAttribute(directoryEntry, AttributeNameGivenName, adUserUpdated.GivenName);
                                updateRequired = true;
                                break;
                            case nameof(AdUser.Mail):
                                _logger.LogWarning("AD user '{Dn}': AD Attribute Mail can not be synced in an Exchange Environment", adUserUpdated.DistinguishedName);
                                break;
                            case nameof(AdUser.TelephoneNumber):
                                UpdateAttribute(directoryEntry, AttributeNameTelephoneNumber, adUserUpdated.TelephoneNumber);
                                updateRequired = true;
                                break;
                            case nameof(AdUser.Mobile):
                                UpdateAttribute(directoryEntry, AttributeNameMobile, adUserUpdated.Mobile);
                                break;
                            case nameof(AdUser.Title):
                                UpdateAttribute(directoryEntry, AttributeNameTitle, adUserUpdated.Title);
                                break;
                            case nameof(AdUser.StreetAddress):
                                updateRequired = true;
                                UpdateAttribute(directoryEntry, AttributeNameStreetAddress, adUserUpdated.StreetAddress);
                                break;
                            case nameof(AdUser.Room):
                                updateRequired = true;
                                UpdateAttribute(directoryEntry, AttributeNameRoom, adUserUpdated.Room);
                                break;
                            case nameof(AdUser.TopLevelUnits):
                                updateRequired = true;
                                UpdateAttribute(directoryEntry, AttributeNameTopLevelUnits, adUserUpdated.TopLevelUnits);
                                break;
                            case nameof(AdUser.JobRole):
                                updateRequired = true;
                                UpdateAttribute(directoryEntry, AttributeNameJobRole, adUserUpdated.JobRole);
                                break;

                            default:
                                _logger.LogWarning("Unknown attribute '{AttributeName}' for AD user '{DN}'", attributeName, adUserUpdated.DistinguishedName);
                                break;
                        }
                        attributes.Append(attributeName).Append(" ");

                    }

                    if (updateRequired)
                    {
                        directoryEntry.CommitChanges();
                        _logger.LogDebug("AD User '{Dn}' updated, Attributes: {Attributes} ", adUserUpdated.DistinguishedName, attributes.ToString().Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AD user '{SamAccountName}'", adUserUpdated.SamAccountName);
                throw;
            }
        }

        public List<AdComputer> GetAdComputers()
        {
            List<AdComputer> adComputers = new();
            try
            {
                foreach (DirectorySearch directorySearch in _adServiceSettings.ComputerOUs)
                {
                    using PrincipalContext ctx = new(ContextType.Domain, null, directorySearch.Dn);
                    using PrincipalSearcher principalSearcher = new();
                    principalSearcher.QueryFilter = new ComputerPrincipal(ctx);
                    using DirectorySearcher searcher = (DirectorySearcher)principalSearcher.GetUnderlyingSearcher();
                    searcher.SearchScope = directorySearch.SearchScopeSubtree ? SearchScope.Subtree : SearchScope.OneLevel;
                    searcher.PageSize = 1000;
                    searcher.PropertiesToLoad.Clear();
                    searcher.PropertiesToLoad.AddRange(new[]
                    {
                        AttributeNameSamAccountName, AttributeNameDistinguishedName, AttributeNameComputerName,
                        AttributeNameComputerStatus
                    });
                    using SearchResultCollection resultCollection = searcher.FindAll();
                    foreach (SearchResult searchResult in resultCollection)
                    {
                        AdComputer adComputer = MapSearchResult2AdComputer(searchResult);
                        adComputers.Add(adComputer);
                    }
                }
                return adComputers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching AD Computers");
                throw;
            }
        }

        public void UpdateAdComputer(AdComputer adComputerUpdated, IEnumerable<string> attributeNames)
        {
            try
            {
                using PrincipalContext context = new(ContextType.Domain);
                using ComputerPrincipal? computerPrincipal = ComputerPrincipal.FindByIdentity(context, IdentityType.SamAccountName, adComputerUpdated.SamAccountName!);
                if (computerPrincipal != null)
                {
                    bool updateRequired = false;
                    StringBuilder attributes = new();

                    using DirectoryEntry directoryEntry = (DirectoryEntry)computerPrincipal.GetUnderlyingObject();

                    foreach (string attributeName in attributeNames)
                    {
                        switch (attributeName)
                        {
                            case nameof(AdComputer.Status):
                                updateRequired = true;
                                UpdateAttribute(directoryEntry, AttributeNameComputerStatus, adComputerUpdated.Status);
                                break;


                            default:
                                _logger.LogWarning("Unknown attribute '{AttributeName}' for AD user '{DN}'", attributeName, adComputerUpdated.DistinguishedName);
                                break;
                        }
                        attributes.Append(attributeName).Append(" ");

                    }

                    if (updateRequired)
                    {
                        directoryEntry.CommitChanges();
                        _logger.LogDebug("AD Computer '{Dn}' updated, Attributes: {Attributes} ", adComputerUpdated.DistinguishedName, attributes.ToString().Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AD Computer '{SamAccountName}'", adComputerUpdated.SamAccountName);
                throw;
            }
        }

        private AdComputer MapSearchResult2AdComputer(SearchResult searchResult)
        {
            AdComputer adComputer = new()
            {
                SamAccountName = GetPropertyValue(searchResult, AttributeNameSamAccountName),
                DistinguishedName = GetPropertyValue(searchResult, AttributeNameDistinguishedName),
                Name = GetPropertyValue(searchResult,  AttributeNameComputerName)!,
                Status = GetPropertyValue(searchResult, AttributeNameComputerStatus)
            };
            return adComputer;
        }


        private static void UpdateAttribute(DirectoryEntry directoryEntry, string attributeName, string? attributeValue)
        {
            if (attributeValue is not null)
                directoryEntry.Properties[attributeName].Value = attributeValue;
            else
                directoryEntry.Properties[attributeName].Clear();
        }

    } // AD Service class
}
