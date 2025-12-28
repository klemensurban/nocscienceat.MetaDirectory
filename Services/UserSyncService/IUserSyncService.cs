using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory.Services.UserSyncService
{
    public interface IUserSyncService
    {
        Task SyncUsersAsync();
    }
}
