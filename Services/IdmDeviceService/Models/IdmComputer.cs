namespace nocscienceat.MetaDirectory.Services.IdmDeviceService.Models
{
    /// <summary>
    /// Represents a CMDB computer record with normalization helpers for downstream sync layers.
    /// </summary>
    public class IdmComputer
    {
        private string _name = null!;
        private string _status = null!;
        private string? _location;
        private string? _serial;
        private string? _inventory;

        /// <summary>
        /// Uppercases and trims the CMDB hostname to ensure deterministic key matching.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value.Trim().ToUpperInvariant();
        }

        /// <summary>
        /// Uses the configured default status when CMDB sends an empty value; otherwise trims the input.
        /// </summary>
        public string Status
        {
            get => _status;
            set => _status = string.IsNullOrWhiteSpace(value) ? IdmDeviceService.DefaultStatus : value.Trim();
        }

        /// <summary>
        /// Optional device location (trimmed, null when empty).
        /// </summary>
        public string? Location
        {
            get => _location;
            set => _location = NormalizeString(value);
        }

        /// <summary>
        /// Optional device serial number (trimmed, null when empty).
        /// </summary>
        public string? Serial
        {
            get => _serial;
            set => _serial = NormalizeString(value);
        }

        /// <summary>
        /// Optional inventory identifier (trimmed, null when empty).
        /// </summary>
        public string? Inventory
        {
            get => _inventory;
            set => _inventory = NormalizeString(value);
        }

        /// <summary>
        /// Applies consistent trimming/nullification rules for CMDB strings.
        /// </summary>
        private static string? NormalizeString(string? value)
        {
            return !string.IsNullOrWhiteSpace(value) ? value!.Trim() : null;
        }

        /// <summary>
        /// Returns the normalized host name for logging and debugging.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}
