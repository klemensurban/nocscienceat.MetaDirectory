namespace nocscienceat.MetaDirectory.Services.IdmUserService.Models
{
    using nocscienceat.MetaDirectory.Services.IdmUserService;

    /// <summary>
    /// Normalized IDM user projection used as the source of truth for synchronization.
    /// </summary>
    public sealed class IdmUser
    {
        private string? _bpkBf;
        private string? _givenName;
        private string? _jobRole;
        private string? _mail;
        private string? _mobile;
        private string? _room;
        private string? _sn;
        private string? _streetAddress;
        private string? _telephoneNumber;
        private string? _title;
        private string? _topLevelUnits;

        /// <summary>
        /// SAP personnel number (primary key for sync).
        /// </summary>
        public string SapPersNr { get; set; } = null!;      // SAP-ID: Key to Synchronize

        /// <summary>
        /// BPK identifier with configured prefix applied when provided.
        /// </summary>
        public string? BpkBf
        {
            get => _bpkBf;
            set => _bpkBf = !string.IsNullOrWhiteSpace(value) ? IdmUserService.BpkPrefix + value!.Trim() : null;
        }

        /// <summary>
        /// Given name trimmed and nullified when empty.
        /// </summary>
        public string? GivenName
        {
            get => _givenName;
            set => _givenName = NormalizeString(value);
        }

        /// <summary>
        /// Job role stored in lower case to match AD attribute semantics.
        /// </summary>
        public string? JobRole                              // extensionAttribute14
        {
            get => _jobRole;
            set => _jobRole = !string.IsNullOrWhiteSpace(value) ? value!.ToLowerInvariant() : null;
        }

        /// <summary>
        /// Email normalized to lower case plus trimmed whitespace.
        /// </summary>
        public string? Mail
        {
            get =>  _mail;
            set => _mail = !string.IsNullOrWhiteSpace(value) ? value!.ToLowerInvariant().Trim() : null;
        }

        /// <summary>
        /// Mobile number stripped of spaces to ensure consistent comparisons.
        /// </summary>
        public string? Mobile
        {
            get => _mobile;
            set => _mobile = !string.IsNullOrWhiteSpace(value) ? value!.Replace(" ", "").Trim() : null;
        }

        /// <summary>
        /// Office room (extensionAttribute6) trimmed/nullified.
        /// </summary>
        public string? Room                                 // extensionAttribute6
        {
            get => _room;
            set => _room = NormalizeString(value);
        } 

        /// <summary>
        /// Surname trimmed/nullified.
        /// </summary>
        public string? Sn
        {
            get => _sn;
            set => _sn = NormalizeString(value);
        }

        /// <summary>
        /// Street address (extensionAttribute5) trimmed/nullified.
        /// </summary>
        public string? StreetAddress                        // extensionAttribute5
        {
            get => _streetAddress;
            set => _streetAddress = NormalizeString(value);
        } 
            
        /// <summary>
        /// Telephone number trimmed/nullified.
        /// </summary>
        public string? TelephoneNumber
        {
            get => _telephoneNumber;
            set => _telephoneNumber = NormalizeString(value);
        }

        /// <summary>
        /// Job title normalized for AD.
        /// </summary>
        public string? Title
        {
            get => _title;
            set => _title = NormalizeString(value);
        }

        /// <summary>
        /// Hierarchical unit info (extensionAttribute7) trimmed/nullified.
        /// </summary>
        public string? TopLevelUnits                        // extensionAttribute7
        {
            get => _topLevelUnits;
            set => _topLevelUnits = NormalizeString(value);
        } 

        /// <summary>
        /// Shared normalization routine for string properties.
        /// </summary>
        private static string? NormalizeString(string? value)
        {
            return !string.IsNullOrWhiteSpace(value) ? value!.Trim() : null;
        }

        /// <summary>
        /// Produces a friendly display name for logging/diagnostics.
        /// </summary>
        public override string ToString()
        {
            return Sn == null && GivenName == null ? nameof(IdmUser) : Sn != null && GivenName != null ? $"{Sn} {GivenName}" : Sn ?? GivenName!;
        }
    }
}