using Rocket.API;
using System.Collections.Generic;
using UnturnedImages.Configuration.Overrides;

namespace UnturnedImages.RocketMod.Configuration
{
    public class UnturnedImagesRocketConfiguration : IRocketPluginConfiguration
    {
        public DefaultRepositoriesConfig DefaultRepositories { get; set; } = new DefaultRepositoriesConfig();

        public List<OverrideConfig> ItemOverrides { get; set; } = new List<OverrideConfig>();

        public List<OverrideConfig> VehicleOverrides { get; set; } = new List<OverrideConfig>();

        public void LoadDefaults()
        {
            DefaultRepositories = new DefaultRepositoriesConfig();
            ItemOverrides = new List<OverrideConfig>
            {
                new OverrideConfig
                {
                    Id = "46000-47000",
                    Repository = "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/example/items/{ItemId}.png"
                },
                new OverrideConfig
                {
                    WorkshopId = "3030546506",
                    Repository = "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/3030546506/items/{ItemId}.png"
                }
            };
            VehicleOverrides = new List<OverrideConfig>
            {
                new OverrideConfig
                {
                    Id = "12000-12100",
                    Repository = "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/example/vehicles/{VehicleId}.png"
                },
                new OverrideConfig
                {
                    Guid = "b1db3548447c4cde9f92d274e22f57a2",
                    Repository = "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/workshop/example/vehicles/{VehicleId}.png"
                }
            };
        }
    }
}
