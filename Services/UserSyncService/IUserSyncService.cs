using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory.Services.UserSyncService
{
    public interface IUserSyncService
    {
        Task SyncUsersAsync();
    }
}
