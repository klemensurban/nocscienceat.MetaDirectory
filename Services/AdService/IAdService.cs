using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nocscienceat.MetaDirectory.Services.AdService.Models;

namespace nocscienceat.MetaDirectory.Services.AdService
{
    public interface IAdService
    {
        IEnumerable<AdUser> GetAdUsers();
    }
}
