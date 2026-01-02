using System.Collections.Generic;

namespace nocscienceat.MetaDirectory.Services.UserSyncService.Models
{
    public class UserSyncServiceSettings
    {
        public string? RoomNullValue { get; set; }
        public List<string> SamAccountNamesToIgnore { get; set; } = new();
        public List<string> SapPersNumbersToIgnore { get; set; } = new();
    }
}
