using Microsoft.Extensions.Logging;
using nocscienceat.CudManager2;
using nocscienceat.CudManager2.Models;
using nocscienceat.MetaDirectory.Services.AdService.Models;
using nocscienceat.MetaDirectory.Services.ComputerSyncService.Models;
using nocscienceat.MetaDirectory.Services.IdmDeviceService.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace nocscienceat.MetaDirectory.Services.ComputerSyncService
{
    internal class ComputerCudDataAdapter : ICudDataAdapter<string, IdmComputer, AdComputer>
    {
        private readonly ILogger<ComputerSyncService> _logger;

        public ComputerCudDataAdapter(ILogger<ComputerSyncService> logger)
        {
            _logger = logger;
        }

        public ComparisonResult<AdComputer> Compare(IdmComputer sourceItem, AdComputer sync2Item)
        {
            List<string> differingProps = new();
            AdComputer sync2ItemUpdated = new(sync2Item);

            bool updateAdComputerstatus = false;

            ComputerStatus computerStatus;

            if (string.IsNullOrWhiteSpace(sync2Item.Status))
            {
                computerStatus = new ComputerStatus();
            }
            else
            {
                try
                {
                    computerStatus = JsonSerializer.Deserialize<ComputerStatus>(sync2Item.Status!) ?? new ComputerStatus();
                }
                catch
                {
                    computerStatus = new ComputerStatus();
                }
            }

            if (!string.Equals(sourceItem.Status, computerStatus.Status))
            {
                computerStatus.Status = sourceItem.Status;
                computerStatus.Updated = DateTime.UtcNow;
                updateAdComputerstatus = true;
            }

            if (!string.Equals(sourceItem.Inventory, computerStatus.Inventory))
            {
                computerStatus.Inventory = sourceItem.Inventory;
                updateAdComputerstatus = true;
            }

            if (!string.Equals(sourceItem.Serial, computerStatus.Serial))
            {
                computerStatus.Serial = sourceItem.Serial;
                updateAdComputerstatus = true;
            }

            if (!string.Equals(sourceItem.Location, computerStatus.Location))
            {
                computerStatus.Location = sourceItem.Location;
                updateAdComputerstatus = true;
            }

            if (updateAdComputerstatus)
            {
                differingProps.Add(nameof(sync2ItemUpdated.Status));
                sync2ItemUpdated.Status = JsonSerializer.Serialize(computerStatus, new JsonSerializerOptions
                {
                    WriteIndented = false
                });
            }

            if (differingProps.Count == 0)
                return new ComparisonResult<AdComputer>.IsEqual();

            return new ComparisonResult<AdComputer>.DiffersBy() { Properties = differingProps, SyncItemUpdated = sync2ItemUpdated };
        }

        public string GetKeyFromSourceItem(IdmComputer sourceItem)
        {
            return sourceItem.Name.ToUpperInvariant();
        }

        public string GetKeyFromSync2Item(AdComputer sync2Item)
        {
            return sync2Item.Name.ToUpperInvariant();
        }
    }
}
