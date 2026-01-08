namespace nocscienceat.MetaDirectory.Services.AdService.Models
{
    public class AdComputer
    {
        public AdComputer () {}
        
        public AdComputer(AdComputer cloneSource)
        {
            DistinguishedName = cloneSource.DistinguishedName;
            SamAccountName = cloneSource.SamAccountName;
            Name = cloneSource.Name;
        }

        public string? SamAccountName { get; set; }
        public string? DistinguishedName { get; set; }
        public string Name { get; set; } = null!;
        public string? Status { get; set; }

        public override string ToString()
        {
            return Name ?? DistinguishedName ?? SamAccountName ?? nameof(AdComputer);
        }
    }
}
