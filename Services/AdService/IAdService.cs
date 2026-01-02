using nocscienceat.MetaDirectory.Services.AdService.Models;
using System.Collections.Generic;

namespace nocscienceat.MetaDirectory.Services.AdService
{
    public interface IAdService
    {
        List<AdUser> GetAdUsers();
        void UpdateAdUser(AdUser adUserUpdated, AdUser adUserCurrent, IEnumerable<string> attributeNames);
    }
}
