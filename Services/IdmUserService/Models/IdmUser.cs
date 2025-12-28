namespace nocscienceat.MetaDirectory.Services.IdmUserService.Models
{
    using nocscienceat.MetaDirectory.Services.IdmUserService;

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

        public string SapPersNr { get; set; } = null!;      // SAP-ID: Key to Synchronize

        public string? BpkBf
        {
            get => _bpkBf;
            set => _bpkBf = !string.IsNullOrWhiteSpace(value) ? IdmUserService.BpkPrefix + value!.Trim() : null;
        }

        public string? GivenName
        {
            get => _givenName;
            set => _givenName = NormalizeString(value);
        }

        public string? JobRole                              // extensionAttribute14
        {
            get => _jobRole;
            set => _jobRole = !string.IsNullOrWhiteSpace(value) ? value!.ToLowerInvariant() : null;
        }

        public string? Mail
        {
            get =>  _mail;
            set => _mail = !string.IsNullOrWhiteSpace(value) ? value!.ToLowerInvariant().Trim() : null;
        }

        public string? Mobile
        {
            get => _mobile;
            set => _mobile = NormalizeString(value);
        }

        public string? Room                                 // extensionAttribute6
        {
            get => _room;
            set => _room = NormalizeString(value);
        } 

        public string? Sn
        {
            get => _sn;
            set => _sn = NormalizeString(value);
        }

        public string? StreetAddress                        // extensionAttribute5
        {
            get => _streetAddress;
            set => _streetAddress = NormalizeString(value);
        } 

        public string? TelephoneNumber
        {
            get => _telephoneNumber;
            set => _telephoneNumber = NormalizeString(value);
        }

        public string? Title
        {
            get => _title;
            set => _title = NormalizeString(value);
        }

        public string? TopLevelUnits                        // extensionAttribute7
        {
            get => _topLevelUnits;
            set => _topLevelUnits = NormalizeString(value);
        } 

        private static string? NormalizeString(string? value)
        {
            return !string.IsNullOrWhiteSpace(value) ? value!.Trim() : null;
        }

        public override string ToString()
        {
            return Sn == null && GivenName == null ? nameof(IdmUser) : Sn != null && GivenName != null ? $"{Sn} {GivenName}" : Sn ?? GivenName!;
        }
    }
}