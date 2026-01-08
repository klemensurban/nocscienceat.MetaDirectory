using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nocscienceat.MetaDirectory.Services.IdmDeviceService.Models;

namespace nocscienceat.MetaDirectory.Services.IdmDeviceService
{
    public interface IIdmDeviceService
    {
        Task<IEnumerable<IdmComputer>> GetComputersAsync();
    }
}
