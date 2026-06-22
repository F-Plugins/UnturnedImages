using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using System.Reflection;
using UnturnedImages.API.Items;
using UnturnedImages.API.Vehicles;
using UnturnedImages.RocketMod.Configuration;
using UnturnedImages.RocketMod.Items;
using UnturnedImages.RocketMod.Vehicles;

namespace UnturnedImages.RocketMod
{
    public class UnturnedImagesRocketPlugin : RocketPlugin<UnturnedImagesRocketConfiguration>
    {
        private ItemImageDirectory? _itemImages;
        private VehicleImageDirectory? _vehicleImages;

        public static UnturnedImagesRocketPlugin? Instance { get; private set; }

        public IItemImageDirectorySync? ItemImages => _itemImages;

        public IItemImageDirectoryAsync? ItemImagesAsync => _itemImages;

        public IVehicleImageDirectorySync? VehicleImages => _vehicleImages;

        public IVehicleImageDirectoryAsync? VehicleImagesAsync => _vehicleImages;

        protected override void Load()
        {
            Instance = this;
            _itemImages = new ItemImageDirectory(Configuration.Instance);
            _vehicleImages = new VehicleImageDirectory(Configuration.Instance);

            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
            Logger.Log($"UnturnedImages RocketMod v{version} loaded.");
        }

        protected override void Unload()
        {
            _itemImages = null;
            _vehicleImages = null;
            Instance = null;

            Logger.Log("UnturnedImages RocketMod unloaded.");
        }

        public void ReloadImageConfiguration()
        {
            _itemImages?.Reload(Configuration.Instance);
            _vehicleImages?.Reload(Configuration.Instance);
        }
    }
}
