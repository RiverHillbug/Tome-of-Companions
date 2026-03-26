using Blish_HUD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TomeOfCompanions.Models;

namespace TomeOfCompanions.Data
{
    public class PlayerEntriesHandler
    {
        private static readonly Logger Logger = Logger.GetLogger<PlayerEntriesHandler>();
        private readonly string _filePath;
        private Dictionary<string, PlayerEntry> _entries = new Dictionary<string, PlayerEntry>();

        public PlayerEntriesHandler( string dataDirectory )
        {
            _filePath = Path.Combine( dataDirectory, "PlayerEntries.json" );
        }

        public void Load()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    _entries = JsonSerializer.Deserialize<Dictionary<string, PlayerEntry>>(json) ?? new Dictionary<string, PlayerEntry>();
                    Logger.Info($"Loaded {_entries.Count} player entries.");
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to load notes: {e.Message}");
                }
            }
            else
            {
                Logger.Info("No existing player entries file found. Starting with an empty list.");
            }
        }

        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch (Exception e)
            {
                Logger.Warn($"Failed to save notes: {e.Message}");
            }
        }

        public IEnumerable<PlayerEntry> GetAll() => _entries.Values;

        public PlayerEntry GetOrCreate(string accountName)
        {
            if ( !_entries.TryGetValue(accountName, out var entry))
            {
                entry = new PlayerEntry { AccountName = accountName };
                _entries[accountName] = entry;
            }
            return entry;
        }

        public void Delete(string accountName) => _entries.Remove(accountName);
    }
}
