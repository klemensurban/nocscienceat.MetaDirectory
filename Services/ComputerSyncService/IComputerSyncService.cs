using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory.Services.ComputerSyncService
{
    public interface IComputerSyncService
    {
        Task SyncComputersAsync();
    }
}