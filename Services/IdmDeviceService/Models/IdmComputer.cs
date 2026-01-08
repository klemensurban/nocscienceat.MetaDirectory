using System;
using System.Text.Json.Serialization;

namespace nocscienceat.MetaDirectory.Services.IdmDeviceService.Models
{
    public class IdmComputer
    {
        private string _name = null!;
        private string _status = null!;
        private string? _location;
        private string? _serial;
        private string? _inventory;

        public string Name
        {
            get => _name;
            set => _name = value.Trim().ToUpperInvariant();
        }

        public string Status
        {
            get => _status;
            set => _status = string.IsNullOrWhiteSpace(value) ? IdmDeviceService.DefaultStatus : value.Trim();
        }


        public string? Location
        {
            get => _location;
            set => _location = NormalizeString(value);
        }

        public string? Serial
        {
            get => _serial;
            set => _serial = NormalizeString(value);
        }

        public string? Inventory
        {
            get => _inventory;
            set => _inventory = NormalizeString(value);
        }

        private static string? NormalizeString(string? value)
        {
            return !string.IsNullOrWhiteSpace(value) ? value!.Trim() : null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
