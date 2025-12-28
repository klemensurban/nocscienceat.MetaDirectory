namespace nocscienceat.MetaDirectory.Services.IdmUserService.Models
{
    using nocscienceat.MetaDirectory.Services.IdmUserService;

    public sealed class IdmUser
    {
        private string? _mail;
        private string? _bpkBf;
        private string? _jobRole;

        public string? Sn { get; set; }
        public string? GivenName { get; set; }
        public string? Title { get; set; }

        public string? BpkBf
        {
            get => _bpkBf;
            set => _bpkBf = value is not null ? IdmUserService.BpkPrefix + value : null;
        }

        public string? Mail
        {
            get =>  _mail;
            set => _mail = value?.ToLowerInvariant();
        }
        public string? StreetAddress { get; set; }          // extensionAttribute5
        public string? Room { get; set; }                   // extensionAttribute6
        public string SapPersNr { get; set; } = null!;

        public string? JobRole                              // extensionAttribute14
        {
            get => _jobRole;
            set => _jobRole = value?.ToLowerInvariant();
        } 

        public string? TopLevelUnits { get; set; }          // extensionAttribute7
        public string? TelephoneNumber { get; set; }
        public string? Mobile { get; set; }

        public override string ToString()
        {
            string? sn = string.IsNullOrWhiteSpace(Sn) ? null : Sn;
            string? givenName = string.IsNullOrWhiteSpace(GivenName) ? null : GivenName;

            if (sn == null && givenName == null)
                return nameof(IdmUser);

            if (sn != null && givenName != null)
                return $"{sn} {givenName}";

            return sn ?? givenName!;
        }
    }
}