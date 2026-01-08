using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.MetaDirectory.Services.IdmDeviceService.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory.Services.IdmDeviceService
{
    public class IdmDeviceService : IIdmDeviceService
    {
        private readonly ILogger<IdmDeviceService> _logger;
        public static string DefaultStatus = null!;

        private readonly IdmDeviceServiceSettings _idmDeviceServiceSettings;

        public IdmDeviceService(IConfiguration configuration, ILogger<IdmDeviceService> logger)
        {
            _logger = logger;
            _idmDeviceServiceSettings = configuration.GetSection("IdmDeviceService").Get<IdmDeviceServiceSettings>() ??
                                        throw new ArgumentNullException(nameof(IdmDeviceServiceSettings));

            DefaultStatus = _idmDeviceServiceSettings.DefaultStatus ?? "Inaktiv";
        }

        public async Task<IEnumerable<IdmComputer>> GetComputersAsync()
        {
            const string sql =
                @"SELECT SAP_Inventarnummer as Inventory, GerBezeichnung, GerTyp, GerStandort_friendly as Location, GerVARaum_friendly, CI_ID, CI_Zustand as Status, MAC_to_Name as Name, Seriennummer as Serial, GerMACAddress
                  FROM OT_CMDB_NAME_FROM_MAC
                  WHERE MAC_to_Name IS NOT NULL AND MAC_to_Name <> '' order by MAC_to_Name";

            if (string.IsNullOrWhiteSpace(_idmDeviceServiceSettings.ConnectionString))
            {
                throw new InvalidOperationException("The IDM device service connection string is null or empty. Please check your configuration.");
            }

            try
            {
                SqlConnection connection = new(_idmDeviceServiceSettings.ConnectionString!);
                using (connection)
                {
                    return await connection.QueryAsync<IdmComputer>(sql);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching IDM users");
                throw;
            }
        }
    }
}
