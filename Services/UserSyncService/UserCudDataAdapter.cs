using nocscienceat.CudManager2;
using nocscienceat.CudManager2.Models;
using nocscienceat.MetaDirectory.Services.AdService.Models;
using nocscienceat.MetaDirectory.Services.IdmUserService.Models;
using System;
using System.Collections.Generic;

namespace nocscienceat.MetaDirectory.Services.UserSyncService
{
    internal class UserCudDataAdapter: ICudDataAdapter<string, IdmUser, AdUser>
    {
        public ComparisonResult Compare(IdmUser sourceItem, AdUser sync2Item)
        {
            var differingProps = new List<string>();

            if (!string.Equals(sourceItem.GivenName, sync2Item.GivenName))
                differingProps.Add(nameof(sourceItem.GivenName));

            if (!string.Equals(sourceItem.Sn, sync2Item.Sn))
                differingProps.Add(nameof(sourceItem.Sn));

            /*
            if (!string.Equals(sourceItem.Mail, sync2Item.Mail, StringComparison.InvariantCultureIgnoreCase))
                differingProps.Add(nameof(sourceItem.Mail));
            */

            /*
            if (!string.Equals(sourceItem.TelephoneNumber, sync2Item.TelephoneNumber))
                differingProps.Add(nameof(sourceItem.TelephoneNumber));

            if (!string.Equals(sourceItem.Mobile, sync2Item.Mobile))
                differingProps.Add(nameof(sourceItem.Mobile));

            if (!string.Equals(sourceItem.Title, sync2Item.Title))
                differingProps.Add(nameof(sourceItem.Title));

            if (!string.Equals(sourceItem.StreetAddress, sync2Item.StreetAddress))
                differingProps.Add(nameof(sourceItem.StreetAddress));

            if (!string.Equals(sourceItem.Room, sync2Item.Room))
                differingProps.Add(nameof(sourceItem.Room));
            */

            /*
            if (!string.Equals(sourceItem.TopLevelUnits, sync2Item.TopLevelUnits))
                differingProps.Add(nameof(sourceItem.TopLevelUnits));
            */

            /*
            if (!string.Equals(sourceItem.JobRole, sync2Item.JobRole))
                differingProps.Add(nameof(sourceItem.JobRole));
            */

            if (!string.Equals(sourceItem.BpkBf, sync2Item.BpkBf))
                differingProps.Add(nameof(sourceItem.BpkBf));

            if (differingProps.Count == 0)
                return new ComparisonResult.IsEqual();

            return new ComparisonResult.DiffersBy { Properties = differingProps };
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
