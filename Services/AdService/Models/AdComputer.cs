namespace nocscienceat.MetaDirectory.Services.AdService.Models
{
    /// <summary>
    /// Minimal AD computer projection used during sync operations.
    /// </summary>
    public class AdComputer
    {
        public AdComputer () {}
        
        /// <summary>
        /// Copy constructor used by adapters to preserve identity fields before mutating state.
        /// </summary>
        public AdComputer(AdComputer cloneSource)
        {
            DistinguishedName = cloneSource.DistinguishedName;
            SamAccountName = cloneSource.SamAccountName;
            Name = cloneSource.Name;
        }

        /// <summary>
        /// SamAccountName backing the directory identity.
        /// </summary>
        public string? SamAccountName { get; set; }

        /// <summary>
        /// Fully qualified distinguished name.
        /// </summary>
        public string? DistinguishedName { get; set; }

        /// <summary>
        /// Canonical computer name (required).
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// JSON payload describing CMDB-derived metadata (extensionAttribute10).
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Friendly identifier for debugging.
        /// </summary>
        public override string ToString()
        {
            return Name ?? DistinguishedName ?? SamAccountName ?? nameof(AdComputer);
        }
    }
}
