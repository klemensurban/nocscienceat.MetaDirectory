using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory.Services.AdService.Models
{
    public class AdServiceSettings
    {
        public DirectorySearch[] UserOUs { get; set; } = Array.Empty<DirectorySearch>();
    }

    public class DirectorySearch
    {
        public string Dn { get; set; } = null!;
        public bool SearchScopeSubtree { get; set; }
    }
}
