using nocscienceat.MetaDirectory.Services.AdService.Models;
using System.Collections.Generic;

namespace nocscienceat.MetaDirectory.Services.AdService
{
    public interface IAdService
    {
        List<AdUser> GetAdUsers();
        List<AdComputer> GetAdComputers();
        void UpdateAdUser(AdUser adUserUpdated, IEnumerable<string> attributeNames);
        void UpdateAdComputer(AdComputer adComputerUpdated, IEnumerable<string> attributeNames);
    }
}
