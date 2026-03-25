using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

// You can rename the namespace to whatever you want. Including the module name in the namespace is good idea.
// If you change this in the future again after you already released a module version, you should let freesnow know about it.
// Because they have to update that in the Sentry Blish Bug tracker, too.
// This namespace does not have to match the namespace in the manifest.json. They are not related. 
// (Side note: the manifest.json namespace has to be set once and must NOT be changed after a module was released)
namespace TomeOfCompanions
{
    [Export( typeof( Module ) )]
    public class TomeOfCompanionsModule : Module
    {
        private static readonly Logger Logger = Logger.GetLogger<TomeOfCompanionsModule>();

        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;

        internal static TomeOfCompanionsModule ModuleInstance;

        [ImportingConstructor]
        public TomeOfCompanionsModule( [Import( "ModuleParameters" )] ModuleParameters moduleParameters ) : base( moduleParameters ) {
            ModuleInstance = this;
        }
          
        protected override void DefineSettings( SettingCollection settings ) { }

        protected override async Task LoadAsync() {
            Logger.Info( "Tome of Companions loading..." );
        }

        protected override void Unload() {
            ModuleInstance = null;
        }
    }
}
