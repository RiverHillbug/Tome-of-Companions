using System;
using System.Collections.Generic;

namespace TomeOfCompanions.Models
{
    public class PlayerEntry
    {
        public string AccountName { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string Notes { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
