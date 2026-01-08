using System;

namespace nocscienceat.MetaDirectory.Services.ComputerSyncService.Models
{
    internal class ComputerStatus
    {
        public string? Status { get; set; } = "Unknown";
        public string? Serial { get; set; } = null;
        public string? Inventory { get; set; } = null;
        public string? Location { get; set; } = null;
        public DateTime Updated { get; set; } = DateTime.UtcNow;
    }
}
