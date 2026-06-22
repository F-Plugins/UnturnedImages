namespace UnturnedImages.RocketMod.Configuration
{
    public class DefaultRepositoriesConfig
    {
        public string Items { get; set; } = "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/vanilla/items/{ItemId}.png";

        public string Vehicles { get; set; } = "https://cdn.jsdelivr.net/gh/F-Plugins/UnturnedImages@images/vanilla/vehicles/{VehicleId}.png";
    }
}
