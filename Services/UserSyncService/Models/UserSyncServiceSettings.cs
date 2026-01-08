using System.Collections.Generic;

namespace nocscienceat.MetaDirectory.Services.UserSyncService.Models
{
    public class UserSyncServiceSettings
    {
        public string? RoomNullValue { get; set; }
        public List<string> SamAccountNamesToIgnore { get; set; } = new();
        public List<string> SapPersNumbersToIgnore { get; set; } = new();
        public UserCudadapterOptions UserCudadapter { get; set; } = new();
    }

    public class UserCudadapterOptions
    {
        public string? RoomNullValue { get; set; }
        public SyncOption TelephoneNumber { get; set; } = SyncOption.Ignore;
        public SyncOption Mobile { get; set; } = SyncOption.Ignore;
        public SyncOption Title { get; set; } = SyncOption.Ignore;
        public SyncOption Mail { get; set; } = SyncOption.Ignore;
        public SyncOption GivenName { get; set; } = SyncOption.Ignore;
        public SyncOption Sn { get; set; } = SyncOption.Ignore;
        
    }

    public enum SyncOption
    {
        Ignore,
        Debug,
        Warn,
        Sync
    }

}
