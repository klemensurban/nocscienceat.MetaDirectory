using nocscienceat.MetaDirectory.Services.IdmUserService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory.Services.IdmUserService
{
    public interface IIdmUserService
    {
        Task<IEnumerable<IdmUser>> GetUsersAsync();
    }
}
