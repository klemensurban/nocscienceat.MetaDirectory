using System;

namespace nocscienceat.MetaDirectory.Services.AdService.Models
{
    public class AdServiceSettings
    {
        public DirectorySearchDefinition[] UserOUs { get; set; } = Array.Empty<DirectorySearchDefinition>();
        public DirectorySearchDefinition[] ComputerOUs { get; set; } = Array.Empty<DirectorySearchDefinition>();
    }

    public class DirectorySearchDefinition
    {
        public string Dn { get; set; } = null!;
        public bool SearchScopeSubtree { get; set; }
    }
}
