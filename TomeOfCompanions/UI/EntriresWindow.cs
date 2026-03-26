using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System;
using TomeOfCompanions.Data;
using TomeOfCompanions.Models;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TomeOfCompanions.UI
{
    public class EntriesWindow : StandardWindow
    {
        private const int COL_NAME_WIDTH = 120;
        private const int COL_TAGS_WIDTH = 200;
        private const int COL_NOTES_WIDTH = 200;
        private const int ROW_HEIGHT = 60;

        private readonly PlayerEntriesHandler _entriesHandler;
        private TextBox _newEntryTextbox;
        private FlowPanel _entryListPanel;

        public EntriesWindow(PlayerEntriesHandler entriesHandler) : base(
            Blish_HUD.GameService.Content.DatAssetCache.GetTextureFromAssetId(155997),
            new Rectangle(25, 26, 580, 640),
            new Rectangle(40, 50, 660, 590))
        {
            _entriesHandler = entriesHandler;
            Parent = Blish_HUD.GameService.Graphics.SpriteScreen;
            Title = "Tome of Companions";
            Location = new Point(300, 300);
            SavesPosition = true;
            Id = "TomeOfCompanions_MainWindow";

            BuildNewEntryBar();
            BuildHeaderRow();
            BuildEntryList();
        }


        private void BuildNewEntryBar()
        {
            _newEntryTextbox = new TextBox() {
                PlaceholderText = "Add player (e.g. Name.1234)...",
                Width = 520,
                Location = new Point(4, 5),
                Parent = this
            };
            _newEntryTextbox.EnterPressed += OnNewEntrySubmitted;
        }

        private void BuildHeaderRow()
        {
            var header = new Panel() {
                Width = 550,
                Height = 25,
                Location = new Point(5, 40),
                Parent = this
            };

            new Label() { Text = "Name", Width = COL_NAME_WIDTH, Location = new Point(0, 0), Parent = header };
            new Label() { Text = "Tags", Width = COL_TAGS_WIDTH, Location = new Point(COL_NAME_WIDTH, 0), Parent = header };
            new Label() { Text = "Notes", Width = COL_NOTES_WIDTH, Location = new Point(COL_NAME_WIDTH + COL_TAGS_WIDTH, 0), Parent = header };
        }

        private void BuildEntryList()
        {
            _entryListPanel = new FlowPanel() {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                Width = 550,
                Height = 520,
                Location = new Point(5, 75),
                CanScroll = true,
                Parent = this
            };

            RefreshEntryList();
        }

        private void RefreshEntryList()
        {
            _entryListPanel.ClearChildren();

            foreach ( var entry in _entriesHandler.GetAll() )
            {
                BuildEntryRow(entry);
            }
        }

        private void BuildEntryRow(PlayerEntry entry)
        {
            var row = new Panel() {
                Width = 545,
                Height = ROW_HEIGHT,
                Parent = _entryListPanel
            };

            // Name column
            new Label() {
                Text = entry.AccountName,
                Width = COL_NAME_WIDTH,
                Height = ROW_HEIGHT,
                Location = new Point(0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Parent = row
            };

            // Tags column
            var tagsPanel = new FlowPanel() {
                FlowDirection = ControlFlowDirection.LeftToRight,
                Width = COL_TAGS_WIDTH,
                Height = ROW_HEIGHT,
                Location = new Point(COL_NAME_WIDTH, 0),
                Parent = row
            };

            foreach ( var tag in entry.Tags )
            {
                var tagLabel = new Label() {
                    Text = tag,
                    Width = Math.Max(tag.Length * 8, 40),
                    Height = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    BackgroundColor = Color.DarkRed,    // TODO: Random colors?
                    BasicTooltipText = "Click to remove",
                    Parent = tagsPanel
                };

                tagLabel.Click += (s, e) => {
                    entry.Tags.Remove(tag);
                    _entriesHandler.Save();
                    RefreshEntryList();
                };
            }

            var newTagBox = new TextBox() {
                PlaceholderText = "Add tag...",
                Width = 80,
                Height = 20,
                Parent = tagsPanel
            };

            newTagBox.EnterPressed += (s, e) => {
                var tag = newTagBox.Text.Trim();
                if ( string.IsNullOrEmpty(tag) ) return;
                entry.Tags.Add(tag);
                _entriesHandler.Save();
                RefreshEntryList();
            };

            // TODO: scrollable or something
            var notesBox = new TextBox() {
                Text = entry.Notes,
                PlaceholderText = "Notes...",
                Width = COL_NOTES_WIDTH - 10,
                Height = ROW_HEIGHT - 10,
                Location = new Point(COL_NAME_WIDTH + COL_TAGS_WIDTH, 5),
                Parent = row
            };

            notesBox.TextChanged += (s, e) => {
                entry.Notes = notesBox.Text;
                _entriesHandler.Save();
            };

            // TODO: maybe add a confim pop-up
            var menu = new ContextMenuStrip();
            menu.AddMenuItem($"Delete {entry.AccountName}").Click += (s, e) => {
                _entriesHandler.Delete(entry.AccountName);
                _entriesHandler.Save();
                RefreshEntryList();
            };
            row.Menu = menu;
        }

        private void OnNewEntrySubmitted(object sender, EventArgs e)
        {
            var accountName = _newEntryTextbox.Text.Trim();
            if ( string.IsNullOrEmpty(accountName) ) return;
            _entriesHandler.GetOrCreate(accountName);
            _entriesHandler.Save();
            _newEntryTextbox.Text = string.Empty;
            RefreshEntryList();
        }
    }
}
