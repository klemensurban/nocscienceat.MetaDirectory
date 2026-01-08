using System;

namespace nocscienceat.MetaDirectory.Services.AdService.Models
{
    public class AdServiceSettings
    {
        public DirectorySearch[] UserOUs { get; set; } = Array.Empty<DirectorySearch>();
        public DirectorySearch[] ComputerOUs { get; set; } = Array.Empty<DirectorySearch>();
    }

    public class DirectorySearch
    {
        public string Dn { get; set; } = null!;
        public bool SearchScopeSubtree { get; set; }


    }
}
