using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.MetaDirectory.Services.AdService.Models;

namespace nocscienceat.MetaDirectory.Services.AdService
{
    public class AdService : IAdService
    {
        private readonly AdServiceSettings _adServiceSettings;
        private readonly ILogger<AdService> _logger;

        public AdService(IConfiguration configuration, ILogger<AdService> logger)
        {
            _adServiceSettings = configuration.GetSection("AdService").Get<AdServiceSettings>() ?? throw new ArgumentNullException(nameof(AdServiceSettings));
            _logger = logger;
        }

        public IEnumerable<AdUser> GetAdUsers()
        {
            throw new NotImplementedException();
        }
    }


}
