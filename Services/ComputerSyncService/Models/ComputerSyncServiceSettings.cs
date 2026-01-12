using System.Collections.Generic;

namespace nocscienceat.MetaDirectory.Services.ComputerSyncService.Models
{
    public class ComputerSyncServiceSettings
    {
        public List<string> AdComputersToIgnore { get; set; } = new();
        public List<string> IdmComputersToIgnore { get; set; } = new();
    }
}
