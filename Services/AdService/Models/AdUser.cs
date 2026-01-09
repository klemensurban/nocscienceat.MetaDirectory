namespace nocscienceat.MetaDirectory.Services.AdService.Models
{
    /// <summary>
    /// Thin projection of an AD user record used for sync comparisons.
    /// </summary>
    public class AdUser
    { 
        public AdUser() {}

        /// <summary>
        /// Copy constructor used by adapters to preserve identity fields before mutating state.
        /// </summary>
        public AdUser(AdUser cloneSource)
        {
            DistinguishedName = cloneSource.DistinguishedName;
            SamAccountName = cloneSource.SamAccountName;
        }

        /// <summary>
        /// Directory logon name.
        /// </summary>
        public string? SamAccountName { get; set; }

        /// <summary>
        /// Full distinguished name.
        /// </summary>
        public string? DistinguishedName { get; set; }

        /// <summary>
        /// SAP personnel number (matches IDM users).
        /// </summary>
        public string? SapPersNr { get; set; }

        /// <summary>
        /// BPK identifier 
        /// </summary>
        public string? BpkBf { get; set; }

        /// <summary>
        /// Surname.
        /// </summary>
        public string? Sn { get; set; }

        /// <summary>
        /// Given name.
        /// </summary>
        public string? GivenName { get; set; }

        /// <summary>
        /// Primary email address.
        /// </summary>
        public string? Mail { get; set; }

        /// <summary>
        /// Office phone number.
        /// </summary>
        public string? TelephoneNumber { get; set; }

        /// <summary>
        /// Mobile phone number.
        /// </summary>
        public string? Mobile { get; set; }

        /// <summary>
        /// Job title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Street address (extensionAttribute5).
        /// </summary>
        public string? StreetAddress { get; set; }

        /// <summary>
        /// Room/office (extensionAttribute6).
        /// </summary>
        public string? Room { get; set; }

        /// <summary>
        /// Organizational units (extensionAttribute7).
        /// </summary>
        public string? TopLevelUnits { get; set; }

        /// <summary>
        /// Role classification (extensionAttribute14).
        /// </summary>
        public string? JobRole { get; set; }

        /// <summary>
        /// Human-readable display used in Debugging.
        /// </summary>
        public override string ToString()
        {
            return Sn == null && GivenName == null ? nameof(AdUser) : Sn != null && GivenName != null ? $"{Sn} {GivenName}" : Sn ?? GivenName!;
        }
    }
}
