using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using TomeOfCompanions.Data;
using TomeOfCompanions.UI;

// You can rename the namespace to whatever you want. Including the module name in the namespace is good idea.
// If you change this in the future again after you already released a module version, you should let freesnow know about it.
// Because they have to update that in the Sentry Blish Bug tracker, too.
// This namespace does not have to match the namespace in the manifest.json. They are not related. 
// (Side note: the manifest.json namespace has to be set once and must NOT be changed after a module was released)
namespace TomeOfCompanions
{
    [Export(typeof(Module))]
    public class TomeOfCompanionsModule : Module
    {
        private static readonly Logger Logger = Logger.GetLogger<TomeOfCompanionsModule>();
        private PlayerEntriesHandler _entriesHandler;

        private EntriesWindow _entriesWindow;
        private CornerIcon _cornerIcon;

        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;

        internal static TomeOfCompanionsModule ModuleInstance;

        [ImportingConstructor]
        public TomeOfCompanionsModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) {
            ModuleInstance = this;
        }
          
        protected override void DefineSettings(SettingCollection settings) { }

        protected override /*async*/ Task LoadAsync()
        {
            var dataDir = DirectoriesManager.GetFullDirectoryPath("tome-of-companions");
            _entriesHandler = new PlayerEntriesHandler(dataDir);
            _entriesHandler.Load();

            _entriesWindow = new EntriesWindow(_entriesHandler);

            _cornerIcon = new CornerIcon() {
                Icon = AsyncTexture2D.FromAssetId(2596976),
                BasicTooltipText = "Tome of Companions",
                Priority = 1923847562,
                Parent = Blish_HUD.GameService.Graphics.SpriteScreen
            };
            _cornerIcon.Click += (s, e) => _entriesWindow.ToggleWindow();

            Logger.Info("Tome of Companions loaded.");

            return Task.CompletedTask;
        }
        
        protected override void Unload()
        {
            _cornerIcon?.Dispose();
            _entriesWindow?.Dispose();
            _entriesHandler?.Save();
            ModuleInstance = null;
        }
    }
}
