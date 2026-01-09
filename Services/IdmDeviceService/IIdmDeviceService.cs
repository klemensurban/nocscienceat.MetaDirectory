using nocscienceat.MetaDirectory.Services.IdmDeviceService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory.Services.IdmDeviceService
{
    public interface IIdmDeviceService
    {
        Task<IEnumerable<IdmComputer>> GetComputersAsync();
    }
}
