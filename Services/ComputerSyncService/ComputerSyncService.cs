using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.CudManager2;
using nocscienceat.MetaDirectory.Services.AdService;
using nocscienceat.MetaDirectory.Services.AdService.Models;
using nocscienceat.MetaDirectory.Services.IdmDeviceService;
using nocscienceat.MetaDirectory.Services.IdmDeviceService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace nocscienceat.MetaDirectory.Services.ComputerSyncService
{
    public class ComputerSyncService : IComputerSyncService
    {
        private readonly IIdmDeviceService _idmDeviceService;
        private readonly IAdService _adService;
        private readonly ILogger<ComputerSyncService> _logger;

        public ComputerSyncService(IIdmDeviceService idmDeviceService, IAdService adService, IConfiguration configuration, ILogger<ComputerSyncService> logger)
        {
            _idmDeviceService = idmDeviceService;
            _adService = adService;
            _logger = logger;
        }

        public async Task SyncComputersAsync()
        {
            IEnumerable<IdmComputer> idmComputers = await _idmDeviceService.GetComputersAsync();
            List<AdComputer> adComputers = _adService.GetAdComputers();
            CudManager<string, IdmComputer, AdComputer> computerCudManager = new(new ComputerCudDataAdapter(_logger), sourceItems: idmComputers, sync2Items: adComputers);
            computerCudManager.CheckItems();
            foreach (var itemLink in computerCudManager.Items2Update)
            {
                _adService.UpdateAdComputer(itemLink.Sync2ItemUpdated, itemLink.DifferingProperties);
            }
            int aktivMissingCount = 0;
            foreach (var item in computerCudManager.Items2Create)
            {
                if (item.Status == "Aktiv")
                {
                    _logger.LogWarning("Computer {Name}, {Serial} with Status 'Aktiv' can not be found in specified Computer OUs", item.Name, item.Serial);
                    aktivMissingCount++;
                }
                else
                    _logger.LogDebug("Computer {Name}, {Serial} with status ‘{Status}’ does not have a corresponding Computer-Object in the specified OUs", item.Name, item.Serial, item.Status);
            }

            foreach (var item in computerCudManager.Items2Delete)
            {
                _logger.LogDebug("Computer-Object {DN} exists in AD but is not registered in CMDB", item.DistinguishedName);
            }
            
            _logger.LogInformation("Number of Computer-Objects in sync with AD: {insync}", computerCudManager.ItemsInSyncCount);
            _logger.LogInformation("Number of Computer-Objects updated in AD: {updated}", computerCudManager.Items2UpdateCount);
            _logger.LogInformation("Number of Computers with status 'Aktiv' without corresponding Computer-Objects in AD: {aktivMissingCount}", aktivMissingCount);
            _logger.LogInformation("Number of Computer-Objects not registered in CMDB : {missing}; set Log-Level to DEBUG to list these Computer-Objects", computerCudManager.Items2DeleteCount);
        }
    }
}
