using System.Collections.Generic;

namespace UnturnedImages.Configuration.Overrides
{
    public class VehicleOverridesConfig
    {
        public ICollection<OverrideConfig> VehicleOverrides { get; set; } = new List<OverrideConfig>();
    }
}
