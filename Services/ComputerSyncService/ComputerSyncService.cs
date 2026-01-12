using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.CudManager2;
using nocscienceat.MetaDirectory.Services.AdService;
using nocscienceat.MetaDirectory.Services.AdService.Models;
using nocscienceat.MetaDirectory.Services.ComputerSyncService.Models;
using nocscienceat.MetaDirectory.Services.IdmDeviceService;
using nocscienceat.MetaDirectory.Services.IdmDeviceService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace nocscienceat.MetaDirectory.Services.ComputerSyncService
{
    public class ComputerSyncService : IComputerSyncService
    {
        private readonly IIdmDeviceService _idmDeviceService;
        private readonly IAdService _adService;
        private readonly ILogger<ComputerSyncService> _logger;
        private readonly ComputerSyncServiceSettings _computerSyncServiceSettings;

        /// <summary>
        /// Captures required dependencies (IDM feed, AD gateway, logging).
        /// </summary>
        public ComputerSyncService(IIdmDeviceService idmDeviceService, IAdService adService, IConfiguration configuration, ILogger<ComputerSyncService> logger)
        {
            _idmDeviceService = idmDeviceService;
            _adService = adService;
            _logger = logger;
            _computerSyncServiceSettings = configuration.GetSection("ComputerSyncService").Get<ComputerSyncServiceSettings>() ??
                                       throw new System.ArgumentNullException(nameof(ComputerSyncServiceSettings));
        }

        /// <summary>
        /// Drives the IDM vs AD comparison, updates AD where needed, and logs summary metrics.
        /// </summary>
        public async Task SyncComputersAsync()
        {
            HashSet<string> adComputersToIgnore = new HashSet<string>(_computerSyncServiceSettings.AdComputersToIgnore, StringComparer.OrdinalIgnoreCase);
            HashSet<string> idmComputersToIgnore = new HashSet<string>(_computerSyncServiceSettings.IdmComputersToIgnore, StringComparer.OrdinalIgnoreCase);

            IEnumerable<IdmComputer> idmComputers = await _idmDeviceService.GetComputersAsync();
            List<AdComputer> adComputers = _adService.GetAdComputers();
            CudManager<string, IdmComputer, AdComputer> computerCudManager = new(new ComputerCudDataAdapter(_logger), sourceItems: idmComputers, sync2Items: adComputers);
            computerCudManager.CheckItems();

            // Persist deltas back to AD.
            foreach (var itemLink in computerCudManager.Items2Update)
            {
                _adService.UpdateAdComputer(itemLink.Sync2ItemUpdated, itemLink.DifferingProperties);
            }

            // Flag CMDB devices missing from the monitored OUs.
            int aktivMissingCount = 0;
            foreach (var item in computerCudManager.Items2Create)
            {
                if (idmComputersToIgnore.Contains(item.Name))
                {
                    _logger.LogDebug("Computer {Name} is configured to be ignored for missing AD Computer-Object checks", item.Name);
                    continue;
                }
                if (item.Status == "Aktiv")
                {
                    _logger.LogWarning("Computer {Name}, {Serial} with Status 'Aktiv' can not be found in specified Computer OUs", item.Name, item.Serial);
                    aktivMissingCount++;
                }
                else
                {
                    _logger.LogTrace("Computer {Name}, {Serial} with status ‘{Status}’ does not have a corresponding Computer-Object in the specified OUs", item.Name, item.Serial, item.Status);
                }
            }

            int adComputersIgnoreCount = 0;
            // Report AD computers not registered in CMDB.
            foreach (var item in computerCudManager.Items2Delete)
            {
                if (adComputersToIgnore.Contains(item.Name))
                {
                    _logger.LogDebug("Computer-Object {DN} is configured to be ignored for missing CMDB registration checks", item.DistinguishedName);
                    adComputersIgnoreCount++;
                    continue;
                }
                _logger.LogDebug("Computer-Object {DN} exists in AD but is not registered in CMDB", item.DistinguishedName);
            }
            
            // Emit aggregate metrics for observability.
            _logger.LogInformation("Number of Computer-Objects in sync with AD: {insync}", computerCudManager.ItemsInSyncCount);
            _logger.LogInformation("Number of Computer-Objects updated in AD: {updated}", computerCudManager.Items2UpdateCount);
            _logger.LogInformation("Number of Computers with status 'Aktiv' without corresponding Computer-Objects in AD: {aktivMissingCount}", aktivMissingCount);
            _logger.LogInformation("Number of Computer-Objects not registered in CMDB : {missing}; set Log-Level to DEBUG to list these Computer-Objects", 
                computerCudManager.Items2DeleteCount - adComputersIgnoreCount);
        }
    }
}
