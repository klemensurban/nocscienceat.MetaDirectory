using Microsoft.Extensions.Logging;
using nocscienceat.CudManager2;
using nocscienceat.CudManager2.Models;
using nocscienceat.MetaDirectory.Services.AdService.Models;
using nocscienceat.MetaDirectory.Services.IdmUserService.Models;
using nocscienceat.MetaDirectory.Services.UserSyncService.Models;
using System;
using System.Collections.Generic;

namespace nocscienceat.MetaDirectory.Services.UserSyncService
{
    internal class UserCudDataAdapter: ICudDataAdapter<string, IdmUser, AdUser>
    {
        private readonly CudadapterOptions _options;
        private readonly ILogger<UserSyncService> _logger;

        public UserCudDataAdapter(CudadapterOptions options, ILogger<UserSyncService> logger)
        {
            _options = options;
            _logger = logger;
        }

        public ComparisonResult<AdUser> Compare(IdmUser sourceItem, AdUser sync2Item)
        {
            List<string> differingProps = new();

            // Create with original as Template for DN and SAMAccountName retention
            AdUser sync2ItemUpdated = new(sync2Item);

            if (!string.Equals(sourceItem.GivenName, sync2Item.GivenName))
            {
                switch (_options.GivenName)
                {
                    case SyncOption.Sync:
                        sync2ItemUpdated.GivenName = sourceItem.GivenName;
                        differingProps.Add(nameof(sync2ItemUpdated.GivenName));
                        break;
                    case SyncOption.Ignore:
                        break;
                    case SyncOption.Debug:
                        _logger.LogDebug("AD User {Dn}: GivenName differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.GivenName, sync2Item.GivenName );
                        break;
                    case SyncOption.Warn:
                        _logger.LogWarning("AD User {Dn} :GivenName differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.GivenName, sync2Item.GivenName);
                        break;
                }
            }

            if (!string.Equals(sourceItem.Sn, sync2Item.Sn))
            {
                switch (_options.Sn)
                {
                    case SyncOption.Sync:
                        sync2ItemUpdated.Sn = sourceItem.Sn;
                        differingProps.Add(nameof(sync2ItemUpdated.Sn));
                        break;
                    case SyncOption.Ignore:
                        break;
                    case SyncOption.Debug:
                        _logger.LogDebug("AD User {Dn}: Sn differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.Sn, sync2Item.Sn);
                        break;
                    case SyncOption.Warn:
                        _logger.LogWarning("AD User {Dn}: Sn differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.Sn, sync2Item.Sn);
                        break;
                }
            }
            
            if (!string.Equals(sourceItem.Mail, sync2Item.Mail, StringComparison.InvariantCultureIgnoreCase))
            {
                switch (_options.Mail)
                {
                    case SyncOption.Sync:
                        sync2ItemUpdated.Mail = sourceItem.Mail;
                        differingProps.Add(nameof(sync2ItemUpdated.Mail));
                        break;
                    case SyncOption.Ignore:
                        break;
                    case SyncOption.Debug:
                        _logger.LogDebug("AD User {Dn}: Mail differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.Mail, sync2Item.Mail);
                        break;
                    case SyncOption.Warn:
                        _logger.LogWarning("AD User {Dn}: Mail differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.Mail, sync2Item.Mail);
                        break;
                }
            }

            if (!string.Equals(sourceItem.TelephoneNumber, sync2Item.TelephoneNumber))
            {
                switch (_options.TelephoneNumber)
                {
                    case SyncOption.Sync:
                        sync2ItemUpdated.TelephoneNumber = sourceItem.TelephoneNumber;
                        differingProps.Add(nameof(sync2ItemUpdated.TelephoneNumber));
                        break;
                    case SyncOption.Ignore:
                        break;
                    case SyncOption.Debug:
                        _logger.LogDebug("AD User {Dn}: TelephoneNumber differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.TelephoneNumber, sync2Item.TelephoneNumber);
                        break;
                    case SyncOption.Warn:
                        _logger.LogWarning("AD User {Dn}: TelephoneNumber differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.TelephoneNumber, sync2Item.TelephoneNumber);
                        break;
                }
            }


            if (!string.Equals(sourceItem.Mobile, sync2Item.Mobile))
            {
                switch (_options.Mobile)
                {
                    case SyncOption.Sync:
                        sync2ItemUpdated.Mobile = sourceItem.Mobile;
                        differingProps.Add(nameof(sync2ItemUpdated.Mobile));
                        break;
                    case SyncOption.Ignore:
                        break;
                    case SyncOption.Debug:
                        _logger.LogDebug("AD User {Dn}: Mobile differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.Mobile, sync2Item.Mobile);
                        break;
                    case SyncOption.Warn:
                        _logger.LogWarning("AD User {Dn}: Mobile differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.Mobile, sync2Item.Mobile);
                        break;
                }
            }

            if (!string.Equals(sourceItem.Title, sync2Item.Title))
            {
                switch (_options.Title)
                {
                    case SyncOption.Sync:
                        sync2ItemUpdated.Title = sourceItem.Title;
                        differingProps.Add(nameof(sync2ItemUpdated.Title));
                        break;
                    case SyncOption.Ignore:
                        break;
                    case SyncOption.Debug:
                        _logger.LogDebug("AD User {Dn}: Title differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.Title, sync2Item.Title);
                        break;
                    case SyncOption.Warn:
                        _logger.LogWarning("AD User {Dn}: Title differs: Source Value {Source} vs {Sync2}", sync2Item.DistinguishedName, sourceItem.Title, sync2Item.Title);
                        break;
                }
            }
           

            // Check if sourceItem.Room is not null and equals "999"
            string? room;
            string? streetAddress;

            if (sourceItem.Room is not null && sourceItem.Room == (_options.RoomNullValue ?? string.Empty))
            {
                room = null;
                streetAddress = null;
            }
            else
            {
                room = sourceItem.Room;
                streetAddress = sourceItem.StreetAddress;
            }

            if (!string.Equals(room, sync2Item.Room))
            {
                sync2ItemUpdated.Room = room;
                differingProps.Add(nameof(sync2ItemUpdated.Room));
            }
            if (!string.Equals(streetAddress, sync2Item.StreetAddress))
            {
                sync2ItemUpdated.StreetAddress = streetAddress;
                differingProps.Add(nameof(sync2ItemUpdated.StreetAddress));
            }



            if (!string.Equals(sourceItem.TopLevelUnits, sync2Item.TopLevelUnits))
            {
                sync2ItemUpdated.TopLevelUnits = sourceItem.TopLevelUnits;
                differingProps.Add(nameof(sync2ItemUpdated.TopLevelUnits));
            }

            if (!string.Equals(sourceItem.JobRole, sync2Item.JobRole))
            {
                sync2ItemUpdated.JobRole = sourceItem.JobRole;
                differingProps.Add(nameof(sync2ItemUpdated.JobRole));
            }

            if (!string.Equals(sourceItem.BpkBf, sync2Item.BpkBf))
            {
                sync2ItemUpdated.BpkBf = sourceItem.BpkBf;
                differingProps.Add(nameof(sync2ItemUpdated.BpkBf));
            }

            if (differingProps.Count == 0)
                return new ComparisonResult<AdUser>.IsEqual();

            return new ComparisonResult<AdUser>.DiffersBy { Properties = differingProps, SyncItemUpdated = sync2ItemUpdated };
        }

        public string GetKeyFromSourceItem(IdmUser sourceItem)
        {
            return sourceItem.SapPersNr;
        }

        public string GetKeyFromSync2Item(AdUser sync2Item)
        {
            return sync2Item.SapPersNr is null ? throw new ArgumentException("Sync2 item does not have a SapPersNr.") : sync2Item.SapPersNr.Trim();
        }
    }
}
