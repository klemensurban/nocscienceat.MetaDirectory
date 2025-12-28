using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nocscienceat.MetaDirectory.Services.IdmUserService.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace nocscienceat.MetaDirectory.Services.IdmUserService
{
    public class IdmUserService : IIdmUserService
    {
        public static string BpkPrefix = null!;

        private readonly IdmUserServiceSettings _idmUserServiceSettings;
        private readonly ILogger<IdmUserService> _logger;

        public IdmUserService(IConfiguration configuration, ILogger<IdmUserService> logger)
        {
            _idmUserServiceSettings =
                configuration.GetSection("IdmUserService").Get<IdmUserServiceSettings>() ??
                throw new ArgumentNullException(nameof(IdmUserServiceSettings));

            _logger = logger;
            BpkPrefix = _idmUserServiceSettings.BpkPrefix ?? "BF:";
        }

        public async Task<IEnumerable<IdmUser>> GetUsersAsync()
        {
            const string sql =
                @"SELECT BED_ZNAME as Sn, BED_VNAME as GivenName, BED_TITEL as Title, BPK as BpkBf, SIB_EMAIL as Mail, SIB_ADRESSE as StreetAddress, 
                  SIB_ZIMMER as Room, SAP as SapPersNr, LEITERTYP as JobRole, ZUORDNUNG as TopLevelUnits, TELEFON as TelephoneNumber, MOBILTELEFON as Mobile
                  FROM V_VERTEILER_AD_EXPORT
                  WHERE SAP IS NOT NULL AND SAP <> '' order by BED_ZNAME, BED_VNAME";

            if (string.IsNullOrWhiteSpace(_idmUserServiceSettings.ConnectionString))
            {
                throw new InvalidOperationException("The IDM user service connection string is null or empty. Please check your configuration.");
            }

            try
            {
                SqlConnection connection = new(_idmUserServiceSettings.ConnectionString!);
                using (connection)
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    return await connection.QueryAsync<IdmUser>(sql);
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
