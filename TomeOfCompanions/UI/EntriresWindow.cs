using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System;
using TomeOfCompanions.Data;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TomeOfCompanions.UI
{
    public class EntriesWindow : StandardWindow
    {
        private readonly PlayerEntriesHandler _entriesHandler;
        private TextBox _newEntryTextbox;
        private FlowPanel _entryListPanel;

        public EntriesWindow( PlayerEntriesHandler entriesHandler ) : base(
               Blish_HUD.Content.AsyncTexture2D.FromAssetId( 155997 ),
               new Rectangle( 25, 26, 560, 640 ),
               new Rectangle( 40, 50, 540, 590 ) )
        {
            _entriesHandler = entriesHandler;

            Parent = Blish_HUD.GameService.Graphics.SpriteScreen;
            Title = "Tome of Companions";
            Location = new Point( 300, 300 );
            SavesPosition = true;
            Id = "TomeOfCompanions_MainWindow";

            BuildNewEntryBar();
            BuildEntryList();
        }

        private void BuildNewEntryBar()
        {
            _newEntryTextbox = new TextBox() {
                PlaceholderText = "Add player (e.g. Name.1234)...",
                Width = 500,
                Location = new Point( 10, 10 ),
                Parent = this
            };

            _newEntryTextbox.EnterPressed += OnNewEntrySubmitted;
        }

        private void BuildEntryList()
        {
            _entryListPanel = new FlowPanel() {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                Width = 500,
                Height = 500,
                Location = new Point( 10, 50 ),
                CanScroll = true,
                Parent = this
            };

            RefreshEntryList();
        }

        private void RefreshEntryList()
        {
            if ( _entriesHandler == null || _entryListPanel == null ) return;

            _entryListPanel.ClearChildren();

            foreach ( var entry in _entriesHandler.GetAll() )
            {
                var label = new Label() {
                    Text = entry.AccountName,
                    Width = 480,
                    AutoSizeHeight = true,
                    Parent = _entryListPanel
                };

                var menu = new ContextMenuStrip();
                menu.AddMenuItem($"Delete {entry.AccountName}").Click += (s, e) => {
                    _entriesHandler.Delete(entry.AccountName);
                    _entriesHandler.Save();
                    RefreshEntryList();
                };
                label.Menu = menu;
            }
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
