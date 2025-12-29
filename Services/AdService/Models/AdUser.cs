namespace nocscienceat.MetaDirectory.Services.AdService.Models
{
    public class AdUser
    { 
        public AdUser() {}

        public AdUser(AdUser cloneSource)
        {
            DistinguishedName = cloneSource.DistinguishedName;
            SamAccountName = cloneSource.SamAccountName;
        }

        public string? SamAccountName { get; set; }
        public string? DistinguishedName { get; set; }
        public string? SapPersNr { get; set; }
        public string? BpkBf { get; set; }
        public string? Sn { get; set; }
        public string? GivenName { get; set; }
        public string? Mail { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? Mobile { get; set; }
        public string? Title { get; set; }
        public string? StreetAddress { get; set; }
        public string? Room { get; set; }
        public string? TopLevelUnits { get; set; }
        public string? JobRole { get; set; }

        public override string ToString()
        {
            return Sn == null && GivenName == null ? nameof(AdUser) : Sn != null && GivenName != null ? $"{Sn} {GivenName}" : Sn ?? GivenName!;
        }
    }
}
